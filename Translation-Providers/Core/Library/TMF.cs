using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrownPeak.CMSAPI;
using CrownPeak.CMSAPI.Services;

/* Some Namespaces are not allowed. */
namespace LocalProject
{
	/*
	 * TMF with Translation Support - version history
	 * ----------------------------------------------
	 * Version | Date       |
	 * ----------------------------------------------
	 * 0.1.0   | 2020-07-10 | Initial release
	 * 0.1.1   | 2020-10-19 | Support for user-configurable translation types
	 * 0.1.2   | 2020-11-19 | Add support for sending folders for translation
	 * 0.1.3   | 2020-11-30 | Performance improvements in CreateTranslatedAsset and FixRelativeLinks
	 * 0.1.4   | 2021-03-02 | Fix to code locating the Translation Config asset
	 * 0.2.0   | 2021-05-07 | Support for multi-asset projects
	 * 0.2.1   | 2021-07-08 | Include upload# bug fix from OCD-21746
	 * 0.2.2   | 2021-07-14 | Add ConfigPostInput to ITMFTranslator
	 */
	#region "LocaleId Class"

	/// <summary>
	///   LocaleId is a class used by other methods that defines properties of the locale, an ID and folder path
	/// </summary>
	public class LocaleId
	{
		public LocaleId(Asset localeAsset)
		{
			FolderRoot = localeAsset.Raw["folder_root"].ToLower().Trim();
			Id = localeAsset.Id.ToString();
		}

		public string FolderRoot { get; private set; }
		public string Id { get; private set; }
	}

	#endregion "LocaleId Class"


	public class TMF
	{
		public static string _TMFConfig = "_TMF";
		public static string Locales_Config = "/Locales Config/";
		public static string Relationships_Config = "/Relationships Config/";

		// Used by ServicesTMF.RemoveTmfDuplicates() for identifying "upload#" prefixes.
		private const string UploadPrefix = "upload#";
		private static readonly int UploadPrefixLen = UploadPrefix.Length;
    
		/// <summary>
		///   Returns TMF Config Folder Name
		/// </summary>
		/// <returns>_TMF</returns>
		private static string _GetTMFConfigFolderName()
		{
			return _TMFConfig;
		}

		/// <summary>
		///   Returns segment of asset path indicated by the nameSegment index
		/// </summary>
		/// <param name="asset">Asset whos path segment will be returned</param>
		/// <param name="nameSegment">index of asset path segment that will be returned</param>
		/// <returns>Segment of an AssetPath</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// string siteName = ServicesTMF.GetSiteName(asset, 3);
		/// ]]></code>
		/// </example>
		public static string GetSiteName(Asset asset, int nameSegment)
		{
			return asset.AssetPath[nameSegment];
		}

		/// <summary>
		///   Searchs for and returns the "_TMF" folder as an asset.
		///   <para>Assuming the "_TMF" folder is in the same folder as the asset parameter, or in one of the parent folders </para>
		/// </summary>
		/// <param name="asset">An asset within the site's folders</param>
		/// <returns>The _TMF folder as an asset</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// Asset tmfFolderAsset = SerevicesTMF.GetSitePath(asset);
		/// ]]></code>
		/// </example>
		public static Asset GetSitePath(Asset asset)
		{
			var tmfConfigFolderName = _GetTMFConfigFolderName();
			var folderPath = string.Empty;

			if (asset.Parent.AssetPath.Count > 0)
			{
				for (var numFolderSegments = asset.Parent.AssetPath.Count; numFolderSegments > 0; numFolderSegments--)
				{
					var tmfConfigAsset = Asset.Load("/" + string.Join("/", asset.AssetPath.Take(numFolderSegments)) + "/" +
																					tmfConfigFolderName);
					if (tmfConfigAsset.IsLoaded)
					{
						folderPath = tmfConfigAsset.AssetPath.ToString();
						break;
					}
				}
			}

			var aReturn = Asset.Load(folderPath);

			return aReturn.IsLoaded ? aReturn : null;
		}

		/// <summary>
		///   Returns a IEnumerable of LocalId objects.
		///   The files in the "Locales Config" folder are projected to LocaleId objects.
		/// </summary>
		/// <param name="sitePath">Path of _TMF folder</param>
		/// <returns>Assets from the folder /_TMF/Locales Config/</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// IEnumerable<LocaleId> ieLID = ServicesTMF.CreateLocaleConfigCache("/SiteQA/TMF_Test_Root/_TMF");
		/// ]]></code>
		/// </example>
		public static IEnumerable<LocaleId> CreateLocaleConfigCache(string sitePath)
		{
			return Asset.Load(sitePath + Locales_Config).GetFileList().Select(aData => new LocaleId(aData));
		}

		/// <summary>
		///   Searches through the assets in a given path and checks if one of the assets' "folder_root" field content
		///   exists anywhere in the path of the "asset" parameter. If so, then it returns the ID of the asset that had
		///   the matching "folder_root" field.
		/// </summary>
		/// <param name="asset">Asset with a path that contains the "folder_root"</param>
		/// <param name="sitePath">_TMF folder path</param>
		/// <returns>ID string of the asset with matching content in the "folder_root" field</returns>
		public static string GetLocaleId(Asset asset, string sitePath)
		{
			var returnID = string.Empty;

			//Load list of config assets in _TMF/Locales Config/ folder
			var localConfigAssetList = Asset.Load(sitePath + Locales_Config).GetFileList();
			var assetPath = asset.AssetPath.ToString().ToUpper();
			if (asset.Type.Equals(AssetType.Folder))
			{
				//If the asset is a folder, add the "/" at the end if it doesn't have one already
				assetPath = asset.AssetPath.ToString().ToUpper() + (Equals(assetPath.Last(), "/") ? "" : "/");
			}

			foreach (var entry in localConfigAssetList)
			{
				if (!string.IsNullOrWhiteSpace(entry.Raw["folder_root"]))
				{
					//If the "folder_root" of the entry exists within the asset's path, return that entry
					if (assetPath.Contains(entry["folder_root"].ToUpper() +
																 (Equals(entry["folder_root"].Last(), "/") ? "" : "/")))
					{
						returnID = entry.Id.ToString();
					}
				}
			}

			return returnID;
		}

		/// <summary>
		///   Searches through the assets in a given path and checks if one of the assets' "folder_root" field content
		///   exists anywhere in the path of the "asset" parameter. If so, then it returns the ID of the asset that had
		///   the matching "folder_root" field.
		/// </summary>
		/// <param name="asset"></param>
		/// <param name="sitePath"></param>
		/// <param name="localeConfigCache"></param>
		/// <returns></returns>
		public static string GetLocaleId(Asset asset, string sitePath, IEnumerable<LocaleId> localeConfigCache = null)
		{
			if (localeConfigCache == null)
			{
				//NOTE: this is a last resort. You should pass a copy in that's created OUTSIDE of all foreach loops.
				localeConfigCache = CreateLocaleConfigCache(sitePath);
			}

			var assetPath = asset.AssetPath.ToString().ToLower();

			if (asset.Type.Equals(AssetType.Folder))
			{
				assetPath += "/";
			}

			foreach (var entry in localeConfigCache)
			{
				if (assetPath.Contains(entry.FolderRoot) && !string.IsNullOrWhiteSpace(entry.FolderRoot))
				{
					return entry.Id;
				}
			}

			return "";
		}

		/// <summary>
		///   Returns an instance name
		/// </summary>
		/// <param name="context">Current context</param>
		/// <returns>"https://cms.crownpeak.net/" + context.ClientName</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// //Requires PostSaveContext
		/// string instanceURL = ServicesTMF.GetInstanceURL(context);
		/// ]]></code>
		/// </example>
		public static string GetInstanceURL(PostSaveContext context)
		{
			var hostName = Util.GetHostName();
			hostName = "https://" + hostName + "/";
			var clientName = context.ClientName;
			var url = hostName + clientName;
			return url;
		}

		/// <summary>
		///   Checks if source folder exists in destination, creates it if it doesn't exist
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="sourceFolderRoot"></param>
		/// <param name="destinationFolderRoot"></param>
		private static void CheckAndCreateTransFolder(string folderPath, string sourceFolderRoot,
			string destinationFolderRoot)
		{
			var folders = folderPath.Replace(sourceFolderRoot, "");

			var folderArray = folders.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			var folderSourcePath = string.Empty;
			var folderDestPath = string.Empty;
			foreach (var szFolderName in folderArray)
			{
				folderSourcePath += (string.IsNullOrWhiteSpace(folderSourcePath) ? sourceFolderRoot : "") + "/" + szFolderName;
				folderDestPath = folderSourcePath.Replace(sourceFolderRoot, destinationFolderRoot);

				var folderDestAsset = Asset.Load(folderDestPath);
				if (!folderDestAsset.IsLoaded)
				{
					var parentFolderArray = folderDestPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					var parentFolderPath = string.Empty;
					var parentFolderCount = 0;
					foreach (var szParentFolderName in parentFolderArray)
					{
						if (parentFolderCount > parentFolderArray.Length - 2)
						{
							break;
						}

						parentFolderPath += "/" + szParentFolderName;

						parentFolderCount++;
					}

					Asset.CreateNewAsset(szFolderName, Asset.Load(parentFolderPath), Asset.Load(folderSourcePath),
						new Dictionary<string, string>());
				}
			}
		}

		/// <summary>
		///   Creates a translated asset and a relationship asset.
		/// </summary>
		/// <param name="contentSourceAsset">Source asset of the content to be translated.</param>
		/// <param name="sourceLanguageAsset">Asset of the source language.</param>
		/// <param name="destinationLanguageAsset">Asset of the destination language.</param>
		/// <param name="relationshipAssetId">
		///   ID of relationship asset if a relationship already exists between content source
		///   asset and a translated asset.
		/// </param>
		/// <param name="sitePath">Path of _TMF folder</param>
		/// <returns>ID of translated asset as a string</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// string transAssetId = ServicesTMF.CreateTranslatedAsset(contentSourceAsset, sourceLanguageAsset, destinationLanguageAsset, relAssetIdString /*if exists*/, sitePath);
		/// ]]></code>
		/// </example>
		public static string CreateTranslatedAsset(Asset contentSourceAsset, Asset sourceLanguageAsset,
			Asset destinationLanguageAsset, string relationshipAssetId, string sitePath)
		{
			//Replace the folder root of the contentSourceAsset with the folder root of the destinationLanguageAsset
			var szNewFolder = contentSourceAsset.Parent.AssetPath.ToString()
				.Replace(sourceLanguageAsset["folder_root"], destinationLanguageAsset["folder_root"]);

			//if the new folder doesn't exist
			if (!Asset.Load(szNewFolder).IsLoaded)
			{
				//if the contentSourceAsset's parent is a folder....not sure this is necessary
				if (Asset.Load(contentSourceAsset.Parent.AssetPath.ToString()).IsFolder)
				{
					//create the "newFolder"
					CheckAndCreateTransFolder(contentSourceAsset.Parent.AssetPath.ToString(), sourceLanguageAsset["folder_root"],
						destinationLanguageAsset["folder_root"]);
				}
				else
				{
					CheckAndCreateTransFolder(szNewFolder, sourceLanguageAsset["folder_root"],
						destinationLanguageAsset["folder_root"]);
				}
			}

			var aDestTmpContent = Asset.Load(szNewFolder + "/" + contentSourceAsset.Label);
			if (!aDestTmpContent.IsLoaded)
			{
				//if the translated asset doesn't exist, copy the contentSourceAsset
				aDestTmpContent = Asset.CopyAsset(contentSourceAsset.Label, Asset.Load(szNewFolder + "/"), contentSourceAsset);
			}

			Asset relationshipAsset;
			if (string.IsNullOrWhiteSpace(relationshipAssetId))
			{
				var dtContent = new Dictionary<string, string>();
				var szLabel = contentSourceAsset.Id + "-" + aDestTmpContent.Id;
				//create relationship asset
				Asset.CreateNewAsset(szLabel, Asset.Load(sitePath + Relationships_Config),
					Asset.Load("/System/Translation Model Framework/_Models/Relationship/Relationship"), dtContent);
				relationshipAsset = Asset.Load(sitePath + Relationships_Config + szLabel);
			}
			else
			{
				relationshipAsset = Asset.Load(relationshipAssetId);
			}

			//save source and destination ID's
			relationshipAsset.SaveContentField("source_id", contentSourceAsset.Id.ToString());
			relationshipAsset.SaveContentField("destination_id", aDestTmpContent.Id.ToString());

			var srcAssetContent = contentSourceAsset.GetContent();
			var relAssetContent = relationshipAsset.GetContent();
			foreach (var kvpSrc in srcAssetContent)
			{
				if (!string.IsNullOrWhiteSpace(kvpSrc.Key) && !string.IsNullOrWhiteSpace(kvpSrc.Value) &&
						!string.Equals(kvpSrc.Key.Substring(0, 1), "_", StringComparison.OrdinalIgnoreCase))
				{
					var newFieldName = "sourceOld_" + kvpSrc.Key;
					if (!relAssetContent.ContainsKey(newFieldName))
					{
						relAssetContent.Add(newFieldName, kvpSrc.Value);
					}
				}
			}

			relationshipAsset.SaveContent(relAssetContent);

			ClearTMFHelperValues(aDestTmpContent);

			return aDestTmpContent.Id.ToString();
		}

		/// <summary>
		///   Clears all field data that contains "tmf_" except for "tmf_folder_link_internal"
		/// </summary>
		/// <param name="contentToClear">Asset containing fields to be cleared.</param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.ClearTMFHelperValues(assetToClear);
		/// ]]></code>
		/// </example>
		public static void ClearTMFHelperValues(Asset contentToClear)
		{
			var fieldsToDelete = new List<string>();
			foreach (var kvpData in contentToClear.GetContent())
			{
				if (kvpData.Key.Contains("tmf_") &&
						!string.Equals(kvpData.Key, "tmf_folder_link_internal", StringComparison.OrdinalIgnoreCase) &&
						!fieldsToDelete.Contains(kvpData.Key))
				{
					fieldsToDelete.Add(kvpData.Key);
				}
			}

			if (fieldsToDelete.Count > 0)
			{
				contentToClear.DeleteContentFields(fieldsToDelete);
			}
		}

		/// <summary>
		///   Creates content of email, Sends email to content owner- post input
		/// </summary>
		/// <param name="contentSource"></param>
		/// <param name="contentDest">Asset that has a "Locales Config" asset with the same "folder_root".</param>
		/// <param name="languageContent"></param>
		/// <param name="instanceName"></param>
		/// <param name="context"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.SendNotification(contentSourceAsset, contentDestAsset, languageContentAsset, instanceName, context, sitePath);
		/// ]]></code>
		/// </example>
		public static void SendNotification(Asset contentSource, Asset contentDest, Asset languageContent,
			string instanceName, PostSaveContext context, string sitePath)
		{
			var localeConfigAsset = Asset.Load(GetLocaleId(contentDest, sitePath));
			var aSiteContent = Asset.Load(Asset.Load(localeConfigAsset["site_select"]).Id);
			var aLangContent = Asset.Load(Asset.Load(localeConfigAsset["lang_select"]).Id);

			var szDirectUrl = GetV3DeepLink(contentDest.Id, context);

			var usrInfo = User.Load(context.UserInfo.Username);
			var szCreateHeader = "Request sent by: " + usrInfo.Firstname + " " + usrInfo.Lastname + "\r\n" +
													 "Original Title: " + contentSource.Label + "\r\n" +
													 "Local Title: " + contentDest.Label + "\r\n" +
													 "Local Asset ID: " + contentDest.Id + "\r\n" +
													 "Url: " + szDirectUrl;

			var szTo = GetNotificationEmails(contentDest, sitePath);
			if (!string.IsNullOrWhiteSpace(szTo))
			{
				var szSubject = "CMS notification - Translation required for " + languageContent.Label + " site";
				var szFrom = "cms-" + instanceName + "@crownpeak.com";
				Util.Email(szSubject, szCreateHeader, szTo, szFrom);
			}
		}

		/// <summary>
		///   Decides who will receive email notifications
		/// </summary>
		/// <param name="contentSource"></param>
		/// <param name="contentDestId"></param>
		/// <param name="instanceName"></param>
		/// <param name="context"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.SendNotificationsToOwners(contentSourceAsset, contentDestIdString, instanceName, PostSaveContext, sitePath);
		/// ]]></code>
		/// </example>
		public static void SendNotificationsToOwners(Asset contentSource, string contentDestId, string instanceName,
			PostSaveContext context, string sitePath)
		{
			List<Asset> laConfigList;
			if (string.IsNullOrWhiteSpace(contentDestId))
			{
				laConfigList = GetRelList(contentSource.Id, "source", sitePath);
			}
			else
			{
				laConfigList = GetRelList(contentSource.Id, "source", sitePath, true);
			}

			foreach (var aConfig in laConfigList)
			{
				var aContentDest = Asset.Load(aConfig["destination_id"]);
				if (aContentDest.IsLoaded)
				{
					var aLocaleContent = Asset.Load(GetLocaleId(aContentDest, sitePath));
					var aSiteContent = Asset.Load(Asset.Load(aLocaleContent["site_select"]).Id);
					var aLangContent = Asset.Load(Asset.Load(aLocaleContent["lang_select"]).Id);

					var szDirectUrl = GetV3DeepLink(aContentDest.Id, context);
					
					var usrInfo = User.Load(context.UserInfo.Username);
					var szCreateHeader = "Request sent by: " + usrInfo.Firstname + " " + usrInfo.Lastname + "\r\n" +
															 "Original Title: " + contentSource.Label + "\r\n" +
															 "Translated Title: " + aContentDest.Label + "\r\n" +
															 "Translated Asset ID: " + aContentDest.Id + "\r\n" +
															 "Click here to edit translation asset: " + szDirectUrl;

					var szTo = GetNotificationEmails(aContentDest, sitePath);
					if (!string.IsNullOrWhiteSpace(szTo))
					{
						var szSubject = "CMS notification - Master Asset has been updated, changes may be required for " +
														aLocaleContent.Label + " site";
						var szFrom = "cms-" + instanceName + "@crownpeak.com";
						Util.Email(szSubject, szCreateHeader, szTo, szFrom);
					}
				}
			}
		}

		/// <summary>
		///   Decides who will receive email notifications
		/// </summary>
		/// <param name="contentSource"></param>
		/// <param name="notifyOriginalAuthor"></param>
		/// <param name="instanceName"></param>
		/// <param name="context"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.SendNotificationsToAuthorsAndOwners(contentSourceAsset, notifyOriginalAuthorString, instanceName, PostSaveContext, sitePath);
		/// ]]></code>
		/// </example>
		public static void SendNotificationsToAuthorsAndOwners(Asset transAsset, string instanceName,
			PostSaveContext context, string sitePath, bool notifyOriginalAuthor = false)
		{
			var aLocaleContent = Asset.Load(GetLocaleId(transAsset, sitePath));
			var aSiteContent = Asset.Load(Asset.Load(aLocaleContent["site_select"]).Id);
			var aLangContent = Asset.Load(Asset.Load(aLocaleContent["lang_select"]).Id);

			var nConfigAssetId = 0;
			foreach (var aConfig in GetRelList(transAsset.Id, "destination", sitePath, listLimit: 1))
			{
				nConfigAssetId = aConfig.Id;
			}

			var aOriginalContent = Asset.Load(nConfigAssetId);

			var szOriginalAuthorEmail = string.Empty;
			if (notifyOriginalAuthor)
			{
				if (!Equals(nConfigAssetId, 0))
				{
					var usrOriginalAuthor = User.Load(aOriginalContent.CreateUserId);
					szOriginalAuthorEmail = usrOriginalAuthor.Email;
				}
			}

			var nSourceId = 0;
			if (!string.IsNullOrWhiteSpace(aOriginalContent["source_id"]))
			{
				nSourceId = Convert.ToInt32(aOriginalContent["source_id"]);
			}

			foreach (var aConfig in GetRelList(nSourceId, "source", sitePath))
			{
				if (Asset.Load(aConfig["destination_id"]).IsLoaded)
				{
					var aContentDest = Asset.Load(aConfig["destination_id"]);
					var szLocaleIdDest = GetLocaleId(aContentDest, sitePath);

					var aLocaleContentDest = Asset.Load(szLocaleIdDest);
					var aSiteIdDest = Asset.Load(aLocaleContentDest["site_select"]);
					if (aSiteIdDest.IsLoaded && Equals(aContentDest.Id, transAsset.Id))
					{
						var usrAuthorUser = User.Load(aContentDest.CreateUserId);

						var szDirectUrl = GetV3DeepLink(aContentDest.Id, context);
						var szDirecturlSource = GetV3DeepLink(transAsset.Id, context);

						var usrInfo = User.Load(context.UserInfo.Username);
						var szCreateHeader = "Request sent by: " + usrInfo.Firstname + " " + usrInfo.Lastname + "\r\n" +
																 "Title: " + aContentDest.Label + "\r\n" +
																 "Translated Url in " + aLocaleContent["page_title"] + ":" + szDirecturlSource +
																 "\r\n" +
																 "Action might be required on the asset, url: " + szDirectUrl;

						var szTo = GetNotificationEmails(aContentDest, sitePath);

						if (!string.IsNullOrWhiteSpace(usrAuthorUser.Email))
						{
							szTo += "," + usrAuthorUser.Email;
						}

						if (!string.IsNullOrWhiteSpace(szOriginalAuthorEmail))
						{
							szTo += "," + szOriginalAuthorEmail;
							szOriginalAuthorEmail = string.Empty;
						}
						//TODO: submitted a ticket [RT: 29668] Part III


						if (string.Equals(szTo.Substring(0, 1), ","))
						{
							szTo = szTo.Substring(1, szTo.Length - 1);
						}

						if (!string.IsNullOrWhiteSpace(szTo))
						{
							var szSubject = "CMS notification - Local " + aLocaleContent["page_title"] +
															" translated asset has been updated";
							var szFrom = "cms-" + instanceName + "@crownpeak.com";
							Util.Email(szSubject, szCreateHeader, szTo, szFrom);
						}
					}
				}
			}
		}

		/// <summary>
		///   Saves previous content to a relationship asset
		/// </summary>
		/// <param name="sourceAsset">The Asset that has the source ID</param>
		/// <param name="sitePath">_TMF folder path</param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.UpdateRelationshipHistory( contentDestinationAsset, sitePath);
		/// ]]></code>
		/// </example>
		public static void UpdateRelationshipHistory(Asset sourceAsset, string sitePath)
		{
			//iterate through every relationship asset with the specified ID in its label
			foreach (var aConfig in GetRelList(sourceAsset.Id, "source", sitePath))
			{
				var aContentRelTMF = Asset.Load(aConfig.Id);
				var aContentSource = Asset.Load(aContentRelTMF["source_id"]);
				var content = new Dictionary<string, string>();
				foreach (var kvpContent in aContentSource.GetContent())
				{
					if (!string.Equals(kvpContent.Key.Substring(0, 1), "_"))
					{
						content.Add("sourceOld_" + kvpContent.Key, kvpContent.Value);
					}
				}

				if (content.Count > 0)
				{
					aContentRelTMF.SaveContent(content);
				}
			}
		}

		/// <summary>
		///   Given a translated asset, finds the source asset using an existing relationship asset, then saves the content from
		///   the source to the relationship asset.
		/// </summary>
		/// <param name="translatedAsset"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.UpdateMasterHistory(contentAsset, sitePath);
		/// ]]></code>
		/// </example>
		public static void UpdateMasterHistory(Asset translatedAsset, string sitePath)
		{
			//Find "Destination" relationship assets
			var laConfigList = GetRelList(translatedAsset.Id, "destination", sitePath);
			//Get ID of the first one if any exist
			var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : "";
			//Load the relationship asset
			var aChangesMasterContent = Asset.Load(szContentRelTMFId);
			if (aChangesMasterContent.IsLoaded)
			{
				//Load the source asset of the relationship asset
				var aMasterContent = Asset.Load(aChangesMasterContent["source_id"]);
				if (aMasterContent.IsLoaded)
				{
					var dtContent = new Dictionary<string, string>();
					//Iterate through all content fields of the source asset, skipping any keys that start with "_"
					foreach (var kvpContent in aMasterContent.GetContent())
					{
						if (!string.Equals(kvpContent.Key.Substring(0, 1), "_"))
						{
							dtContent.Add("sourceOld_" + kvpContent.Key, kvpContent.Value);
						}
					}

					dtContent.Add("updated_date_time", DateTime.Now.ToString("G"));
					//Save the updated field's to the relationship asset
					aChangesMasterContent.SaveContent(dtContent);
				}
			}
		}

		/// <summary>
		///   Fixes relative links in derived content
		/// </summary>
		/// <param name="contentDest">The asset with the links to fix.</param>
		/// <param name="sourceLanguageContent"></param>
		/// <param name="destLanguageContent"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.FixRelativeLinks(contentDestinationAsset, sourceLanguageAsset, destLanguageAsset, sitePath);
		/// ]]></code>
		/// </example>
		public static void FixRelativeLinks(Asset contentDest, Asset sourceLanguageContent, Asset destLanguageContent,
			string sitePath)
		{
			var szCurrId = contentDest.Id.ToString();

			foreach (var kvpData in contentDest.GetContent())
			{
				if (kvpData.Key.StartsWith("upload#") && !string.IsNullOrWhiteSpace(kvpData.Value))
				{
					//load the linked asset
					var aLinkedItem = Asset.Load(kvpData.Value);
					var linkedItemPath = aLinkedItem.AssetPath.ToString();
					var szDestinationPath =
						linkedItemPath.Replace(sourceLanguageContent["folder_root"], destLanguageContent["folder_root"]);

					var nFixLinkFlag = 0;
					if (Asset.Load(szDestinationPath).IsLoaded && !string.Equals(linkedItemPath, szDestinationPath))
					{
						nFixLinkFlag = 1;
					}
					else
					{
						var szDestinationId = string.Empty;
						//iterate through the relationship assets where the linked asset is the "source"
						foreach (var aConfig in GetRelList(aLinkedItem.Id, "source", sitePath))
						{
							//if the destination asset's path (from the relationship) contains the "folder_root" of destLanguageContent
							if (Asset.Load(aConfig["destination_id"]).AssetPath.ToString().ToLower()
								.Contains(destLanguageContent["folder_root"].ToLower()))
							{
								szDestinationId = aConfig["destination_id"];
								break;
							}
						}

						if (!string.IsNullOrWhiteSpace(szDestinationId))
						{
							szDestinationPath = szDestinationId;
							nFixLinkFlag = 1;
						}
						else
						{
							nFixLinkFlag = 0;
						}
					}

					if (Equals(nFixLinkFlag, 1))
					{
						var aCurrId = Asset.Load(szCurrId);
						aCurrId.DeleteContentField(kvpData.Key);

						var aItemDest = Asset.Load(szDestinationPath);
						aCurrId.SaveContentField(kvpData.Key, "/cpt_internal/" + aItemDest.Id);
					}
				}
			}
		}

		/// <summary>
		///   Loops through relation config folder and creates list of relationship assets
		/// </summary>
		/// <param name="id">Asset ID.</param>
		/// <param name="relationshipType">
		///   Specifies if the nId Asset has a "source" or "destination" relationship with another
		///   asset.
		/// </param>
		/// <param name="sitePath">_TMF folder path.</param>
		/// <param name="addDestinationId">Adds nId as a "destination" as well, even if it is a "source".</param>
		/// <param name="listLimit">Sets a limit to the number of assets in the list.</param>
		/// <returns>Filtered list of assets.</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// List<Asset> relationshipList = ServicesTMF.GetRelList(id, "source", sitePath);
		/// ]]></code>
		/// </example>
		public static List<Asset> GetRelList(int id, string relationshipType, string sitePath,
			bool addDestinationId = false, int listLimit = 500)
		{
			var fpFilter = new FilterParams();

			if (relationshipType.Equals("source", StringComparison.OrdinalIgnoreCase))
			{
				fpFilter.Add(AssetPropertyNames.Label, Comparison.StartsWith, id + "-");
			}
			else
			{
				fpFilter.Add(AssetPropertyNames.Label, Comparison.EndsWith, "-" + id);
			}

			if (addDestinationId)
			{
				fpFilter.Add(AssetPropertyNames.Label, Comparison.EndsWith, "-" + id);
			}

			fpFilter.Limit = listLimit;

			return Asset.Load(sitePath + Relationships_Config).GetFilterList(fpFilter);
		}

		/// <summary>
		///   Returns comma delimited list of email addresses from local, language and site configs for a page
		/// </summary>
		/// <param name="asset">Asset that has owner(s)</param>
		/// <param name="sitePath">_TMF folder path</param>
		/// <returns>Comma delimited string</returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// string notifEmails = ServicesTMF.GetNotificationEmails( asset,  sitePath);
		/// ]]></code>
		/// </example>
		public static string GetNotificationEmails(Asset asset, string sitePath)
		{
			var szEmails = string.Empty;

			if (asset.IsLoaded)
			{
				var aLocaleContent = Asset.Load(GetLocaleId(asset, sitePath));
				if (aLocaleContent.IsLoaded)
				{
					var aSiteContent = Asset.Load(Asset.Load(aLocaleContent["site_select"]).Id);
					var aLangContent = Asset.Load(Asset.Load(aLocaleContent["lang_select"]).Id);

					foreach (var peLocale in aLocaleContent.GetPanels("site_owner_list"))
					{
						int userId;
						var userisValid = int.TryParse(peLocale["site_owner_list"], out userId);
						if (userisValid)
						{
							var usrLocal = User.Load(userId);
							if (!szEmails.Contains(usrLocal.Email))
							{
								szEmails += usrLocal.Email + ",";
							}
						}
					}

					foreach (var peSite in aSiteContent.GetPanels("site_owner_list"))
					{
						int userId;
						var userisValid = int.TryParse(peSite["site_owner_list"], out userId);
						if (userisValid)
						{
							var usrSite = User.Load(Convert.ToInt32(peSite["site_owner_list"]));
							if (!szEmails.Contains(usrSite.Email))
							{
								szEmails += usrSite.Email + ",";
							}
						}
					}

					foreach (var peLang in aLangContent.GetPanels("site_owner_list"))
					{
						int userId;
						var userisValid = int.TryParse(peLang["site_owner_list"], out userId);
						if (userisValid)
						{
							var usrLang = User.Load(userId);
							if (!szEmails.Contains(usrLang.Email))
							{
								szEmails += usrLang.Email + ",";
							}
						}
					}

					if (!string.IsNullOrWhiteSpace(szEmails))
					{
						szEmails = szEmails.Substring(0, szEmails.Length - 1);
					}
				}
			}

			return szEmails;
		}

		/// <summary>
		///   When one of the assets in the master folder is branched, the users action is captured and the derived asset is also
		///   branched if a relationship exists.
		///   This method should be used in the copy.aspx template file.
		///   <remarks>Requires /System/Translation Model Framework/_Models/Relationship/ </remarks>
		/// </summary>
		/// <param name="asset">Asset that is branched (Current asset)</param>
		/// <param name="context"></param>
		/// <param name="sitePath"></param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.UpdateBranch(asset, PostSaveContext, sitePath);
		/// ]]></code>
		/// </example>
		public static void UpdateBranch(Asset asset, PostSaveContext context, string sitePath)
		{
			if (string.Equals(asset["_task"], "branch", StringComparison.OrdinalIgnoreCase))
			{
				var fpFilter = new FilterParams();
				fpFilter.Add(AssetPropertyNames.Label, Comparison.Equals, asset.Label);
				fpFilter.Add(AssetPropertyNames.Id, Comparison.NotEquals, asset.Id);
				fpFilter.Add(Comparison.Equals, AssetType.File);
				//Get a list of assets that have the same lable but different ID (branch of the asset)
				var laContentParent = Asset.Load(asset.FolderId).GetFilterList(fpFilter);
				if (laContentParent.Count > 0)
				{
					var branchedAsset = Asset.Load(laContentParent[0].Id);
					fpFilter = new FilterParams();
					fpFilter.Add("source_id", Comparison.Equals, branchedAsset.Id);
					//Find the relationship asset that has the branched asset as the "source_id"
					var laConfigList = Asset.Load(sitePath + Relationships_Config).GetFilterList(fpFilter);
					foreach (var relAsset in laConfigList)
					{
						//Load the "destination" asset from the rel asst
						var destinationAsset = Asset.Load(relAsset["destination_id"]);
						if (destinationAsset.IsLoaded)
						{
							//Add a field to the destination asset: tmf_masterbranch_id : currAst.Id
							destinationAsset.SaveContentField("tmf_masterbranch_id", asset.Id.ToString());

							var destinationAssetBranch = Asset.Load(destinationAsset.BranchId); //ASSUMES A BRANCH EXISTS
							if (destinationAssetBranch.IsLoaded)
							{
								var sourceAssetBranch = Asset.Load(destinationAssetBranch["tmf_masterbranch_id"]);
								//Create a relationship asset between the source asset branch and destination asset branch
								Asset.CreateNewAsset(sourceAssetBranch.Id + "-" + destinationAssetBranch.Id,
									Asset.Load(sitePath + Relationships_Config),
									Asset.Load("/System/Translation Model Framework/_Models/Relationship/"),
									new Dictionary<string, string>());
								var branchRelationshipAsset =
									Asset.Load(sitePath + Relationships_Config + sourceAssetBranch.Id + "-" + destinationAssetBranch.Id);
								if (branchRelationshipAsset.IsLoaded)
								{
									var dtTMFTmpContent = new Dictionary<string, string>();
									dtTMFTmpContent.Add("source_id", destinationAssetBranch["tmf_masterbranch_id"]);
									dtTMFTmpContent.Add("destination_id", destinationAssetBranch.Id.ToString());
									dtTMFTmpContent.Add("filter_string",
										"|" + destinationAssetBranch["tmf_masterbranch_id"] + "|" + destinationAssetBranch.Id + "|");

									branchRelationshipAsset.SaveContent(dtTMFTmpContent);

									UpdateRelationshipHistory(sourceAssetBranch, sitePath);
									SendNotificationsToOwners(sourceAssetBranch, destinationAssetBranch.Id.ToString(), context.ClientName,
										context, sitePath);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		///   Automatically creates relationships between source and destination
		/// </summary>
		/// <param name="contentSource">Source Asset</param>
		/// <param name="contentDest">Destination Asset</param>
		/// <param name="sitePath">_TMF Folder path</param>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// ServicesTMF.AutoLinkLocales(contentSourceAsset, contentDestinationAsset, sitePath);
		/// ]]></code>
		/// </example>
		public static void AutoLinkLocales(Asset contentSource, Asset contentDest, string sitePath)
		{
			if (!Asset.Load(sitePath + Relationships_Config + contentSource.Id + "-" + contentDest.Id).IsLoaded)
			{
				var aRelationship = Asset.CreateNewAsset(contentSource.Id + "-" + contentDest.Id,
					Asset.Load(sitePath + Relationships_Config),
					Asset.Load("/System/Translation Model Framework/_Models/Relationship/Relationship"),
					new Dictionary<string, string>());

				var dtTmpContent = new Dictionary<string, string>();
				dtTmpContent.Add("source_id", contentSource.Id.ToString());
				dtTmpContent.Add("destination_id", contentDest.Id.ToString());
				dtTmpContent.Add("source_last_modified_date", contentSource.ModifiedDate.ToString());
				dtTmpContent.Add("filter_string", "|" + contentSource.Id + "|" + contentDest.Id + "|");

				foreach (var kvpData in contentSource.GetContent())
				{
					if (!string.Equals(kvpData.Key.Substring(0, 1), "_", StringComparison.OrdinalIgnoreCase))
					{
						dtTmpContent.Add("sourceOld_" + kvpData.Key, kvpData.Value);
					}
				}

				aRelationship.SaveContent(dtTmpContent);
			}
		}

		/// <summary>
		///   Returns list of tmf related fields
		/// </summary>
		/// <param name="currentContent"></param>
		/// <returns></returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// Dictionary<string, string> tmfFields = ServicesTMF.GetTMFTemplateFields(asset);
		/// ]]></code>
		/// </example>
		public static Dictionary<string, string> GetTMFTemplateFields(Asset currentContent)
		{
			var dtReturn = new Dictionary<string, string>();
			dtReturn.Add("_tmf_process", "false");

			var szTemplateID = currentContent.TemplateId.ToString();

			var aTemplConfig = Asset.Load("/System/Translation Model Framework/Global TMF config/Templates configuration");
			var szDefaultLogic = string.Empty;
			szDefaultLogic = aTemplConfig["tmf_default_logic"];
			var bProcessTemplate = false;
			if (string.IsNullOrWhiteSpace(szDefaultLogic))
			{
				szDefaultLogic = "include";
			}

			if (string.Equals(szDefaultLogic, "include", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrWhiteSpace(aTemplConfig["tmftemplate_" + szTemplateID]))
				{
					bProcessTemplate = true;
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(aTemplConfig["tmftemplate_" + szTemplateID]))
				{
					bProcessTemplate = true;
				}
			}

			if (bProcessTemplate)
			{
				dtReturn["_tmf_process"] = "true";

				foreach (var kvpField in currentContent.GetContent())
				{
					var nShowItem = 1;

					var szKey = kvpField.Key;
					var bField = true;
					if (szKey.Length >= 7)
					{
						if (string.Equals(szKey.Substring(0, 7), "upload#", StringComparison.OrdinalIgnoreCase))
						{
							bField = false;
						}
					}

					if (szKey.Length >= 4)
					{
						if (string.Equals(szKey.Substring(0, 4), "tmf_", StringComparison.OrdinalIgnoreCase))
						{
							bField = false;
						}
					}

					if (string.Equals(szKey, "id", StringComparison.OrdinalIgnoreCase) ||
							string.Equals(szKey, "show_tmf", StringComparison.OrdinalIgnoreCase) ||
							string.Equals(szKey, "duplicate_id", StringComparison.OrdinalIgnoreCase))
					{
						bField = false;
					}

					if (!bField)
					{
						nShowItem = 0;
					}
					else
					{
						var szFieldName = szKey;
						if (szFieldName.Contains(":"))
						{
							szFieldName = szFieldName.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
						}

						if (string.Equals(szDefaultLogic, "include", StringComparison.OrdinalIgnoreCase))
						{
							if (!string.IsNullOrWhiteSpace(aTemplConfig["tmffield_" + szTemplateID + "_" + szFieldName]))
							{
								nShowItem = 0;
							}
						}
						else
						{
							if (string.IsNullOrWhiteSpace(aTemplConfig["tmffield_" + szTemplateID + "_" + szFieldName]))
							{
								nShowItem = 0;
							}
						}
					}

					if (Equals(nShowItem, 1))
					{
						if (!dtReturn.ContainsKey(szKey))
						{
							dtReturn.Add(szKey, currentContent[szKey]);
						}
					}
				}


				if (dtReturn.Count > 0)
				{
					var listReturn = dtReturn.Keys.ToList();
					listReturn.Sort();

					var dtTempReturn = new Dictionary<string, string>();
					foreach (var szKey in listReturn)
					{
						dtTempReturn.Add(szKey, dtReturn[szKey]);
					}

					dtReturn.Clear();
					dtReturn = dtTempReturn;
				}
			}

			return dtReturn;
		}

		/// <summary>
		///   Checks if the Master Asset has been changed.
		/// </summary>
		/// <param name="asset"></param>
		/// <param name="sitePath"></param>
		/// <returns></returns>
		/// <example>
		///   <code lang="C#"><![CDATA[
		/// bool isChanged = ServicesTMF.IsMasterAssetChanges(asset, sitePath);
		/// ]]></code>
		/// </example>
		public static bool IsMasterAssetChanges(Asset asset, string sitePath)
		{
			var bRet = false;

			var dtChangesSourceContent = GetTMFTemplateFields(asset);
			if (string.Equals(dtChangesSourceContent["_tmf_process"], "true", StringComparison.OrdinalIgnoreCase))
			{
				var szSourceLanguageId = string.Empty;
				var laLocales = Asset.Load(sitePath + Locales_Config).GetFileList();
				foreach (var aLang in laLocales)
				{
					if (asset.AssetPath.ToString().ToLower()
						.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
					{
						szSourceLanguageId = aLang.Id.ToString();
						break;
					}
				}

				var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
				Asset aChangesMasterContent = null;
				if (aSourceLanguageContent.IsLoaded)
				{
					var laConfigList = GetRelList(asset.Id, "destination", sitePath);
					var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : string.Empty;

					aChangesMasterContent = Asset.Load(szContentRelTMFId);
				}

				var szMasterLanguageId = string.Empty;
				foreach (var aLang in laLocales)
				{
					if (Asset.Load(aChangesMasterContent.Raw["source_id"]).AssetPath.ToString().ToLower()
						.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
					{
						szMasterLanguageId = aLang.Id.ToString();
						break;
					}
				}

				var aMasterLanguage = Asset.Load(szMasterLanguageId);
				if (aMasterLanguage.IsLoaded)
				{
					foreach (var kvpChangeSourceContent in dtChangesSourceContent)
					{
						var szMasterValue = Asset.Load(aChangesMasterContent.Raw["source_id"]).Raw[kvpChangeSourceContent.Key]
							.Trim();
						var szOldValue = aChangesMasterContent.Raw["sourceOld_" + kvpChangeSourceContent.Key].Trim();

						if (!kvpChangeSourceContent.Key.Contains("tmf_") &&
								!string.Equals(kvpChangeSourceContent.Key.Substring(0, 1), "_", StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(kvpChangeSourceContent.Key, "show_tmf", StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(kvpChangeSourceContent.Key, "id", StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(kvpChangeSourceContent.Key, "duplicate_id", StringComparison.OrdinalIgnoreCase))
						{
							if (!string.Equals(szOldValue, szMasterValue, StringComparison.OrdinalIgnoreCase))
							{
								bRet = true;
							}
						}
					}
				}
			}

			return bRet;
		}

		/// <summary>
		///   Prepares and formats a deep link to edit an asset in V3
		/// </summary>
		/// <param name="assetId">Asset id</param>
		/// <param name="context">Instance context</param>
		/// <returns>A string containing the URL to the asset edit view in V3 for a specific instance</returns>
		public static string GetV3DeepLink(int assetId, PostSaveContext context)
		{
			var asset = Asset.Load(assetId);
			return string.Format("{0}/#/content;folder={1};asset={2};viewAsset={2};view=Edit;tab={2}",
				GetInstanceURL(context), asset.FolderId, asset.Id);
		}

		/// <summary>
		/// Removes duplicate properties resulting from the way TMF content is processed.
		///
		/// Example:
		/// 
		/// "choice_block_list_panel_blocks_related_content_item_panel_links_list_linkaddress_link_internal"
		///
		/// vs.
		/// 
		/// "upload#choice_block_list_panel_blocks_related_content_item_panel_links_list_linkaddress_link_internal"
		///
		/// The TMF processing results in non-upload# prefixed property names which must be removed.
		/// Based on the complexity of TMF processing and the number of operations and paths involved in generating the updated asset
		/// content, it was deemed easiest to simply check for and eliminate the duplicates in this manner at the conclusion of the
		/// processing.
		/// </summary>
		/// <param name="content">The list of properties to check.</param>
		/// <returns>Filtered list of properties</returns>
		public static Dictionary<string, string> RemoveTmfDuplicates(Dictionary<string, string> content)
		{
			var toRemove = new List<string>();
			foreach (var entry in content)
			{
				// Keys in question will ALWAYS be prefixed with "upload#".  Check to see if this is one of them.
				// What we want to do here is remove any key/value for which there is a duplicate non-prefixed
				// entry with the same value.
				if (entry.Key.StartsWith(UploadPrefix))
				{
					// Get the value because we want to use it to test for a complete match.
					var value = content[entry.Key];

					// Remove the "upload#" prefix from the key by using substring to skip past the prefix to the end of the key.
					string strippedKey = entry.Key.Substring(UploadPrefixLen);

					// If we have an unwanted duplicate of both the the unprefixed key and it's value, add the key to a list for removal.
					if (content.ContainsKey(strippedKey) && content[strippedKey] != null && content[strippedKey].Equals(value))
					{
						toRemove.Add(strippedKey);
					}
				}
			}

			// Remove any unwanted duplicates
			foreach (var key in toRemove)
			{
				content.Remove(key);
			}

			return content;
		}

		/// <summary>
		///   PostInput implementation for TMF
		/// </summary>
		public class PostInput : TMF
		{
			/// <summary>
			///   Added to post input template file in order to enable TMF
			/// </summary>
			/// <param name="asset">Current Asset</param>
			/// <param name="postInputContext">Current context</param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.PostInput.LoadPostInput(asset, postInputContext);
			/// ]]></code>
			/// </example>
			public static void LoadPostInput(Asset asset, PostInputContext postInputContext)
			{
				//reserved for future.
			}
			
			public static void LoadProjectPostInput(Asset asset, PostInputContext context)
			{
				var createOnSave = context.InputForm["create_project"] == "true";
				if (createOnSave)
				{
					var languagePanels = asset.GetPanels("tmf_language_selected");
					var selectedLanguages = languagePanels.Select(p => p.Raw["tmf_language_selected"]).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
					if (!selectedLanguages.Any())
					{
						context.ValidationErrorFields.Add(languagePanels.First().GetFieldName("tmf_language_selected"), "Please select at least one target language.");
					}

					var foundAsset = false;
					var errorField = "";
					foreach (var panel in context.InputForm.GetPanels("select_assets"))
					{
						if (errorField == "") errorField = panel.GetFieldName("selected_type");
						if ((panel["selected_type"] == "File" && !string.IsNullOrWhiteSpace(panel["selected_file"]))
						    || (panel["selected_type"] == "Folder" && !string.IsNullOrWhiteSpace(panel["selected_folder"])))
						{
							foundAsset = true;
							break;
						}
					}
					if (!foundAsset)
					{
						context.ValidationErrorFields.Add(errorField, "Please select at least one file or folder.");
					}

					// Check all assets are in the same locale
					var tmfPath = TMF.GetSitePath(asset).AssetPath.ToString();
					var locales = TMF.CreateLocaleConfigCache(tmfPath);
					var selected = context.InputForm.GetPanels("select_assets").Select(p => p["selected_type"] == "File" ? p["selected_file"] : p["selected_folder"]);
					var selectedLocales = selected.GroupBy(a => TMF.GetLocaleId(Asset.Load(a), tmfPath, locales)).ToArray();
					if (selectedLocales.Length != 1)
					{
						context.ValidationErrorFields.Add(context.InputForm.GetPanels("select_assets").First().GetFieldName("selected_type"), "All selected assets must be in the same locale.");
					}
				}

				var translator = Translation.GetTmfTranslator(asset);
				translator.ProjectPostInput(asset, context);
			}
		}

		public class Input : TMF
		{
			/// <summary>
			///   Added to input template file in order to enable TMF
			/// </summary>
			/// <param name="asset">Current Asset</param>
			/// <param name="context">Current context</param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Input.LoadInput(asset, inputContext);
			/// ]]></code>
			/// </example>
			public static void LoadInput(Asset asset, InputContext context)
			{
				var aTMFConfig = GetSitePath(asset);
				if (aTMFConfig != null)
				{
					var szSitePath = GetSitePath(asset).AssetPath.ToString();

					var aContentSource = Asset.Load(asset.Id);
					//Asset aTMFConfig = Asset.Load("/System/Translation Model Framework/Global TMF config/Global TMF config");
					var szSourceLanguageId = string.Empty;

					var localeConfigCache = CreateLocaleConfigCache(szSitePath);
					//Out.DebugWriteLine(szSitePath + "/Locales Config/");
					var laLocalesList = Asset.Load(szSitePath + Locales_Config).GetFileList();
					//Out.DebugWriteLine("asset.AssetPath.ToString(): " + asset.AssetPath.ToString());
					foreach (var aLang in laLocalesList)
					{
						//Out.DebugWriteLine("folder_root: " + aLang.Raw["folder_root"]);
						if (asset.AssetPath.ToString().ToLower()
							.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
						{
							szSourceLanguageId = aLang.Id.ToString();
						}
					}

					var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
					if (aSourceLanguageContent.IsLoaded)
					{
						CrownPeak.CMSAPI.Input.StartControlPanel("Translation Model Framework");
						CrownPeak.CMSAPI.Input.ShowMessage(aSourceLanguageContent.Label);
						CrownPeak.CMSAPI.Input.AddHiddenField("show_tmf", "0");

						var lsDeleteFields = new List<string>();
						foreach (var kvpData in asset.GetContent())
						{
							if (kvpData.Key.Contains("tmf_") &&
									!string.Equals(kvpData.Key.Substring(0, 1), "_", StringComparison.OrdinalIgnoreCase))
							{
								if (!string.Equals(kvpData.Key, "tmf_folder_link_internal", StringComparison.OrdinalIgnoreCase))
								{
									lsDeleteFields.Add(kvpData.Key);
								}
							}
						}

						if (lsDeleteFields.Count > 0)
						{
							asset.DeleteContentFields(lsDeleteFields);
						}

						asset.DeleteContentFields(new List<string> { "tmf_related_link_internal", "upload#tmf_related_link_internal" });

						var laConfigList = GetRelList(asset.Id, "destination", szSitePath);

						var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : "";
						var szContentRel = szContentRelTMFId;
						var szContentRelTMFDest = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : "";

						var lsTitles = new List<string>();
						lsTitles.Add("Send for Translation");
						//lsTitles.Add("Send folder(s) for Translation");
						lsTitles.Add("Manage relationships");
						if (!string.IsNullOrWhiteSpace(szContentRelTMFDest))
						{
							lsTitles.Add("Refresh Translation");
						}

						CrownPeak.CMSAPI.Input.StartTabbedPanel(lsTitles);

						var laConfigListContentSource = GetRelList(aContentSource.Id, "source", szSitePath);

						var nIndex = 1;

						if (context.UIType != UIType.V3)
							CrownPeak.CMSAPI.Input.ShowHeader("<h3>Send for Translation</h3>");
						else
							CrownPeak.CMSAPI.Input.ShowHeader("Send for Translation");

						foreach (var aRule in laLocalesList)
						{
							if (!string.Equals(aRule.Id.ToString(), szSourceLanguageId))
							{
								var aDestLanguageContent = Asset.Load(aRule.Id);

								szContentRelTMFId = string.Empty;

								foreach (var aConfig in laConfigListContentSource)
								{
									var aContentTransSource = Asset.Load(aConfig["destination_id"]);
									var szLocaleId = GetLocaleId(aContentTransSource, szSitePath, localeConfigCache);
									if (string.Equals(szLocaleId, aDestLanguageContent.Id.ToString()))
									{
										szContentRelTMFId = aConfig.Id.ToString();
										break;
									}
								}

								var nDestinationId = 0;
								if (!string.IsNullOrWhiteSpace(szContentRelTMFId))
								{
									var szDestinationId = Asset.Load(szContentRelTMFId)["destination_id"];
									nDestinationId = Convert.ToInt32(szDestinationId);
								}

								var bExist = false;
								if (string.Equals(aSourceLanguageContent.Raw["use_translation_rule"], "y",
									StringComparison.OrdinalIgnoreCase))
								{
									foreach (var peLocale in aSourceLanguageContent.GetPanels("translation_rule_panel"))
									{
										if (!string.IsNullOrWhiteSpace(peLocale.Raw["translation_rule_dest_locale"]))
										{
											if (string.Equals(peLocale.Raw["translation_rule_dest_locale"],
												aDestLanguageContent.Id.ToString()))
											{
												bExist = true;
											}
										}
									}
								}
								else
								{
									bExist = true;
								}

								if (bExist)
								{
									var bDest = false;
									if (Equals(nDestinationId, 0))
									{
										var szDest = Asset.Load(aDestLanguageContent.Raw["folder_root"]).AssetPath.ToString();
										for (var nFolderIndex = Asset.Load(aDestLanguageContent.Raw["folder_root"]).AssetPath.Count;
											nFolderIndex < asset.Parent.AssetPath.Count;
											nFolderIndex++)
										{
											szDest += "/" + asset.AssetPath[nFolderIndex];
										}

										szDest += "/" + asset.Label;

										var aDest = Asset.Load(szDest);
										if (aDest.IsLoaded)
										{
											bDest = true;
										}
									}

									CrownPeak.CMSAPI.Input.AddHiddenField("tmf_language_selected_panel:" + nIndex,
										(nIndex - 1).ToString());
									//if (!int.Equals(nDestinationId, 0) || bDest)
									CrownPeak.CMSAPI.Input.StartControlPanel(aDestLanguageContent.Label);

									var szHelpMsg = string.Empty;
									if (!Equals(nDestinationId, 0))
									{
										szHelpMsg = "Note: This checkbox needs to be checked to re-send for translation";
									}

									CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_language_selected:" + nIndex,
										aDestLanguageContent.Id.ToString(),
										aDestLanguageContent.Label, szHelpMsg);
									if (Equals(nDestinationId, 0))
									{
										//CMSAPI.Input.ShowMessage(asset.Parent.AssetPath.Count.ToString());
										//Input.ShowMessage(Asset.Load(aDestLanguageContent.Raw["folder_root"]).AssetPath.Count.ToString());
										var szDest = Asset.Load(aDestLanguageContent.Raw["folder_root"]).AssetPath.ToString();
										for (var nFolderIndex = Asset.Load(aDestLanguageContent.Raw["folder_root"]).AssetPath.Count;
											nFolderIndex < asset.Parent.AssetPath.Count;
											nFolderIndex++)
										{
											szDest += "/" + asset.AssetPath[nFolderIndex];
										}

										szDest += "/" + asset.Label;

										var aDest = Asset.Load(szDest);
										if (aDest.IsLoaded)
										{
											CrownPeak.CMSAPI.Input.ShowMessage("'" + aDest.AssetPath + "' exists without relationship.");
											CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_overwrite_existing_asset:" + nIndex, "y",
												"Create Relationship and Overwrite Existing Asset");
										}

										//CMSAPI.Input.ShowMessage(szTempFolder);
										//CMSAPI.Input.ShowMessage(asset.Label + " : " + aDestLanguageContent.Label + " : " + aDestLanguageContent.AssetPath.ToString());
									}

									if (!Equals(nDestinationId, 0))
									{
										szHelpMsg =
											"Note: This will overwrite when 'Translate Current Asset Only' is selected in option below and the checkbox above is checked.";
										CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_overwrite_existing_asset:" + nIndex, "y",
											"Overwrite Existing Translation Asset", szHelpMsg);
										CrownPeak.CMSAPI.Input.ShowLink(nDestinationId,
											"Existing Translation: " + Asset.Load(nDestinationId).Label + " (" + nDestinationId + ")",
											InputLinkType.EditTab);
									}

									if (!string.IsNullOrWhiteSpace(aDestLanguageContent.Raw["locale_code"]))
										TMF.Translation.GetTmfTranslator(asset).TmfInput(asset, context, nIndex, !Equals(nDestinationId, 0));

									//if (!int.Equals(nDestinationId, 0) || bDest)
									CrownPeak.CMSAPI.Input.EndControlPanel();

									nIndex++;
								}
							}
						}

						CrownPeak.CMSAPI.Input.StartDropDownContainer("Option", "tmf_selection_type",
							new Dictionary<string, string>
							{
								{"Translate Current Asset Only", "0"},
								{"Translate Multiple Folder(s)", "1"}
							}, "0");
						CrownPeak.CMSAPI.Input.NextDropDownContainer();

						if (context.UIType != UIType.V3)
							CrownPeak.CMSAPI.Input.ShowHeader("<h3>Send folder(s) for Translation</h3>");
						else
							CrownPeak.CMSAPI.Input.ShowHeader("Send folder(s) for Translation");
						if (nIndex > 1)
						{
							CrownPeak.CMSAPI.Input.StartControlPanel("Folder(s)");
							var szTMFConfigFolderPath = GetSitePath(asset).AssetPath.ToString();
							if (!string.IsNullOrWhiteSpace(szTMFConfigFolderPath))
							{
								var szTempPath = string.Empty;
								var arrTMFConfig = szTMFConfigFolderPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
								for (var nTMFIndex = 0; nTMFIndex < arrTMFConfig.Length; nTMFIndex++)
								{
									szTempPath += "/" + asset.AssetPath[nTMFIndex];
								}

								if (!string.IsNullOrWhiteSpace(szTempPath))
								{
									foreach (var aFolder in Asset.Load(szTempPath)
										.GetFolderList(SortOrder.OrderBy(AssetPropertyNames.Label)))
									{
										var fpFilter = new FilterParams();
										fpFilter.Add(Comparison.Equals, AssetType.Folder);
										var laSubFolders = aFolder.GetFilterList(fpFilter);

										CrownPeak.CMSAPI.Input.StartControlPanel(aFolder.Label);

										CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_folder_" + aFolder.Id, "y",
											aFolder.AssetPath.ToString());

										if (laSubFolders.Count > 0)
										{
											foreach (var aSubFolder in laSubFolders)
											{
												CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_folder_" + aSubFolder.Id, "y",
													aSubFolder.AssetPath.ToString());
											}
										}

										CrownPeak.CMSAPI.Input.EndControlPanel();
									}

									CrownPeak.CMSAPI.Input.ShowMessage("This operation will not overwrite any existing file.");
								}
							}
							else
							{
								CrownPeak.CMSAPI.Input.ShowMessage("TMF configuration files do not exist.");
							}

							CrownPeak.CMSAPI.Input.EndControlPanel();

							//string szMsg = "Send entire ";
							//if (!string.IsNullOrWhiteSpace(asset["tmf_folder_link_internal"]))
							//    szMsg += Asset.Load(asset["tmf_folder_link_internal"]).AssetPath.ToString() + " ";

							//szMsg += "folder for translation or select another folder:";

							//Input.ShowCheckBox("", "tmf_process_folder", "y", szMsg);
							//Input.ShowSelectFolder("", "tmf_folder_link_internal");
						}
						else
						{
							CrownPeak.CMSAPI.Input.ShowMessage("There is no configuration for your Locale.");
						}

						CrownPeak.CMSAPI.Input.EndDropDownContainer();


						CrownPeak.CMSAPI.Input.NextTabbedPanel();

						if (context.UIType != UIType.V3)
							CrownPeak.CMSAPI.Input.ShowHeader("<h3>Manage relationships</h3>");
						else
							CrownPeak.CMSAPI.Input.ShowHeader("Manage relationships");
						CrownPeak.CMSAPI.Input.ShowMessage(
							"Please use this section only if you are creating manual relationships between content.");

						var dtRelationshipType = new Dictionary<string, string>();
						dtRelationshipType.Add("Select Master asset (current Content becomes Derived)", "1");
						dtRelationshipType.Add("Select Derived asset (current Content becomes Master)", "2");
						CrownPeak.CMSAPI.Input.ShowRadioButton("", "tmf_relationship_type", dtRelationshipType, "2");

						var sapDoc = new ShowAcquireParams();
						sapDoc.ShowBrowse = true;
						sapDoc.ShowUpload = true;
						CrownPeak.CMSAPI.Input.ShowAcquireDocument("", "tmf_related_link_internal", sapDoc);

						if (!string.IsNullOrWhiteSpace(szContentRelTMFDest))
						{
							CrownPeak.CMSAPI.Input.NextTabbedPanel();
							if (context.UIType != UIType.V3)
								CrownPeak.CMSAPI.Input.ShowHeader("<h3>Refresh Translation</h3>");
							else
								CrownPeak.CMSAPI.Input.ShowHeader("Refresh Translation");
							if (IsMasterAssetChanges(asset, szSitePath))
							{
								CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_confirm_translation", "y",
									"Update master(" + aSourceLanguageContent.Label + ") changes");
							}
							else
							{
								var szUpdatedTime = "N/A";
								var aContentRel = Asset.Load(szContentRel);
								if (aContentRel.IsLoaded)
								{
									if (!string.IsNullOrWhiteSpace(aContentRel["updated_date_time"]))
									{
										szUpdatedTime = aContentRel["updated_date_time"];
									}
								}

								CrownPeak.CMSAPI.Input.ShowMessage("Updated on " + szUpdatedTime);
							}
						}

						CrownPeak.CMSAPI.Input.EndTabbedPanel();

						CrownPeak.CMSAPI.Input.EndControlPanel();
					}
				}
			}

			public static void LoadProjectInput(Asset asset, InputContext context)
			{
				CrownPeak.CMSAPI.Input.StartControlPanel(asset.Label);

				CrownPeak.CMSAPI.Input.StartTabbedPanel("Project Details", "Assets");
				CrownPeak.CMSAPI.Input.ShowTextBox("Project Name", "name", requiredField: true);

				var tmfPath = TMF.GetSitePath(asset).AssetPath.ToString();
				if (string.IsNullOrWhiteSpace(tmfPath))
				{
					CrownPeak.CMSAPI.Input.ShowMessage("Unable to locate TMF", MessageType.Warning);
					CrownPeak.CMSAPI.Input.NextTabbedPanel();
					CrownPeak.CMSAPI.Input.EndTabbedPanel();
				}
				else
				{
					var thisLanguage = TMF.GetLocaleId(asset, tmfPath);
					var allLanguages = TMF.CreateLocaleConfigCache(tmfPath).ToArray();
					var thisLanguageFolder = allLanguages.First(loc => loc.Id == thisLanguage).FolderRoot;
					var languages = allLanguages.Where(loc => loc.Id != thisLanguage).ToArray();
					var onlyOneLanguage = languages.Length == 1;
					CrownPeak.CMSAPI.Input.StartExpandPanel("Target Language" + (onlyOneLanguage ? "" : "s"));
					var index = 1;
					foreach (var language in languages)
					{
						var languageAsset = Asset.Load(language.Id);
						CrownPeak.CMSAPI.Input.ShowCheckBox("", "tmf_language_selected:" + index++, language.Id, languageAsset["page_title"], unCheckedValue: "", defaultChecked: onlyOneLanguage);
					}
					CrownPeak.CMSAPI.Input.EndExpandPanel();
					var translator = Translation.GetTmfTranslator(asset);
					translator.ProjectInput(asset, context);
					CrownPeak.CMSAPI.Input.NextTabbedPanel();
					var choices = new[] { "File", "Folder" };
					var sap = new ShowAcquireParams
					{
						DefaultFolder = thisLanguageFolder,
						ShowUpload = false
					};
					while (CrownPeak.CMSAPI.Input.NextPanel("select_assets"))
					{
						CrownPeak.CMSAPI.Input.StartDropDownContainer("Type", "selected_type", choices.ToDictionary(c => c, c => c), "Asset");
						CrownPeak.CMSAPI.Input.ShowAcquireDocument("", "selected_file", sap);
						CrownPeak.CMSAPI.Input.NextDropDownContainer();
						CrownPeak.CMSAPI.Input.ShowSelectFolder("", "selected_folder", thisLanguageFolder);
						CrownPeak.CMSAPI.Input.ShowCheckBox("", "selected_folder_descendents", "true", "Include all descendents (not just children)", defaultChecked: false);
						CrownPeak.CMSAPI.Input.EndDropDownContainer();
					}
					CrownPeak.CMSAPI.Input.EndTabbedPanel();

					CrownPeak.CMSAPI.Input.ShowCheckBox("", "create_project", "true", "Create project on Save?", defaultChecked: false);
				}
				CrownPeak.CMSAPI.Input.EndControlPanel();
			}

			/// <summary>
			///   Creates a dropdown list of users
			/// </summary>
			public static void ShowSelectUsers()
			{
				CrownPeak.CMSAPI.Input.ShowHeader("Notification List (Hit 'Ctrl' to select more than one)");

				var dtSiteOwnerList = new Dictionary<string, string>();
				dtSiteOwnerList.Add("None", "");

				try
				{
					for (var nIndex = 0; nIndex < 300; nIndex++)
					{
						var usrTest = User.Load(nIndex);
						if (usrTest != null)
						{
							dtSiteOwnerList.Add(usrTest.Username, nIndex.ToString());
						}
					}
				}
				catch
				{
				}

				CrownPeak.CMSAPI.Input.ShowDropDown("Notification List", "site_owner_list", dtSiteOwnerList, size: 20,
					multiple: true);
			}
		}

		public class Output : TMF
		{
			/// <summary>
			/// </summary>
			/// <param name="asset"></param>
			/// <param name="sitePath"></param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Output.GetMasterAsset(asset, sitePath);
			/// ]]></code>
			/// </example>
			private static void GetMasterAsset(Asset asset, string sitePath)
			{
				var szSourceLanguageId = string.Empty;
				foreach (var aLang in Asset.Load(sitePath + Locales_Config).GetFileList())
				{
					if (asset.AssetPath.ToString().ToLower()
						.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
					{
						szSourceLanguageId = aLang.Id.ToString();
					}
				}

				var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
				if (aSourceLanguageContent.IsLoaded)
				{
					var laConfigList = GetRelList(asset.Id, "destination", sitePath);

					var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : "";

					var aChangesMasterContent = Asset.Load(szContentRelTMFId);
					if (aChangesMasterContent.IsLoaded)
					{
						Out.WriteLine("<div><a href=\"javascript:void(0)\" assetid=\"" + aChangesMasterContent["source_id"] +
													"\" onclick=\"cpInlineLink(" + aChangesMasterContent["source_id"] +
													")\" style=\"font-size:11px; padding:0px; margin:0px;\">Back to Master asset</a></div>");
					}
					else
					{
						Out.WriteLine(
							"<div style='padding-top:100px;'><div style='margin-left:auto; margin-right:auto; width:400px;'><div style='float:left; width:120px;'><img src='{0}' /></div><div style='float:left; width:280px; padding-top:30px; color:#CCC; font-family: sans-serif; font-size:32px;'>Not Available</div></div></div>",
							Asset.Load("/System/Translation Model Framework/_Assets/images/not_available.jpg").GetLink());
					}
				}
			}

			/// <summary>
			///   Outputs meta to create TMF button
			/// </summary>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Output.SetTMFButton();
			/// ]]></code>
			/// </example>
			private static void SetTMFButton()
			{
				Out.WriteLine("<meta name=\"cp-buttons\" value=\"TMF:show_tmf\" />");
			}

			/// <summary>
			///   Gets master asset lists of locale assets for dropdown
			/// </summary>
			/// <param name="asset"></param>
			/// <param name="sitePath"></param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Output.SetMasterAssetDropdownlist(asset, sitePath);
			/// ]]></code>
			/// </example>
			private static void SetMasterAssetDropdownlist(Asset asset, string sitePath)
			{
				var szSourceLanguageId = "";
				foreach (var aLang in Asset.Load(sitePath + Locales_Config).GetFileList())
				{
					if (asset.AssetPath.ToString().ToLower()
						.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
					{
						szSourceLanguageId = aLang.Id.ToString();
					}
				}

				var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
				if (aSourceLanguageContent.IsLoaded)
				{
					var laConfigList = GetRelList(asset.Id, "destination", sitePath);

					var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : "";

					var aChangesMasterContent = Asset.Load(szContentRelTMFId);
					if (aChangesMasterContent.IsLoaded)
					{
						Out.WriteLine("<meta name=\"cp-languages\" value=\"{0}\" />",
							"Master Asset:" + aChangesMasterContent["source_id"]);
					}
				}
			}

			/// <summary>
			///   Should be included in output_changes template file to enable TMF
			/// </summary>
			/// <param name="asset">Current Asset</param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Output.LoadMasterAssetChanges(asset);
			/// ]]></code>
			/// </example>
			public static void LoadMasterAssetChanges(Asset asset)
			{
				var szSitePath = GetSitePath(asset).AssetPath.ToString();

				var dtChangesSourceContent = GetTMFTemplateFields(asset);
				if (string.Equals(dtChangesSourceContent["_tmf_process"], "true", StringComparison.OrdinalIgnoreCase))
				{
					var szSourceLanguageId = string.Empty;
					var laLocales = Asset.Load(szSitePath + Locales_Config).GetFileList();
					foreach (var aLang in laLocales)
					{
						if (asset.AssetPath.ToString().ToLower()
							.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
						{
							szSourceLanguageId = aLang.Id.ToString();
							break;
						}
					}

					var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
					Asset aChangesMasterContent = null;
					if (aSourceLanguageContent.IsLoaded)
					{
						var laConfigList = GetRelList(asset.Id, "destination", szSitePath);
						var szContentRelTMFId = laConfigList.Count > 0 ? laConfigList[0].Id.ToString() : string.Empty;

						aChangesMasterContent = Asset.Load(szContentRelTMFId);
					}

					var szMasterLanguageId = string.Empty;
					foreach (var aLang in laLocales)
					{
						if (Asset.Load(aChangesMasterContent.Raw["source_id"]).AssetPath.ToString().ToLower()
							.Contains(Asset.Load(aLang.Raw["folder_root"]).AssetPath.ToString().ToLower()))
						{
							szMasterLanguageId = aLang.Id.ToString();
							break;
						}
					}

					var aMasterLanguage = Asset.Load(szMasterLanguageId);
					if (aMasterLanguage.IsLoaded)
					{
						Out.WriteLine(
							"<html><body><table style='width:100%; padding:0px; margin:0px; border:solid 1px #CCC; font-family: san-serif; font-size:12px;'>");
						Out.WriteLine("<tr style='color:#FFFFFF; background: #4d6c81 !important; font-size:16px;'>");
						Out.WriteLine(
							"<td style='width:10%; word-wrap:break-word; padding:5px; margin:5px; border-right:solid 1px #CCC;'>Field Name</td>");
						Out.WriteLine(
							"<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px; border-right:solid 1px #CCC;'>Translated/Localized Value (" +
							aSourceLanguageContent.Label + ")</td>");
						Out.WriteLine("<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px;'>Previous Master (" +
													aMasterLanguage.Label + ")</td>");
						Out.WriteLine(
							"<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px; border-right:solid 1px #CCC;'>Current Master (" +
							aMasterLanguage.Label + ")</td>");
						Out.WriteLine("</tr>");

						foreach (var kvpChangeSourceContent in dtChangesSourceContent)
						{
							var szBackColor = string.Empty;
							var szCurrentValue = asset.Raw[kvpChangeSourceContent.Key].Trim();
							var szMasterValue = Asset.Load(aChangesMasterContent.Raw["source_id"]).Raw[kvpChangeSourceContent.Key]
								.Trim();
							var szOldValue = aChangesMasterContent.Raw["sourceOld_" + kvpChangeSourceContent.Key].Trim();

							if (!string.Equals(szOldValue, szMasterValue, StringComparison.OrdinalIgnoreCase))
							{
								szBackColor = " style='background: #ffffdb !important;'";
							}

							if (!kvpChangeSourceContent.Key.Contains("tmf_") &&
									!string.Equals(kvpChangeSourceContent.Key.Substring(0, 1), "_", StringComparison.OrdinalIgnoreCase) &&
									!string.Equals(kvpChangeSourceContent.Key, "show_tmf", StringComparison.OrdinalIgnoreCase) &&
									!string.Equals(kvpChangeSourceContent.Key, "id", StringComparison.OrdinalIgnoreCase) &&
									!string.Equals(kvpChangeSourceContent.Key, "duplicate_id", StringComparison.OrdinalIgnoreCase))
							{
								Out.WriteLine("<tr{0}>", szBackColor);
								Out.WriteLine(
									"<td style='width:10%; word-wrap:break-word; padding:5px; margin:5px; border-top:solid 1px #CCC; border-right:solid 1px #CCC; background:#d1d1d1 !important; color:#4d4d4d;'>{0}</td>",
									kvpChangeSourceContent.Key);
								Out.WriteLine(
									"<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px; border-top:solid 1px #CCC; border-right:solid 1px #CCC;'{0}>{1}</td>",
									Util.Editable(kvpChangeSourceContent.Key, null, asset), szCurrentValue);
								Out.WriteLine(
									"<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px; border-top:solid 1px #CCC; border-right:solid 1px #CCC;'>{0}</td>",
									szOldValue);
								Out.WriteLine(
									"<td style='width:30%; word-wrap:break-word; padding:5px; margin:5px; border-top:solid 1px #CCC;'>{0}</td>",
									szMasterValue);
								Out.WriteLine("</tr>");
							}
						}

						Out.WriteLine(dtChangesSourceContent.Count > 0 ? "</table></body></html>" : "");
					}
					else
					{
						GetMasterAsset(asset, szSitePath);
					}
				}
				else
				{
					GetMasterAsset(asset, szSitePath);
				}
			}

			/// <summary>
			/// </summary>
			/// <param name="asset"></param>
			/// <param name="context"></param>
			/// <param name="sitePath"></param>
			public static void SetDropdownlistOutput(Asset asset, OutputContext context, string sitePath)
			{
				if (!context.IsPublishing)
				{
					var localeConfigCache = CreateLocaleConfigCache(sitePath);
					var rbOutput = new StringBuilder();

					var rlist1 = GetRelList(asset.Id, "source", sitePath);
					var rlist2 = GetRelList(asset.Id, "destination", sitePath);
					var r = new List<string>();

					rlist1.AddRange(rlist2);


					foreach (var rc in rlist1)
					{
						if (rc.Raw["source_id"] != asset.Id.ToString() && !r.Contains(rc.Raw["source_id"]))
						{
							if (!r.Contains(rc.Raw["source_id"]))
							{
								r.Add(rc.Raw["source_id"]);
							}
						}

						if (rc.Raw["destination_id"] != asset.Id.ToString() && !r.Contains(rc.Raw["destination_id"]))
						{
							if (!r.Contains(rc.Raw["destination_id"]))
							{
								r.Add(rc.Raw["destination_id"]);
							}
						}
					}

					foreach (var ritem in r)
					{
						if (ritem != "" && ritem != asset.Id.ToString())
						{
							var rcontent = Asset.Load(ritem);
							if (rcontent.IsLoaded)
							{
								var localeId = GetLocaleId(rcontent, sitePath, localeConfigCache);
								var localeName = Asset.Load(localeId).Label;
								var szLinkTxt = localeName + ":" + rcontent.Id;
								if (rbOutput.Length > 0)
								{
									rbOutput.Append(",");
								}

								if (!rbOutput.ToString().Contains(szLinkTxt))
								{
									rbOutput.AppendFormat("{0}", szLinkTxt);
								}
							}
						}
					}

					//Out.DebugWriteLine(rbOutput.ToString());
					var szCPLanguages = rbOutput.ToString();
					Out.WriteLine(rbOutput.Length > 0 ? "<meta name=\"cp-languages\" value=\"" + szCPLanguages + "\" />" : "");
					SetMasterAssetDropdownlist(asset, sitePath);
				}
			}

			/// <summary>
			///   Should be included in output template file to enable TMF
			/// </summary>
			/// <param name="asset">Current Asset</param>
			/// <param name="context">Current context</param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.Output.LoadOutput(asset, outputContext);
			/// ]]></code>
			/// </example>
			public static void LoadOutput(Asset asset, OutputContext context)
			{
				if (!context.IsPublishing)
				{
					SetTMFButton();
					var aTMFConfig = GetSitePath(asset);
					if (aTMFConfig != null)
					{
						SetDropdownlistOutput(asset, context, aTMFConfig.AssetPath.ToString());
					}
				}
			}
			
			public static void LoadProjectOutput(Asset asset, OutputContext context)
			{
				context.IsGeneratingDependencies = false;

				Func<string, string> formatTime = (dtString) => dtString.Length > 16 ? dtString.Substring(0, 16).Replace("T", " ") : "";
				var count = 50;
				var tmfPath = TMF.GetSitePath(asset);
				var fp = new FilterParams
				{
					Limit = count,
					SortOrder = SortOrder.OrderByDescending(AssetPropertyNames.Id),
					FieldNames = new List<string> { "project_id", "source_locale", "source_id", "dest_locale", "complete_date" }
				};
				fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "TMF Translation Log");
				fp.Add("type", Comparison.Equals, "project");
				var projects = tmfPath.GetFilterList(fp);
				if (projects.Any())
				{
					Out.WriteLine("<style>");
					Out.WriteLine("table.projects { border: 1px solid silver; border-width: 1px 0 0 1px; }");
					Out.WriteLine("table.projects thead td { font-weight: bolder }");
					Out.WriteLine("table.projects tr:nth-child(even) { background-color: #eee }");
					Out.WriteLine("table.projects td { border: 1px solid silver; border-width: 0 1px 1px 0; vertical-align: top; padding: 3px; }");
					Out.WriteLine("</style>");
					Out.WriteLine("<table class=\"projects\" cellspacing=\"0\" cellpadding=\"0\">");
					Out.WriteLine("<thead><tr><td>XTM Project</td><td>Started</td><td>Source Locale</td><td>Dest Locale(s)</td><td>Assets</td><td>Completed?</td><td>Finished</td></tr></thead>");
					//var locales = TMF.CreateLocaleConfigCache(tmfPath.AssetPath.ToString()).ToArray();
					var siteRoot = Asset.GetSiteRoot(asset).AssetPath.ToString();
					foreach (var project in projects)
					{
						var sources = project.GetPanels("source_id").Select(p => Asset.LoadDirect(p["source_id"]).AssetPath.ToString()).ToArray();
						Out.WriteLine("<tr><td>{0}</td><td>{6}</td><td>{1}</td><td>{2}</td><td><details><summary>{3}</summary>{4}</details></td><td>{5}</td><td>{7}</td></tr>",
							project["project_id"],
							project["source_locale"],
							string.Join(", ", project.GetPanels("source_id").SelectMany(s => s.GetPanels("dest_locale").Select(d => d["dest_locale"])).GroupBy(g => g).Select(g => g.First())),
							sources.Length + " asset" + (sources.Length == 1 ? "" : "s"),
							string.Join("<br/>\n", sources.Select(s => s.Substring(siteRoot.Length))),
							project.Raw["completed"] == "true" ? "Yes" : "No",
							formatTime(project.CreateDate.Value.ToString("o")),
							string.IsNullOrWhiteSpace(project.Raw["complete_date"]) ? "&nbsp;" : formatTime(project.Raw["complete_date"])
						);
					}
					Out.WriteLine("</table>");
					if (projects.Count >= count)
						Out.WriteLine("<p>Only the most recent {0} projects are shown here.</p>", count);
				}
			}
		}

		public class PostSave : TMF
		{
			/// <summary>
			///   Should be included in post_save template file to enable TMF
			/// </summary>
			/// <param name="asset">Current Asset</param>
			/// <param name="context">Current context</param>
			/// <example>
			///   <code lang="C#"><![CDATA[
			/// ServicesTMF.PostSave.LoadPostSave(asset, postSaveContext);
			/// ]]></code>
			/// </example>
			public static void LoadPostSave(Asset asset, PostSaveContext context)
			{
				var szSitePath = GetSitePath(asset).AssetPath.ToString();

				var aContentSource = asset;

				var localeConfigCache = CreateLocaleConfigCache(szSitePath);

				var szSourceLanguageId = GetLocaleId(aContentSource, szSitePath, localeConfigCache);

				if (!string.IsNullOrWhiteSpace(szSourceLanguageId))
				{
					var translator = Translation.GetTmfTranslator(asset);
					var aSourceLanguageContent = Asset.Load(szSourceLanguageId);
					if (string.Equals(asset.Raw["tmf_selection_type"], "0"))
					{
						var laConfigList = GetRelList(aContentSource.Id, "source", szSitePath);

						var nIndexPanel = 1;
						foreach (var peSelectedLangList in asset.GetPanels("tmf_language_selected_panel"))
						{
							if (!string.IsNullOrWhiteSpace(peSelectedLangList.Raw["tmf_language_selected"]))
							{
								var aLanguageContent = Asset.Load(peSelectedLangList["tmf_language_selected"]);
								if (aLanguageContent.IsLoaded)
								{
									var szConfigAssetId = "";
									foreach (var aConfigList in laConfigList)
									{
										if (Asset.Load(aConfigList.Raw["destination_id"]).AssetPath.ToString().ToLower()
											.Contains(aLanguageContent["folder_root"].ToLower()))
										{
											szConfigAssetId = aConfigList.Id.ToString();
											break;
										}
									}

									var aConfigContent = Asset.Load(szConfigAssetId);
									Asset aDestTmpContent = null;
									if (string.IsNullOrWhiteSpace(szConfigAssetId) ||
											!Asset.Load(aConfigContent["destination_id"]).IsLoaded)
									{
										var szTranslatedId = CreateTranslatedAsset(aContentSource, aSourceLanguageContent, aLanguageContent,
											szConfigAssetId, szSitePath);
										aDestTmpContent = Asset.Load(szTranslatedId);
										FixRelativeLinks(aDestTmpContent, aSourceLanguageContent, aLanguageContent, szSitePath);
										SendNotification(aContentSource, aDestTmpContent, aLanguageContent, context.ClientName, context,
											szSitePath);
									}
									else
									{
										if (string.Equals(peSelectedLangList.Raw["tmf_overwrite_existing_asset"], "y",
											StringComparison.OrdinalIgnoreCase))
										{
											//Out.DebugWriteLine("inside overwrite");

											var tempSourcContent = asset.GetContent().ToDictionary(x => x.Key, x => x.Value);

											// Because TMF processing duplicates certain content properties, we need to remove them before saving back to the asset.
											var adjustedContent = ServicesTMF.RemoveTmfDuplicates(tempSourcContent);

											Asset.Load(aConfigContent.Raw["destination_id"]).SaveContent(adjustedContent);
											aDestTmpContent = Asset.Load(aConfigContent["destination_id"]);

											//Out.DebugWriteLine("aDestTmpContent.Id=" + aDestTmpContent.Id);
											//Out.DebugWriteLine("aSourceLanguageContent.Id=" + aSourceLanguageContent.Id);
											//Out.DebugWriteLine("aLanguageContent.Id=" + aLanguageContent.Id);
											//Out.DebugWriteLine("szSitePath=" + szSitePath);

											FixRelativeLinks(aDestTmpContent, aSourceLanguageContent, aLanguageContent, szSitePath);
										}

										SendNotificationsToOwners(aContentSource,
											Asset.Load(aConfigContent.Raw["destination_id"]).Id.ToString(), context.ClientName, context,
											szSitePath);
									}

									if (aDestTmpContent != null && aDestTmpContent.IsLoaded)
									{
										var values = translator.TmfInputValues(asset, peSelectedLangList, nIndexPanel);
										if (values.Any())
										{
											translator.TranslateAsset(values, aContentSource, aDestTmpContent);
										}
									}

									asset.DeleteContentField("tmf_language_selected:" + nIndexPanel);
									asset.DeleteContentField("tmf_language_selected_panel:" + nIndexPanel);
								}
							}

							nIndexPanel++;
						}
					}
					else
					{
						foreach (var kvpData in asset.GetContent())
						{
							if (kvpData.Key.Contains("tmf_folder_") &&
									string.Equals(kvpData.Value, "y", StringComparison.OrdinalIgnoreCase))
							{
								var szFolderId = kvpData.Key.Replace("tmf_folder_", "");
								var fpPackage = new FilterParams();
								fpPackage.Add(Comparison.Equals, AssetType.File);

								fpPackage.SortOrder = SortOrder.OrderBy(AssetPropertyNames.Label);

								var nAssetCount = 1;
								foreach (var aPackage in Asset.Load(szFolderId).GetFilterList(fpPackage))
								{
									nAssetCount++;

									var laConfigList = GetRelList(aPackage.Id, "source", szSitePath);
									var nIndexPanel = 1;
									foreach (var peSelectedLangList in asset.GetPanels("tmf_language_selected_panel"))
									{
										var aLanguageContent = Asset.Load(peSelectedLangList["tmf_language_selected"]);
										if (aLanguageContent.IsLoaded)
										{
											var szConfigAssetId = "";
											foreach (var aConfig in laConfigList)
											{
												if (Asset.Load(aConfig["destination_id"]).AssetPath.ToString().ToLower()
													.Contains(aLanguageContent["folder_root"].ToLower()))
												{
													szConfigAssetId = aConfig.Id.ToString();
													break;
												}
											}

											var aConfigContent = Asset.Load(szConfigAssetId);
											Asset aDestTmpContent = null;
											if (!aConfigContent.IsLoaded)
											{
												var szTranslatedId = CreateTranslatedAsset(aPackage, aSourceLanguageContent, aLanguageContent,
													szConfigAssetId, szSitePath);
												aDestTmpContent = Asset.Load(szTranslatedId);
												SendNotification(aPackage, Asset.Load(szTranslatedId), aLanguageContent, context.ClientName,
													context, szSitePath);
												FixRelativeLinks(aDestTmpContent, aSourceLanguageContent, aLanguageContent, szSitePath);
											}
											else
											{
												SendNotificationsToOwners(aPackage, Asset.Load(aConfigContent["destination_id"]).Id.ToString(),
													context.ClientName, context, szSitePath);
											}

											if (aDestTmpContent != null && aDestTmpContent.IsLoaded)
											{
												var values = translator.TmfInputValues(asset, peSelectedLangList, nIndexPanel);
												if (values.Any())
												{
													translator.TranslateAsset(values, aPackage, aDestTmpContent);
												}
											}
										}
										nIndexPanel++;
									}
								}
							}
						}
					}

					if (!aContentSource.Type.Equals(AssetType.Folder) &&
							!string.IsNullOrWhiteSpace(asset.Raw["tmf_related_link_internal"]))
					{
						var szContentDestId = Asset.Load(asset.Raw["tmf_related_link_internal"]).Id.ToString();
						var szSourceId = string.Equals(asset.Raw["tmf_relationship_type"], "1")
							? szContentDestId
							: aContentSource.Id.ToString();

						var szDestinationId = string.Equals(asset.Raw["tmf_relationship_type"], "1")
							? aContentSource.Id.ToString()
							: szContentDestId;
						var szLabel = szSourceId + "-" + szDestinationId;

						var aTMFTmpContent = Asset.Load(szSitePath + Relationships_Config + szLabel);

						if (!aTMFTmpContent.IsLoaded)
						{
							aTMFTmpContent = Asset.CreateNewAsset(szLabel, Asset.Load(szSitePath + Relationships_Config),
								Asset.Load("/System/Translation Model Framework/_Models/Relationship/Relationship"),
								new Dictionary<string, string>());
						}

						aTMFTmpContent.SaveContentField("source_id", szSourceId);
						aTMFTmpContent.SaveContentField("destination_id", szDestinationId);
						aTMFTmpContent.SaveContentField("filter_string", "|" + szSourceId + "|" + szDestinationId + "|");

						UpdateRelationshipHistory(aTMFTmpContent, szSitePath);

						if (string.Equals(asset.Raw["tmf_new_notifyowner"], "y", StringComparison.OrdinalIgnoreCase))
						{
							SendNotificationsToOwners(aContentSource, szContentDestId, context.ClientName, context, szSitePath);
						}
					}

					if (string.Equals(asset.Raw["tmf_confirm_translation"], "y", StringComparison.OrdinalIgnoreCase))
					{
						UpdateMasterHistory(aContentSource, szSitePath);
					}

					ClearTMFHelperValues(asset);
				}
			}

			public static void LoadProjectPostSave(Asset asset, PostSaveContext context)
			{
				var createOnSave = asset["create_project"] == "true";
				if (!createOnSave) return;

				asset.DeleteContentField("create_project");
				var tmfPath = GetSitePath(asset).AssetPath.ToString();
				var thisLanguage = GetLocaleId(asset, tmfPath);
				var allLanguages = CreateLocaleConfigCache(tmfPath).ToArray();
				var languages = allLanguages.Where(loc => loc.Id != thisLanguage).ToArray();
				var selectedLanguages = languages.Where(l => asset.GetPanels("tmf_language_selected").Select(p => p.Raw["tmf_language_selected"]).Where(lan => !string.IsNullOrWhiteSpace(lan)).Contains(l.Id)).ToArray();

				var sourceLocale = Asset.Load(thisLanguage).Raw["locale_code"];
				var selectedAssets = new List<Asset>();
				foreach (var panel in asset.GetPanels("select_assets"))
				{
					if (panel.Raw["selected_type"] == "File")
					{
						var a = Asset.Load(panel.Raw["selected_file"]);
						if (a.IsLoaded && !a.IsBinary) selectedAssets.Add(a);
					}
					else
					{
						var f = Asset.Load(panel.Raw["selected_folder"]);
						if (f.IsLoaded)
						{
							List<Asset> theseAssets;
							if (panel.Raw["selected_folder_descendents"] == "true")
							{
								var fp = new FilterParams();
								fp.Add(Comparison.Equals, AssetType.File);
								theseAssets = f.GetFilterList(fp);
							}
							else
							{
								var ap = new AssetParams();
								theseAssets = f.GetFileList(ap);
							}
							selectedAssets.AddRange(theseAssets.Where(a => a.IsLoaded && !a.IsBinary));
						}
					}
				}
				// Deduplicate
				selectedAssets = selectedAssets.GroupBy(a => a.AssetPath).Select(g => g.First()).ToList();

				var fieldsToSave = new Dictionary<string, string>
				{
					{"started", "true"}
				};
				var aSourceLanguageContent = Asset.Load(thisLanguage);
				var targetLanguages = selectedLanguages.Select(l => Asset.Load(l.Id).Raw["locale_code"]).ToArray();
				var targets = new List<List<string>>();
				var index = 1;
				foreach (var sourceAsset in selectedAssets)
				{
					var targetList = new List<string>();
					fieldsToSave.Add("source_asset:" + index, "/" + context.ClientName + "/cpt_internal/" + sourceAsset.Id);
					var relationshipAssets = TMF.GetRelList(sourceAsset.Id, "source", tmfPath);
					var subindex = 1;
					foreach (var lang in selectedLanguages)
					{
						var aLanguageContent = Asset.Load(lang.Id);
						if (aLanguageContent.IsLoaded)
						{
							var configAssetId = "";
							var locale = aLanguageContent.Raw["locale_code"];

							foreach (var aConfig in relationshipAssets)
							{
								if (Asset.Load(aConfig["destination_id"]).AssetPath.ToString().ToLower()
									.Contains(aLanguageContent["folder_root"].ToLower()))
								{
									configAssetId = aConfig.Id.ToString();
									break;
								}
							}

							var aConfigContent = Asset.Load(configAssetId);
							Asset aDestTmpContent = null;
							if (!aConfigContent.IsLoaded)
							{
								//Out.WriteLine("Creating new dest asset from " + sourceAsset.AssetPath + " (" + sourceAsset.Id + ")<br/>\n");
								var szTranslatedId = TMF.CreateTranslatedAsset(sourceAsset, aSourceLanguageContent, aLanguageContent, configAssetId, tmfPath);
								aDestTmpContent = Asset.Load(szTranslatedId);
								FixRelativeLinks(aDestTmpContent, aSourceLanguageContent, aLanguageContent, tmfPath);
								SendNotification(sourceAsset, Asset.Load(szTranslatedId), aLanguageContent, context.ClientName, context, tmfPath);
							}
							else
							{
								aDestTmpContent = Asset.Load(aConfigContent["destination_id"]);
								//Out.WriteLine("Found existing dest asset at " + aDestTmpContent.AssetPath + " (" + aDestTmpContent.Id + ")<br/>\n");
								FixRelativeLinks(aDestTmpContent, aSourceLanguageContent, aLanguageContent, tmfPath);
								SendNotificationsToOwners(sourceAsset, aDestTmpContent.Id.ToString(), context.ClientName, context, tmfPath);
							}

							if (aDestTmpContent != null && aDestTmpContent.IsLoaded)
							{
								fieldsToSave.Add("destination_asset:" + subindex + ":" + index, "/" + context.ClientName + "/cpt_internal/" + aDestTmpContent.Id);
								fieldsToSave.Add("destination_locale:" + subindex + ":" + index, locale);
								targetList.Add(aDestTmpContent.Id.ToString());
								//Out.WriteLine("Sending " + sourceAsset.AssetPath + " for translation into " + aDestTmpContent.AssetPath + "<br/>\n");
							}
						}
						subindex++;
					}
					targets.Add(targetList);
					index++;
				}
				//asset.SaveContent(fieldsToSave);

				var translator = Translation.GetTmfTranslator(asset);
				translator.TranslateProject(asset, selectedAssets.ToArray(), targetLanguages.ToArray(), targets.Select(t => t.ToArray()).ToArray());

				// Clear the content for the next run
				asset.DeleteContentFields(asset.GetContent().Select(c => c.Key).ToList());
			}
		}

		public class Translation : TMF
		{
			public static Asset GetTranslationConfig(Asset asset)
			{
				var path = GetSitePath(asset);
				var fp = new FilterParams();
				fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "TMF Translation Config");
				var configs = path.GetFilterList(fp).ToArray();
				// Prioritise loading one called "Translation Config"
				if (configs.Any(c => c.Label == "Translation Config"))
					return configs.First(c => c.Label == "Translation Config");
				return configs.FirstOrDefault() ?? Asset.Load(-1);
			}

			public static Dictionary<string, string> GetTmfTranslators(Asset asset)
			{
				return new Dictionary<string, string>
				{
					{ "None", "none" },
					{ "Human Name", "internal_name" } // TODO
				};
			}

			public static ITMFTranslator GetTmfTranslator(string translatorName)
			{
				switch (translatorName)
				{
					case "internal_name":
						return new ExampleTranslator(); // TODO
					case "none":
						return new NullTranslator();
					default:
						throw new Exception("GetTmfTranslator: Unknown translation provider.");
				}
			}

			public static ITMFTranslator GetTmfTranslator(Asset asset)
			{
				var config = GetTranslationConfig(asset);
				if (!config.IsLoaded)
				{
					throw new Exception("GetTmfTranslator: Unable to find translation config.");
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(asset.Raw["translation_provider"]))
					{
						// This lets us support log files produced by another translation provider
						return GetTmfTranslator(asset.Raw["translation_provider"]).Init(config);
					}
					return GetTmfTranslator(config.Raw["translation_provider"]).Init(config);
				}
			}

			public static string GenerateHash(Asset asset)
			{
				var fields = TMF.TemplateTranslation.GetFieldsForTranslation(asset);
				var data = UTF8Encoding.UTF8.GetBytes(string.Join(";", fields.Select(f => f.Key + "=" + f.Value)));
				var hash = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(data);
				return string.Join("", hash.Select(h => h.ToString("X2")));
			}

			public static bool VerifyHash(Asset asset)
			{
				var existingHash = asset.Raw["translation_hash"];
				if (string.IsNullOrWhiteSpace(existingHash)) return false;

				return GenerateHash(asset) == existingHash;
			}

			public static void SaveHash(Asset asset)
			{
				asset.SaveContentField("translation_hash", GenerateHash(asset));
			}

			public static void DeleteHash(Asset asset)
			{
				asset.DeleteContentField("translation_hash");
			}
		}

		public interface ITMFTranslator
		{
			ITMFTranslator Init(Asset config);
			void ConfigInput(Asset asset, InputContext context);
			void ConfigPostInput(Asset asset, PostInputContext context);
			void TranslateAsset(Dictionary<string, string> inputValues, Asset source, Asset destination);
			void TranslateProject(Asset project, Asset[] sources, string[] targetLocales, string[][] targets);
			Asset CreateLog(string type, Asset source, string sourceLocale, Asset destination, string destLocale, object response);
			Asset CreateProjectLog(Asset project, Asset[] sources, string[] targetLocales, string[][] targets, object response);
			void UpdateLog(Asset log);
			void DisplayLog(Asset log);
			void TmfInput(Asset asset, InputContext context, int index, bool isResend);
			void ProjectInput(Asset asset, InputContext context);
			void ProjectPostInput(Asset asset, PostInputContext context);
			Dictionary<string, string> TmfInputValues(Asset asset, PanelEntry panel, int index);
		}

		public class NullTranslator : ITMFTranslator
		{
			public ITMFTranslator Init(Asset config)
			{
				return this;
			}

			public void ConfigInput(Asset asset, InputContext context)
			{ }

			public void ConfigPostInput(Asset asset, PostInputContext context)
			{ }

			public void TranslateAsset(Dictionary<string, string> inputValues, Asset source, Asset destination)
			{ }

			public void TranslateProject(Asset project, Asset[] sources, string[] targetLocales, string[][] targets)
			{ }

			public Asset CreateLog(string type, Asset source, string sourceLocale, Asset destination, string destLocale, object response)
			{
				return Asset.Load(-1);
			}

			public Asset CreateProjectLog(Asset project, Asset[] sources, string[] targetLocales, string[][] targets, object response)
			{
				return Asset.Load(-1);
			}

			public void UpdateLog(Asset log)
			{ }

			public void DisplayLog(Asset log)
			{ }

			public void TmfInput(Asset asset, InputContext context, int index, bool isResend)
			{ }

			public void ProjectInput(Asset asset, InputContext context)
			{ }

			public void ProjectPostInput(Asset asset, PostInputContext context)
			{ }

			public Dictionary<string, string> TmfInputValues(Asset asset, PanelEntry panel, int index)
			{
				return new Dictionary<string, string>();
			}
		}

		public static class TemplateTranslation
		{
			private static Asset GetTemplateTranslationConfig(Asset asset)
			{
				var site = TMF.GetSitePath(asset);
				var fp = new FilterParams
				{
					Limit = 1,
					FieldNames = new List<string> { "fields", "field", "opt_in" }
				};
				fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "Template Translation Config");
				fp.Add("template", Comparison.EndsWith, "/" + asset.TemplateId);
				var templateConfig = site.GetFilterList(fp).FirstOrDefault() ?? Asset.Load(-1);

				return templateConfig;
			}

			public static Dictionary<string, string> GetFieldsForTranslation(Asset asset)
			{
				var templateConfig = GetTemplateTranslationConfig(asset);

				var content = asset.GetContent();
				// Remove TMF fields
				content = content.Where(c => !c.Key.StartsWith("tmf_")
																		 && !c.Key.StartsWith("upload#tmf_")
																		 && !c.Key.StartsWith("upload_name#tmf_")
																		 && !string.IsNullOrWhiteSpace(c.Value))
					.ToDictionary(c => c.Key, c => c.Value);
				if (templateConfig.IsLoaded)
				{
					var fields = templateConfig.GetPanels("fields").Select(p => p.Raw["field"]).ToArray();
					if (templateConfig.Raw["opt_in"] == "yes")
					{
						// Allow list just the fields they want
						return content.Where(c => fields.Contains(c.Key) || fields.Any(f => c.Key.StartsWith(f + ":")))
							.ToDictionary(c => c.Key, c => c.Value);
					}
					else
					{
						// Deny list removing the fields they don't want
						return content.Where(c => !fields.Contains(c.Key) && !fields.Any(f => c.Key.StartsWith(f + ":")))
							.ToDictionary(c => c.Key, c => c.Value);
					}
				}

				return content;
			}
		}

		public static class Utils
		{
			private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			public static DateTime FromEpochTime(double time)
			{
				return _epoch.AddMilliseconds(time);
			}
		}
	}
}