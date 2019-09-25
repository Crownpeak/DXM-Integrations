<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# Salesforce Web-to-Lead Integration for Crownpeak Web Content Optimizer (WCO)
<a href="https://www.salesforce.com/" target="_blank">Salesforce's</a> Customer Relationship Management (CRM) solution
gives your sales teams the power to close deals like never before with cloud-based tools that increase productivity,
keep pipeline filled with leads, and score more wins.

This turnkey integration creates a connector between Crownpeak DXM and Salesforce CRM that will generate leads.
The connector also allows you to create content in WCO that is built in to the Crownpeak DXM platform and
allows you to personalise content based on lead data.

![WCO/Salesforce Logical Diagram](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-logical-diagram.png?raw=true "WCO/Salesforce Logical Diagram")

## Configuration Steps
Open your Salesforce account and find your Organisational ID. In the Salesforce Quick Find box type ‘Company’.

![Salesforce Find Company Dialog](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-find-company.png?raw=true "Salesforce Find Company Dialog")

Click on ‘Company Information’. In the Company Information window, locate the Organization ID. Save this ID for later.

![Salesforce Find Organization ID](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-organization-id.png?raw=true "Salesforce Find Organization ID")

Within DXM, open WCO Standalone.

![DXM WCO Standalone](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-standalone.png?raw=true "DXM WCO Standalone")

 1. Open Web Content Optimizer.
 2. Select Settings -> Global Settings.
 3. Select Manage Connectors.
 4. Select the Salesforce Connector.
 5. Add the Organisational ID you copied previously.
 
    ![Add Organization ID to WCO](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-organization-id-wco.png?raw=true "Add Organization ID to WCO")
 
 6. Enter your OID in the Value field and save the connector.
 7. You can now go back to your V3 instance to continue the setup.
 8. Open WCO and select Forms. 

    ![WCO Forms](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-forms.png?raw=true "WCO Forms")

Now, create a new form. We will use this form on a Web Page to inject visitor data to WCO and Salesforce using the
Connector we created earlier. 

![WCO Form Setup](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-form-setup.png?raw=true "WCO Form Setup")

This example form will create an initial lead in Salesforce. You may add fields you want to map. 

Click on Edit for each field and ensure each Form Element’s Name maps to your Salesforce Data Field Names exactly. 

![Map Fields](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-map-fields.png?raw=true "Map Fields")

To find the names of your Salesforce Lead fields, open Salesforce and in the Quick Find box type ‘Leads’.
Select Build -> Customize -> Leads -> Fields.

![Customize Fields](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-customize-fields.png?raw=true "Customize Fields")

You can now map your Form Field Names with Salesforce Lead Field Names:

![Map All Fields](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-map-fields-all.png?raw=true "Map All Fields")

Click Next in the Form Builder tool. 

* **General (tab):** Give your form a name i.e. Salesforce Lead Form;
* **Notification:** Add details if you want to inform an individual that a form was submitted. (_N.B._, you can also set
this up in Salesforce when the Lead is generated).
* **Rules:** Add a redirect page URL for when the form is submitted. In the Connector, click on the dropbox and select the
Salesforce Connector created previously. 

    ![Select Connector](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-select-connector.png?raw=true "Select Connector")

* **Auto-Reply:** Add a standard response email that will be sent when the visitor submits the form.

Now, save the form. The form is now ready for insertion into a Page. 

Open an Aasset that has a WCO enabled WYSIWYG field embedded in it. Click on the option to ‘Create Snippet’.

![Create Snippet](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-create-snippet.png?raw=true "Create Snippet")

Provide a suitable name for the Snippet and then save the asset.

Now, click on the ‘Form’ option.

![Insert Form](../../../../images/screenshots/Salesforce-Sales-Cloud/wco-insert-form.png?raw=true "Insert Form")

Save and Preview the page. (_N.B._, the form will not show styling in the WYSIWYG unless it has been bound with a
StyleSheet; however, Preview should show the form exactly how it will look on the site.)

You can now Publish the Page with your Lead Generating Form and test it out. When a visitor submits the form, it will
create a record in WCO and a Lead in Salesforce.

## Considerations
At the time of writing, the Salesforce integration only works to inject data in to the CRM; however, it is possible to
gather lead data at the front-end using a coded solution (covered in a separate Quick Start Guide).

You may create personalisation rules around the lead data that was gathered in WCO. In the example within this guide,
that could be set to deliver content dependant on the visitor’s Company or other data that you have chosen acquire. 