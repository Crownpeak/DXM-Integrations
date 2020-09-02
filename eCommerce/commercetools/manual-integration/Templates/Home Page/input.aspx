<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.InputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses InputContext as its context class type%>
<%
	// input.aspx: template file to specify content entry input form
	var sap = new ShowAcquireParams { DefaultFolder = Asset.GetSiteRoot(asset).AssetPath + "/img" };

	Input.StartControlPanel("Top-left");
	Input.ShowAcquireImage("Background Image", "top_left_background", sap);
	Input.ShowTextBox("Title", "top_left_title");
	Input.ShowTextBox("Paragraph", "top_left_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "top_left_cta_link");
	Input.ShowTextBox("CTA Text", "top_left_cta_text");
	Input.EndControlPanel();

	Input.StartControlPanel("Top-right");
	Input.ShowAcquireImage("Background Image", "top_right_background", sap);
	Input.ShowTextBox("Title", "top_right_title");
	Input.ShowTextBox("Paragraph", "top_right_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "top_right_cta_link");
	Input.ShowTextBox("CTA Text", "top_right_cta_text");
	Input.EndControlPanel();

	Input.StartControlPanel("Middle");
	Input.ShowAcquireImage("Background Image", "middle_background", sap);
	Input.ShowAcquireImage("Top Image", "middle_top", sap);
	Input.ShowTextBox("Title", "middle_title");
	Input.ShowTextBox("Subtitle", "middle_subtitle");
	Input.ShowTextBox("Paragraph", "middle_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "middle_cta_link");
	Input.ShowTextBox("CTA Text", "middle_cta_text");
	Input.EndControlPanel();

	Input.StartControlPanel("Bottom-left");
	Input.ShowAcquireImage("Background Image", "bottom_left_background", sap);
	Input.ShowTextBox("Title", "bottom_left_title");
	Input.ShowTextBox("Paragraph", "bottom_left_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "bottom_left_cta_link");
	Input.ShowTextBox("CTA Text", "bottom_left_cta_text");
	Input.EndControlPanel();

	Input.StartControlPanel("Bottom-right");

	Input.StartControlPanel("Top-left");
	Input.ShowAcquireImage("Background Image", "bottom_right_top_left_background", sap);
	Input.ShowAcquireImage("Top Image", "bottom_right_top_left_top", sap);
	Input.ShowTextBox("Title", "bottom_right_top_left_title");
	Input.EndControlPanel();

	Input.StartControlPanel("Top-right");
	Input.ShowAcquireImage("Background Image", "bottom_right_top_right_background", sap);
	Input.ShowTextBox("Title", "bottom_right_top_right_title");
	Input.ShowTextBox("Paragraph", "bottom_right_top_right_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "bottom_right_top_right_cta_link");
	Input.ShowTextBox("CTA Text", "bottom_right_top_right_cta_text");
	Input.EndControlPanel();

	Input.StartControlPanel("Bottom");
	Input.ShowAcquireImage("Background Image", "bottom_right_bottom_background", sap);
	Input.ShowTextBox("Title", "bottom_right_bottom_title");
	Input.ShowTextBox("Paragraph", "bottom_right_bottom_paragraph");
	new Href().ComponentInput(asset, context, "CTA Link", "bottom_right_bottom_cta_link");
	Input.ShowTextBox("CTA Text", "bottom_right_bottom_cta_text");
	Input.EndControlPanel();

	Input.EndControlPanel();

%>
