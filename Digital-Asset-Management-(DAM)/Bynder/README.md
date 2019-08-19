<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

#Bynder Integration for DXM
<a href="https://www.bynder.com/en/" target="_blank">Bynder's</a> digital asset management solution improves your
digital asset flow from creation to approval, to deliver consistent content across all of your channels.

This integration allows a DXM content author to use a Draggable-Component, populated by Bynder, to be used within a
DXM-generated experience.

##Capabilities
The Bynder Integration for DXM allows content authors to use a Bynder "aware" Component to enhance their digital
experience.

![Bynder Component](../../images/screenshots/Bynder/bynder-component.png?raw=true "Bynder Component")

Upon click of the image, you will need to login to your Bynder account.

![Bynder Login](../../images/screenshots/Bynder/bynder-login-1.png?raw=true "Bynder Login")

![Bynder Login](../../images/screenshots/Bynder/bynder-login-2.png?raw=true "Bynder Login")

You will now be able to use Bynder's Compact View modal overlay to search for and select an image.

![Bynder Compact View](../../images/screenshots/Bynder/bynder-compact-view.png?raw=true "Bynder Company View")

Selecting an image and clicking "Add Media" will add the content to the DXM interface. Click the image to re-select
from the Bynder Compact View.

![Bynder Asset in Situ](../../images/screenshots/Bynder/bynder-asset-in-situ.png?raw=true "Bynder Asset in Situ")

You can now save the Asset as normal and the image will be persisted on the Asset.

##Configuration Steps
Within your Project, create a new component called "Bynder Image" and set the following pattern:
```
<img src="" alt="Select Bynder image" style="visibility: hidden" data-cp-integration="bynder" />
<div style="display: none">{image : Text} {image_alt : Text}</div>
```
![Create Component](../../images/screenshots/Bynder/create-component.png?raw=true "Create Component")

Edit the generated Components_BynderImage.cs, inside the ComponentOutput function (before the return) add:
```
if (context.IsPublishing)
{
    // TODO: change this if you modify the component markup
    ComponentMarkup = @"<img src=""{image : Text}"" alt=""{image_alt : Text}"" />";
}
```
![Component Output Amends](../../images/screenshots/Bynder/component-output-amends.png?raw=true "Component Output Amends")

Upload <a href="./js/bynder-integration.js?raw=true" target="_blank">bynder-integration.js</a> from this repository into a suitable folder. No workflow will be required, as it doesn't
need to be published.
![Binder Integration JS](../../images/screenshots/Bynder/bynder-integration-js.png?raw=true "Binder Integration JS")

In each applicable output.aspx using Drag-and-Drop, above the first Out.ShowDragDrop line, paste (in ASPX context):
```
if (!context.IsPublishing) {
    // TODO: change path below to match the location of your bynder-integration.js
    Out.WriteLine("<script src=\"" + Asset.Load(Asset.GetSiteRoot(asset).AssetPath + "/_Assets/js/bynder-integration.js").GetLink(LinkType.Include) + "\"></script>");
}
```
![Template Amends](../../images/screenshots/Bynder/template-amends.png?raw=true "Template Amends")