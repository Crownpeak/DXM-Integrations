<a href="https://www.crownpeak.com/" target="_blank">![Crownpeak Logo](../../images/logo/crownpeak-logo.png?raw=true "Crownpeak Logo")</a>

# Salesforce Live Agent Integration for Crownpeak DXM
<a href="https://www.salesforce.com/" target="_blank">Salesforce's</a> Live Agent allows you to initiate a personalised
live chat, based on how a customer or prospect is engaging with your site. Now with Snap-ins, you can also chat with
customers in your apps. Even better, with multilingual support you can deliver the same great enterprise chat support
around the globe.

In this Quick Start we will integrate Salesforce Live Agent with Crownpeak DXM, empowering editorial teams to choose
when and where they want to place the Live Agent function in their sites. We will use a free Salesforce Development
account, however, you will require a licenced version of Salesforce Live Agent to apply the exact same steps in your own
environments. 

## Capabilities
Enables the deployment of Salesforce Live Agent into a Asset (page) managed by Crownpeak DXM, with full preview
capabilities.

![Live Agent in DXM Preview](../../images/screenshots/Salesforce-Service-Cloud/live-agent-screenshot-1.png?raw=true "Live Agent in DXM Preview")

## Configuration Steps

### Setting up Salesforce
 1) Sign up for a free Salesforce Development account at <a href="https://developer.salesforce.com/signup/" target="_blank">https://developer.salesforce.com/signup</a>
and login to your account.

### Create a Live Agent Skill to direct chat requests to the correct Agent.
 1) In Salesforce, go to **My Development Account**. Open the users account that you want to act as an Agent.
**Administer -> Manage Users -> Users -> Edit**

    ![Salesforce Find User](../../images/screenshots/Salesforce-Service-Cloud/live-agent-list-users.png?raw=true "Salesforce Find User")

    _N.B._, Note, you may also use the Quick Find box to locate the menu option you require at any time:

    ![Salesforce Search](../../images/screenshots/Salesforce-Service-Cloud/live-agent-search.png?raw=true "Salesforce Search")

 2) Type ‘**Users**’ in the above and the menu option will be found.

 3) Once you have selected the desired agent’s user account, switch on the option for them to access the function in
Salesforce.

    ![Configure User](../../images/screenshots/Salesforce-Service-Cloud/live-agent-configure-user.png?raw=true "Configure User")

 4) Save the user profile.

 5) In the Quick Find box, type ‘**Skill**’.

    ![Salesforce Search Skills](../../images/screenshots/Salesforce-Service-Cloud/live-agent-search-skills.png?raw=true "Salesforce Search Skills")

 6) Click on ‘**Skills**’. In the Skills window, click on ‘**New**’.

    ![Salesforce Configure Skill](../../images/screenshots/Salesforce-Service-Cloud/live-agent-configure-skill.png?raw=true "Salesforce Create Skill")

    _N.B._, you will only see users that have the Live Agent check box selected in their profile in the Available Users box.
    We did this in the previous step. Your Profile list may be different to the one above as it will be specific to your
    organisation. You should select the profiles that the selected user/s will map to. e.g., if you have a Live Agent who
    will look after technical support then this could be a profile. 

### Create your chat button in Salesforce (optional)

_N.B._, this step is optional as you can apply your own buttons in Crownpeak DXM later if you would prefer to give the
editorial team the option to change the button choice. 

 1) In the Quick Find bar enter ‘Static Resources’.

    ![Salesforce Search Static Resources](../../images/screenshots/Salesforce-Service-Cloud/live-agent-search-static-resources.png?raw=true "Salesforce Search Static Resources")

 2) Open Static Resources.

 3) Click on ‘New’.
 
     ![Salesforce New Static Resource](../../images/screenshots/Salesforce-Service-Cloud/live-agent-new-static-resource.png?raw=true "Salesforce New Static Resource")

 4) Upload an offline button of your choice from your local desktop and save the resource.
 
 5) Now repeat this step for an online button. 

    ![Salesforce View Static Resources](../../images/screenshots/Salesforce-Service-Cloud/live-agent-view-static-resources.png?raw=true "Salesforce View Static Resources")

You now have two buttons available for your site to show when an agent is online and offline. These buttons will be
fetched by the imported JavaScript that you will create later in this tutorial. 

### Create scripts to run your online and offline status on your site

 1) In the Quick Find box enter ‘Chat buttons’.
 
 2) Open **Build -> Customize -> Live Agent -> Chat Buttons & Invitations**.
 
 3) Complete the following details:
 
    1) **Basic Information**
    
       ![Basic Information](../../images/screenshots/Salesforce-Service-Cloud/live-agent-basic-information.png?raw=true "Basic Information")
    2) **Routing Information** - To guide the site visitor to the Agent with skills to address their matter. 
    
       ![Routing Information](../../images/screenshots/Salesforce-Service-Cloud/live-agent-routing-information.png?raw=true "Routing Information")
    3) **Chat Button Customization**. Optional, assign the buttons uploaded previously to Salesforce to be used on the
    site. Note, you can also define other options, such as a Pre-chat Form (which you can use to generate an instant
    lead in Salesforce) and Post conversation URLs (to direct customers to where you want them to go after a chat
    session is complete i.e. a survey) etc. 
    
       ![Chat Button Customization](../../images/screenshots/Salesforce-Service-Cloud/live-agent-chat-button-customization.png?raw=true "Chat Button Customization")
 
 4) Save the Chat Button options.
 
    _N.B._, you will need a Site selected before you can save. If you do not have a site defined in Salesforce, then
    apply the following steps to create a site placeholder.
    
 5) Create a Site by entering ‘**Site**’ in the Quick Find box or navigating to **Build -> Develop -> Sites**
    
    ![Create Site](../../images/screenshots/Salesforce-Service-Cloud/live-agent-create-site.png?raw=true "Create Site")
 
 6) Complete the requisite entries in the Site similar to those shown above.
 
    _N.B._, the details of the site, for this tutorial, are not important as we will not use this site. In essence, it
    is a placeholder only and we will apply the site in Crownpeak DXM.
 
 7) Save the site.
 
### Get the Script for loading the buttons in your site

 1) In the Quick Find box type ‘**Chat buttons**’ and open **Build -> Customize -> Live Agent -> Chat Buttons and
    Invitations.**
 
 2) Open your Chat Button and Invitation created previously.
 
 3) Scroll to the bottom and you will see your Chat Button Code Script, copy this script to your clipboard for later
    use.
 
    ![Chat Button Code](../../images/screenshots/Salesforce-Service-Cloud/live-agent-chat-button-code.png?raw=true "Chat Button Code")
 
### Create a new Agent

 1) In the Quick Find box enter ‘**Live agent configurations**’.
 
 2) Open Live Agent Configurations.
 
 3) Setup the client to your needs. In this example, the default set up was used.
    
    ![Live Agent Configuration](../../images/screenshots/Salesforce-Service-Cloud/live-agent-configuration.png?raw=true "Live Agent Configuration")
    
    _N.B._, in the above configuration, you can setup how your Live Agent will function whilst in session. You can find
    more details at <a href="https://help.salesforce.com/articleView?id=live_agent_configuration_settings.htm&type=5" target="_blank">https://help.salesforce.com/articleView?id=live_agent_configuration_settings.htm&type=5</a>
 
 4) Save the Live Agent Configuration.

### Setup your Live Agent Deployment

 1) In the Quick Find box type ‘Deployments’.
 
 2) Open **Build -> Customize -> Live Agent -> Deployments**.
 
 3) Create your Live Agent Deployment and Save.
    
    ![Live Agent Deployment](../../images/screenshots/Salesforce-Service-Cloud/live-agent-deployment.png?raw=true "Live Agent Deployment")
 
 4) Open the Live Agent Deployment.
 
 5) Copy the Script Tags to your clipboard, for later use.
    
    ![Live Agent Deployment Code](../../images/screenshots/Salesforce-Service-Cloud/live-agent-deployment-code.png?raw=true "Live Agent Deployment Code")
    
    _N.B._, the deployment JavaScript that will control Live Agent functionality on your site will be stored in
    Salesforce and accessed through the Deployment Code.
    
    Whilst you could download and rebuild the JavaScript in your
    local web application, this is not recommended as the functionality is dynamic and using the method above will
    ensure future updates to your deployment’s functionality will automatically be applied to your site.

### Create a Salesforce App to provide a communication channel to your Live Agent function

 1) In the Quick Find box type ‘**Apps**’ and select **Build -> Create -> Apps**.
 
 2) Click the ‘**New**’ button.
    
    ![Live Agent New App](../../images/screenshots/Salesforce-Service-Cloud/live-agent-new-app.png?raw=true "Live Agent New App")
 
 3) Create your App using the default settings
    
    ![Live Agent App Default Settings](../../images/screenshots/Salesforce-Service-Cloud/live-agent-app-settings.png?raw=true "Live Agent App Default Settings")
    
    Map your Salesforce data that you want to associate with Live Agent. In this example, we are linking the chat to
    Lead generation only. 
    
    ![Map Data](../../images/screenshots/Salesforce-Service-Cloud/live-agent-map-data.png?raw=true "Map Data")      
 
 4) Save.
 
 Salesforce Live Agent is now setup and ready to be used in your site/s.
 
 You can switch on Live Agent to reflect the status in your site once we deploy the coded scripts you have saved in
 your clipboard.
 
 To access Live Agent, login as a user you defined as being a Live Agent. In the top right menu switch to your App.
 e.g., ‘**Marketing Leads with LiveAgent**’ that we created above.
 
 ![Access App](../../images/screenshots/Salesforce-Service-Cloud/live-agent-access-app.png?raw=true "Access App")
 
 At the bottom right of your new window you will see the Live Agent button. Click this button.
 
 ![Live Agent Button](../../images/screenshots/Salesforce-Service-Cloud/live-agent-button.png?raw=true "Live Agent Button")
 
 Set your Live Agent status to ‘**Online**’.
 
 ![Live Agent Status](../../images/screenshots/Salesforce-Service-Cloud/live-agent-status.png?raw=true "Live Agent Status")
 
### Implementing Live Agent in Crownpeak DXM

 1) Open your instance of Crownpeak DXM.
 
 2) Create a new template and open the Output Template File (output.aspx).
 
 3) Add the Button Code to your template from the saved code in your clipboard (example below):
 
     ```
     <img id="liveagent_button_online_5731v0000008gLS" style="display: none; border: 0px none; cursor: pointer" onclick="liveagent.startChat('5731v0000008gLS')" src="https://blacksheep101-developeredition.eu17.force.com/resource/1536332317000/LiveAgentOnline" />
     <img id="liveagent_button_offline_5731v0000008gLS" style="display: none; border: 0px none;" src="https://blacksheep101-developeredition.eu17.force.com/resource/1536333181000/LiveAgentOffline" />
     <script type="text/javascript">
        if (!window._laq) { window._laq = []; }
        window._laq.push(function() {
           liveagent.showWhenOnline('5731v0000008gLS', document.getElementById('liveagent_button_online_5731v0000008gLS'));
           liveagent.showWhenOffline('5731v0000008gLS',
           document.getElementById('liveagent_button_offline_5731v0000008gLS')); 
        });
     </script>
     ```
 
 4) At the end of your template, add your Deployment Script tags (example below):
 
     ``` 
     <script type='text/javascript' src='https://c.la1-c1-
        fra.salesforceliveagent.com/content/g/js/44.0/deployment.js'></script>
        <script type='text/javascript'>
        liveagent.init('https://d.la1-c1-fra.salesforceliveagent.com/chat',
        '5721v0000008jYF', '00D0Y000000ZAbf');
     </script>
     ```
 
 5) Save the template and select Preview.
 
    _N.B._, you can add your HTML and Content Managed Fields as required.
 
 6) You will see the button loads from Salesforce and is set to show the Offline button.
 
 7) If you now set Live Agent to ‘Online’ in Salesforce you will see the Online button become active in your page and
 you can start the chat. You may need to refresh the view to see the updated status.
 
    ![Live Agent Working](../../images/screenshots/Salesforce-Service-Cloud/live-agent-working.png?raw=true "Live Agent Working")

## Considerations

Here is an example template, with a standard Bootstrap design, that uses editor imported buttons from CMS:

* <a href="https://github.com/richardhamlyn/dxm-templates/blob/master/liveagent-output" target="_blank">https://github.com/richardhamlyn/dxm-templates/blob/master/liveagent-output</a>
* <a href="https://github.com/richardhamlyn/dxm-templates/blob/master/liveagent-input" target="_blank">https://github.com/richardhamlyn/dxm-templates/blob/master/liveagent-input</a>

Note, you could also give your editors the option to change the buttons:

```
<a class="button_bLeft slidebttn" id="button_bLeft" onclick="liveagent.startChat('5731v0000008gLS')">
  <img src="<%= asset["la_image"]%>" width="25">
</a>
```

_N.B._, do not change the onclick event call, or it will stop working!

For which, within the Input Template File (input.aspx), you would add:

```
Input.ShowHeader("Widget Options");
Input.ShowAcquireImage("LiveAgent Image", "la_image");
```