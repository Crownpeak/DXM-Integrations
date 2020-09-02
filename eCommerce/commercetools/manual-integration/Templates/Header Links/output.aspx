<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.OutputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<% 
	// output.aspx: template file to specify the published content in site HTML
	// if no preview.aspx exists, then this is used by default for preview
	foreach (var panel in asset.GetPanels("list_items"))
	{
		var index = panel.GetFieldName("text").Split(":".ToCharArray()).LastOrDefault() ?? "";
		if (!string.IsNullOrWhiteSpace(index)) index = ":" + index;
%>
<li class="list-item-help">
    <a href="<%= new Href().ComponentOutput(asset, context, "link", index) %>" class="link-help"><%= panel["text"] %></a>
</li>
<%
	}
%>