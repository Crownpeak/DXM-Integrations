using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using CrownPeak.CMSAPI;
using CrownPeak.CMSAPI.Services;
/* Some Namespaces are not allowed. */
namespace LocalProject
{
	/*
	 * TMF with Translation Support - XTM - version history
	 * ----------------------------------------------
	 * Version | Date       |
	 * ----------------------------------------------
	 * 0.1.0   | 2020-10-19 | Initial release
	 * 0.1.1   | 2020-11-19 | Remove empty fields from XML document sent for translation
	 * 0.2.0   | 2021-05-07 | Support for multi-asset projects
	 * 0.2.1   | 2021-07-14 | Add ConfigPostInput for Access Token generation
	 * 0.2.2   | 2021-07-22 | Stop project name from exceeding 100 character limit
	 * 0.2.3   | 2021-07-28 | Add test mode to webhooks
	 * 0.2.4   | 2021-09-27 | Fix bug with attached files in Visual Mode
	 * 0.2.5   | 2021-09-28 | Fix bug with attached files from projects in Visual Mode
	 * 0.2.6   | 2021-10-04 | Add overwrite support for projects and fix it for individual files
	 * 0.3.0   | 2021-10-07 | Updates to properly support overwrite after translation, relinking, and notification after failure to overwrite
	 */
	public class XtmTranslator : TMF.ITMFTranslator
	{
		private const string XTM_API_ENDPOINT = "xtm_api_endpoint";
		private const string XTM_ACCESS_TOKEN_OPTION = "xtm_access_token_option";
		private const string XTM_ACCESS_TOKEN = "xtm_access_token";
		private const string XTM_GENERATE_TOKEN_CLIENT = "xtm_generate_token_client";
		private const string XTM_GENERATE_TOKEN_USERID = "xtm_generate_token_userid";
		private const string XTM_GENERATE_TOKEN_PASSWORD = "xtm_generate_token_password";
		private const string XTM_GENERATE_TOKEN_INTEGRATION_KEY = "xtm_generate_token_integration_key";
		private const string XTM_CUSTOMER_ID = "xtm_customer_id";
		private const string XTM_TEMPLATE_LIST = "xtm_template_list";
		private const string XTM_TEMPLATE_NAME = "xtm_template_name";
		private const string XTM_TEMPLATE_ID = "xtm_template_id";
		private const string XTM_TMF_FIELD = "tmf_xtm_translate";
		private const string XTM_PROJECT_NAME_FIELD = "name";
		private const string XTM_PROJECT_FIELD = "translation_type";
		private const string XTM_TMF_DUE_DATE_FIELD = "tmf_xtm_translate_due_date";
		private const string XTM_PROJECT_DUE_DATE_FIELD = "due_date";
		private const string XTM_PROJECT_OPTIONS_FIELD = "project_options";
		private const string CROWNPEAK_API_USERNAME = "xtm_crownpeak_api_username";
		private const string CROWNPEAK_API_PASSWORD = "xtm_crownpeak_api_password";
		private const string CROWNPEAK_API_DEVELOPER_KEY = "xtm_crownpeak_api_developer_key";
		private const string CROWNPEAK_API_INCOMING_FOLDER_ID = "xtm_crownpeak_api_folder_id";
		private const string CROWNPEAK_API_MODEL_ID = "xtm_crownpeak_api_model_id";
		private const string XTM_WEBHOOK_URL = "xtm_webhook_url";
		private const string XTM_ERRORS_EMAIL = "xtm_errors_email";
		private const string XTM_RETRY_COUNT = "xtm_retry_count";
		private const string XTM_VISUAL_MODE_HEADER = "xtm_visual_mode_header";
		private const string XTM_VISUAL_MODE_FOOTER = "xtm_visual_mode_footer";
		private Asset _config = null;

		public TMF.ITMFTranslator Init(Asset config)
		{
			_config = config;
			return this;
		}

		public void ConfigInput(Asset asset, InputContext context)
		{
			Input.StartControlPanel("XTM Configuration");
			Input.ShowTextBox("API Endpoint", XTM_API_ENDPOINT);
			var options = new[] {"Enter Token", "Generate Token"}.ToDictionary(t => t);
			Input.StartDropDownContainer("Provide XTM Token", XTM_ACCESS_TOKEN_OPTION, options, options.Values.Select(v => v).First());
			// Enter token
			Input.ShowTextBox("Access Token", XTM_ACCESS_TOKEN);
			Input.NextDropDownContainer();
			// Generate token
			Input.ShowTextBox("Client", XTM_GENERATE_TOKEN_CLIENT);
			Input.ShowTextBox("User ID", XTM_GENERATE_TOKEN_USERID, helpMessage:"This is numeric, and can be found by hovering over the 'i' icon on the right of each row on the Users screen.");
			Input.ShowPassword("Password", XTM_GENERATE_TOKEN_PASSWORD);
			Input.ShowTextBox("Integration Key", XTM_GENERATE_TOKEN_INTEGRATION_KEY, helpMessage:"This is optional, and the value will be provided by XTM if you need to use it.");
			Input.EndDropDownContainer();
			Input.ShowTextBox("Customer Id", XTM_CUSTOMER_ID);
			Input.EndControlPanel();
			Input.StartControlPanel("XTM Template Ids");
			while (Input.NextPanel(XTM_TEMPLATE_LIST, displayName: "XTM Template Ids"))
			{
				Input.ShowTextBox("Name", XTM_TEMPLATE_NAME);
				Input.ShowTextBox("Template Id", XTM_TEMPLATE_ID);
			}
			Input.EndControlPanel();
			Input.StartControlPanel("XTM Webhook Configuration");
			Input.ShowTextBox("Access API Key", CROWNPEAK_API_DEVELOPER_KEY);
			Input.ShowTextBox("Username", CROWNPEAK_API_USERNAME);
			Input.ShowPassword("Password", CROWNPEAK_API_PASSWORD);
			Input.ShowSelectFolder("Incoming Folder", CROWNPEAK_API_INCOMING_FOLDER_ID);
			Input.ShowAcquireDocument("Incoming Model Asset", CROWNPEAK_API_MODEL_ID);
			Input.ShowTextBox("Webhook Url", XTM_WEBHOOK_URL);
			Input.EndControlPanel();
			Input.StartControlPanel("XTM Visual Mode Configuration");
			Input.ShowMessage("To use XTM's Visual Mode, supply a header and footer below.");
			Input.ShowTextBox("Header", XTM_VISUAL_MODE_HEADER, height:5);
			Input.ShowTextBox("Footer", XTM_VISUAL_MODE_FOOTER, height:5);
			Input.EndControlPanel();
			Input.StartControlPanel("XTM Error Handling");
			Input.ShowTextBox("Email Addresses for Error Notification", XTM_ERRORS_EMAIL, helpMessage: "Separate multiple addresses with ; (semi-colon)");
			Input.ShowTextBox("Retry Count for Translation Status", XTM_RETRY_COUNT, helpMessage: "How many times to retry before giving up? Zero means keep retrying.");
			Input.EndControlPanel();
			Input.StartControlPanel("Versions");
			Input.ShowMessage("TMF Translations v0.3.0 (2021-10-07)");
			Input.ShowMessage("TMF XTM Integration v0.3.0 (2021-10-07)");
			Input.EndControlPanel();
		}

		public void ConfigPostInput(Asset asset, PostInputContext context)
		{
			if (!context.ValidationErrorFields.Any() && context.InputForm[XTM_ACCESS_TOKEN_OPTION] == "Generate Token")
			{
				if (!context.InputForm.HasField(XTM_GENERATE_TOKEN_CLIENT) || string.IsNullOrWhiteSpace(context.InputForm[XTM_GENERATE_TOKEN_CLIENT]))
				{
					context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_CLIENT, "Please enter the Client if you are generating a new token.");
				}
				if (!context.InputForm.HasField(XTM_GENERATE_TOKEN_USERID) || string.IsNullOrWhiteSpace(context.InputForm[XTM_GENERATE_TOKEN_USERID]))
				{
					context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_USERID, "Please enter the User ID if you are generating a new token.");
				}
				else
				{
					int temp;
					if (!int.TryParse(context.InputForm[XTM_GENERATE_TOKEN_USERID], out temp))
					{
						context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_USERID, "The User ID must be a number.");
					}
				}
				if (!context.InputForm.HasField(XTM_GENERATE_TOKEN_PASSWORD) || string.IsNullOrWhiteSpace(context.InputForm[XTM_GENERATE_TOKEN_PASSWORD]))
				{
					context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_PASSWORD, "Please enter the Password if you are generating a new token.");
				}
				if (context.ValidationErrorFields.Any()) return;

				// Prepare the credentials for the token generation call
				var creds = new AuthRequest
				{
					Client = context.InputForm[XTM_GENERATE_TOKEN_CLIENT],
					UserId = int.Parse(context.InputForm[XTM_GENERATE_TOKEN_USERID]),
					Password = context.InputForm[XTM_GENERATE_TOKEN_PASSWORD]
				};
				if (context.InputForm.HasField(XTM_GENERATE_TOKEN_INTEGRATION_KEY) && !string.IsNullOrWhiteSpace(context.InputForm[XTM_GENERATE_TOKEN_INTEGRATION_KEY]))
				{
					creds.IntegrationKey = context.InputForm[XTM_GENERATE_TOKEN_INTEGRATION_KEY];
				}

				var endpoint = context.InputForm[XTM_API_ENDPOINT];
				var tokenParms = new PostHttpParams
				{
					ContentType = "application/json",
					PostData = Util.SerializeDataContractJson(creds)
				};
				var tokenResponse = Util.PostHttp(endpoint + "/auth/token", tokenParms);
				if (tokenResponse.StatusCode != 200)
				{
					context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_CLIENT, "Response code " + tokenResponse.StatusCode + " received from XTM when trying to generate a token.");
				}
				else
				{
					var result = Util.DeserializeDataContractJson(tokenResponse.ResponseText, typeof(AuthResponse)) as AuthResponse;
					if (result == null)
					{
						context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_CLIENT, "Empty response received from XTM when trying to generate a token.");
					}
					else if (string.IsNullOrWhiteSpace(result.Token))
					{
						context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_CLIENT, "Empty token received from XTM when trying to generate a token.");
					}
					else
					{
						// Finally make sure that the token is actually valid to get some data
						var parms = new GetHttpParams();
						parms.AddHeader("Authorization: XTM-Basic " + result.Token);
						var projectsResponse = Util.GetHttp(endpoint + "/projects?customerId=9999&page=9999&pageSize=1", parms);
						if (projectsResponse.StatusCode != 200)
						{
							context.ValidationErrorFields.Add(XTM_GENERATE_TOKEN_CLIENT, "Response code " + projectsResponse.StatusCode + " received from XTM when trying to validate the token.");
						}
						else
						{
							// Success!
							context.InputForm[XTM_ACCESS_TOKEN] = result.Token;
							context.InputForm[XTM_ACCESS_TOKEN_OPTION] = "Enter Token";
							context.InputForm.Remove(new [] {XTM_GENERATE_TOKEN_CLIENT, XTM_GENERATE_TOKEN_USERID, XTM_GENERATE_TOKEN_PASSWORD, XTM_GENERATE_TOKEN_INTEGRATION_KEY});
						}
					}
				}
			}
		}

		public void TmfInput(Asset asset, InputContext context, int index, bool isResend)
		{
			var configuration = new XtmConfiguration(_config);
			Input.StartControlPanel("XTM");
			var re = isResend ? "re-" : "";
			var radios = new Dictionary<string, string>
			{
				{ "Do not send for " + re + "translation", "" },
			};
			foreach (var template in configuration.Templates)
			{
				radios.Add(string.Format("Send for {0} {1}translation", template.Name.ToLowerInvariant(), re), template.Id);
			}
			Input.ShowRadioButton("", XTM_TMF_FIELD + ":" + index, radios, radios.Values.First());
			Input.ShowSelectDate("Translation due date (optional)", XTM_TMF_DUE_DATE_FIELD + ":" + index);
			Input.EndControlPanel();
		}

		public void ProjectInput(Asset asset, InputContext context)
		{
			var config = new XtmConfiguration(_config);
			var radios = config.Templates.ToDictionary(t => t.Name + " translation", t => t.Id);
			Input.ShowRadioButton("", XTM_PROJECT_FIELD, radios, radios.Values.First());
			Input.ShowSelectDate("Translation due date (optional)", XTM_PROJECT_DUE_DATE_FIELD);
			var options = new Dictionary<string, string>
			{
				{"No options", "none"},
				{"Join Files", "join_files"}
			};
			var helpMessage = "";
			if (!string.IsNullOrWhiteSpace(config.VisualModeHeader) && !string.IsNullOrWhiteSpace(config.VisualModeFooter))
			{
				options.Add("Visual Mode", "visual_mode");
			}
			else
			{
				helpMessage = "Visual Mode is not configured and therefore not available.";
			}
			Input.ShowRadioButton("Project options", XTM_PROJECT_OPTIONS_FIELD, options, options.First().Value, helpMessage);
			var yesno = new Dictionary<string, string>
			{
				{ "Yes", "y" },
				{ "No", "n" }
			};
			Input.ShowRadioButton("Overwrite existing translation asset(s) (if applicable)", "overwrite_existing_asset", yesno, "n");
		}

		public void ProjectPostInput(Asset asset, PostInputContext context)
		{
			
		}

		public Dictionary<string, string> TmfInputValues(Asset asset, PanelEntry panel, int index)
		{
			if (panel != null)
			{
				return new Dictionary<string, string>
				{
					{XTM_TMF_FIELD, panel.Raw[XTM_TMF_FIELD]},
					{XTM_TMF_DUE_DATE_FIELD, panel.Raw[XTM_TMF_DUE_DATE_FIELD]}
				};
			}
			return new Dictionary<string, string>
			{
				{XTM_TMF_FIELD, asset.Raw[XTM_TMF_FIELD]},
				{XTM_TMF_DUE_DATE_FIELD, asset.Raw[XTM_TMF_DUE_DATE_FIELD]}
			};
		}

		public bool IsTranslationRequired(Asset asset, PanelEntry panel, int index)
		{
			if (panel != null)
			{
				return !string.IsNullOrWhiteSpace(panel.Raw[XTM_TMF_FIELD]) && panel.Raw[XTM_TMF_FIELD] != "none";
			}
			return !string.IsNullOrWhiteSpace(asset.Raw[XTM_TMF_FIELD]) && asset.Raw[XTM_TMF_FIELD] != "none";
		}

		public void TranslateAsset(Dictionary<string, string> inputValues, Asset source, Asset destination, bool overwrite, Context context)
		{
			try
			{
				if (_config == null || !_config.IsLoaded)
				{
					throw new Exception("TranslateAsset: Unable to find translation config.");
				}

				if (!inputValues.ContainsKey(XTM_TMF_FIELD))
				{
					throw new Exception("TranslateAsset: Unable to find translation type field.");
				}
				if (!inputValues.ContainsKey(XTM_TMF_DUE_DATE_FIELD))
				{
					throw new Exception("TranslateAsset: Unable to find translation due date field.");
				}

				var type = inputValues[XTM_TMF_FIELD];
				if (string.IsNullOrWhiteSpace(type) || type == "none") return;
				
				DateTime? dueDate = null;
				if (!string.IsNullOrWhiteSpace(inputValues[XTM_TMF_DUE_DATE_FIELD]))
					dueDate = DateTime.ParseExact(inputValues[XTM_TMF_DUE_DATE_FIELD], "M/d/yyyy", null);

				var configuration = new XtmConfiguration(_config);

				// Get the locales for the source and destination assets
				var sitePath = TMF.GetSitePath(source).AssetPath.ToString();
				var locales = TMF.CreateLocaleConfigCache(sitePath).ToArray();
				var sourceLocale = Asset.Load(TMF.GetLocaleId(source, sitePath, locales)).Raw["locale_code"];
				if (string.IsNullOrWhiteSpace(sourceLocale))
				{
					throw new Exception("TranslateAsset: No source locale code found.");
				}

				var destLocale = Asset.Load(TMF.GetLocaleId(destination, sitePath, locales)).Raw["locale_code"];
				if (string.IsNullOrWhiteSpace(destLocale))
				{
					throw new Exception("TranslateAsset: No destination locale code found.");
				}

				// Set the hash on the destination so we can identify later edits
				TMF.Translation.SaveHash(destination);

				var data = new XtmTranslationData(source);
				var response = SendForTranslation(type, data, configuration, source, sourceLocale, destination, destLocale, dueDate);

				if (response == null)
				{
					throw new Exception("TranslateAsset: Null response received from SendForTranslation.");
				}

				CreateLog(type, source, sourceLocale, destination, destLocale, overwrite, context, response);
			}
			catch (Exception ex)
			{
				SendErrorNotification(source, destination, "", ex);
			}
		}

		public void TranslateProject(Asset project, Asset[] sources, string[] targetLocales, string[][] targets, bool overwrite, Context context)
		{
			try
			{
				if (_config == null || !_config.IsLoaded)
				{
					throw new Exception("TranslateAsset: Unable to find translation config.");
				}

				var type = project[XTM_PROJECT_FIELD];
				if (string.IsNullOrWhiteSpace(type) || type == "none") return;

				DateTime? dueDate = null;
				if (!string.IsNullOrWhiteSpace(project[XTM_PROJECT_DUE_DATE_FIELD]))
					dueDate = DateTime.ParseExact(project[XTM_PROJECT_DUE_DATE_FIELD], "M/d/yyyy", null);

				var joinFiles = project[XTM_PROJECT_OPTIONS_FIELD] == "join_files";
				var visualMode = project[XTM_PROJECT_OPTIONS_FIELD] == "visual_mode";

				var configuration = new XtmConfiguration(_config);

				// Get the locales for the source and destination assets
				var sitePath = TMF.GetSitePath(sources[0]).AssetPath.ToString();
				var locales = TMF.CreateLocaleConfigCache(sitePath).ToArray();
				var sourceLocale = Asset.Load(TMF.GetLocaleId(sources[0], sitePath, locales)).Raw["locale_code"];
				if (string.IsNullOrWhiteSpace(sourceLocale))
				{
					throw new Exception("TranslateAsset: No source locale code found.");
				}

				// Set the hash on the destinations so we can identify later edits
				foreach (var targetCollection in targets)
				{
					foreach (var target in targetCollection)
					{
						TMF.Translation.SaveHash(Asset.LoadDirect(target));
					}
				}

				var data = sources.Select(s => new XtmTranslationData(s)).ToArray();
				var projectId = project.Id.ToString();
				var projectName = project[XTM_PROJECT_NAME_FIELD];
				var response = SendFilesForTranslation(type, data, configuration, projectId, projectName, sources, sourceLocale, targetLocales, dueDate, joinFiles, visualMode);

				if (response == null)
				{
					throw new Exception("TranslateAsset: Null response received from SendForTranslation.");
				}

				CreateProjectLog(project, sources, targetLocales, targets, overwrite, context, response);
			}
			catch (Exception ex)
			{
				//Out.WriteLine("DEBUG: " + ex.Message);
				// TODO: what items to use here?
				SendErrorNotification("TranslateProject error: " + ex.Message);
			}
		}
		public Asset CreateLog(string type, Asset source, string sourceLocale, Asset destination, string destLocale, bool overwrite, Context context, object translationResponse)
		{
			var response = translationResponse as XtmMyProjectCreateResponse;
			if (response == null)
			{
				throw new Exception("CreateLog: TranslationResponse object is not of the correct type.");
			}

			var date = DateTime.UtcNow.ToString("o");
			var data = new Dictionary<string, string>
			{
				{ "translation_provider", "xtm" },
				{ "source_id", source.Id.ToString() },
				{ "source_locale", sourceLocale },
				{ "dest_id", destination.Id.ToString() },
				{ "dest_locale", destLocale },
				{ "last_update", date },
				{ "statuses", "0" },
				{ "http_status_code", response.StatusCode.ToString() },
				{ "http_status_description", response.StatusDescription },
				{ "raw_response", "N/A" },
				{ "status_date", date },
				{ "project_id", response.ProjectId.ToString() },
				{ "job_id", response.JobId.ToString() },
				{ "overwrite", overwrite ? "true" : "false" },
				{ "user_name", context.UserInfo.Username },
				{ "status", "" }
			};
			var label = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fffff") + "-" + source.Id + "-" + destination.Id;
			var folder = Asset.Load(TMF.GetSitePath(source).AssetPath + "/Translation Logs");

			// Find the model we want to use
			var fp = new FilterParams { Limit = 1, ExcludeProjectTypes = false };
			fp.Add(Comparison.Equals, AssetType.File);
			fp.Add(AssetPropertyNames.Label, Comparison.Equals, "Translation Log");
			var model = Asset.GetProject(source).GetFilterList(fp).FirstOrDefault();

			if (model != null)
			{
				return Asset.CreateNewAsset(label, folder, model, data);
			}
			else
			{
				throw new Exception("CreateLog: Unable to find Model.");
			}
		}

		public Asset CreateProjectLog(Asset project, Asset[] sources, string[] targetLocales, string[][] targets, bool overwrite, Context context, object translationResponse)
		{
			var response = translationResponse as XtmMyProjectCreateResponse;
			if (response == null)
			{
				throw new Exception("CreateProjectLog: TranslationResponse object is not of the correct type.");
			}

			var sitePath = TMF.GetSitePath(sources[0]).AssetPath.ToString();
			var locales = TMF.CreateLocaleConfigCache(sitePath).ToArray();

			var date = DateTime.UtcNow.ToString("o");
			var data = new Dictionary<string, string>
			{
				{ "translation_provider", "xtm" },
				{ "type", "project" },
				{ "source_locale", Asset.Load(TMF.GetLocaleId(sources[0], sitePath, locales)).Raw["locale_code"] },
				{ "last_update", date },
				{ "statuses", "0" },
				{ "http_status_code", response.StatusCode.ToString() },
				{ "http_status_description", response.StatusDescription },
				{ "raw_response", "N/A" },
				{ "status_date", date },
				{ "project_id", response.ProjectId.ToString() },
				{ "job_id", response.JobId.ToString() },
				{ "overwrite", overwrite ? "true" : "false" },
				{ "user_name", context.UserInfo.Username },
				{ "status", "" }
			};
			var index = 1;
			foreach (var source in sources)
			{
				data.Add("source_id:" + index, source.Id.ToString());
				var subindex = 1;
				foreach (var targetLocale in targetLocales)
				{
					data.Add("dest_id:" + subindex + ":" + index, targets[index - 1][subindex - 1]);
					data.Add("dest_locale:" + subindex + ":" + index, targetLocale);
					subindex++;
				}
				index++;
			}
			var label = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fffff") + "-" + project["name"];
			var folder = Asset.Load(TMF.GetSitePath(sources.First()).AssetPath + "/Translation Logs");

			// Find the model we want to use
			var fp = new FilterParams { Limit = 1, ExcludeProjectTypes = false };
			fp.Add(Comparison.Equals, AssetType.File);
			fp.Add(AssetPropertyNames.Label, Comparison.Equals, "Translation Log");
			var model = Asset.GetProject(sources.First()).GetFilterList(fp).FirstOrDefault();

			if (model != null)
			{
				return Asset.CreateNewAsset(label, folder, model, data);
			}
			else
			{
				throw new Exception("CreateProjectLog: Unable to find Model.");
			}
		}

		public void UpdateLogMetrics(Asset log)
		{
			try
			{
				var configuration = new XtmConfiguration(_config);
				int characters, words, segments, uniqueWords, uniqueSegments;
				GetProjectMetrics(configuration, log["project_id"], out characters, out words, out segments, out uniqueWords, out uniqueSegments);

				var data = new Dictionary<string, string>
				{
					{"count_characters", characters.ToString()},
					{"count_segments", segments.ToString()},
					{"count_segments_unique", uniqueSegments.ToString()},
					{"count_words", words.ToString()},
					{"count_words_unique", uniqueWords.ToString()}
				};
				log.SaveContent(data);
			}
			catch (Exception ex)
			{
				// Reload the log after
				SendErrorNotification(log, ex);
				UpdateLogError(log, ex);
			}
		}

		public void UpdateLog(Asset log)
		{
			var configuration = new XtmConfiguration(_config);
			try
			{
				// We've already been marked as failed
				if (log.Raw["failed"] == "true") return;
				var response = GetTranslationStatus(configuration, log.Raw["project_id"]);
				if (log.Raw["type"] != "project" && !string.IsNullOrWhiteSpace(response.XmlResponse))
				{
					ProcessTranslationComplete(response.ReferenceId, log.Raw["project_id"], response.XmlResponse, false);
				}
				else if (response.XmlNames != null && response.XmlNames.Length > 0)
				{
					ProcessProjectTranslationComplete(log.Raw["project_id"], response.XmlNames, response.XmlResponses, false);
				}
				UpdateLog(log, response);
			}
			catch (Exception ex)
			{
				// Reload the log after
				SendErrorNotification(log, ex);
				UpdateLogError(log, ex);
			}
		}

		private void UpdateLog(Asset log, object translationResponse)
		{
			var response = translationResponse as XtmProjectStatusResponse;
			var status = "";
			if (response == null)
			{
				status = "TranslationResponse object is not of the correct type.";
			}
			else
			{
				status = response.FinishDate > 0 ? "Complete" : "";
			}
			var fields = log.GetContent();
			var nextStatus = 2;
			var data = new Dictionary<string, string>();
			if (fields.ContainsKey("statuses"))
			{
				// This has only one status panel at present, so copy with an index
				data.Add("statuses:1", "0");
				data.Add("status:1", fields["status"]);
				data.Add("status_date:1", fields["status_date"]);

				var fieldsToRemove = new[] {"statuses", "status", "status_date"};
				log.DeleteContentFields(fieldsToRemove.ToList());
			}
			else
			{
				while (fields.ContainsKey("statuses:" + nextStatus)) nextStatus++;
			}
			var date = DateTime.UtcNow.ToString("o");
			data.Add("last_update", date);
			data.Add("statuses:" + nextStatus, (nextStatus - 1).ToString());
			data.Add("status:" + nextStatus, status);
			data.Add("status_date:" + nextStatus, date);
			data.Add("completed", status == "Complete" ? "true" : "false");
			if (status == "Complete") data.Add("complete_date", date);

			if (status != "Complete")
			{
				var count = GetMaxRetryCount();
				// We've already run too many times
				if (count > 0 && !fields.ContainsKey("failed") && fields.ContainsKey("statuses:" + count))
				{
					data.Add("failed", "true");
				}
			}

			log.SaveContent(data);

			if (data.ContainsKey("failed"))
			{
				throw new Exception("UpdateLog: Translation has taken too many retries.");
			}
		}

		private void UpdateLogError(Asset log, Exception ex)
		{
			var status = ex.Message;
			var fields = log.GetContent();
			var nextStatus = 2;
			var data = new Dictionary<string, string>();
			if (fields.ContainsKey("statuses"))
			{
				// This has only one status panel at present, so copy with an index
				data.Add("statuses:1", "0");
				data.Add("status:1", fields["status"]);
				data.Add("status_date:1", fields["status_date"]);

				var fieldsToRemove = new[] { "statuses", "status", "status_date" };
				log.DeleteContentFields(fieldsToRemove.ToList());
			}
			else
			{
				while (fields.ContainsKey("statuses:" + nextStatus)) nextStatus++;
			}
			var date = DateTime.UtcNow.ToString("o");
			data.Add("last_update", date);
			data.Add("statuses:" + nextStatus, (nextStatus - 1).ToString());
			data.Add("status:" + nextStatus, status);
			data.Add("status_date:" + nextStatus, date);

			var count = GetMaxRetryCount();
			// We've already run too many times
			if (count > 0 && !fields.ContainsKey("failed") && fields.ContainsKey("statuses:" + count))
			{
				data.Add("failed", "true");
			}

			log.SaveContent(data);

			if (data.ContainsKey("failed"))
			{
				SendErrorNotification(log, new Exception("UpdateLogError: Translation has taken too many retries."));
			}
		}

		public void DisplayLog(Asset log)
		{
			log.GetContent();

			if (log.Raw["completed"] != "true")
			{
				UpdateLog(log);
				log.GetContent();
			}
			if (string.IsNullOrWhiteSpace(log.Raw["count_characters"]))
			{
				UpdateLogMetrics(log);
				log.GetContent();
			}
			Out.WriteLine("<table cellspacing=\"0\" cellpadding=\"2\" border=\"1\">");
			Out.WriteLine("<tr><td>Translation provider:</td><td>XTM</td></tr>");
			if (log.Raw["type"] == "project")
			{
				var sources = string.Join(", ", log.GetPanels("source_id").Select(s => Asset.Load(s["source_id"]).AssetPath));
				var destLocales = string.Join(", ", log.GetPanels("source_id").SelectMany(s => s.GetPanels("dest_locale").Select(d => d["dest_locale"])).GroupBy(g => g).Select(g => g.First()));
				var dests = string.Join(", ", log.GetPanels("source_id").Select(s => string.Join(", ", s.GetPanels("dest_id").Select(d => Asset.Load(d["dest_id"]).AssetPath))));
				Out.WriteLine("<tr><td>Source assets:</td><td>" + sources + "</td></tr>");
				Out.WriteLine("<tr><td>Source locale:</td><td>" + log["source_locale"] + "</td></tr>");
				Out.WriteLine("<tr><td>Destination assets:</td><td>" + dests + "</td></tr>");
				Out.WriteLine("<tr><td>Destination locales:</td><td>" + destLocales + "</td></tr>");
			}
			else
			{
				var source = Asset.Load(log["source_id"]);
				var dest = Asset.Load(log["dest_id"]);
				Out.WriteLine("<tr><td>Source asset:</td><td>" + source.AssetPath + "</td></tr>");
				Out.WriteLine("<tr><td>Source locale:</td><td>" + log["source_locale"] + "</td></tr>");
				Out.WriteLine("<tr><td>Destination asset:</td><td>" + dest.AssetPath + "</td></tr>");
				Out.WriteLine("<tr><td>Destination locale:</td><td>" + log["dest_locale"] + "</td></tr>");
			}
			var user = User.Load(log["user_name"]);
			if (user != null)
				Out.WriteLine("<tr><td>User:</td><td>" + user.Email + "</td></tr>");
			Out.WriteLine("<tr><td>Xtm project id:</td><td>" + log["project_id"] + "</td></tr>");
			Out.WriteLine("<tr><td>Xtm job id:</td><td>" + log["job_id"] + "</td></tr>");
			if (!string.IsNullOrWhiteSpace(log.Raw["count_characters"]))
			{ 
				Out.WriteLine("<tr><td>Characters:</td><td>" + log["count_characters"] + "</td></tr>");
				Out.WriteLine("<tr><td>Words:</td><td>" + log["count_words"] + " (" + log["count_words_unique"] + " unique)</td></tr>");
				Out.WriteLine("<tr><td>Segments:</td><td>" + log["count_segments"] + " (" + log["count_segments_unique"] + " unique)</td></tr>");
			}
			Out.WriteLine("<tr><td>Last update:</td><td>" + log["last_update"] + "</td></tr>");
			Out.WriteLine("<tr><td>HTTP response status:</td><td>" + log["http_status_code"] + " " + log["http_status_description"] + "</td></tr>");
			Out.WriteLine("</table>");
			Out.WriteLine("<table cellspacing=\"0\" cellpadding=\"2\" border=\"1\">");
			Out.WriteLine("<tr><th>Date</th><th>Status</th></tr>");
			foreach (var panel in log.GetPanels("statuses"))
			{
				Out.WriteLine("<tr><td>" + panel["status_date"] + "</td><td>" + panel["status"] + "</td></tr>");
			}
			Out.WriteLine("</table>");
		}

		private int GetMaxRetryCount()
		{
			if (_config != null && _config.IsLoaded)
			{
				int count;
				if (int.TryParse(_config.Raw[XTM_RETRY_COUNT], out count))
				{
					return count >= 0 ? count : 0;
				}
			}

			return 0;
		}

		private Asset FindLogByProjectId(Asset asset, string documentId)
		{
			var fp = new FilterParams
			{
				Limit = 1,
				SortOrder = SortOrder.OrderByDescending(AssetPropertyNames.Id)
			};
			fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "TMF Translation Log");
			fp.Add("project_id", Comparison.Equals, documentId);
			var log = Asset.GetSiteRoot(asset).GetFilterList(fp).FirstOrDefault();
			return log ?? Asset.Load(-1);
		}

		private XtmProjectStatusResponse GetTranslationStatus(XtmConfiguration config, string projectId)
		{
			var parms = new GetHttpParams
			{
				TimeOut = 60
			};
			parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);
			var response = Util.GetHttp(config.EndPoint + "/projects/" + projectId, parms);
			if (response.StatusCode < 200 || response.StatusCode > 299)
			{
				throw new Exception("GetTranslationStatus: Response code " + response.StatusCode + " received from XTM.");
			}

			var xtmResponse = Util.DeserializeDataContractJson(response.ResponseText, typeof(XtmProjectStatusResponse)) as XtmProjectStatusResponse;
			if (xtmResponse == null)
			{
				throw new Exception("GetTranslationStatus: Failed to parse JSON from XTM response.");
				xtmResponse = new XtmProjectStatusResponse
				{
					ErrorMessage = "Failed to parse JSON from XTM response"
				};
			}

			xtmResponse.StatusCode = response.StatusCode;
			xtmResponse.StatusDescription = response.StatusDescription;

			if (xtmResponse.FinishDate > 0)
			{
				// Fetch the xml for processing the response
				parms = new GetHttpParams
				{
					TimeOut = 60,
				};
				var url = config.CallbackUrl;
				if (url.IndexOf("?") > 0) url += "&";
				else url += "?";
				url += "xtmProjectId=" + projectId + "&xtmCustomerId=" + xtmResponse.CustomerId + "&inline=true";
				response = Util.GetHttp(url, parms);
				var text = response.ResponseText.Trim();
				if (text.IndexOf(".xml\n") > 0)
				{
					var names = new List<string>();
					var xmls = new List<string>();
					var lines = text.Replace("\r", "").Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					var xml = "";
					foreach (var line in lines)
					{
						if (line.EndsWith(".xml"))
						{
							names.Add(line);
							if (xml != "") xmls.Add(xml);
							xml = "";
						}
						else
						{
							xml += line;
						}
					}
					if (xml != "") xmls.Add(xml);
					xtmResponse.XmlResponse = xmls.First();
					xtmResponse.XmlNames = names.ToArray();
					xtmResponse.XmlResponses = xmls.ToArray();
				}
				else
				{
					xtmResponse.XmlResponse = text;
					xtmResponse.XmlNames = new[] { "" };
					xtmResponse.XmlResponses = new[] {text};
				}
			}

			return xtmResponse;
		}

		private void GetProjectMetrics(XtmConfiguration config, string projectId, out int characters, out int words, out int segments, out int uniqueWords, out int uniqueSegments)
		{
			characters = words = segments = uniqueWords = uniqueSegments = 0;
			var parms = new GetHttpParams
			{
				TimeOut = 60
			};
			parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);
			var response = Util.GetHttp(config.EndPoint + "/projects/" + projectId + "/metrics", parms);
			if (response.StatusCode < 200 || response.StatusCode > 299)
			{
				throw new Exception("GetTranslationStatus: Response code " + response.StatusCode + " received from XTM.");
			}

			var xtmResponse = Util.DeserializeDataContractJson(response.ResponseText, typeof(XtmProjectMetricsResponse[])) as XtmProjectMetricsResponse[];
			if (xtmResponse == null)
			{
				throw new Exception("GetProjectMetrics: Failed to parse JSON from XTM response.");
			}
			else if (xtmResponse.Length == 0)
			{
				throw new Exception("GetProjectMetrics: No metrics returned.");
			}

			characters = xtmResponse[0].CoreMetrics.TotalCharacters;
			words = xtmResponse[0].CoreMetrics.TotalWords;
			segments = xtmResponse[0].CoreMetrics.TotalSegments;
			uniqueWords = xtmResponse[0].CoreMetrics.TotalWords - xtmResponse[0].CoreMetrics.RepeatWords;
			uniqueSegments = xtmResponse[0].CoreMetrics.TotalSegments - xtmResponse[0].CoreMetrics.RepeatSegments;
		}

		private XtmMyProjectCreateResponse SendForTranslation(string type, XtmTranslationData data, XtmConfiguration config, Asset source, string sourceLocale, Asset destination, string destLocale, DateTime? dueDate)
		{
			var documentId = source.Id + "|" + destination.Id + "|" + destLocale + "|" + DateTime.UtcNow.ToString("o");

			var boundary = GetBoundary();

			var request = new StringBuilder(10240);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"referenceId\"");
			request.AppendLine();
			request.AppendLine(documentId);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"customerId\"");
			request.AppendLine();
			request.AppendLine(config.CustomerId);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"name\"");
			request.AppendLine();
      var namePrefix = "Translate asset '";
      var nameSuffix = "' (" + source.Id + ")";
      var name = namePrefix + source.Label + nameSuffix;
      if (name.Length > 100)
      {
      	name = namePrefix + source.Label.Substring(0, 97 - namePrefix.Length - nameSuffix.Length) + "..." + nameSuffix;
      }
			request.AppendLine(name);
			if (dueDate.HasValue)
			{
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"dueDate\"");
				request.AppendLine();
				request.AppendLine(dueDate.Value.ToString("o"));
			}
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"sourceLanguage\"");
			request.AppendLine();
			request.AppendLine(sourceLocale.Replace('-', '_'));
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"targetLanguages\"");
			request.AppendLine();
			request.AppendLine(destLocale.Replace('-', '_'));
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"templateId\"");
			request.AppendLine();
			request.AppendLine(type);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"callbacks.projectFinishedCallback\"");
			request.AppendLine();
			request.AppendLine(config.CallbackUrl);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"translationFiles[0].file\"; filename=\"content.xml\"");
			request.AppendLine("Content-Type: text/xml");
			request.AppendLine();
			request.AppendLine(data.Serialize());
			request.AppendLine("--" + boundary + "--");

			var parms = new PostHttpParams
			{
				TimeOut = 60,
				ContentType = "multipart/form-data; boundary=" + boundary,
				PostData = request.ToString()
			};
			parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);
			
			var response = Util.PostHttp(config.EndPoint + "/projects", parms);
			if (response.StatusCode < 200 || response.StatusCode > 299)
			{
				throw new Exception("SendForTranslation: Response code " + response.StatusCode + " received from XTM.");
			}

			XtmMyProjectCreateResponse myResponse;
			var xtmResponse = Util.DeserializeDataContractJson(response.ResponseText, typeof(XtmProjectCreateResponse)) as XtmProjectCreateResponse;
			if (xtmResponse == null)
			{
				throw new Exception("SendForTranslation: Failed to parse JSON from XTM response.");
				myResponse = new XtmMyProjectCreateResponse
				{
					ErrorMessage = "Failed to parse JSON from XTM response"
				};
			}
			else
			{
				myResponse = new XtmMyProjectCreateResponse
				{
					ProjectId = xtmResponse.ProjectId,
					Name = xtmResponse.Name,
					JobId = xtmResponse.Jobs.First().JobId
				};
			}
			myResponse.StatusCode = response.StatusCode;
			myResponse.StatusDescription = response.StatusDescription;

			if (!string.IsNullOrWhiteSpace(myResponse.Name) && !string.IsNullOrWhiteSpace(config.VisualModeHeader) && !string.IsNullOrWhiteSpace(config.VisualModeFooter))
			{
				// First wait for the project analysis to complete
				// TODO:

				// Now upload the preview file
				boundary = GetBoundary();

				// Pull out any images
				var images = new StringBuilder(10240);
				var content = source.Show();
				var regexes = new[]
				{
					new Regex("\\/(?<cms>[A-Z][A-Za-z0-9]+)\\/upload\\/([a-z0-9\\/]*)(?<file>easset_[a-z0-9_]+\\.[a-z0-9]+)"),
					new Regex("\\/(?<cms>[A-Z][A-Za-z0-9]+)\\/cpt_internal\\/(([0-9]*\\/)*)(?<id>[0-9]+)")
				};
				var counter = 1;
				foreach (var regex in regexes)
				{
					var match = regex.Match(content);
					while (match.Success)
					{
						var img = BinaryFile.LoadAsBase64(match.Value);
						if (!string.IsNullOrWhiteSpace(img))
						{
							string extension;
							var mimeType = GetMimeType(match.Value, out extension);
							if (!string.IsNullOrWhiteSpace(mimeType))
							{
								// TODO: This should work but I can't convert a byte array to a string without needing encoding
								//images.AppendLine("--" + boundary);
								//images.AppendLine("Content-Disposition: form-data; name=\"previewFiles[" + counter + "].file\"; filename=\"image" + counter + "." + extension + "\"");
								//images.AppendLine("Content-Type: " + mimeType);
								//images.AppendLine();
								//images.Append(Convert.FromBase64String(img)).AppendLine();
								// Replace all occurrences of this string in the resulting image to avoid sending duplicate images
								//content = content.Replace(match.Value, "image" + counter + "." + extension);

								// Workaround using data-uris which does work
								content = content.Replace(match.Value, "data:" + mimeType + ";base64," + img);
								counter++;
							}
						}
						match = regex.Match(content, match.Index + 1);
					}
				}

				request.Clear();
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"previewFiles[0].file\"; filename=\"content.html\"");
				request.AppendLine("Content-Type: text/html");
				request.AppendLine();
				request.AppendLine(config.VisualModeHeader + content + config.VisualModeFooter);
				request.Append(images);
				request.AppendLine("--" + boundary + "--");

				parms = new PostHttpParams
				{
					TimeOut = 60,
					ContentType = "multipart/form-data; boundary=" + boundary,
					PostData = request.ToString()
				};
				parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);

				response = Util.PostHttp(config.EndPoint + "/projects/" + myResponse.ProjectId + "/files/preview-files/upload", parms);
				if (response.StatusCode < 200 || response.StatusCode > 299)
				{
					throw new Exception("SendForTranslation: Response code " + response.StatusCode + " received from XTM upload preview.");
				}
			}

			return myResponse;
		}

		private XtmMyProjectCreateResponse SendFilesForTranslation(string type, XtmTranslationData[] data, XtmConfiguration config, string projectId, string projectName, Asset[] sources, string sourceLocale, string[] destLocales, DateTime? dueDate, bool joinFiles, bool visualMode)
		{
			var boundary = GetBoundary();

			var request = new StringBuilder(10240);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"customerId\"");
			request.AppendLine();
			request.AppendLine(config.CustomerId);
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"name\"");
			request.AppendLine();
			request.AppendLine(projectName);
			if (dueDate.HasValue)
			{
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"dueDate\"");
				request.AppendLine();
				request.AppendLine(dueDate.Value.ToString("o"));
			}
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"sourceLanguage\"");
			request.AppendLine();
			request.AppendLine(sourceLocale.Replace('-', '_'));
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"targetLanguages\"");
			request.AppendLine();
			request.AppendLine(string.Join(",", destLocales.Select(d => d.Replace('-', '_'))));
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"templateId\"");
			request.AppendLine();
			request.AppendLine(type);
			if (joinFiles)
			{
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"fileProcessType\"");
				request.AppendLine();
				request.AppendLine("JOIN");
			}
			request.AppendLine("--" + boundary);
			request.AppendLine("Content-Disposition: form-data; name=\"callbacks.projectFinishedCallback\"");
			request.AppendLine();
			//var callbackUrl = config.CallbackUrl;
			//callbackUrl += callbackUrl.Contains("?") ? "&" : "?"
			//	+ "cmsProjectId=" + projectId;
			//request.AppendLine(callbackUrl);
			request.AppendLine(config.CallbackUrl);
			int i = 0;
			foreach (var item in data)
			{
				request.AppendLine("--" + boundary);
				request.AppendLine("Content-Disposition: form-data; name=\"translationFiles[" + i + "].file\"; filename=\"" + sources[i].Id + ".xml\"");
				request.AppendLine("Content-Type: text/xml");
				request.AppendLine();
				request.AppendLine(item.Serialize());
				i++; 
			}
			request.AppendLine("--" + boundary + "--");

			var parms = new PostHttpParams
			{
				TimeOut = 60,
				ContentType = "multipart/form-data; boundary=" + boundary,
				PostData = request.ToString()
			};
			parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);

			var response = Util.PostHttp(config.EndPoint + "/projects", parms);
			if (response.StatusCode < 200 || response.StatusCode > 299)
			{
				throw new Exception("SendForTranslation: Response code " + response.StatusCode + " received from XTM.");
			}

			XtmMyProjectCreateResponse myResponse;
			var xtmResponse = Util.DeserializeDataContractJson(response.ResponseText, typeof(XtmProjectCreateResponse)) as XtmProjectCreateResponse;
			if (xtmResponse == null)
			{
				throw new Exception("SendForTranslation: Failed to parse JSON from XTM response.");
				myResponse = new XtmMyProjectCreateResponse
				{
					ErrorMessage = "Failed to parse JSON from XTM response"
				};
			}
			else
			{
				myResponse = new XtmMyProjectCreateResponse
				{
					ProjectId = xtmResponse.ProjectId,
					Name = xtmResponse.Name,
					JobId = xtmResponse.Jobs.First().JobId
				};
			}
			myResponse.StatusCode = response.StatusCode;
			myResponse.StatusDescription = response.StatusDescription;

			if (!joinFiles && visualMode && !string.IsNullOrWhiteSpace(myResponse.Name) && !string.IsNullOrWhiteSpace(config.VisualModeHeader) && !string.IsNullOrWhiteSpace(config.VisualModeFooter))
			{
				// First wait for the project analysis to complete
				// TODO:

				// Now upload the preview file
				boundary = GetBoundary();

				i = 0;
				request.Clear();
				foreach (var source in sources)
				{
					// Pull out any images
					var images = new StringBuilder(10240);
					var content = source.Show();
					var regexes = new[]
					{
						new Regex("\\/(?<cms>[A-Z][A-Za-z0-9]+)\\/upload\\/([a-z0-9\\/]*)(?<file>easset_[a-z0-9_]+\\.[a-z0-9]+)"),
						new Regex("\\/(?<cms>[A-Z][A-Za-z0-9]+)\\/cpt_internal\\/(([0-9]*\\/)*)(?<id>[0-9]+)")
					};
					var counter = 1;
					foreach (var regex in regexes)
					{
						var match = regex.Match(content);
						while (match.Success)
						{
							var img = BinaryFile.LoadAsBase64(match.Value);
							if (!string.IsNullOrWhiteSpace(img))
							{
								string extension;
								var mimeType = GetMimeType(match.Value, out extension);
								if (!string.IsNullOrWhiteSpace(mimeType))
								{
									// TODO: This should work but I can't convert a byte array to a string without needing encoding
									//images.AppendLine("--" + boundary);
									//images.AppendLine("Content-Disposition: form-data; name=\"previewFiles[" + counter + "].file\"; filename=\"image" + counter + "." + extension + "\"");
									//images.AppendLine("Content-Type: " + mimeType);
									//images.AppendLine();
									//images.Append(Convert.FromBase64String(img)).AppendLine();
									// Replace all occurrences of this string in the resulting image to avoid sending duplicate images
									//content = content.Replace(match.Value, "image" + counter + "." + extension);

									// Workaround using data-uris which does work
									content = content.Replace(match.Value, "data:" + mimeType + ";base64," + img);
									counter++;
								}
							}

							match = regex.Match(content, match.Index + 1);
						}
					}

					request.AppendLine("--" + boundary);
					request.AppendLine("Content-Disposition: form-data; name=\"previewFiles[" + i + "].file\"; filename=\"" + sources[i].Id + ".html\"");
					request.AppendLine("Content-Type: text/html");
					request.AppendLine();
					request.AppendLine(config.VisualModeHeader + content + config.VisualModeFooter);
					request.Append(images);
					i++;
				}
				request.AppendLine("--" + boundary + "--");

				parms = new PostHttpParams
				{
					TimeOut = 60,
					ContentType = "multipart/form-data; boundary=" + boundary,
					PostData = request.ToString()
				};
				parms.AddHeader("Authorization: XTM-Basic " + config.AccessToken);

				response = Util.PostHttp(config.EndPoint + "/projects/" + myResponse.ProjectId + "/files/preview-files/upload", parms);
				if (response.StatusCode < 200 || response.StatusCode > 299)
				{
					throw new Exception("SendForTranslation: Response code " + response.StatusCode + " received from XTM upload preview.");
				}
			}

			return myResponse;
		}

		private string GetMimeType(string link, out string extension)
		{
			extension = "";
			if (link.IndexOf(".") >= 0)
			{
				extension = link.Substring(link.LastIndexOf(".") + 1).ToLowerInvariant();
			}
			else
			{
				var asset = Asset.Load(link);
				if (asset.Label.IndexOf(".") >= 0)
				{
					extension = asset.Label.Substring(asset.Label.LastIndexOf(".") + 1).ToLowerInvariant();
				}
			}

			switch (extension)
			{
				case "png":
					return "image/png";
				case "gif":
					return "image/gif";
				case "jpeg":
				case "jpg":
					return "image/jpeg";
				case "svg":
					return "image/svg+xml";
			}
			return "application/octet-stream";
		}

		private void SendErrorNotification(Asset log, Exception ex)
		{
			var content = log.GetContent();
			var body = ex.Message + "\n\n";
			if (content.ContainsKey("source_id"))
			{
				var source = Asset.Load(log["source_id"]);
				var dest = Asset.Load(log["dest_id"]);
				body += "Source asset: " + source.AssetPath + "\n" +
				        "Destination asset: " + dest.AssetPath + "\n" +
				        "XTM project id: " + log.Raw["project_id"];
			}
			else
			{
				// This is a project log
				var sources = log.GetPanels("source_id").Select(s => Asset.Load(s["source_id"]).AssetPath.ToString());
				var dests = log.GetPanels("source_id").Select(s => string.Join(", ", s.GetPanels("dest_id").Select(d => Asset.Load(d["dest_id"]).AssetPath.ToString())));
				body += "Source assets: " + string.Join(", ", sources) + "\n" +
				        "Destination assets: " + string.Join(", ", dests) + "\n" +
				        "XTM project id: " + log.Raw["project_id"];
			}
			SendErrorNotification(body);
		}

		private void SendErrorNotification(Asset source, Asset destination, string xtmProjectId, Exception ex)
		{
			var body = ex.Message + "\n\n" +
			           "Source asset: " + (source.IsLoaded ? source.AssetPath.ToString() : "(Not loaded)") + "\n" +
			           "Destination asset: " + (destination.IsLoaded ? destination.AssetPath.ToString() : "(Not loaded)");
			if (!string.IsNullOrWhiteSpace(xtmProjectId))
			{
				body += "\nXTM project id: " + xtmProjectId;
			}
			SendErrorNotification(body);
		}

		private void SendErrorNotification(string body)
		{
			var email = _config.Raw[XTM_ERRORS_EMAIL];
			if (!string.IsNullOrWhiteSpace(email))
			{
				Util.Email("XTM Error", body, email.Split(";".ToCharArray()).Select(e => e.Trim()).ToList());
			}
		}

		public void ProcessIncomingFile(Asset asset, bool overwrite)
		{
			var error = "";
			try
			{
				ProcessTranslationComplete(asset, overwrite);
				DeleteIncomingTranslationAsset(asset);
			}
			catch (Exception ex)
			{
				error = ex.Message;

				// Try to read the details from the asset
				var cmsDocumentId = asset.Raw["cms_document_id"];
				string sourceId, destinationId, destinationLocale;
				ExtractCmsIdParts(cmsDocumentId, out sourceId, out destinationId, out destinationLocale);
				var source = Asset.LoadDirect(sourceId);
				var destination = Asset.LoadDirect(destinationId);
				var xtmProjectId = asset.Raw["xtm_document_id"];

				SendErrorNotification(source, destination, xtmProjectId, ex);
			}
			if (!string.IsNullOrWhiteSpace(error))
			{
				asset.SaveContentField("error", error);
			}
		}

		public void ProcessTranslationComplete(Asset incomingAsset, bool overwrite)
		{
			var fields = incomingAsset.GetContent();
			if (fields.ContainsKey("test") && fields["test"] == "ing")
			{
				// Test only - nothing to do
			}
			else if (!fields.ContainsKey("xml_response") && fields.ContainsKey("xml_response:1"))
			{
				var names = incomingAsset.GetPanels("xml_name").Select(p => p["xml_name"]).ToArray();
				var xmls = incomingAsset.GetPanels("xml_response").Select(p => p["xml_response"]).ToArray();
				ProcessProjectTranslationComplete(incomingAsset.Raw["xtm_document_id"], names, xmls, overwrite);
			}
			else
				ProcessTranslationComplete(incomingAsset.Raw["cms_document_id"], incomingAsset.Raw["xtm_document_id"], incomingAsset.Raw["xml_response"], overwrite);
		}

		private void ExtractCmsIdParts(string cmsDocumentId, out string sourceId, out string destinationId, out string destinationLocale)
		{
			sourceId = destinationId = destinationLocale = "";
			if (!string.IsNullOrWhiteSpace(cmsDocumentId))
			{
				var cmsIdParts = cmsDocumentId.Split("|".ToCharArray());
				sourceId = cmsIdParts[0];
				if (cmsIdParts.Length > 1)
				{
					destinationId = cmsIdParts[1];
					if (cmsIdParts.Length > 2)
					{
						destinationLocale = cmsIdParts[2];
					}
				}
			}
		}

		public void ProcessTranslationComplete(string cmsDocumentId, string xtmProjectId, string xml, bool overwrite)
		{
			string sourceId, destinationId, destinationLocale;
			ExtractCmsIdParts(cmsDocumentId, out sourceId, out destinationId, out destinationLocale);

			var asset = Asset.LoadDirect(destinationId);
			if (!asset.IsLoaded)
			{
				throw new Exception("ProcessTranslationComplete: Unable to load destination asset");
			}

			// Update the log
			var log = FindLogByProjectId(asset, xtmProjectId);
			if (log != null && log.IsLoaded)
			{
				UpdateLog(log, new XtmProjectStatusResponse
				{
					FinishDate = 1
				});
				overwrite = log["overwrite"] == "true";
			}

			// Get the fields from our XML
			var response = XtmTranslationData.Deserialize(xml);
			if (response == null || response.Fields == null || response.Fields.Length == 0)
			{
				throw new Exception("ProcessTranslationComplete: No translated data returned from XTM.");
			}

			if (!overwrite)
			{
				if (!TMF.Translation.VerifyHash(asset))
				{
					if (log != null && log.IsLoaded && User.Load(log["user_name"]) != null)
					{
						var body = "The translation has not been applied to the destination asset, as it was changed in the CMS before the translation was completed.\n\n" +
						           "To avoid this in future, check the 'Overwrite Existing Translation Asset' box to ensure the completed translation overwrites the destination asset in the CMS.\n\n" +
						           "Source asset: " + Asset.LoadDirect(sourceId).AssetPath + "\n" +
						           "Destination asset: " + asset.AssetPath + "\n" +
						           "XTM project id: " + log.Raw["project_id"];
						Util.Email("XTM Translation - Translation not applied", body, User.Load(log["user_name"]).Email);
						return;
					}
					else
					{
						throw new Exception("ProcessTranslationComplete: Destination asset has been changed while awaiting translation.");
					}
				}
			}

			// Success, so save the data to the asset
			asset.SaveContent(response.Fields.ToDictionary(f => f.Key, f => f.Value));

			// Fix links
			var sitePath = TMF.GetSitePath(asset).AssetPath.ToString();
			var localeConfigCache = TMF.CreateLocaleConfigCache(sitePath).ToArray();
			var sourceLanguageId = TMF.GetLocaleId(Asset.LoadDirect(sourceId), sitePath, localeConfigCache);
			var destLanguageId = TMF.GetLocaleId(asset, sitePath, localeConfigCache);
			TMF.FixRelativeLinks(asset, Asset.Load(sourceLanguageId), Asset.Load(destLanguageId), sitePath);

			// Remove the checksum
			TMF.Translation.DeleteHash(asset);
		}

		private Asset FindDestinationAsset(Asset log, string sourceId, string locale)
		{
			foreach (var sourcePanel in log.GetPanels("source_id"))
			{
				if (sourcePanel.Raw["source_id"] == sourceId)
				{
					foreach (var destPanel in sourcePanel.GetPanels("dest_locale"))
					{
						if (locale == "" || destPanel.Raw["dest_locale"].Replace("-", "_") == locale.Replace("-", "_"))
						{
							return Asset.LoadDirect(destPanel["dest_id"]);
						}
					}
				}
			}
			return Asset.Load(-1);
		}

		public void ProcessProjectTranslationComplete(string xtmProjectId, string[] names, string[] xmls, bool overwrite)
		{
			var sources = names.Select(n => n.Split("/".ToCharArray()).Last().Replace(".xml", "")).GroupBy(n => n).Select(g => g.First()).ToArray();
			string[] destLocales;
			if (names.Any(n => n.Contains("/")))
			{
				destLocales = names.Select(n => n.Split("/".ToCharArray()).First()).GroupBy(n => n).Select(g => g.First()).ToArray();
			}
			else
			{
				destLocales = new[] {""}; // single blank locale
			}
			var log = FindLogByProjectId(Asset.LoadDirect(sources[0]), xtmProjectId);
			if (log == null || !log.IsLoaded)
			{
				throw new Exception("ProcessProjectTranslationComplete: Unable to load log asset");
			}
			var logContent = log.GetContent();
			overwrite = log["overwrite"] == "true";

			if (!destLocales.Any())
			{
				destLocales = new[] {logContent.First(f => f.Key.StartsWith("dest_locale")).Value.Replace("-", "_")};
			}

			string sitePath = null;
			IEnumerable<LocaleId> localeConfigCache = null;
			var failedList = new List<string>();
			foreach (var source in sources)
			{
				foreach (var locale in destLocales)
				{
					var asset = FindDestinationAsset(log, source, locale);
					if (!asset.IsLoaded)
					{
						throw new Exception("ProcessProjectTranslationComplete: Unable to load destination asset");
					}

					var xml = "";
					for (var i = 0; i < names.Length; i++)
					{
						var name = names[i];
						if (name == locale + "/" + source + ".xml" || name == source + ".xml")
						{
							xml = xmls[i];
							break;
						}
					}
					// Get the fields from our XML
					var response = XtmTranslationData.Deserialize(xml);
					if (response == null || response.Fields == null)
					{
						throw new Exception("ProcessProjectTranslationComplete: No translated data returned from XTM.");
					}

					var failed = false;
					if (!overwrite)
					{
						if (!TMF.Translation.VerifyHash(asset))
						{
							failed = true;
							failedList.Add("ProcessProjectTranslationComplete: Destination asset (" + asset.Id + ") has been changed while awaiting translation.");
						}
					}

					if (!failed)
					{
						// Success, so save the data to the asset
						if (response.Fields.Any())
						{
							asset.SaveContent(response.Fields.ToDictionary(f => f.Key, f => f.Value));

							// Fix links
							if (sitePath == null)
							{
								sitePath = TMF.GetSitePath(asset).AssetPath.ToString();
								localeConfigCache = TMF.CreateLocaleConfigCache(sitePath).ToArray();
							}
							var sourceLanguageId = TMF.GetLocaleId(Asset.LoadDirect(source), sitePath, localeConfigCache);
							var destLanguageId = TMF.GetLocaleId(asset, sitePath, localeConfigCache);
							TMF.FixRelativeLinks(asset, Asset.Load(sourceLanguageId), Asset.Load(destLanguageId), sitePath);
						}
						// Remove the checksum
						TMF.Translation.DeleteHash(asset);
					}
				}
			}

			if (failedList.Any())
			{
				if (log != null && log.IsLoaded && User.Load(log["user_name"]) != null)
				{
					//var failedSources = log.GetPanels("source_id").Select(s => Asset.Load(s["source_id"]).AssetPath.ToString());
					//var failedDests = log.GetPanels("source_id").Select(s => string.Join(", ", s.GetPanels("dest_id").Select(d => Asset.Load(d["dest_id"]).AssetPath.ToString())));
					var plural = failedList.Count > 1;
					var body = string.Format("The translation has not been applied to the destination asset{0}, as {1} changed in the CMS before the translation was completed.\n\n", plural ? "s" : "", plural ? "they were" : "it was") +
					           "To avoid this in future, check the 'Overwrite Existing Translation Asset' box to ensure the completed translation overwrites the destination asset in the CMS.\n\n" +
					           string.Join("\n\n", failedList) + "\n\n" +
					           //"Source assets: " + string.Join(", ", failedSources) + "\n" +
					           //"Destination assets: " + string.Join(", ", failedDests) + "\n" +
					           "XTM project id: " + log.Raw["project_id"];
					Util.Email("XTM Translation - Translation not applied", body, User.Load(log["user_name"]).Email);
				}
				else
				{
					throw new Exception(string.Join("\n\n", failedList));
				}
			}

			// Update the log
			if (log != null && log.IsLoaded)
			{
				UpdateLog(log, new XtmProjectStatusResponse
				{
					FinishDate = 1
				});
			}
		}

		public void DeleteIncomingTranslationAsset(Asset incomingAsset)
		{
			incomingAsset.Delete();
		}

		public void DeleteIncomingTranslationAsset(Asset asset, string cmsDocumentId, string xtmProjectId)
		{
			var siteRoot = Asset.GetSiteRoot(asset);
			var fp = new FilterParams();
			fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "XTM Incoming");
			fp.Add("cms_document_id", Comparison.Equals, cmsDocumentId);
			fp.Add("xtm_document_id", Comparison.Equals, xtmProjectId);
			siteRoot.GetFilterList(fp).ForEach(DeleteIncomingTranslationAsset);
		}

		public void GetAccessApiCredentials(out string username, out string password, out string developerKey, out int incomingFolderId, out int modelAssetId)
		{
			username = _config.Raw[CROWNPEAK_API_USERNAME];
			password = _config.Raw[CROWNPEAK_API_PASSWORD];
			developerKey = _config.Raw[CROWNPEAK_API_DEVELOPER_KEY];
			incomingFolderId = Asset.Load(_config.Raw[CROWNPEAK_API_INCOMING_FOLDER_ID]).Id;
			modelAssetId = Asset.Load(_config.Raw[CROWNPEAK_API_MODEL_ID]).Id;
		}

		public void GetXtmApiCredentials(out string endpoint, out string token, out string customerId)
		{
			endpoint = _config.Raw[XTM_API_ENDPOINT];
			token = _config.Raw[XTM_ACCESS_TOKEN];
			customerId = _config.Raw[XTM_CUSTOMER_ID];
		}

		#region Classes for XTM serialization
		[DataContract]
		private class XtmTranslationData
		{
			[DataMember(Name = "fields", Order = 0)]
			public XtmTranslationDataField[] Fields { get; set; }

			public XtmTranslationData(Asset asset)
			{
				Fields = TMF.TemplateTranslation.GetFieldsForTranslation(asset).Select(kvp => new XtmTranslationDataField(kvp)).ToArray();
			}

			public string Serialize()
			{
				return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Util.SerializeDataContractXml(this);
			}

			public static XtmTranslationData Deserialize(string xml)
			{
				return Util.DeserializeDataContractXml(xml, typeof(XtmTranslationData)) as XtmTranslationData;;
			}
		}

		[DataContract(Name = "field")]
		private class XtmTranslationDataField
		{
			[DataMember(Name = "_key", Order = 0)]
			public string Key { get; set; }
			[DataMember(Name = "value", Order = 1)]
			public string Value { get; set; }

			public XtmTranslationDataField(KeyValuePair<string, string> kvp)
			{
				Key = kvp.Key;
				Value = kvp.Value.Replace("\r", "").Replace("\n", "");
			}
		}

		public class XtmMyProjectCreateResponse : XtmResponse
		{
			public bool IsSuccess { get; set; }
			public int ProjectId { get; set; }
			public string Name { get; set; }
			public int JobId { get; set; }
		}

		[DataContract]
		public class XtmProjectCreateResponse
		{
			[DataMember(Name = "projectId")]
			public int ProjectId { get; set; }
			[DataMember(Name = "name")]
			public string Name { get; set; }
			[DataMember(Name = "jobs")]
			public XtmProjectCreateJobResponse[] Jobs { get; set; }
		}

		[DataContract]
		public class XtmProjectCreateJobResponse
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

		[DataContract]
		public class XtmResponse
		{
			public string ErrorMessage { get; set; }
			public int StatusCode { get; set; }
			public string StatusDescription { get; set; }
		}

		[DataContract]
		public class XtmProjectStatusResponse : XtmResponse
		{
			[DataMember(Name = "id")]
			public int Id { get; set; }
			[DataMember(Name = "name")]
			public string Name { get; set; }
			[DataMember(Name = "customerId")]
			public int CustomerId { get; set; }
			[DataMember(Name = "referenceId")]
			public string ReferenceId { get; set; }
			[DataMember(Name = "sourceLanguage")]
			public string SourceLanguage { get; set; }
			[DataMember(Name = "targetLanguages")]
			public string[] TargetLanguages { get; set; }
			[DataMember(Name = "finishDate")]
			public long FinishDate { get; set; }
			public string XmlResponse { get; set; }
			public string[] XmlNames { get; set; }
			public string[] XmlResponses { get; set; }
		}

		[DataContract]
		public class XtmProjectMetricsResponse
		{
			[DataMember(Name = "targetLanguage")]
			public string TargetLanguage { get; set; }
			[DataMember(Name = "coreMetrics")]
			public XtmProjectMetricsCoreResponse CoreMetrics { get; set; }
		}

		[DataContract]
		public class XtmProjectMetricsCoreResponse
		{
			[DataMember(Name = "totalCharacters")]
			public int TotalCharacters { get; set; }
			[DataMember(Name = "totalWords")]
			public int TotalWords { get; set; }
			[DataMember(Name = "totalSegments")]
			public int TotalSegments { get; set; }
			[DataMember(Name = "repeatsWords")]
			public int RepeatWords { get; set; }
			[DataMember(Name = "repeatsSegments")]
			public int RepeatSegments { get; set; }
		}

		[DataContract]
		public class AuthRequest
		{
			[DataMember(Name = "client")]
			public string Client { get; set; }
			[DataMember(Name = "userId")]
			public int UserId { get; set; }
			[DataMember(Name = "password")]
			public string Password { get; set; }
			[DataMember(Name = "integrationKey", EmitDefaultValue = false)]
			public string IntegrationKey { get; set; }
		}

		[DataContract]
		public class AuthResponse
		{
			[DataMember(Name = "token")]
			public string Token { get; set; }
		}

		public string GetBoundary()
		{
			// Supports creation of multipart/form-data posting
			var prefix = "--------------------------"; // 26 hyphens
			var content = "";
			while (content.Length < 24)
			{
				content += new Random().NextDouble().ToString().Substring(2);
			}
			return prefix + content.Substring(0, 24);
		}

		#endregion

		private class XtmConfiguration
		{
			public string EndPoint { get; private set; }
			public string AccessToken { get; private set; }
			public string CustomerId { get; private set; }
			public XtmConfigurationTemplate[] Templates { get; private set; }
			public string CallbackUrl { get; private set; }
			public string VisualModeHeader { get; private set; }
			public string VisualModeFooter { get; private set; }

			public XtmConfiguration(Asset config)
			{
				EndPoint = config.Raw[XTM_API_ENDPOINT];
				AccessToken = config.Raw[XTM_ACCESS_TOKEN];
				CustomerId = config.Raw[XTM_CUSTOMER_ID];
				Templates = config.GetPanels(XTM_TEMPLATE_LIST).Select(p => new XtmConfigurationTemplate(p)).ToArray();
				CallbackUrl = config.Raw[XTM_WEBHOOK_URL];
				VisualModeHeader = config.Raw[XTM_VISUAL_MODE_HEADER];
				VisualModeFooter = config.Raw[XTM_VISUAL_MODE_FOOTER];
			}
		}

		private class XtmConfigurationTemplate
		{
			public string Name { get; private set; }
			public string Id { get; private set; }

			public XtmConfigurationTemplate(PanelEntry config)
			{
				Name = config.Raw[XTM_TEMPLATE_NAME];
				Id = config.Raw[XTM_TEMPLATE_ID];
			}
		}
	}
}