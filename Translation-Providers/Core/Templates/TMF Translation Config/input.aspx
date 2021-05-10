<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="LocalProject" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses InputContext as its context class type%>
<%
	Input.StartTabbedPanel("Translation Configuration");

	var providers = TMF.Translation.GetTmfTranslators(asset);
	Input.StartDropDownContainer("Translation Provider", "translation_provider", providers, providers.Values.First());
	// None
	Input.NextDropDownContainer();
	// TODO: Add translation providers here
	// new ExampleTranslator().ConfigInput(asset, context);
	// Input.NextDropDownContainer();
	Input.EndDropDownContainer();
	Input.EndTabbedPanel();
%>
