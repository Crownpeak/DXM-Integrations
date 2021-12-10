<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PreviewInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<%@ Import Namespace="LocalProject" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%//This plugin uses OutputContext as its context class type%>
<%
	context.IsGeneratingDependencies = false;
	
	var previewAsset = Asset.Load(asset["preview_asset"]);
	if (!previewAsset.IsLoaded)
	{
		var templateAsset = Asset.Load(asset["template"]);
		if (!templateAsset.IsLoaded)
		{
			Out.WriteLine("<p>No preview asset and no template were chosen.</p>");
		}
		else
		{
			var fp = new FilterParams
			{
				Limit = 1
			};
			fp.Add(AssetPropertyNames.TemplateId, Comparison.Equals, templateAsset.Id);
			previewAsset = Asset.GetSiteRoot(asset).GetFilterList(fp).FirstOrDefault() ?? Asset.Load(-1);
		}
	}
	if (!previewAsset.IsLoaded)
	{
		Out.WriteLine("<p>No preview asset was chosen or could be found.</p>");
	}
	else
	{
		Out.WriteLine("<h3>Preview for {0}</h3>", previewAsset.AssetPath);
		Out.WriteLine("<p>Showing fields that will be sent for translation for {0}.<p/>", previewAsset.AssetPath);
		var fields = TMF.TemplateTranslation.GetFieldsForTranslation(previewAsset);
		if (!fields.Any())
		{
			Out.WriteLine("<p>No fields would be sent for translation.</p>");
		}
		Out.WriteLine("<table border=1 cellspacing=0 cellpadding=2><tr><th>Name</th><th>Value</th></tr>");
		foreach (var field in fields.OrderBy(f => f.Key))
		{
			var value = Util.HtmlEncode(field.Value);
			if (value.Length > 100) value = "<details><summary>" + value.Substring(0, 100) + "... (" + value.Length + " characters)</summary>" + value + "</details>";
			Out.WriteLine("<tr><td>" + Util.HtmlEncode(field.Key) + "</td><td>" + value + "</td></tr>");
		}
		Out.WriteLine("</table>");
    
		var fieldsSkipped = TMF.TemplateTranslation.GetFieldsSkippedForTranslation(previewAsset);
		Out.WriteLine("<br/><p>Showing fields that will <strong>NOT</strong> be sent for translation for {0}.<p/>", previewAsset.AssetPath);
		Out.WriteLine("<table border=1 cellspacing=0 cellpadding=2><tr><th>Name</th><th>Value</th></tr>");
		foreach (var skippedField in fieldsSkipped.OrderBy(f => f.Key))
		{
			var value = Util.HtmlEncode(skippedField.Value);
			if (value.Length > 100) value = "<details><summary>" + value.Substring(0, 100) + "... (" + value.Length + " characters)</summary>" + value + "</details>";
			Out.WriteLine("<tr><td>" + Util.HtmlEncode(skippedField.Key) + "</td><td>" + value + "</td></tr>");
		}
		Out.WriteLine("</table>");
	}
%>