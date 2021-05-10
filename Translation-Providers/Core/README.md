<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# Translation Providers core for TMF using DXM
Crownpeak's Translation Model Framework (TMF) is a collection of templates created and managed by Crownpeak that provides the author with the ability to manage multi-lingual content within a site, or localization activities across sites.

## Capabilities
The core module includes the TMF source code, and provides an interface to implement to provide interaction with third-party translation services. By implementing this interface, developers can enhance the core TMF capabilities by adding single- and multi-asset translation functionality.

## Installation

### Library class
1) Navigate to your project's **Library** folder, and choose **File > New > Custom C# Class** from the menu

2) Enter the name as **TMF.cs** and click **Create**
3) Click **Edit** and paste the code from the **Library/TMF.cs** file.

### Templates
1) Navigate to your project's **Templates** folder

2) For each folder inside the **Templates** folder in the repository:

    1) Choose **File > New > Template** from the menu
    2) Enter a name to match the folder from the repository and click **Create**
    3) For each file inside the template folder in the repository:

        1) Choose **File > New > \*.aspx** if it does not exist, or double-click to open
        2) Paste the code from the corresponding file in the repository

### Models
1) Navigate to your project's **Models** folder

2) For each folder inside the **Models** folder in the repository:

    1) Choose **File > New > Folder** from the menu
    2) Enter a name to match the folder from the repository and click **Create**
    3) For reach file inside the model folder in the repository:
        
        1) Choose **File > New > File** from the menu
        2) Enter a name to match the file from the repository
        3) Click **Browse** and choose the template that matches this file name
        4) Do not choose a workflow
        5) Click **Create**

## Configuration

### Locales
1) Navigate to your site's **_TMF** folder
2) Right-click on the **Locales Config** folder and choose **Edit Properties**
3) Click **Model** on the Properties panel and click **Browse**
4) Select the **Locales** folder and click **OK**
5) Click **Save** to apply the model

If there are any files already inside your **Locales Config** folder:
1) Right-click any file in the **Locales Config** folder and choose **Edit Properties**
2) Click **Template** on the Properties panel and click **Browse**
3) Select the **Locale Config** template folder and click **OK**
4) Click **Apply to additional assets**
5) On the **Relate By...** box, choose **File Siblings**
6) Check the box next to the **Label** heading to select all rows
7) Click on **Save changes for n asset(s)**

Now create or edit locales as required. Ensure that each locale you will use for translation has a valid **Loacale Code** according to RFC 5646, for example **en-US** or **es-MX**.

### Translation Logs folder
1) Navigate to your site's **_TMF** folder
2) Choose **File > New > Folder** from the menu
3) Enter **Translation Logs** for the name and click **Create**

### TMF Configuration
1) Navigate to your site's **_TMF** folder
2) Choose **File > New > File** from the menu
3) Enter **Translation Config** as the name
4) Click **Browse** and choose the template that matches this file name
5) Do not choose a workflow
6) Click **Create**

Until you create a translation provider, this asset should remain blank

### Template Translations
1) Navigate to your site's **_TMF** folder

2) Choose **File > New > Folder** from the menu
3) Enter **Template Translations Config** as the name
4) Click **Browse** and choose the **Template Translations** model folder, then click **OK**
5) Click **Create**
6) For each template which will have content for translation:

    1) Choose **File > New > Template Translation** from the menu
    2) Enter the template name as the name and click **Create**
    3) Click **Select** and choose your content template, then click **OK**
    4) Check the **Opt-in** box or not, according to your requirements
    5) Enter one or more field names in the **Fields** list panel
    6) Click **Save**

### Translation Log Monitor (optional)
If you wish to run regular checks on the progress of existing translation requests:

1) Navigate to your site's **_TMF/Translation Logs** folder
2) Choose **File > New > File** from the menu
3) Enter **_Monitor** as the name
4) Click **Browse** and choose the template that matches this file name
5) Select a workflow having a **Live** state
6) Click **Create**
7) Right-click on the new **_Monitor** asset and choose **Edit Properties**
8) Click on **Set Schedule** in the Properties panel
9) Choose your desired scheduling options and click **Save**