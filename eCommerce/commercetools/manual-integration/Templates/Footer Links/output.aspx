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
%>
<div class="col-sm-2">
  <div class="footer-col">
    <ul>
      <li class="footer-title hidden-xs"><%= asset["column_1_title"] %></li>
<%
	foreach (var panel in asset.GetPanels("column_1_items"))
	{
		var index = panel.GetFieldName("column_1_text").Split(":".ToCharArray()).LastOrDefault() ?? "";
		if (!string.IsNullOrWhiteSpace(index)) index = ":" + index;
%>
<li><a href="<%= new Href().ComponentOutput(asset, context, "column_1_link", index) %>"><%= panel["column_1_text"] %></a></li>
<%
	}
%>
    </ul>
  </div>
</div>
<div class="col-sm-2 hidden-xs">
  <div class="footer-col">
    <ul>
        <li class="footer-title hidden-xs"><%= asset["column_2_title"] %></li>
<%
	foreach (var panel in asset.GetPanels("column_2_items"))
	{
		var index = panel.GetFieldName("column_2_text").Split(":".ToCharArray()).LastOrDefault() ?? "";
		if (!string.IsNullOrWhiteSpace(index)) index = ":" + index;
%>
<li><a href="<%= new Href().ComponentOutput(asset, context, "column_2_link", index) %>"><%= panel["column_2_text"] %></a></li>
<%
	}
%>
    </ul>
    </div>
</div>
<div class="col-sm-2 hidden-xs">
  <div class="footer-col">
    <ul>
        <li class="footer-title hidden-xs"><%= asset["column_3_title"] %></li>
<%
	foreach (var panel in asset.GetPanels("column_3_items"))
	{
		var index = panel.GetFieldName("column_3_text").Split(":".ToCharArray()).LastOrDefault() ?? "";
		if (!string.IsNullOrWhiteSpace(index)) index = ":" + index;
%>
<li><a href="<%= new Href().ComponentOutput(asset, context, "column_3_link", index) %>"><%= panel["column_3_text"] %></a></li>
<%
	}
%>
    </ul>
  </div>
</div>
