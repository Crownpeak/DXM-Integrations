<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses InputContext as its context class type%>
<%
	// input.aspx: template file to specify content entry input form
	while (Input.NextPanel("list_items"))
	{
		new Href().ComponentInput(asset, context, "Link", "link");
		Input.ShowTextBox("Text", "text");
	}
%>
