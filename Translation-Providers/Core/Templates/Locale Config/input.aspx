<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="LocalProject" %>
<%
    context.ShowSaveEditButton = true;
    Input.StartControlPanel("Locale settings");
        Input.ShowTextBox("Locale Title", "page_title");

        ShowAcquireParams sapDoc = new ShowAcquireParams();
        sapDoc.DefaultFolder = asset.Parent.Parent.AssetPath.ToString() + "/Country Sites config/";
        sapDoc.ShowBrowse = true;
        sapDoc.ShowUpload = false;
    
        Input.ShowAcquireDocument("Select Country Site", "site_select", sapDoc);

        sapDoc.DefaultFolder = asset.Parent.Parent.AssetPath.ToString() + "/Languages config/";
        Input.ShowAcquireDocument("Select Language", "lang_select", sapDoc);

        sapDoc.DefaultFolder = asset.Parent.Parent.Parent.Parent.AssetPath.ToString() + "/";
        Input.ShowSelectFolder("Root Folder for this Locale", "folder_root_path");
        Input.ShowTextBox("Locale Code", "locale_code", helpMessage:"See RFC 5646, e.g. \"en-US\" or \"es-ES\"");
    Input.EndControlPanel();

    Input.StartControlPanel("Translation Rule");
        Input.ShowMessage("This will override Site Configuration");
        Input.StartDropDownContainer("Use Translation Rule", "use_translation_rule", new Dictionary<string, string> { { "No", "n" }, { "Yes", "y" } });
        Input.NextDropDownContainer();
            Dictionary<string, string> dtLocales = new Dictionary<string, string>();
            dtLocales.Add("Please select a Locale", "");
            foreach (Asset aLocale in asset.Parent.GetFileList(SortOrder.OrderBy(AssetPropertyNames.Label)))
            {
                if (!int.Equals(asset.Id, aLocale.Id))
                    dtLocales.Add(aLocale.Label, aLocale.Id.ToString());
            }
           
            while (Input.NextPanel("translation_rule_panel"))
                Input.ShowDropDown("Destination Locale", "translation_rule_dest_locale", dtLocales);
            
        Input.EndDropDownContainer();
    Input.EndControlPanel();

    Input.StartControlPanel("Folder Selection");
        if (string.IsNullOrWhiteSpace(asset["folder_root_path"]))
        {
            Input.ShowMessage("Please select a root folder for this locale and save.");
        }
        else
        {
            Input.ShowHeader("Folders to exclude");
            Asset aFolderRoot = Asset.Load(asset["folder_root_path"]);
            if (aFolderRoot.IsLoaded)
            {
                foreach (Asset aFolder in aFolderRoot.GetFolderList(SortOrder.OrderBy(AssetPropertyNames.Label)))
                {
                    Input.ShowCheckBox("", "folder_id_" + aFolder.Id.ToString(), "1", aFolder.Label, unCheckedValue: "0");
                }
            }
        }        
    Input.EndControlPanel();
    
    TMF.Input.ShowSelectUsers();
%>