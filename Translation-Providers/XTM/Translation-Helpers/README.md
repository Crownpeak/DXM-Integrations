<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# XTM Helper
Crownpeak's Translation Model Framework (TMF) is a collection of templates created and managed by Crownpeak that provides the author with the ability to manage multi-lingual content within a site, or localization activities across sites.

When preparing a site to go into production, it is often necessary to send large quantities of content for translation, and it can be more efficient to do this using large single-use translation projects, instead of making a project for each content item.

## Capabilities
The **XTM Helper** tool provides a number of different capabilities, both specific to XTM, and more generally within the CMS and TMF.

* **Export to a folder**: export a tree of content assets from the CMS into a folder on disk, in XML format suitable for sending for translation in XTM;

* **Create translation project from a folder**: create a single project in XTM to request translation of a folder containing content items from one language to another;
* **Download translation project results from XTM**: download a zip file from XTM containing translation results;
* **Import zip file into Crownpeak**: import a zip file containing translation results into Crownpeak;
* **Import a single file into Crownpeak**: import one xml file containing a single translated content file into Crownpeak;
* **TMF operations**:
    * **Create folder tree**: copy a folder tree (excluding content assets) from one locale to another;
    * **Copy assets**: copy the content assets in a tree from one locale to another;
    * **Relink**: move relative links between content to point to the appropriate asset within the target locale;
    * **TMF link**: create TMF link assets between all assets in source and target locales.

Use the **Help** button in the bottom-left of the tool's main window for further details on each function.

## Installation

1) Open the **XTM Helper.sln** in Visual Studio 2019

2) Click **Start** to build and run the solution
3) Enter the credentials required for the functions you wish to use (see Help for details)