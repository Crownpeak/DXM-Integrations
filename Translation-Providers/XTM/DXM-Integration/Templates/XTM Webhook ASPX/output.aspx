<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.OutputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="LocalProject" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<%
	var translator = TMF.Translation.GetTmfTranslator(asset);
	string username = "", password = "", developerKey = "";
	string xtmApiEndpoint = "", xtmApiToken = "", xtmApiCustomerId = "";
	int folderId = -1, modelId = -1;
	if (translator is XtmTranslator)
	{
		var xtmTranslator = (XtmTranslator) translator;
		xtmTranslator.GetAccessApiCredentials(out username, out password, out developerKey, out folderId, out modelId);
		xtmTranslator.GetXtmApiCredentials(out xtmApiEndpoint, out xtmApiToken, out xtmApiCustomerId);
	}
	var instance = Asset.Load(0).GetLink(LinkType.Internal).Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
%>
<$@ Import Namespace="System.IO" $>
<$@ Import Namespace="System.IO.Compression" $>
<$@ Import Namespace="System.Net" $>
<$@ Import Namespace="System.Runtime.Serialization" $>
<$@ Import Namespace="System.Runtime.Serialization.Json" $>
<$@ Import Namespace="System.Threading" $>
<$@ Import Namespace="System.Threading.Tasks" $>
<script runat="server" language="c#">
	private const string INSTANCE = "<%= instance %>";
	private const string USERNAME = "<%= username %>";
	private const string PASSWORD = "<%= password %>";
	private const string DEVELOPER_KEY = "<%= developerKey %>";
	private const string XTM_API_ENDPOINT = "<%= xtmApiEndpoint %>";
	private const string XTM_API_TOKEN = "<%= xtmApiToken %>";
	private const int FOLDER_ID = <%= folderId %>;
	private const int MODEL_ID = <%= modelId %>;

	protected void Page_Load(object sender, EventArgs e)
	{
		System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
		try
		{
			var customerId = Request.QueryString["xtmCustomerId"];
			var xtmId = Request.QueryString["xtmProjectId"];
			if (customerId != null && xtmId != null)
			{
				var referenceId = GetProjectReference(xtmId, customerId);
				var xmlFiles = GetProjectResults(xtmId);
				if (Request.QueryString["inline"] == "true")
				{
					if (!string.IsNullOrEmpty(referenceId))
					{
						Response.Write(xmlFiles.Keys.First() + "\n");
						Response.Write(xmlFiles.Values.First());
					}
					else
					{
						foreach (var key in xmlFiles.Keys)
						{
							Response.Write(key + "\n");
							Response.Write(xmlFiles[key] + "\n");
						}
					}
				}
				else
				{
					UpdateCmsReceiver(referenceId ?? "", xtmId, xmlFiles);
				}
			}
		}
		catch (WebException ex)
		{
			Response.Write(ex.Status);
		}
	}

	public void UpdateCmsReceiver(string cmsDocumentId, string xtmDocumentId, Dictionary<string, string> xmlFiles)
	{
		var api = new AccessApi();
		api.Init(INSTANCE, USERNAME, PASSWORD, DEVELOPER_KEY);
		if (api.Login())
		{
			AccessApi.Asset asset;
			var assetName = cmsDocumentId.Replace("|", "_");
			if (string.IsNullOrWhiteSpace(assetName)) assetName = DateTime.UtcNow.ToString("o");
			if (api.CreateAsset(assetName, FOLDER_ID, MODEL_ID, out asset))
			{
				var dictionary = new Dictionary<string, string>();
				dictionary.Add("cms_document_id", cmsDocumentId);
				dictionary.Add("xtm_document_id", xtmDocumentId);
				if (!string.IsNullOrEmpty(cmsDocumentId))
					dictionary.Add("xml_response", xmlFiles.Values.First());
				else
				{
					var index = 1;
					foreach (var key in xmlFiles.Keys)
					{
						dictionary.Add("xml_name:" + index, key);
						dictionary.Add("xml_response:" + index, xmlFiles[key]);
						index++;
					}
				}

				if (api.UpdateAsset(asset.Id, dictionary, new string[0], false, true, out asset))
				{
					Response.Write("Success");
				}
			}
			api.Logout();
		}
	}

	public Dictionary<string, string> GetProjectFiles(string projectId, int fileId)
	{
		// Works to download and unzip a single file and return its content
		var result = new Dictionary<string, string>();
		var client = new System.Net.WebClient();
		client.Headers.Add("Authorization", "XTM-Basic " + XTM_API_TOKEN);
		var bytes = client.DownloadData(XTM_API_ENDPOINT + "/projects/" + projectId + "/files/" + fileId + "/download?fileScope=PROJECT");
		using (var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read))
		{
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".xml"))
				{
					using (var entryStream = entry.Open())
					{
						using (var ms = new MemoryStream())
						{
							entryStream.CopyTo(ms);
							result.Add(entry.FullName, System.Text.Encoding.UTF8.GetString(ms.ToArray()));
						}
					}
				}
			}
		}
		return result;
	}

	public Dictionary<string, string> GetProjectResults(string projectId)
	{
		var client = new System.Net.WebClient();
		client.Headers.Add("Authorization", "XTM-Basic " + XTM_API_TOKEN);

		// First ask for our file to be generated
		var json = client.UploadString(XTM_API_ENDPOINT + "/projects/" + projectId + "/files/generate?fileType=TARGET", "POST", "");
		var response = DeserializeJson(json, typeof(GenerateFileResponse[])) as GenerateFileResponse[];
		if (response != null && response.Length > 0)
		{
			var count = 100;
			// Now wait for our file to be generated
			while (--count >= 0)
			{
				json = client.DownloadString(XTM_API_ENDPOINT + "/projects/" + projectId + "/files/status?fileScope=PROJECT&fileIds=" + response[0].FileId);
				var statusResponse = DeserializeJson(json, typeof(FileStatusResponse[])) as FileStatusResponse[];
				if (statusResponse != null && statusResponse.Length > 0 && statusResponse[0].Status == "FINISHED")
				{
					return GetProjectFiles(projectId, response[0].FileId);
				}
				System.Threading.Thread.Sleep(500);
			}
		}
		return new Dictionary<string, string>();
	}

	public string GetProjectReference(string projectId, string customerId)
	{
		// Works to get a specific project
		var client = new System.Net.WebClient();
		client.Headers.Add("Authorization", "XTM-Basic " + XTM_API_TOKEN);
		var json = client.DownloadString(XTM_API_ENDPOINT + "/projects/" + projectId);
		var project = DeserializeJson(json, typeof(ProjectResponse)) as ProjectResponse;
		if (project != null && project.CustomerId.ToString() == customerId)
			return project.ReferenceId;
		return "";
	}

	private static string SerializeJson(object source)
	{
		var result = "";
		var settings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true};
		var serializer = new DataContractJsonSerializer(source.GetType(), settings);
		using (var ms = new MemoryStream())
		{
			serializer.WriteObject(ms, source);
			ms.Position = 0;
			using (var sr = new StreamReader(ms))
			{
				result = sr.ReadToEnd();
				sr.Close();
				ms.Close();
			}
		}
		return result;
	}

	private static object DeserializeJson(string json, Type type)
	{
		object result;
		using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
		{
			var serializer = new DataContractJsonSerializer(type);
			result = serializer.ReadObject(ms);
			ms.Close();
		}
		return result;
	}

	public class AccessApi : IDisposable
	{
		private const string WEBAPI_ROOT = "/cpt_webservice/accessapi";
		private const string SERVER = "cms.crownpeak.net";

		private string Instance = "";
		private string Username = "";
		private string Password = "";
		private string DeveloperKey = "";
		private bool _authenticated = false;
		private WebClient _client;
		private string _cookie;

		public void Init(string instance, string username, string password, string developerKey)
		{
			Instance = instance;
			Username = username;
			Password = password;
			DeveloperKey = developerKey;
		}

		public bool Login()
		{
			return Login(this.Username, this.Password);
		}

		public bool Login(string username, string password)
		{
			if (_authenticated) return true;

			var request = new AuthenticateRequest
			{
				Instance = Instance,
				Username = username,
				Password = password,
				RememberMe = false,
				TimeZoneOffsetMinutes = 0
			};

			_client = new WebClient();
			var result = SendRequest("POST", "/Auth/Authenticate", SerializeJson(request));
			var response = DeserializeJson(result, typeof(AuthenticateResponse)) as AuthenticateResponse;
			_authenticated = response.IsSuccessful;

			if (_authenticated)
			{
				_cookie = _client.ResponseHeaders["Set-Cookie"];
			}

			return _authenticated;
		}

		[DataContract]
		public class CreateAssetRequest
		{
			[DataMember(Name = "newName")]
			public string Label { get; set; }
			[DataMember(Name = "destinationFolderId")]
			public int FolderId { get; set; }
			[DataMember(Name = "modelId")]
			public int ModelId { get; set; }
			[DataMember(Name = "type")]
			public int Type { get; set; }
			[DataMember(Name = "devTemplateLanguage")]
			public int DevTemplateLanguage { get; set; }
			[DataMember(Name = "templateId")]
			public int TemplateId { get; set; }
			[DataMember(Name = "workflowId")]
			public int WorkflowId { get; set; }
			[DataMember(Name = "subtype")]
			public int Subtype { get; set; }
			[DataMember(Name = "runNew")]
			public bool RunNew { get; set; }
			[DataMember(Name = "createChildren")]
			public bool CreateChildren { get; set; }
		}

		[DataContract]
		public class CreateAssetResponse : ResultClass
		{
			[DataMember(Name = "asset")]
			public Asset Asset { get; set; }
		}

		[DataContract]
		public class UpdateAssetRequest
		{
			[DataMember(Name = "assetId")]
			public int AssetId { get; set; }
			[DataMember(Name = "fields")]
			public Dictionary<string, string> Fields { get; set; }
			[DataMember(Name = "fieldsToDelete")]
			public string[] FieldsToDelete { get; set; }
			[DataMember(Name = "runPostInput")]
			public bool RunPostInput { get; set; }
			[DataMember(Name = "runPostSave")]
			public bool RunPostSave { get; set; }
		}

		[DataContract]
		public class UpdateAssetResponse : ResultClass
		{
			[DataMember(Name = "asset")]
			public Asset Asset { get; set; }
		}

		[DataContract]
		public class Asset
		{
			[DataMember(Name = "id")]
			public int Id { get; set; }
		}

		public bool CreateAsset(string label, int folderId, int modelId, out Asset asset)
		{
			asset = null;
			var request = new CreateAssetRequest
			{
				Label = label,
				FolderId = folderId,
				ModelId = modelId,
				Type = 2,
				RunNew = true,
				Subtype = -1
			};
			var result = SendRequest("POST", "/Asset/Create", SerializeJson(request));
			var response = DeserializeJson(result, typeof(CreateAssetResponse)) as CreateAssetResponse;
			if (response != null && response.IsSuccessful) asset = response.Asset;

			return response != null && response.IsSuccessful;
		}

		public bool UpdateAsset(int assetId, Dictionary<string, string> fields, string[] fieldsToDelete, bool runPostInput, bool runPostSave, out Asset asset)
		{
			asset = null;
			var request = new UpdateAssetRequest
			{
				AssetId = assetId,
				Fields = fields,
				FieldsToDelete = fieldsToDelete,
				RunPostInput = runPostInput,
				RunPostSave = runPostSave
			};
			var result = SendRequest("POST", "/Asset/Update", SerializeJson(request));
			var response = DeserializeJson(result, typeof(UpdateAssetResponse)) as UpdateAssetResponse;
			if (response.IsSuccessful) asset = response.Asset;

			return response.IsSuccessful;
		}

		public bool Logout()
		{
			if (_authenticated)
			{
				if (_client != null)
				{
					SendRequest("POST", "/Auth/Logout", "");
					_client = null;
				}
				_authenticated = false;
			}
			return true;
		}

		public string SendRequest(string method, string path, string data)
		{
			var task = SendRequestAsync(method, path, data);
			task.Wait();
			return task.Result;
		}

		public Task<string> SendRequestAsync(string method, string path, string data)
		{
			path = string.Concat("/", Instance, WEBAPI_ROOT, path);

			_client.Headers[HttpRequestHeader.ContentType] = "text/json";
			_client.Headers["x-api-key"] = DeveloperKey;
			_client.Encoding = Encoding.UTF8;

			if (_authenticated && !string.IsNullOrWhiteSpace(_cookie))
			{
				_client.Headers.Add(HttpRequestHeader.Cookie, _cookie);
				_cookie = null;
			}

			return Task.Run(() =>
			{
				while (true)
				{
					try
					{
						if (string.IsNullOrEmpty(data) && method.ToUpper() == "GET")
						{
							return _client.DownloadString(string.Concat("https://", SERVER, path));
						}
						else
						{
							return _client.UploadString(string.Concat("https://", SERVER, path), method, data ?? "");
						}
					}
					catch (WebException ex)
					{
						if (ex.Status == WebExceptionStatus.ProtocolError
							&& ex.Message.Contains("(429)")) // Rate Limit Exceeded
						{
							int timeout = 1000;
							// Response should include a Retry-After header giving the time to wait in seconds
							// But we'll be defensive anyway, in case it's not there
							if (ex.Response != null && ex.Response.Headers != null && ex.Response.Headers["Retry-After"] != null)
							{
								int requestedTimeout;
								if (int.TryParse(ex.Response.Headers["Retry-After"], out requestedTimeout))
								{
									// Turn it into milliseconds for our sleep
									timeout = requestedTimeout * 1000;
								}
							}
							Thread.Sleep(timeout);

							// Make sure we set the content type for the retry
							_client.Headers["Content-Type"] = "text/json";
						}
						else
						{
							throw;
						}
					}
				}
			});
		}

		public void Dispose()
		{
			Logout();
		}
	}

	[DataContract]
	public class AuthenticateRequest
	{
		[DataMember(Name = "instance")]
		public string Instance;

		[DataMember(Name = "username")]
		public string Username;

		[DataMember(Name = "password")]
		public string Password;

		[DataMember(Name = "remember_me")]
		public bool RememberMe;

		[DataMember(Name = "timeZoneOffsetMinutes")]
		public int TimeZoneOffsetMinutes;
	}

	[DataContract]
	public class AuthenticateResponse : ResultClass
	{
		[DataMember(Name = "needsExpirationWarning")]
		public bool NeedsExpirationWarning { get; set; }

		[DataMember(Name = "daysToExpire")]
		public int DaysToExpire { get; set; }
	}
	[DataContract]
	public class ResultClass
	{
		[DataMember(Name = "resultCode")]
		public string ResultCode { get; set; }
		[DataMember(Name = "internalCode")]
		public int InternalCode { get; set; }
		[DataMember(Name = "errorMessage")]
		public string ErrorMessage { get; set; }

		public bool IsSuccessful
		{
			get { return ResultCode.Equals("conWS_Success"); }
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
	public class ProjectResponse
	{
		[DataMember(Name = "referenceId")]
		public string ReferenceId { get; set; }
		[DataMember(Name = "customerId")]
		public int CustomerId { get; set; }
	}

</script>
