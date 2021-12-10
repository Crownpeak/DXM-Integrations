<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<%
	Input.StartControlPanel("Template translation settings");

	var siteRoot = Asset.GetSiteRoot(asset);
	var defaultFolder = siteRoot.IsLoaded && siteRoot.Id != 0
		? Asset.GetProject(asset).AssetPath + "/Templates"
		: "/System/Templates";
	Input.ShowSelectFolder("Template", "template", defaultFolder);

	var options = new[] { "Version 1", "Version 2" };
	if (string.IsNullOrWhiteSpace(asset["version"])) asset["version"] = "1";
	Input.StartDropDownContainer("Version", "version", options.ToDictionary(o => o, o => o.Split(' ').Last()), "2");

	// Version 1
	Input.ShowCheckBox("Opt-In", "opt_in", "yes", "", helpMessage: "Check to provide an allow-list of fields to include. Leave unchecked to provide a deny-list of fields to exclude.", defaultChecked:false);
	while (Input.NextPanel("Fields"))
	{
		Input.ShowTextBox("Field", "field");
	}

	Input.NextDropDownContainer();
	// Version 2
	Input.ShowMessage("Fields with names beginning 'ommsnippetid#', 'ommvariantid#', 'ommvarianttype#', 'upload#' and 'upload_name#' are automatically excluded, as is the 'translation_hash' field.");
	while (Input.NextPanel("fields2", displayName:"Fields"))
	{
		Input.StartHorizontalWrapContainer();
		var radios = new[] { "Key", "Value" };
		Input.ShowRadioButton("Apply to", "field_key", radios.ToDictionary(r => r, r => r.ToLowerInvariant()), radios.First().ToLowerInvariant());
		radios = new[] { "Include", "Exclude" };
		Input.ShowRadioButton("Include or exclude field", "field_include", radios.ToDictionary(r => r, r => r.ToLowerInvariant()), radios.First().ToLowerInvariant());
		Input.EndHorizontalWrapContainer();
		Input.ShowTextBox("Field", "field2");
	}
	var include = new[] { "Include", "Exclude" };
	Input.ShowRadioButton("Include or exclude everything not matched above?", "everything_else", include.ToDictionary(r => r, r => r.ToLowerInvariant()), include.Last().ToLowerInvariant());

	Input.EndDropDownContainer();

	Input.StartControlPanel("Preview");
	var sap = new ShowAcquireParams
	{
		DefaultFolder = Asset.GetSiteRoot(asset).AssetPath.ToString(),
		ShowBrowse = true,
		ShowUpload = false
	};
	Input.ShowAcquireDocument("Preview asset", "preview_asset", sap);
	Input.ShowMessage("If no preview asset is chosen, any asset using the chosen template will be used for the preview.");
	Input.EndControlPanel();

	Input.EndControlPanel();
%>