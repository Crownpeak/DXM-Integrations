using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CrownPeak.AccessAPI;
using CrownPeak.AccessApiHelper;
using CrownPeak.AccessApiHelper.ApiAccessor;

namespace XTM_Helper
{
	public partial class Form1 : Form
	{
		private CmsApi _cms = new CmsApi(new SimpleApiAccessor());
		private System.Text.RegularExpressions.Regex reNumeric = new System.Text.RegularExpressions.Regex("^[0-9]+$");

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFolderSource.Text) || string.IsNullOrWhiteSpace(txtFolderTarget.Text) ||
			    !int.TryParse(txtFolderSource.Text, out _) || !int.TryParse(txtFolderTarget.Text, out _))
			{
				MessageBox.Show("Please provide source and target IDs");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var sourceId = int.Parse(txtFolderSource.Text);
			var targetId = int.Parse(txtFolderTarget.Text);
			CloneFolderTree(sourceId, targetId);
			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}


		private void button2_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFolderSource.Text) || string.IsNullOrWhiteSpace(txtFolderTarget.Text) ||
			    string.IsNullOrWhiteSpace(txtRelationships.Text) || string.IsNullOrWhiteSpace(txtRelationshipModel.Text) ||
					!int.TryParse(txtFolderSource.Text, out _) || !int.TryParse(txtFolderTarget.Text, out _) ||
			    !int.TryParse(txtRelationships.Text, out _) || !int.TryParse(txtRelationshipModel.Text, out _))
			{
				MessageBox.Show("Please provide all values");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var sourceId = int.Parse(txtFolderSource.Text);
			var targetId = int.Parse(txtFolderTarget.Text);
			var relationshipsId = int.Parse(txtRelationships.Text);
			var relationshipModelId = int.Parse(txtRelationshipModel.Text);
			CloneFolderContent(sourceId, targetId, relationshipsId, relationshipModelId);
			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFolderSource.Text) || string.IsNullOrWhiteSpace(txtFolderTarget.Text) ||
			    !int.TryParse(txtFolderSource.Text, out _) || !int.TryParse(txtFolderTarget.Text, out _))
			{
				MessageBox.Show("Please provide source and target IDs");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var sourceId = int.Parse(txtFolderSource.Text);
			var targetId = int.Parse(txtFolderTarget.Text);

			Relink(sourceId, targetId);

			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtZipFile.Text) || string.IsNullOrWhiteSpace(txtFolderSourceForZip.Text) ||
			    string.IsNullOrWhiteSpace(txtFolderTargetForZip.Text) || 
			    !int.TryParse(txtFolderSourceForZip.Text, out _) || !int.TryParse(txtFolderTargetForZip.Text, out _))
			{
				MessageBox.Show("Please provide all details");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var locale = txtLocale.Text;
			var file = txtZipFile.Text;
			var sourceId = int.Parse(txtFolderSourceForZip.Text);
			var targetId = int.Parse(txtFolderTargetForZip.Text);

			ImportZip(file, locale, sourceId, targetId);

			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.CheckPathExists = true;
			dialog.FileName = txtZipFile.Text;
			dialog.DefaultExt = "zip";
			dialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				txtZipFile.Text = dialog.FileName;
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFolderSource.Text) || string.IsNullOrWhiteSpace(txtFolderTarget.Text) ||
			    string.IsNullOrWhiteSpace(txtRelationships.Text) || string.IsNullOrWhiteSpace(txtRelationshipModel.Text) ||
					!int.TryParse(txtFolderSource.Text, out _) || !int.TryParse(txtFolderTarget.Text, out _) ||
			    !int.TryParse(txtRelationships.Text, out _) || !int.TryParse(txtRelationshipModel.Text, out _))
			{
				MessageBox.Show("Please provide all details");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var sourceId = int.Parse(txtFolderSource.Text);
			var targetId = int.Parse(txtFolderTarget.Text);
			var relationshipsId = int.Parse(txtRelationships.Text);
			var relationshipModelId = int.Parse(txtRelationshipModel.Text);

			_cms.Asset.Read(relationshipsId, out var relationshipsAsset);
			var relationshipsPath = relationshipsAsset.FullPath;

			CreateTmfRelationships(sourceId, targetId, relationshipsId, relationshipModelId, relationshipsPath);
			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtEndpoint.Text) || string.IsNullOrWhiteSpace(txtToken.Text))
			{
				MessageBox.Show("Please provide XTM endpoint and token");
				return;
			}

			if (string.IsNullOrWhiteSpace(txtProjectId.Text) || !int.TryParse(txtProjectId.Text, out _))
			{
				MessageBox.Show("Please provide a project ID");
				return;
			}

			var client = new System.Net.WebClient();
			client.Headers.Add("Content-Type", "application/json");
			client.Headers.Add("Authorization", "XTM-Basic " + txtToken.Text);

			// First ask for our file to be generated
			ShowStatus("Requesting target file generation...");
			var json = client.UploadString(txtEndpoint.Text + "/projects/" + txtProjectId.Text + "/files/generate?fileType=TARGET", "POST", "");
			var response = Deserialize(json, typeof(GenerateFileResponse[])) as GenerateFileResponse[];
			if (response != null && response.Length > 0)
			{
				ShowStatus("Waiting for target file...");
				var count = 1000;
				// Now wait for our file to be generated
				while (--count >= 0)
				{
					json = client.DownloadString(txtEndpoint.Text + "/projects/" + txtProjectId.Text + "/files/status?fileScope=PROJECT&fileIds=" + response[0].FileId);
					var statusResponse = Deserialize(json, typeof(FileStatusResponse[])) as FileStatusResponse[];
					if (statusResponse != null && statusResponse.Length > 0 && statusResponse[0].Status == "FINISHED")
					{
						//return "FileId is " + response[0].FileId;
						//return GetProjectFile(projectId, response[0].FileId);
						ShowStatus("Downloading target file...");
						var bytes = client.DownloadData(txtEndpoint.Text + "/projects/" + txtProjectId.Text + "/files/" + response[0].FileId + "/download?fileScope=PROJECT");
						var dialog = new SaveFileDialog();
						dialog.CheckPathExists = true;
						dialog.DefaultExt = "zip";
						dialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
						var result = dialog.ShowDialog();
						if (result == DialogResult.OK)
						{
							using (var fs = dialog.OpenFile())
							{
								fs.Write(bytes, 0, bytes.Length);
								fs.Close();
							}
							ShowStatus("");
							MessageBox.Show("Finished");
							return;
						}
						else
						{
							return;
						}
					}
					System.Threading.Thread.Sleep(500);
				}
				ShowStatus("");
				MessageBox.Show("Timed out waiting for project result");
			}
		}

		private void button8_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtExportSource.Text) || string.IsNullOrWhiteSpace(txtExportFolder.Text) ||
			    !int.TryParse(txtExportSource.Text, out _))
			{
				MessageBox.Show("Please provide all details");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var sourceId = int.Parse(txtExportSource.Text);
			var folder = txtExportFolder.Text;
			var excludeIds = txtExcludeIds.Text.Split(",".ToCharArray()).Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => int.Parse(t)).ToArray();
			var templateIds = txtTemplateIds.Text.Split(",".ToCharArray()).Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => int.Parse(t)).ToArray();

			ExportFolder(sourceId, excludeIds, folder, templateIds);

			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();

		}

		private void button9_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				txtExportFolder.Text = dialog.SelectedPath;
			}
		}

		private void button10_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				txtProjectFolder.Text = dialog.SelectedPath;
			}
		}

		private void button11_Click(object sender, EventArgs e)
		{
			var srcLocale = txtSourceLocale.Text.Replace("-", "_");
			var destLocale = txtDestLocale.Text.Replace("-", "_");
			var projectName = txtProjectName.Text;
			var folder = txtProjectFolder.Text;

			if (string.IsNullOrWhiteSpace(txtEndpoint.Text) || string.IsNullOrWhiteSpace(txtToken.Text) ||
			    string.IsNullOrWhiteSpace(txtTemplateId.Text) || string.IsNullOrWhiteSpace(txtCustomerId.Text) ||
			    !int.TryParse(txtTemplateId.Text, out _) || !int.TryParse(txtCustomerId.Text, out _))
			{
				MessageBox.Show("Please provide all XTM details");
				return;
			}

			if (string.IsNullOrWhiteSpace(srcLocale) || string.IsNullOrWhiteSpace(destLocale) ||
			    string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(folder))
			{
				MessageBox.Show("Please provide all details");
				return;
			}

			var projectId = CreateProject(txtEndpoint.Text, txtToken.Text, int.Parse(txtCustomerId.Text), int.Parse(txtTemplateId.Text), projectName, srcLocale, destLocale, folder);
			ShowStatus("");
			MessageBox.Show("Finished creating project " + projectId);
		}

		private void button12_Click(object sender, EventArgs e)
		{
			var assetId = txtAssetId.Text;
			var xmlFile = txtXmlFile.Text;
			if (string.IsNullOrWhiteSpace(assetId) || string.IsNullOrWhiteSpace(xmlFile))
			{
				MessageBox.Show("Please provide all details");
				return;
			}

			ShowStatus("Logging in...");
			_cms.Init("cms.crownpeak.net", txtInstance.Text, txtDeveloperKey.Text);
			_cms.Login(txtUsername.Text, txtPassword.Text);
			ShowStatus("");

			var xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlFile);
			var fields = xmlDoc.SelectNodes("//field").Cast<XmlNode>().ToArray();
			var fieldsToSave = fields.ToDictionary(f => GetNodeValue(f.SelectSingleNode("_key")), f => GetNodeValue(f.SelectSingleNode("value")));
			if (fieldsToSave.Any())
			{
				WorklistAsset asset;
				ShowStatus("Updating asset " + assetId + " ...");
				_cms.Asset.Update(int.Parse(assetId), fieldsToSave, out asset);
			}
			ShowStatus("");
			MessageBox.Show("Finished");

			_cms.Logout();
		}

		private void button13_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.CheckPathExists = true;
			dialog.FileName = txtXmlFile.Text;
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				txtXmlFile.Text = dialog.FileName;
			}
		}


		private void button14_Click(object sender, EventArgs e)
		{
			if (!rtHelp.Visible)
			{
				rtHelp.Location = new Point(0, 0);
				rtHelp.Size = this.Size;
				rtHelp.Height -= 60;
				rtHelp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				rtHelp.Visible = true;

				button14.Anchor = AnchorStyles.None;
				button14.Left = Width - button14.Width - 21;
				button14.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				button14.Text = "Close Help";
			}
			else
			{
				rtHelp.Visible = false;
				button14.Anchor = AnchorStyles.None;
				button14.Left = 21;
				button14.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				button14.Text = "Help";
			}
		}

		private void CloneFolderTree(int sourceId, int targetId)
		{
			WorklistAsset asset;

			ShowStatus("Getting source assets...");
			var sourceAssets = GetAllChildren(sourceId);
			ShowStatus("Getting target assets...");
			var targetAssets = GetAllChildren(targetId);

			ShowStatus("Creating target folders...");
			foreach (var source in sourceAssets.Where(a => a.type == 4))
			{
				var target = targetAssets.FirstOrDefault(a => a.label == source.label && a.type == 4);
				var subTargetId = -1;
				if (target == null)
				{
					ShowStatus("Creating copy of " + source.FullPath + " ...");
					if (source.model_id.HasValue)
						_cms.Asset.CreateFolderWithModel(source.label, targetId, source.model_id.Value, out asset);
					else
						_cms.Asset.Create(source.label, targetId, 0, 4, 0, 0, 0, out asset);
					subTargetId = asset.id;
				}
				else
				{
					subTargetId = target.id;
				}
				CloneFolderTree(source.id, subTargetId);
			}
		}

		private void CloneFolderContent(int sourceId, int targetId, int relationshipsId, int relationshipModelId)
		{
			WorklistAsset asset, reln;

			ShowStatus("Getting source assets...");
			var sourceAssets = GetAllChildren(sourceId);
			ShowStatus("Getting target assets...");
			var targetAssets = GetAllChildren(targetId);

			sourceAssets = RemoveBranches(sourceAssets);
			targetAssets = RemoveBranches(targetAssets);

			var ids = sourceAssets.Where(a => a.type == 2 && targetAssets.All(t => t.label != a.label)).Select(a => a.id).ToArray();
			if (ids.Length > 0)
			{
				ShowStatus("Copying assets [" + string.Join(",", ids) + "] ...");
				var chunkSize = 3;
				if (ids.Length < chunkSize)
					_cms.Asset.Copy(ids, targetId, 0);
				else
				{
					var count = 0;
					while (count < ids.Length)
					{
						var someIds = ids.Skip(count).Take(chunkSize).ToArray();
						if (someIds.Any())
						{
							ShowStatus("Copying assets [" + string.Join(",", someIds) + "] ...");
							_cms.Asset.Copy(someIds, targetId, 0);
						}
						count += chunkSize;
					}
				}
			}
			foreach (var source in sourceAssets.Where(a => a.type == 4))
			{
				var target = targetAssets.FirstOrDefault(a => a.label == source.label && a.type == 4);
				if (target != null)
				{
					var subTargetId = target.id;
					CloneFolderContent(source.id, subTargetId, relationshipsId, relationshipModelId);
				}
			}
		}

		private void CreateTmfRelationships(int sourceId, int targetId, int relationshipsId, int relationshipModelId, string relationshipsPath)
		{
			WorklistAsset reln;

			ShowStatus("Getting source assets...");
			var sourceAssets = GetAllChildren(sourceId);
			ShowStatus("Getting target assets...");
			var targetAssets = GetAllChildren(targetId);

			sourceAssets = RemoveBranches(sourceAssets);
			targetAssets = RemoveBranches(targetAssets);

			foreach (var source in sourceAssets.Where(a => a.type == 2))
			{
				ShowStatus("Creating relationship for " + source.FullPath + " ...");
				var target = targetAssets.FirstOrDefault(a => a.label.Trim() == source.label.Trim() && a.type == 2);
				if (target != null)
				{
					var relationship = _cms.Asset.Exists(relationshipsPath + source.id + "-" + target.id, out _);
					if (!relationship)
					{
						var fields = _cms.Asset.Fields(source.id);
						_cms.Asset.Create(source.id + "-" + target.id, relationshipsId, relationshipModelId, 2, 0, 0, 0, out reln);
						fields = fields.ToDictionary(f => "sourceold_" + f.Key, f => f.Value);
						fields.Add("source_id", source.id.ToString());
						fields.Add("destination_id", target.id.ToString());
						_cms.Asset.Update(reln.id, fields, out reln);
					}
				}
			}
			ShowStatus("");
			foreach (var source in sourceAssets.Where(a => a.type == 4))
			{
				var target = targetAssets.FirstOrDefault(a => a.label == source.label && a.type == 4);
				if (target != null)
				{
					CreateTmfRelationships(source.id, target.id, relationshipsId, relationshipModelId, relationshipsPath);
				}
			}
		}

		private int AssetExists(string path)
		{
			if (_cms.Asset.Exists(path, out var id))
			{
				return id;
			}
			return -1;
		}

		private List<WorklistAsset> GetAllChildren(int folderId)
		{
			var result = new List<WorklistAsset>();
			IEnumerable<WorklistAsset> assets;
			int normalCount, hiddenCount, deletedCount;
			var page = 0;
			do
			{
				_cms.Asset.GetList(folderId, page++, 100, "Label", OrderType.Ascending, VisibilityType.Normal, true, false, out assets, out normalCount, out hiddenCount, out deletedCount);
				result.AddRange(assets);
			} while (result.Count < normalCount && assets.Count() > 0);
			return result;
		}

		private List<WorklistAsset> RemoveBranches(List<WorklistAsset> assets)
		{
			foreach (var asset in assets)
			{
				if (asset.branchId < 0) asset.branchId = asset.id;
			}
			return assets.OrderByDescending(a => a.id).GroupBy(a => a.branchId).Select(g => g.First()).OrderBy(a => a.FullPath).ToList();
		}

		private List<WorklistAsset> GetTree(int folderId)
		{
			var result = new List<WorklistAsset>();

			var assets = GetAllChildren(folderId);
			result.AddRange(assets);

			foreach (var a in assets.Where(a => a.type == 4))
			{
				result.AddRange(GetTree(a.id));
			}

			return result;
		}

		private void Relink(int sourceId, int targetId)
		{
			WorklistAsset asset;
			_cms.Asset.Read(sourceId, out asset);
			var sourcePath = asset.FullPath;
			_cms.Asset.Read(targetId, out asset);
			var targetPath = asset.FullPath;

			ShowStatus("Getting source assets...");
			var sources = GetTree(sourceId);
			ShowStatus("Getting target assets...");
			var targets = GetTree(targetId);

			foreach (var target in targets)
			{
				RelinkAsset(target, sources, sourcePath, targets, targetPath);
			}
		}

		private void RelinkAsset(WorklistAsset asset, List<WorklistAsset> sources, string sourcePath, List<WorklistAsset> targets, string targetPath)
		{
			ShowStatus("Relinking " + asset.FullPath + " ...");
			var fields = _cms.Asset.Fields(asset.id);
			var fieldsToUpdate = new Dictionary<string, string>();
			foreach (var field in fields.Where(f => f.Key.StartsWith("upload#") && f.Value.IndexOf("/cpt_internal/") >= 0))
			{
				var value = RelinkValue(field.Value, sources, sourcePath, targets, targetPath);
				if (value != field.Value)
				{
					fieldsToUpdate.Add(field.Key, value);
				}
			}
			foreach (var field in fields.Where(f => !f.Key.StartsWith("upload#") && f.Value.IndexOf("/cpt_internal/") >= 0))
			{
				var value = RelinkField(field.Value, sources, sourcePath, targets, targetPath);
				if (value != field.Value)
				{
					fieldsToUpdate.Add(field.Key, value);
				}
			}

			if (fieldsToUpdate.Any())
			{
				WorklistAsset temp;
				_cms.Asset.Update(asset.id, fieldsToUpdate, out temp);
			}
		}

		private string RelinkField(string value, List<WorklistAsset> sources, string sourcePath, List<WorklistAsset> targets, string targetPath)
		{
			var re = new Regex("(?:\\/[a-z0-9\\-]+?)\\/cpt_internal\\/(?:[0-9]+\\/)*([0-9]+)", RegexOptions.IgnoreCase);
			return re.Replace(value, m => RelinkValue(m.Value, sources, sourcePath, targets, targetPath));
		}

		private string RelinkValue(string value, List<WorklistAsset> sources, string sourcePath, List<WorklistAsset> targets, string targetPath)
		{
			var idPart = value.Split("/".ToCharArray()).Last();
			if (!string.IsNullOrWhiteSpace(idPart) && reNumeric.IsMatch(idPart))
			{
				var linkedAssetId = int.Parse(idPart);
				var linkedAsset = sources.FirstOrDefault(a => a.id == linkedAssetId);
				if (linkedAsset != null)
				{
					var path = linkedAsset.FullPath;
					if (path.StartsWith(sourcePath))
					{
						var destPath = targetPath + path.Substring(sourcePath.Length);
						var newLinkedAsset = targets.FirstOrDefault(a => a.FullPath == destPath);
						if (newLinkedAsset != null)
						{
							return string.Format("/{0}/cpt_internal/{1}", txtInstance.Text, newLinkedAsset.id);
						}
					}
				}
			}

			return value;
		}

		private void ImportZip(string file, string locale, int sourceId, int targetId)
		{
			var errors = new List<string>();

			WorklistAsset asset;
			_cms.Asset.Read(sourceId, out asset);
			var sourcePath = asset.FullPath;
			_cms.Asset.Read(targetId, out asset);
			var targetPath = asset.FullPath;

			ShowStatus("Getting source assets...");
			var sources = GetTree(sourceId);
			ShowStatus("Getting target assets...");
			var targets = GetTree(targetId);

			using (var archive = ZipFile.OpenRead(file))
			{
				foreach (var entry in archive.Entries)
				{
					if (entry.Name.EndsWith(".xml"))
					{
						var folder = "";
						if (entry.FullName.Length > entry.Name.Length)
							folder = entry.FullName.Substring(0, entry.FullName.Length - entry.Name.Length - 1);
						if (folder == locale)
						{
							using (var entryStream = entry.Open())
							{
								using (var ms = new MemoryStream())
								{
									entryStream.CopyTo(ms);
									var xml = Encoding.UTF8.GetString(ms.ToArray());
									var error = ImportFile(entry.Name, xml, sources, sourcePath, targets, targetPath);
									if (!string.IsNullOrEmpty(error)) errors.Add(error);
								}
							}
						}
					}
				}
			}
			if (errors.Any())
			{
				MessageBox.Show(string.Join("\n", errors), errors.Count + " errors during import");
			}
		}

		private string ImportFile(string name, string xml, List<WorklistAsset> sources, string sourcePath, List<WorklistAsset> targets, string targetPath)
		{
			ShowStatus("Importing " + name + " ...");

			var error = "";
			WorklistAsset asset;

			var sourceId = int.Parse(name.Split(".-".ToCharArray()).FirstOrDefault());
			var source = sources.FirstOrDefault(a => a.id == sourceId && a.FullPath.StartsWith(sourcePath));
			if (source == null)
			{
				try
				{
					_cms.Asset.Read(sourceId, out asset);
				}
				catch
				{
					asset = null;
				}
				if (asset != null && asset.id == sourceId && asset.branchId > 0)
				{
					source = sources.FirstOrDefault(a => a.branchId == asset.branchId && a.FullPath.StartsWith(sourcePath));
				}
				if (source == null)
				{
					// Try again with the first field, which contains branchid and path
					var xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(xml);
					var firstField = xmlDoc.SelectSingleNode("//field");
					if (firstField != null)
					{
						var key = System.Net.WebUtility.HtmlDecode(GetNodeValue(firstField.SelectSingleNode("_key")));
						var value = GetNodeValue(firstField.SelectSingleNode("value"));
						if (value == "" && key.IndexOf("/") > 0)
						{
							var branchId = int.Parse(key.Substring(0, key.IndexOf("/")));
							source = sources.FirstOrDefault(a => a.branchId == branchId && a.FullPath.StartsWith(sourcePath));
							if (source == null)
							{
								var path = key.Substring(key.IndexOf("/"));
								source = sources.FirstOrDefault(a => a.FullPath == path && a.FullPath.StartsWith(sourcePath));
							}
						}
					}
				}
			}

			if (source == null)
			{
				error = "Source not found for " + sourceId;
			}
			else
			{
				var destPath = targetPath + source.FullPath.Substring(sourcePath.Length);
				var target = targets.FirstOrDefault(a => a.FullPath == destPath);
				if (target == null)
				{
					error = "Target not found for " + sourceId + ", " + source.FullPath;
				}
				else
				{
					var xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(xml);
					var fields = xmlDoc.SelectNodes("//field").Cast<XmlNode>().ToArray();
					var fieldsToSave = fields.Where(f => !string.IsNullOrEmpty(GetNodeValue(f.SelectSingleNode("value")))).ToDictionary(f => GetNodeValue(f.SelectSingleNode("_key")), f => GetNodeValue(f.SelectSingleNode("value")));
					if (fieldsToSave.Any())
					{
						ShowStatus("Updating " + target.FullPath + " ...");
						_cms.Asset.Update(target.id, fieldsToSave, out asset);
					}
				}
			}

			if (!string.IsNullOrEmpty(error))
			{
				Console.Write(error);
			}

			return error;
		}

		private static string GetNodeValue(XmlNode node)
		{
			if (node == null) return "";
			return node.InnerText;
		}

		private static object Deserialize(string json, Type toType)
		{
			using (var stream = new MemoryStream())
			{
				byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				var deserializer = new DataContractJsonSerializer(toType);
				return deserializer.ReadObject(stream);
			}
		}

		private void ExportFolder(int sourceId, int[] excludeIds, string outputFolder, int[] templateIds)
		{
			WorklistAsset asset;
			// First find the TMF for our folder
			_cms.Asset.Read(sourceId, out asset);
			var segments = asset.FullPath.Split("/".ToCharArray());
			var tmfPath = "";
			var tmfId = -1;
			for (var i = 1; i < segments.Length; i++)
			{
				var path = string.Join("/", segments.Take(segments.Length - i)) + "/_TMF";
				if (_cms.Asset.Exists(path, out tmfId))
				{
					tmfPath = path;
					break;
				}
			}

			if (string.IsNullOrWhiteSpace(tmfPath))
			{
				System.Windows.Forms.MessageBox.Show("Unable to find _TMF folder");
				return;
			}

			_cms.Asset.Exists(tmfPath + "/Template Translations Config", out tmfId);
			ShowStatus("Getting template translation configurations...");
			var templates = GetTranslationConfigs(tmfId);

			ShowStatus("Getting assets for export...");
			var assets = RemoveBranches(GetTree(sourceId));

			// Find items that match the id or are in a folder that matches the id
			var excludes = assets.Where(a => excludeIds.Contains(a.id) || (a.folder_id.HasValue && excludeIds.Contains(a.folder_id.Value))).ToArray();
			while (excludes.Any())
			{
				assets = assets.Except(excludes).ToList();
				var subExcludeIds = excludes.Select(a => a.id).ToArray();
				// Work down a folder from the things we excluded last time
				excludes = assets.Where(a => subExcludeIds.Contains(a.id) || (a.folder_id.HasValue && subExcludeIds.Contains(a.folder_id.Value))).ToArray();
			}

			if (templateIds.Any())
			{
				// Filter the templates if they provided a list
				templates = templates.Where(t => templateIds.Contains(t.Id)).ToArray();
			}
			ShowStatus("Exporting assets...");
			foreach (var template in templates)
			{
				foreach (var a in assets.Where(a => a.template_id.HasValue && a.template_id.ToString() == template.TemplateId))
				{
					ShowStatus("Exporting " + a.FullPath + " ...");
					var fields = GetFieldsForExport(_cms.Asset.Fields(a.id).Where(f => !string.IsNullOrWhiteSpace(f.Value)).ToDictionary(f => f.Key, f => f.Value), template).ToArray();
					if (fields.Any())
					{
						var doc = MakeXml(a, fields);
						doc.Save(outputFolder + "\\" + a.id + "-" + a.label + ".xml");
					}
				}
			}
			ShowStatus("");
		}

		public XmlDocument MakeXml(WorklistAsset asset, IEnumerable<KeyValuePair<string, string>> fields)
		{
			var xml = new XmlDocument();
			var pi = xml.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
			xml.AppendChild(pi);
			var element = xml.CreateElement("XtmTranslator.XtmTranslationData");
			xml.AppendChild(element);
			var fieldsElement = xml.CreateElement("fields");
			element.AppendChild(fieldsElement);

			var branchId = asset.branchId;
			if (branchId < 0) branchId = asset.id;
			fieldsElement.AppendChild(MakeField(xml, branchId + System.Net.WebUtility.HtmlEncode(asset.FullPath), ""));

			foreach (var kvp in fields)
			{
				fieldsElement.AppendChild(MakeField(xml, kvp.Key, kvp.Value));
			}
			return xml;
		}

		private XmlNode MakeField(XmlDocument xml, string key, string value)
		{
			var field = xml.CreateElement("field");
			var keyNode = xml.CreateElement("_key");
			try
			{
				keyNode.InnerXml = key.Replace("&", "&amp;");
			}
			catch (Exception)
			{
				keyNode.InnerText = value;
			}
			var valueNode = xml.CreateElement("value");
			try
			{
				valueNode.InnerXml = value.Replace("&", "&amp;");
			}
			catch (Exception)
			{
				valueNode.InnerText = value;
			}
			field.AppendChild(keyNode);
			field.AppendChild(valueNode);
			return field;
		}

		private IEnumerable<KeyValuePair<string, string>> GetFieldsForExport(Dictionary<string, string> fields, TemplateDefinition templateDefinition)
		{
			var results = new List<KeyValuePair<string, string>>();
			foreach (var field in fields)
			{
				var name = field.Key;
				var value = field.Value;
				if (templateDefinition.IsOptIn)
				{
					if (templateDefinition.Fields.Any(f => name == f || name.StartsWith(f + ":")))
					{
						//Response.Write("DEBUG: included " + name + "<br/>\n");
						results.Add(new KeyValuePair<string, string>(name, value));
					}
					else
					{
						//Response.Write("DEBUG: excluded " + name + "<br/>\n");
					}
				}
				else
				{
					if (!templateDefinition.Fields.Any(f => name == f || name.StartsWith(f + ":")))
					{
						//Response.Write("DEBUG: included " + name + "<br/>\n");
						results.Add(new KeyValuePair<string, string>(name, value));
					}
					else
					{
						//Response.Write("DEBUG: excluded " + name + "<br/>\n");
					}
				}
			}
			return results.ToArray();
		}


		private TemplateDefinition[] GetTranslationConfigs(int folderId)
		{
			var files = GetAllChildren(folderId).Where(a => a.template_label == "Template Translation Config").ToArray();
			return files.Select(f => new TemplateDefinition(_cms, f)).ToArray();
		}

		private void ShowStatus(string message)
		{
			toolStripStatusLabel1.Text = message;
			Application.DoEvents();
		}

		private int CreateProject(string endpoint, string token, int customerId, int templateId, string name, string sourceLanguage, string targetLanguage, string sourceFolder)
		{
			// Works to create a project
			ShowStatus("Creating project...");
			var client = new System.Net.WebClient();
			var boundary = GetBoundary();
			client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
			client.Headers.Add("Authorization", "XTM-Basic " + token);

			var request = new StringBuilder(10240);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"customerId\"");
			request.AppendLine();
			request.AppendLine(customerId.ToString());
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"name\"");
			request.AppendLine();
			request.AppendLine(name);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"sourceLanguage\"");
			request.AppendLine();
			request.AppendLine(sourceLanguage);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"templateId\"");
			request.AppendLine();
			request.AppendLine(templateId.ToString());
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"targetLanguages\"");
			request.AppendLine();
			request.AppendLine(targetLanguage);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"fileProcessType\"");
			request.AppendLine();
			request.AppendLine("JOIN");
			request.AppendLine("--" + boundary);
			var index = 0;
			foreach (var file in Directory.GetFiles(sourceFolder).Where(f => f.EndsWith(".xml")))
			{
				var filename = file.Split("\\".ToCharArray()).Last();
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"translationFiles[" + index++ + "].file\"; filename=\"" + filename + "\"");
				request.AppendLine("Content-Type: text/xml");
				request.AppendLine();
				request.AppendLine(File.ReadAllText(file));
			}
			request.AppendLine("--" + boundary + "--");

			var json = client.UploadString(endpoint + "/projects", "POST", request.ToString());
			var response = Deserialize(json, typeof(ProjectCreateResponse)) as ProjectCreateResponse;
			return response.ProjectId;
		}

		private string GetBoundary()
		{
			var prefix = "--------------------------"; // 26 hyphens
			var content = "";
			while (content.Length < 24)
			{
				content += new Random().NextDouble().ToString().Substring(2);
			}
			return prefix + content.Substring(0, 24);
		}

		private class TemplateDefinition
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string TemplateId { get; set; }
			public string[] Fields { get; set; }
			public bool IsOptIn { get; set; }

			public TemplateDefinition(CmsApi cms, WorklistAsset asset)
			{
				Id = asset.id;
				var fields = cms.Asset.Fields(asset.id);
				IsOptIn = fields.ContainsKey("opt_in") && fields["opt_in"] == "yes";
				Name = asset.label;
				TemplateId = fields.ContainsKey("template") ? fields["template"].Split("/".ToCharArray()).Last() : "";
				Fields = fields.Where(kvp => kvp.Key == "field" || kvp.Key.StartsWith("field:")).Select(kvp => kvp.Value.Trim()).ToArray();
			}
		}

		[DataContract]
		public class GenerateFileResponse
		{
			[DataMember(Name = "fileId")]
			public int FileId { get; set; }
		}

		[DataContract]
		public class FileStatusResponse
		{
			[DataMember(Name = "fileId")]
			public int FileId { get; set; }
			[DataMember(Name = "message")]
			public string Message { get; set; }
			[DataMember(Name = "status")]
			public string Status { get; set; }
		}

		[DataContract]
		public class ProjectCreateResponse
		{
			[DataMember(Name = "projectId")]
			public int ProjectId { get; set; }
			[DataMember(Name = "status")]
			public bool Status { get; set; }
			[DataMember(Name = "jobs")]
			public ProjectCreateJobResponse[] Jobs { get; set; }
		}

		[DataContract]
		public class ProjectCreateJobResponse
		{
			[DataMember(Name = "jobId")]
			public int JobId { get; set; }
			[DataMember(Name = "fileName")]
			public string Filename { get; set; }
			[DataMember(Name = "sourceLanguage")]
			public string SourceLanguage { get; set; }
			[DataMember(Name = "targetLanguage")]
			public string TargetLanguage { get; set; }
		}
	}
}
