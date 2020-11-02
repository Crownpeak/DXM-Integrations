# Release Management

What is included:

In the Content Xcelerator import file are the following assets:

1. Release template: Used to create the 'release' unit of operation. Releases can be based on:
    * Workflow matching: with this option you specify a root folder to start from and the template will find all assets matching the chosen "from" workflow state.
    * Asset selection: editors can pick specific assets to include in the release. This allows assets in a mix of current workflow states to be selected and they will all be routed to the target workflow state.
    * No assets: used for releases that just specify assets to retire.

    Releases can also specify a list of assets to retire.
1. Release Configuration template: Use to create a release configuration asset. The configuration should list out the workflows to allow releases to be based on as well as the states from and to which content can be routed. The state configuration also permits restricting the transition by specific groups.
1. Release workflow: The workflow that Release assets should be using.

Installation:

1. Use [Content Xcelerator](https://github.com/Crownpeak/Content-Xcelerator) and import the Release Management install file. Import the configuration into an empty content folder as we're going to be moving the contents to the final destination.
1. Create a folder under /System called `Release Management`.
1. Copy the resources to the System folder according to the following map

    Imported Resource | Target Destination
    --- | ---
    /Release Management/_Releases | /System/Release Management/Sample Configuration
    /Release Management/Project/Models | /System/Release Management/Models
    /Release Management/Project/Templates | /System/Release Management/Templates
    /Release Management/Project/Templates/Scripts | /System/Release Management/Scripts
    /Release Management/Project/Workflows/Release Workflow | /System/Workflows/

1. Go to Settings > Workflow > Workflows and ensure that the Release Workflow is being listed.
2. Edit Release Workflow. Click on the `Completed` status in the overview to go to the detail view for the workflow step. Correct the property `Execute File` points to `/System/Release Management/Scripts/release_v3.aspx`.

Setting up for use:

Once you have the elements of Release Management installed, you can set up individual sites with a release configuration.

1. Create a folder under your site root -- we recommend '_Releases' -- and set the model to be `/System/Release Management/Models/Releases`. The leading underscore isn't significant but it does ensure that the folder appears at the top of the list of files. When creating the folder
2. In the new folder, create an asset called `Release Configuration` that uses the template `/System/Release Management/Templates/Release Configuration` with no workflow assigned.
3. Edit the new `Release Configuration` asset and set up the workflows, transitions and groups permitted to use each transition.
4. (Optional) Set up access control on the `_Releases` folder so that only appropriate users can see and access the folder and its content.

Notes:

* Implementations will need to decide on whether and how they want to restrict use of the release management features. As a suggestion, set up a new group 'Release Admin' that will be the only group with visibility of the _Releases folder.
* Once a release has been deployed, the release contents cannot be modified. You can however use the workflow command "Redeploy" to run the release deployment process again with the same configuration. This may be useful in cases where you have a publishing error that needs to be corrected before attempting the redeploy.
  