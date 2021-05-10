<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# XTM Translation Provider for TMF using DXM
Crownpeak's Translation Model Framework (TMF) is a collection of templates created and managed by Crownpeak that provides the author with the ability to manage multi-lingual content within a site, or localization activities across sites.

## Capabilities
The **XTM Translation Provider for TMF** provides the ability to send individual assets to XTM for translation. With additional configuration, it is also possible to send groups of assets to XTM for translation as a single project.

## Installation
Before installing the XTM Translation Provider, you should install the **<a href="../../Core/README.md">Translation Providers core for TMF using DXM.</a>**

### Library class
1) Navigate to your project's **Library** folder, and choose **File > New > Custom C# Class** from the menu

2) Enter the name as **TMF_XTM.cs** and click **Create**
3) Click **Edit** and paste the code from the **Library/TMF_XTM.cs** file.

4) Double-click on **TMF.cs** to edit it
5) Navigate to line **2371** and add an entry for XTM into the dictionary:
```c#
{"XTM", "xtm"}
```
6) Navigate to line **2379** and add an entry for XTM into the `switch` statment:
```c#
case "xtm":
    return new XtmTranslator();
```
8) Click **Save**

### Templates
1) Navigate to your project's **Templates** folder

2) Choose **File > New > Template** from the menu
3) Enter the name as **XTM Incoming** and click **Create**
4) For each file inside the template folder in the repository:

    1) Choose **File > New > \*.aspx** if it does not exist, or double-click to open
    2) Paste the code from the corresponding file in the repository
    3) Click **Save**

For items 5-10, choose either the ASPX or the JSP version of the webhook template, depending on your hosting configuration.

5) Choose **File > New > Template** from the menu

6) Enter the name as **XTM Webhook** and click **Create**
7) Double-click on **output.aspx** to edit
8) Paste the code from the corresponding file in the repository
9) Click **Save**
10) If your site has particular file naming or URL requirements, you may also need to implement **filename.aspx** and **url.aspx**

11) Expand the **TMF Translation Config** folder, and double-click on **input.aspx** to edit
12) Replace the line **TODO: Add translation providers here** and the following commented lines with:
```c#
new XtmTranslator().ConfigInput(asset, context);
Input.NextDropDownContainer();
```
13) Click **Save**

### Models
1) Navigate to your project's **Models** folder

2) Choose **File > New > Folder** from the menu
3) Enter the name as **XTM Incoming** and click **Create**
4) Choose **File > New > File** from the menu
5) Enter the name as **XTM Incoming File**
6) Click **Browse** and choose the **XTM Incoming** template
7) Do not choose a workflow
8) Click **Create**

If you wish to use the facility to send multiple assets for translation using a single project:

9) Navigate to your project's **Models** folder

10) Enter the name as **Translation Projects** and click **Create**
11) Choose **File > New > File** from the menu
12) Enter the name as **Translation Project**
13) Click **Browse** and choose the **TMF Translation Project** template
14) Do not choose a workflow
15) Click **Create**

### Webhook
The webhook is responsible for receiving results from XTM. It must be accessible without a username and password.
1) Navigate to a suitable content folder

2) Choose **File > New > File** from the menu
3) Enter a name for the webhook asset
4) Click **Browse** and choose the **XTM Webhook** template
5) Click **Create**
6) Publish the webhook asset to an environment without a username and password requirement

## Configuration

### Incoming Files
1) Navigate to your site's **_TMF/TMF Config** folder
2) Choose **File > New > Folder** from the menu
3) Enter **Translation Logs** for the name
4) Click **Browse** and choose the **XTM Incoming** model folder
5) Click **Create**

### TMF Configuration
1) Navigate to your site's **_TMF/TMF Config** folder
2) Right-click on **Translation Config** and choose **Edit Form**
3) Choose **XTM** from the **Translation Provider** dropdown
4) Populate **all** fields

### Multiple Translation Configuration
If you wish to use the facility to send multiple assets for translation using a single project:

1) Navigate to a suitable content folder within your site
2) Choose **File > New > Folder** from the menu
3) Enter the name as **Translation Projects**
4) Click **Browse** and choose the **Translation Projects** model folder
5) Click **Create**

### Component Library
If you are using Crownpeak's Component Library, checking the **Enable TMF** box on the **Template Builder** will generate code that references the **ServicesTMF** library, which does not include the translation enhancements. This cannot be modified at this time.

It is possible to edit the template code, once the files have been generated, but any modifications will be overwritten without warning the next time that the **Template Definition** is saved.

Our recommended solution is to create a new **TMF** component inside the Component Library, and to add this to each Template Definition with which you wish to use the TMF translation enhancements, and uncheck the **Enable TMF** box before saving. To do this:

1) Navigate to your project's **Component Library/Component Definitions** folder

2) Choose **File > New > Component** from the menu
3) Enter the name as **Tmf** and click **Create**
4) Leave the **Pattern** empty
5) Go to the **Advanced** tab, and check the **Hide from Content Block Panel?** box
6) Click **Save**
7) Go back to the **Advanced** tab, and click on the **Components_Tmf.cs** link (at the bottom of the page)
8) Replace the class definition (lines 13-36) with the following code:
```c#
public override void ComponentInput(Asset asset, InputContext context, string label, string name)
{
    try { LocalProject.TMF.Input.LoadInput(asset, context); } catch (Exception ex){}
}

public override void ComponentPostInput(Asset asset, PostInputContext context, string name, string index = "")
{
    try { LocalProject.TMF.PostInput.LoadPostInput(asset, context); } catch (Exception ex){}
}

public override string ComponentOutput(Asset asset, OutputContext context, string name, string index = "", bool isDrag = false)
{
    try { LocalProject.TMF.Output.LoadOutput(asset, context); } catch (Exception ex){}
    return string.Empty;
}
```
9) Click **Save**

10) For each component that you wish to use with the TMF translation enhancemens:
    1) Navigate to your project's **Component Library/Template Definitions** folder
    2) Right-click on the template definition file and choose **Edit**
    3) On the **Components** tab, expand the **Layouts** section
    4) Click the last **+** to add a new section at the end of the list
    5) Enter the section name as **TMF**
    6) Expand the **List: Components**
    6) In the **Selected Component** dropdown, choose **Tmf**
    7) On the **Settings** tab, uncheck the **Enable TMF** box
    8) Click **Save**