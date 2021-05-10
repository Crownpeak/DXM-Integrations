<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PostSaveInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="LocalProject" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses PostSaveContext as its context class type%>
<%
	var translator = TMF.Translation.GetTmfTranslator(asset) as XtmTranslator;
	if (translator == null)
	{
		asset.SaveContentField("error", "Unable to get XTM Translator");
	}
	else
	{
		translator.ProcessIncomingFile(asset, false);
	}
%>