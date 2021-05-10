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

	Input.ShowCheckBox("Opt-In", "opt_in", "yes", "", helpMessage: "Check to provide an allow-list of fields to include. Leave unchecked to provide a deny-list of fields to exclude.", defaultChecked:false);
	while (Input.NextPanel("Fields"))
	{
		Input.ShowTextBox("Field", "field");
	}

	Input.EndControlPanel();
%>