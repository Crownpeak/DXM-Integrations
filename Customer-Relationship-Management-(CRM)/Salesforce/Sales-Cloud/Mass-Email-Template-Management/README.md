<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# Salesforce Mass Email Template Integration for Crownpeak Digital Experience Management (DXM)
<a href="https://www.salesforce.com/" target="_blank">Salesforce's</a> Customer Relationship Management (CRM) solution
gives your sales teams the power to close deals like never before with cloud-based tools that increase productivity,
keep pipeline filled with leads, and score more wins.

This integration creates a connection between Crownpeak DXM and Salesforce CRM that will enable the publication of email
templates for mass distribution, keeping authoring capabilities location within a single location, regardless of the
consumer channel.

## Capabilities
Allows the authoring of Salesforce Email Templates within the DXM platform, to be pushed to Salesforce upon successful
workflow operation.

![DXM-Managed Email Template](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-screenshot-1.png?raw=true "DXM-Managed Email Template")

Triggering "Release" workflow within DXM, distributes fully-managed content to Salesforce Sales Cloud.

![Salesforce Mass Email](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-screenshot-2.png?raw=true "Salesforce Mass Email")

Email Template previewed within Salesforce CRM.

## Configuration Steps

### Setting Up Salesforce

 1) Login to Salesforce -> click on **Setup** and search for ‘**Templates**’
 
 2) Click on **Classic Email Templates**
 
    ![Classic Email Templates](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-classic-email-templates.png?raw=true "Classic Email Templates")

 3) Click on '**Create New Folder**' and enter the folder details:
 
    ![Create New Folder](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-create-new-folder.png?raw=true "Create New Folder")

 4) Save your new folder.  This is where you will inject your Email Template from Crownpeak in to.

 5) You will need the FolderId later, let’s get that now. 

    ![Edit Folder](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-edit-folder.png?raw=true "Edit Folder")

 6) Select '**Edit**' and then look in the Address bar of your browser where you will see the FolderId.
 
    ![Get folderid](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-get-folderid.png?raw=true "Get folderid")

    e.g., In the example above, the FolderId: 00l4J000000Qdfk. Remember this for later when we code the Email Template
    Upload from the CMS.
    
### Setting Up an App

 1) Click on '**Setup**' and search for **Apps**.
 
 2) Select **Build -> Create -> Apps**.
 
    ![Create App](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-create-app.png?raw=true "Create App")

 3) In '**Connected Apps**', click '**New**'.
 
 4) Complete the following mandatory fields:
 
    1) **Connected App Name:**  DX Developer _(or whatever you want to call the App)_.
    
    2) **API Name:** DX_Developer
    
    3) **Contact Email:** you@yourdomain.com _(replace with your details)_.
  
       ![Configuration](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-configure-1.png?raw=true "Configuration")

    4) **Callback URL:** https://localhost _(we used a Postman callback in the image, but this can be any URI)_.
 
       ![Configuration](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-configure-2.png?raw=true "Configuration")

    5) **Selected OAuth Scopes:** Access and Manage your data (api)
    
       ![Configuration](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-configure-3.png?raw=true "Configuration")

    6) **Require Secret for Web Server Flow:** Check this box
    
 5) Save the App. You will use this to connect to your Salesforce Instance from Crownpeak DXM. Now you need your
    Consumer Key and Consumer Secret Key for the App.

 6) Click on '**Setup**' and search for '**Apps**' again.
 
 7) Select **Build -> Create -> Apps**
 
 8) Click on the '**Connected App Name**' of your new connected app.

    ![Connected App Name](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-connected-app-name.png?raw=true "Connected App Name")

 9) In the '**API (Enable OAuth Settings)**' note down your **Consumer Key** and **Consumer Secret Key**.
 
### Setting up DXM
 
 In this example, we want to create an email template in the CMS and allow the editorial or marketing team to push this
 to Salesforce so that it can be used in conjunction with a particular campaign.  This affords the campaign team the
 opportunity to keep the branding and style of a campaign in line with a particular campaign as well as sharing assets,
 such as images, documents, video etc.  Furthermore, the editorial team can control the release of the email template
 through the same release workflow as that of the web assets.
  
 This particular integration will employ DXM Workflow to trigger the integration and push the Email Template into
 Salesforce.
 
 **Overview of Steps:**
 
  * Step 1 - Create an Email Template (DXM Template).
  
  * Step 2 - Create the integration code.
  
  * Step 3 - Create a Salesforce Configuration Asset.
  
  * Step 4 - Create a Workflow Step.
  
  * Step 5 - Create the Email Template Asset.
 
#### Step 1 - Create an Email Template (DXM Template)
 
 In the **Project/Template** folder in CMS, create a new **Template**.  Give it a suitable name. Copy the following
 code into the relevant Template files:
 
 **input.aspx**
 ```
<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%
Input.ShowHeader("Body");

var wysiwygparams = ServicesInput.FullWYSIWYG();
wysiwygparams.PreviewStylesheet = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css";
wysiwygparams.Stylesheet = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css";
wysiwygparams.InsertHTML = true;
wysiwygparams.DesignEditTabs = true; //switches on code button for source editing

Input.ShowWcoControls("email_content");
Input.ShowWysiwyg("Email HTML Content", "email_content", wysiwygparams);

//Config - move to seperate asset later
Input.ShowHeader("Config");
Input.ShowTextBox("Name of template", "name");
Input.ShowTextBox("Folder Id", "folder_id");
Input.ShowTextBox("Email Subject", "subject");  
Input.ShowTextBox("Template Description","description"); 

%>
```

_N.B._, setting the PreviewStylesheet and Stylesheet parameter on the WYSIWYG is not mandatory or you could use your
own stylesheets if you wished.
 
**output.aspx**
```
<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.OutputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>

<!DOCTYPE html>
<html>
<!-- Metadata -->
<head>
<title><%= asset["page_title"] %></title>
  	<meta charset="UTF-8">
  	<meta name="description" content='<%= asset["page_description"] %>'>
  	<meta name="keywords" content='<%= asset["page_keywords"] %>'>
  	<meta name="author" content="Crownpeak Sales Engineering">
  	<meta name="viewport" content="width=device-width, initial-scale=1.0">
</head>
<body>
  	<%= asset["email_content"]%>  
</body>
</html>

```

#### Step 2 - Create the Integration Code

 1) In the **Project/Templates** folder in the CMS, create a new '**Template Folder**' called '**Workflow Scripts**'.
 
    ![Workflow Scripts](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-workflow-scripts.png?raw=true "Workflow Scripts")

 2) Inside the Workflow Scripts folder select **File -> New -> Template**. Name the folder something logical, such as
    SFDC or Salesforce.
    
    ![Create Template](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-create-template.png?raw=true "Create Template")

 3) Inside the Template, delete the **output.aspx** Template File, as this is not required.
 
 4) Rename the **input.aspx** Template File to **sfdc_email_template.aspx**, as shown.
  
    ![Rename Template File](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-rename-template-file.png?raw=true "Rename Template File")

 5) Open the **sfdc_email_template.aspx** asset and add the following code:
 
     ```
     <%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PostSaveInit" %>
     <%@ Import Namespace="CrownPeak.CMSAPI" %>
     <%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
     <%@ Import Namespace="CrownPeak.CMSAPI.CustomLibrary" %>
     <%@ Import Namespace="Component_Library.Component_Project.Components"%>
     <%@ Import Namespace="System.Runtime.Serialization" %>
     <!--DO NOT MODIFY CODE ABOVE THIS LINE-->
     
     <!-- Class for authenticating with Salesforce -->
     <script runat="server" data-cpcode="true">
         //Salesforce Authentication response
         [DataContract]
         public class SFDCAuthentication
         {
             [DataMember(Name = "access_token", Order = 0)]
             public string access_token { get; set; }
             [DataMember(Name = "token_type", Order = 1)]
             public string token_type { get; set; }
         }
    
         [DataContract]
         public class SFDCEmailTemplate
         {
             [DataMember(Name = "HtmlValue", Order = 0)]
             public string HtmlValue { get; set; }
             [DataMember(Name = "Name", Order = 1)]
             public string Name { get; set; }
             [DataMember(Name = "Subject", Order = 2)]
             public string Subject { get; set; }
             [DataMember(Name = "FolderId", Order = 3)]
             public string FolderId { get; set; }
             [DataMember(Name = "TemplateType", Order = 4)]
             public string TemplateType { get; set; }
             [DataMember(Name = "IsActive", Order = 5)]
             public string IsActive { get; set; }
             [DataMember(Name = "Description", Order = 6)]
             public string Description { get; set; }
             [DataMember(Name = "DeveloperName", Order = 7)]
             public string DeveloperName { get; set; }
         }
     </script>
     
     <% 
         // Authenticate with Salesforce and get an Access Token for the function
         //Load configuration settings asset
         Asset sfdcConfigAsset = Asset.Load("<your_config_asset_path>");
         string grant_type = "password"; //Fixed ** do not change
         string client_id = sfdcConfigAsset.Raw["client_id"]; //Get from Salesforce
         string client_secret = sfdcConfigAsset.Raw["client_secret"]; //Get from Salesforce
         string auth_endpoint = sfdcConfigAsset.Raw["integration_auth_endpoint"];
         string endpoint = sfdcConfigAsset.Raw["integration_endpoint"];
         string username = sfdcConfigAsset.Raw["username"];
         string password = sfdcConfigAsset.Raw["password"];
     
         //Build the authentication post request using the Salesforce variables above
         var boundary = GetBoundary();
         
         var content = new StringBuilder(10240);
         content.AppendLine("--" + boundary);
         content.AppendLine("Content-Disposition: form-data; name=\"username\"");
         content.AppendLine();
         content.AppendLine(username);
         content.AppendLine("--" + boundary);
         content.AppendLine("Content-Disposition: form-data; name=\"password\"");
         content.AppendLine();
         content.AppendLine(password);
         content.AppendLine("--" + boundary);
         content.AppendLine("Content-Disposition: form-data; name=\"client_id\"");
         content.AppendLine();
         content.AppendLine(client_id);
         content.AppendLine("--" + boundary);
         content.AppendLine("Content-Disposition: form-data; name=\"client_secret\"");
         content.AppendLine();
         content.AppendLine(client_secret);
         content.AppendLine("--" + boundary);
         content.AppendLine("Content-Disposition: form-data; name=\"grant_type\"");
         content.AppendLine();
         content.AppendLine(grant_type);
         content.AppendLine("--" + boundary + "--"); // Extra -- to end
         
         Out.WriteLine("<pre>{0}</pre>", Util.HtmlEncode(content.ToString()));
         
         var postParams = new PostHttpParams
         {
                ContentType = "multipart/form-data; boundary=" + boundary,
                 PostData = content.ToString()
         };
        
         // Post authentication request to Salesforce to acquire token
         var postResponse = Util.PostHttp(auth_endpoint, postParams);
         
         string response = postResponse.ResponseText;
     
         SFDCAuthentication tokenResponse = Util.DeserializeDataContractJson(response, typeof(SFDCAuthentication)) as SFDCAuthentication;

         if (postResponse != null)
         {
            Util.Log("Token found: " + tokenResponse.access_token);
            var templateResponse = UploadEmailTemplate(tokenResponse.access_token);
         }
         else
         {
            Util.Log("No token acquired!");
         }
     %>
     
     <!-- Class for HTTP-Post to SFDC -->
     <script runat="server" data-cpcode="true"> 
         [DataContract]
         public class MyFolder
         {
             [DataMember(Name = "id", Order = 0)]
             public int Id { get; set; }
             
             [DataMember(Name = "type", Order = 1)]
             public readonly string Type = "Folder";
             
             public MyFolder(int id)
             {
                 Id = id;
             }
             
             public MyFolder(string id) : this(int.Parse(id))
             { }
         }
         
         public string GetBoundary()
         {
             var prefix = "--------------------------"; // 26 hyphens
             var content = "";
             
             while (content.Length < 24)
             {
                 content += new Random().NextDouble().ToString().Substring(2);
             }
             
             return prefix + content.Substring(0, 24);
         }
         
         public string UploadEmailTemplate(string token)
         {
             Asset sfdcConfigAsset = Asset.Load("<your_config_asset_path>");
             
             string url = sfdcConfigAsset.Raw["integration_endpoint"] + "/EmailTemplate";
     
             SFDCEmailTemplate emailTemplate = new SFDCEmailTemplate();
             emailTemplate.HtmlValue = Util.ReplaceCptInternals(asset.Raw["email_content"], true, ProtocolType.Https);
             emailTemplate.Name = asset.Id + ": " + asset.Raw["name"];dd
             emailTemplate.Subject = asset.Raw["subject"];
             emailTemplate.FolderId = asset.Raw["folder_id"];
             emailTemplate.TemplateType = "Custom";
             emailTemplate.IsActive = "true";
             emailTemplate.Description = asset.Raw["description"];
             emailTemplate.DeveloperName = "Crownpeak_Developer_" + asset.Id;
             
             String contentJson = Util.SerializeDataContractJson(emailTemplate);
             
             var postParams = new PostHttpParams();
             postParams.AddHeader("Authorization: Bearer " + token);
             postParams.PostData = contentJson;
             postParams.ContentType = "application/json";
             var postResponse = Util.PostHttp(url, postParams);
             
             return postResponse.StatusCode.ToString();
         }
     </script>
     ```
     
 6) Save the template. In the code, you will see two entries that we need to map to a new Salesforce Config Asset 
    **<your_config_asset_path>**. We will create the Config Asset next.

 
#### Step 3 - Create a Salesforce Configuration Asset

 1) In your **Project/Templates** folder, create a new Template named ‘**Email Settings Config**’.
 
 2) Copy the following code into the relevant Template file:
 
    **input.aspx**
    ```
    <%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
    <%@ Import Namespace="CrownPeak.CMSAPI" %>
    <%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
    <!--DO NOT MODIFY CODE ABOVE THIS LINE-->
    <%@ Import Namespace="Component_Library.Component_Project.Components" %>
    <%    
        Input.ShowHeader("Campaign Integration Configuration");
        Input.ShowMessage("Configuration used in the Email Template Upload Tool", MessageType.Basic);
        Input.ShowTextBox("Client ID", "client_id");
        Input.ShowTextBox("Client Secret", "client_secret");
        Input.ShowTextBox("Integration Endpoint", "integration_endpoint");
        Input.ShowTextBox("Integration Auth Endpoint", "integration_auth_endpoint");
        Input.ShowTextBox("Username", "username");
        Input.ShowPassword("Password", "password");
    %>
    ```
 
 3) Inside the Template, delete the **output.aspx** Template File, as this is not required.
 
 4) In the CMS, navigate to your Site Root, (e.g., **/Your Instance/Your Site**. Select **New -> File**.
 
 5) Complete as shown:
 
    ![New Config Asset](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-new-config-asset.png?raw=true "New Config Asset")

    _N.B._, Navigate to '**Project/Templates/Email Settings Config**' Template you created previously.
    
 6) Click '**Create**'.
 
 7) Open the Asset and complete as shown (with your details collected from Salesforce at the start of this document).
    Switch to Forms mode to see the field view of the Asset.
    
    1) **Client ID:** Your Consumer Key from Salesforce
    
    2) **Client Secret:** Your Consumer Secret Key from Salesforce
    
    3) **Integration Endpoint:** https://um5.salesforce.com/services/data/v39.0/sobjects
    
    4) **Integration Auth Endpoint:** https://login.salesforce.com/services/oauth2/token
    
    5) **Username:** Your Salesforce login account _(you can create one specific for this task if you wish)_.
    
    6) **Password:** Associated Salesforce login account password

 8) Save the '**_SFDC Config**' Asset.
 
 9) Right-click the '**_SFDC Config**' Asset in the File View and select '**Copy Path**'.
 
 10) Now re-open the '**sfdc_email_template.aspx**' Template File again and replace '**<your_config_asset_path>**' with
     your '**_SFDC Config**' Asset path (copied previously).
     
 11) Save '**sfdc_email_template.aspx**'.
 
#### Step 4 - Create a Workflow Step

 1) In DXM, go to the Settings UI.
 
    ![DXM Settings UI](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-settings-ui.png?raw=true "DXM Settings UI")

 2) Select **Workflow -> Workflows**. Click the '**New Workflow**' button. _N.B._, you can also add this as a step in
    an existing Workflow if you wish.
    
 3) Give your Workflow a suitable **name** and **description**.
 
 4) Create a '**new step**'.

    ![New Workflow Step](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-new-workflow-step.png?raw=true "New Workflow Step")
    
 5) Scroll down in the Release step and click '**Browse**' for the '**Execute File**' field.
 
 6) Navigate to the '**Project/Templates/Workflow Scripts/sfdc_email_template.aspx**' file.
 
    ![Execute File](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-execute-file.png?raw=true "Execute File")
    
 7) Save the Workflow.
 
#### Step 5 - Create the Email Template Asset

 1) In the CMS, navigate to the location where you want to create your Email Template.
 
 2) Select **File -> New -> File**
 
    ![New File](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-new-file.png?raw=true "New File")
    
    1) Enter a suitable name for your Email Template asset.
    
    2) Browse to your **Salesforce Email Template** (created previously).
    
    3) Select the **Workflow** that has the **Executable step** (created previously).

 3) Click '**Create**'.
 
 4) Open the **Salesforce Email Template** asset you just created.
 
 5) Select **Form** mode.
 
 6) In the **Config** section supply the details.  Make sure you use the **FolderID* you ascertained when setting up
    Salesforce at the beginning of this guide.

    ![Create Template Asset](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-create-template-asset.png?raw=true "Create Template Asset")
 
 7) Expand the **Email HTML Content** inside the **Body**. _N.B._, you can either build your Email HTML template here
    or paste in an existing template.
    
 8) On the WYSIWYG select the **Code** button.
 
    ![Source](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-source.png?raw=true "Source")
  
    _N.B._, to set Salesforce Lead variables in your template, use field names from the Lead Database, per the example\
    below. You can find your Lead Database field names by selecting **Setup** in Salesforce and searching for **leads**.
    
    ```
    {!Lead.First_Name}
    ```
    
    ![Salesforce Leads](../../../../images/screenshots/Salesforce-Sales-Cloud/mass-email-templates-salesforce-leads.png?raw=true "Salesforce Leads") 