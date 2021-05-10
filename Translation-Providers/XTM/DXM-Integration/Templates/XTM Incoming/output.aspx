<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.OutputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<% 
	// output.aspx: template file to specify the published content in site HTML
	// if no preview.aspx exists, then this is used by default for preview
	var error = asset.Raw["error"];
	if (!string.IsNullOrWhiteSpace(error))
	{
		Out.WriteLine("<p>Error: {0}</p>", error);
	}
	else
	{
		Out.WriteLine("No error so this asset should have been deleted. Or something didn't work!");
	}
%>