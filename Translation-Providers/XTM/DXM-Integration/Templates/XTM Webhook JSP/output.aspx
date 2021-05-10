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
<$@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"
  import="
  java.net.HttpURLConnection,
  java.net.URL,
	java.io.BufferedReader,
	java.io.ByteArrayInputStream,
	java.io.ByteArrayOutputStream,
	java.io.InputStream,
  java.io.InputStreamReader,
  java.io.IOException,
  java.io.OutputStream,
  java.io.BufferedReader,
	java.time.Instant,
  java.util.HashMap,
  java.util.Map,
	java.util.zip.ZipEntry,
	java.util.zip.ZipInputStream
  "$>
<$!
private static String Server = "cms.crownpeak.net";
private static String Instance = "<%= instance %>";
private static String PublicKey = "<%= developerKey %>";
private static String ApiRoot = "/cpt_webservice/accessapi";
private static String Username = "<%= username %>";
private static String Password = "<%= password %>";
private static String XtmApiEndpoint = "<%= xtmApiEndpoint %>";
private static String XtmApiToken = "<%= xtmApiToken %>";
private static int FolderId = <%= folderId %>;
private static int ModelId = <%= modelId %>;
private static String Cookie = "";

private static String tidy(String string) {
	if (string == null || string.length() == 0) {
		return "";
	}

	char         c = 0;
	int          i;
	int          len = string.length();
	StringBuilder sb = new StringBuilder(len + 4);
	String       t;

	for (i = 0; i < len; i += 1) {
		c = string.charAt(i);
		switch (c) {
			case '\\':
			case '"':
				sb.append('\\');
				sb.append(c);
				break;
			case '/':
//				if (b == '<') {
					sb.append('\\');
//				}
				sb.append(c);
				break;
			case '\b':
				sb.append("\\b");
				break;
			case '\t':
				sb.append("\\t");
				break;
			case '\n':
				sb.append("\\n");
				break;
			case '\f':
				sb.append("\\f");
				break;
			case '\r':
				sb.append("\\r");
				break;
			default:
				if (c < ' ') {
					t = "000" + Integer.toHexString(c);
					sb.append("\\u" + t.substring(t.length() - 4));
				} else {
					sb.append(c);
				}
		}
	}
	return sb.toString();
}

private static String sendRequest(JspWriter out, String method, String path, String data) throws IOException {

	HttpURLConnection con = null;
	OutputStream os = null;
	BufferedReader in = null;

	try {
		String url = "https://" + Server + "/" + Instance + ApiRoot + path;

		URL obj = new URL(url);
	        con = (HttpURLConnection) obj.openConnection();
	
		con.setRequestMethod(method);
		con.setRequestProperty("User-Agent", "MyUserAgent");
		con.setRequestProperty("Content-Type", "text/json");
		con.setRequestProperty("x-api-key", PublicKey);
		if (!Cookie.equals("")) {
			con.setRequestProperty("Cookie", Cookie);
		}

		if (method.equals("POST")) {
			con.setDoOutput(true);
		        os = con.getOutputStream();
		        os.write(data.getBytes());
		        os.flush();
		        os.close();
		}

		int responseCode = con.getResponseCode();
		//out.println("ResponseCode is " + responseCode);

		if (responseCode == HttpURLConnection.HTTP_OK) { // success
			String cookie = con.getHeaderField("Set-Cookie");
			// If there's a Set-Cookie header and we don't already have one...
			if (Cookie.equals("") && cookie != null && !cookie.equals("")) {
				StringBuffer cookieBuffer = new StringBuffer();
				int n = 0;
				String c = "";
				while (c != null) {
					c = con.getHeaderField(n);
					String k = con.getHeaderFieldKey(n);
					if (k != null && k.equals("Set-Cookie")) {
						if (cookieBuffer.length() > 0) {
							cookieBuffer.append(",");
						}
						cookieBuffer.append(c);
					}
					n++;
				}
				Cookie = cookieBuffer.toString();
			}

			in = new BufferedReader(new InputStreamReader(con.getInputStream()));
			String inputLine;
			StringBuffer resp = new StringBuffer();

			while ((inputLine = in.readLine()) != null) {
				resp.append(inputLine);
			}
			in.close();

			return resp.toString();
		}
	}
	catch (Exception ex) {
	}
	finally {
		if (os != null) {
			try {
				os.close();
			}
			catch (IOException e) {
			}
		}
		if (in != null) {
			try {
				in.close();
			}
			catch (IOException e) {
			}
		}
		if (con != null) {
			con.disconnect();
		}
	}
	return "";
}

private static Boolean login(JspWriter out, String username, String password) throws IOException {
	String data = String.format("{\"instance\":\"%s\",\"username\":\"%s\",\"password\":\"%s\",\"remember_me\":false,\"timeZoneOffsetMinutes\":0}",
			tidy(Instance), tidy(username), tidy(password));
	String result = sendRequest(out, "POST", "/Auth/Authenticate", data);
	return result.contains("\"conWS_Success\"");
}

private static void logout(JspWriter out) throws IOException {
	sendRequest(out, "POST", "/Auth/Logout", "");
	Cookie = "";
}

private static int assetCreate(JspWriter out, int folderId, String label, int modelId) throws IOException {
	StringBuffer data = new StringBuffer();
	data.append(String.format("{\"destinationFolderId\":%d,", folderId));
	data.append(String.format("\"newName\":\"%s\",", tidy(label.replace('|', '_'))));
	data.append(String.format("\"modelId\":%d,", modelId));
	data.append("\"type\":2,");
	data.append("\"devTemplateLanguage\":0,");
	data.append("\"templateId\":0,");
	data.append("\"workflowId\":0,");
	data.append("\"subtype\":-1,");
	data.append("\"runNew\":true}");
	String result = sendRequest(out, "POST", "/Asset/Create", data.toString());
	if (result.contains("conWS_Success")) {
		int start = result.indexOf("\"id\":");
		if (start > 0) {
			int finish = result.indexOf(",", start);
			if (finish > start) {
				return Integer.parseInt(result.substring(start + 5, finish).trim());
			}
		}
	}
	return -1;
}

private static Boolean assetUpdate(JspWriter out, int id, Map<String, String> fields) throws IOException {
	StringBuffer data = new StringBuffer();
	data.append(String.format("{\"assetId\":%d,\"fields\":{", id));
	Boolean first = true;
	for (Map.Entry<String, String> entry : fields.entrySet()) {
		if (!first) {
			data.append(",");
		}
		first = false;
		data.append(String.format("\"%s\":\"%s\"", tidy(entry.getKey()), tidy(entry.getValue())));
	}
	data.append("},");
	data.append("\"runPostSave\":true");
	data.append("}");
	String result = sendRequest(out, "POST", "/Asset/Update", data.toString());
	return result.contains("conWS_Success");
}

private static String xtmSendRequest(JspWriter out, String method, String path, String data) throws IOException {
	return new String(xtmSendRequestBinary(out, method, path, data));
}

private static byte[] xtmSendRequestBinary(JspWriter out, String method, String path, String data) throws IOException {

	HttpURLConnection con = null;
	OutputStream os = null;
	BufferedReader in = null;

	try {
		String url = XtmApiEndpoint + path;

		URL obj = new URL(url);
	        con = (HttpURLConnection) obj.openConnection();
	
		con.setRequestMethod(method);
		con.setRequestProperty("Content-Type", "application/json");
		con.setRequestProperty("Authorization", "XTM-Basic " + XtmApiToken);

		if (method.equals("POST") && data.length() > 0) {
			con.setDoOutput(true);
		        os = con.getOutputStream();
		        os.write(data.getBytes());
		        os.flush();
		        os.close();
		}

		int responseCode = con.getResponseCode();
		//out.println("ResponseCode is " + responseCode);

		if (responseCode == HttpURLConnection.HTTP_OK) { // success
			ByteArrayOutputStream buffer = new ByteArrayOutputStream();

			int nRead;
			byte[] temp = new byte[4096];

			while ((nRead = con.getInputStream().read(temp, 0, temp.length)) != -1) {
			  buffer.write(temp, 0, nRead);
			}

			return buffer.toByteArray();
		}
	}
	catch (Exception ex) {
	}
	finally {
		if (os != null) {
			try {
				os.close();
			}
			catch (IOException e) {
			}
		}
		if (in != null) {
			try {
				in.close();
			}
			catch (IOException e) {
			}
		}
		if (con != null) {
			con.disconnect();
		}
	}
	return new byte[0];
}

private static int generateTarget(JspWriter out, String projectId) throws IOException {
	String result = xtmSendRequest(out, "POST", "/projects/" + projectId + "/files/generate?fileType=TARGET", "");
	if (result.indexOf("[{\"fileId\":") == 0) {
		result = result.substring(11);
		result = result.substring(0, result.indexOf('}'));
		return Integer.parseInt(result);
	}
	return -1;
}

private static boolean waitForFile(JspWriter out, String projectId, int fileId) throws IOException, InterruptedException {
	int count = 100;
	while (count-- >= 0) {
		String result = xtmSendRequest(out, "GET", "/projects/" + projectId + "/files/status?fileScope=PROJECT&fileIds=" + fileId, "");
		if (result.indexOf("FINISHED") > 0) return true;
		Thread.sleep(500);
	}
	return false;
}

private static byte[] downloadFile(JspWriter out, String projectId, int fileId) throws IOException {
	byte[] result = xtmSendRequestBinary(out, "GET", "/projects/" + projectId + "/files/" + fileId + "/download?fileScope=PROJECT", "");
	return result;
}

private static HashMap<String, String> extractFile(JspWriter out, byte[] zipContent) throws IOException {
	HashMap<String, String> contents = new HashMap<String, String>();
	ZipInputStream zipIn = new ZipInputStream(new ByteArrayInputStream(zipContent));
	ZipEntry entry = zipIn.getNextEntry();
	while (entry != null) {
		String name = entry.getName();
		if (name.endsWith(".xml")) {
	    ByteArrayOutputStream bos = new ByteArrayOutputStream();
	    byte[] buffer = new byte[10240];
	    int read = 0;
	    while ((read = zipIn.read(buffer)) != -1) {
        bos.write(buffer, 0, read);
	    }
	    bos.close();
       contents.put(name, bos.toString());
		}
    zipIn.closeEntry();
    entry = zipIn.getNextEntry();
  }
  zipIn.close();
  return contents;
}
private static String getReferenceId(JspWriter out, String projectId, String customerId) throws IOException {
	String result = xtmSendRequest(out, "GET", "/projects/" + projectId, "");
	if (result.indexOf("\"customerId\":" + customerId + ",") > 0 && result.indexOf("\"referenceId\":\"") > 0) {
		result = result.substring(result.indexOf("\"referenceId\":\"") + 15);
		result = result.substring(0, result.indexOf('"'));
		return result;
	}
	return "";
}

$><$

String customerId = request.getParameter("xtmCustomerId");
String xtmId = request.getParameter("xtmProjectId");

if (customerId == null) {
	//out.println("Error: Missing customerId");
} else if (xtmId == null) {
	//out.println("Error: Missing xtmId");
} else {

	String referenceId = getReferenceId(out, xtmId, customerId);
  //out.println("Found " + referenceId);

	int fileId = generateTarget(out, xtmId);
	boolean finished = waitForFile(out, xtmId, fileId);
	byte[] content = downloadFile(out, xtmId, fileId);
	HashMap<String, String> xmlFiles = extractFile(out, content);

	if ("true".equals(request.getParameter("inline"))) {
		for (String name : xmlFiles.keySet()) {
			out.println(name);
			out.println(xmlFiles.get(name));
		}
	} else {

		Boolean loggedIn = login(out, Username, Password);
		//out.println("DEBUG: Login success = " + loggedIn);

		if (!loggedIn) {
	      //out.println("Error: Unable to log in");
	    } else {
				String assetName = referenceId;
				if (assetName.length() == 0) assetName = Instant.now().toString();
    		int assetId = assetCreate(out, FolderId, assetName, ModelId);
				//out.println("DEBUG: Create success = " + (assetId > 0));
    		if (assetId > 0) {    	
				Map<String, String> fields = new HashMap<String, String>();
				fields.put("cms_document_id", referenceId);
				fields.put("xtm_document_id", xtmId);
				if (referenceId.length() > 0) {
					for (String name : xmlFiles.keySet()) {
						fields.put("xml_response", xmlFiles.get(name));
						break;
					}
				} else {
					int index = 1;
					for (String name : xmlFiles.keySet()) {
						fields.put("xml_name:" + index, name);
						fields.put("xml_response:" + index, xmlFiles.get(name));
						index++;
					}
				}
				Boolean success = assetUpdate(out, assetId, fields);
				//out.println("DEBUG: Update success = " + success);
				if (success) {
					out.println("Success");
				}
   		}
			// Tidy up
			logout(out);
    }
	}
}
$>