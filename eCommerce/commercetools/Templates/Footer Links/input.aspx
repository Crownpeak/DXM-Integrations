<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses InputContext as its context class type%>
<%
	// input.aspx: template file to specify content entry input form
	Input.StartTabbedPanel("Column 1", "Column 2", "Column 3");
	Input.ShowTextBox("Title", "column_1_title");
	while (Input.NextPanel("column_1_items", displayName: "Items"))
	{
		new Href().ComponentInput(asset, context, "Link", "column_1_link");
		Input.ShowTextBox("Text", "column_1_text");
	}

	Input.NextTabbedPanel();
	Input.ShowTextBox("Title", "column_2_title");
	while (Input.NextPanel("column_2_items", displayName: "Items"))
	{
		new Href().ComponentInput(asset, context, "Link", "column_2_link");
		Input.ShowTextBox("Text", "column_2_text");
	}

	Input.NextTabbedPanel();
	Input.ShowTextBox("Title", "column_3_title");
	while (Input.NextPanel("column_3_items", displayName: "Items"))
	{
		new Href().ComponentInput(asset, context, "Link", "column_3_link");
		Input.ShowTextBox("Text", "column_3_text");
	}

	Input.EndTabbedPanel();
%>
