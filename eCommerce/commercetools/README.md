<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# commercetools Integration for DXM

<a href="https://commercetools.com/" target="_blank">commercetools</a> - The eCommerce Solution for Innovators and
Visionaries. Increase business agility and innovate across new business models. Build your commerce architecture of the
future with the most flexible, cloud based commerce API solution on the market.

This integration allows a content author to fully-manage the non-ecommerce/product managemenbt experience of the
commercetools Sunrise demonstration site from within DXM.

## Capabilities
The commercetools Integration for DXM leverages <a href="https://github.com/commercetools/sunrise-spa" target="_blank">Sunrise SPA</a>,
commercetool's sample single page application site, and extends it to allow full content management from within DXM.

A <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>
manifest is included, to allow you to provision Sunrise SPA within DXM in minutes.

Optionally, an Amazon Web Services (AWS) CloudFormation Template is included within this repo, to allow rapid provisioning of
the public-facing experience within a serverless AWS environment.

![Sunrise SPA Preview in DXM](../../images/screenshots/commercetools/commercetools-screenshot-1.png?raw=true "Sunrise SPA Preview in DXM")

Sunrise SPA preview within DXM Interface.

![Sunrise SPA Inline in DXM](../../images/screenshots/commercetools/commercetools-screenshot-2.png?raw=true "Sunrise SPA Inline in DXM")

Images swapped from items within DXM DAM, simply using Drag & Drop to replace the default ones.

![Sunrise SPA Inline Showing Footer Links Changed](../../images/screenshots/commercetools/commercetools-screenshot-3.png?raw=true "Sunrise SPA Inline Showing Footer Links Changed")

Footer links changed, adding Crownpeak link.

![Published Sunrise SPA - on AWS Serverless](../../images/screenshots/commercetools/commercetools-screenshot-4.png?raw=true "Published Sunrise SPA - on AWS Serverless")

Published commercetools Demonstration Site to AWS Serverless configuration.

## Configuration Steps (with <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>)
 1) Download <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a> and
    install, per the instructions.

 2) Download <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> to your
    favourite location.

 3) Open <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a> and
    select **Import Data**.
 
    ![Crownpeak-Content-Xcelerator℠ - Import Data](../../images/screenshots/commercetools/content-xcelerator-import-data.png?raw=true "Crownpeak-Content-Xcelerator℠ - Import Data")
 
 4) Complete information within "Import" screen with your details (see the instructions on the <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>
    if you are unsure).
    
    ![Crownpeak-Content-Xcelerator℠ - Authentication](../../images/screenshots/commercetools/content-xcelerator-authentication.png?raw=true "Crownpeak-Content-Xcelerator℠ - Authentication")
         
 5) Select <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> and pick
    all Assets under the commercetools site root.
    
    ![Crownpeak-Content-Xcelerator℠ - Pick Assets](../../images/screenshots/commercetools/content-xcelerator-export-settings.png?raw=true "Crownpeak-Content-Xcelerator℠ - Pick Assets")
 
 6) Click **Next** and wait for any **Problems** to be identified. Assuming none are, click **Next** again.
 
 7) Choose the DXM Folder Path where you would like the content deployed, and click **Go**.
 
    (_N.B._ A top-level folder called commercetools will be created automatically.)
 
    ![Crownpeak-Content-Xcelerator℠ - Choose Folder Path](../../images/screenshots/commercetools/content-xcelerator-top-folder.png?raw=true "Crownpeak-Content-Xcelerator℠ - Choose Folder Path")
        
 8) Wait for <a href="./Crownpeak-Content-Xcelerator℠/Sunrise-SPA-DXM.xml" target="_blank">Sunrise-SPA-DXM.xml</a> to
    be deployed.
    
    ![Sunrise SPA Import Complete](../../images/screenshots/commercetools/content-xcelerator-import-complete.png?raw=true "Sunrise SPA Import Complete")    
    
 9) Browse to your deployed Sunrise SPA site and open the **Home** Asset in Preview Mode.
 
    ![Sunrise SPA Preview in DXM](../../images/screenshots/commercetools/commercetools-screenshot-1.png?raw=true "Sunrise SPA Preview in DXM")

 10) Finally, configure your **publishing** requirements within DXM.

## Configuration Steps (Manually)
We recommend using the steps above, combined with <a href="https://github.com/crownpeak/content-xcelerator" target="_blank">Crownpeak Content Xcelerator℠</a>
to install Sunrise SPA into DXM - however, if you do choose to install manually, you may follow the steps below.

 1) Clone/download <a href="https://github.com/commercetools/sunrise-spa" target="_blank">Sunrise SPA</a> from
    commercetool's GitHub repository.
    
 2) Follow all installation/configuration instructions in the <a href="https://github.com/commercetools/sunrise-spa" target="_blank">Sunrise SPA</a>
    repository.
    
 3) Add/replace source files within the contents of <a href="./src/" target="_blank">./src/</a> including all sub-folders within
    this repository.
    
 4) Copy files within <a href="./public/" target="_blank">./public/</a> into /public/ within your repository.
    
 5) Re-build the Sunrise SPA site.
 
    ```
    npm run build
    ```
    
 6) Upload the contents of **/dist/** within your site into a new Folder within DXM.

 7) Create new **Project** in DXM to hold Templates & Library Files. Ensure that Project is created include **Component Library.**
 
 8) Create New **Library Files**, per contents of <a href="./Library/" target="_blank">./Library/</a>
 
 9) Create New **Templates**, per contents of <a href="./Templates/" target="_blank">./Templates/</a>

 10) Create relevant **Assets** using the **Templates** that you have configured.
 
 11) Finally, configure your **publishing** requirements within DXM. 

## Configuration Steps (AWS Serverless Deployment)
This is an optional step, using an Amazon Web Services (AWS) CloudFormation Template, to allow rapid provisioning of
the public-facing experience within a serverless AWS environment (S3, CloudFront and Lambda).

 1) Login to your **Amazon Web Services Console** and visit **CloudFormation**.
 
 2) Select the **Region** that you wish to deploy the CloudFormation Stack into and click **Create Stack**.
 
 3) Upload <a href="./AWS-CloudFormation/Sunrise-SPA-DXM-Serverless.yaml" target="_blank">Sunrise-SPA-DXM-Serverless.yaml</a>.
 
 4) Give the **Stack** a name (_TIP_: I use the Fully-Qualified Domain Name (FQDN), to make it easy to find in future).
 
 5) Supply the **ARN** for your pre-generated **SSL Certificate**, managed by **AWS Certificate Manager**.
 
 6) Supply the **FQDN**.
 
 7) Complete the wizard to provision the **CloudFormation Stack**.
 
 8) Once completed, create an **IAM Policy** to enable the DXM to upload files to the **S3 Bucket**. See below for an
    example:
    
    ```
    {
        "Version": "2012-10-17",
        "Statement": [
            {
                "Effect": "Allow",
                "Action": "s3:ListAllMyBuckets",
                "Resource": "arn:aws:s3:::*"
            },
            {
                "Effect": "Allow",
                "Action": [
                    "s3:ListBucket",
                    "s3:GetBucketLocation"
                ],
                "Resource": [
                    "{The_S3_Bucket_ARN}"
                ]
            },
            {
                "Effect": "Allow",
                "Action": [
                    "s3:ListBucket",
                    "s3:PutObject",
                    "s3:GetObject",
                    "s3:DeleteObject"
                ],
                "Resource": [
                    "{The_S3_Bucket_ARN}/*"
                ]
            }
        ]
    }
    ```
    
 9) Create an **IAM User**, inheriting the **IAM Policy**. Configure **publishing** from within DXM, using the
    **Export S3** settings.
    
 10) Browse to your site.
 
   ![Published Sunrise SPA - on AWS Serverless](../../images/screenshots/commercetools/commercetools-screenshot-4.png?raw=true "Published Sunrise SPA - on AWS Serverless")