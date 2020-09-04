<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# commercetools Integration for DXM

<a href="https://commercetools.com/" target="_blank">commercetools</a> - The eCommerce Solution for Innovators and
Visionaries. Increase business agility and innovate across new business models. Build your commerce architecture of the
future with the most flexible, cloud based commerce API solution on the market.

This integration allows a content author to fully-manage the non-ecommerce/product management experience of the
commercetools Sunrise demonstration site from within DXM.

## Capabilities
The commercetools Integration for DXM leverages <a href="https://github.com/commercetools/sunrise-spa" target="_blank">Sunrise SPA</a>,
commercetool's sample single page application site, and extends it to allow full content management from within DXM.

A <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>
manifest is included, to allow you to provision Sunrise SPA within DXM in minutes.

![Sunrise SPA Preview in DXM](../../../images/screenshots/commercetools/commercetools-screenshot-5.png?raw=true "Sunrise SPA Preview in DXM")

Sunrise SPA preview within DXM Interface.

![Sunrise SPA Inline in DXM](../../../images/screenshots/commercetools/commercetools-screenshot-6.png?raw=true "Sunrise SPA Inline in DXM")

Supporting Drag & Drop content editing experience.

## Configuration Steps (with <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>)
 1) Download <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a> and
    install, per the instructions.

 2) Download <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> to your
    favourite location.

 3) Open <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a> and
    select **Import Data**.
 
    ![Crownpeak-Content-Xcelerator℠ - Import Data](../../../images/screenshots/commercetools/content-xcelerator-import-data.png?raw=true "Crownpeak-Content-Xcelerator℠ - Import Data")
 
 4) Complete information within "Import" screen with your details (see the instructions on the <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>
    if you are unsure).
    
    ![Crownpeak-Content-Xcelerator℠ - Authentication](../../../images/screenshots/commercetools/content-xcelerator-authentication.png?raw=true "Crownpeak-Content-Xcelerator℠ - Authentication")
         
 5) Select <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> and pick
    the "commercetools Vue SDK" root folder.
    
 6) Click **Next** and wait for any **Problems** to be identified. Assuming none are, click **Next** again.
 
 7) Choose the DXM Folder Path where you would like the content deployed, and click **Go**.
 
    (_N.B._ A top-level folder called "commercetools Vue SDK" will be created automatically.)
        
 8) Wait for <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> to
    be deployed.
    
    ![Sunrise SPA Import Complete](../../../images/screenshots/commercetools/content-xcelerator-import-complete.png?raw=true "Sunrise SPA Import Complete")    
    
 9) Browse to your deployed Sunrise SPA site and open the **Homepage** Asset in Preview Mode.
 
 10) Finally, configure your **publishing** requirements within DXM, including Search G2 Collection configuration and 
    add these to the .env file, as required per the <a href="https://github.com/Crownpeak/dxm-vuejs-sdk" target="_blank">DXM Vue.JS SDK</a>
    requirements to support data collection & scaffolding.
    
 11) Run ``yarn start`` to start the local development server.
 
        ![Sunrise SPA running locally](../../../images/screenshots/commercetools/commercetools-screenshot-7.png?raw=true "Sunrise SPA running locally")
    
 12) Make any required changes to Vue.JS Components and then re-scaffold to DXM, as required.
