<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.OutputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="LocalProject" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<%
	context.IsGeneratingDependencies = false;

	if (context.IsPublishing && context.PublishingPackage.PackageName.IndexOf("live", StringComparison.OrdinalIgnoreCase) >= 0)
	{
		var fp = new FilterParams {FieldNames = new List<string> {"document_id"}, Limit = 99999};
		fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "TMF Translation Log");
		fp.Add("completed", Comparison.NotEquals, "true");
		fp.Add("failed", Comparison.NullOrNotInSet, new List<string>{"true"});
		var files = asset.Parent.GetFilterList(fp);
		foreach (var file in files)
		{
			Out.WriteLine("Processing: " + file.AssetPath + "<br/>\n");
			TMF.Translation.GetTmfTranslator(file).UpdateLog(file);
		}
		Out.WriteLine("Done<br/>\n");
	}
%>
