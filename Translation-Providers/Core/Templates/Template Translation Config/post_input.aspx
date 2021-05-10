<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PostInputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%//This plugin uses PostInputContext as its context class type%>
<%
	if (!context.InputForm.HasField("template"))
	{
		context.ValidationErrorFields.Add("template", "Please select a template.");
	}
	else
	{
		var templateAsset = Asset.Load(context.InputForm["template"]);
		if (!templateAsset.IsLoaded)
		{
			context.ValidationErrorFields.Add("template", "Please select a template that can be loaded.");
		}
		else if (!templateAsset.IsFolder)
		{
			context.ValidationErrorFields.Add("template", "Please select a template folder.");
		}
		else if (!asset.Label.Equals(templateAsset.Label))
			asset.Rename(templateAsset.Label);
	}
%>