<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.CopyInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses CopyContext as its context class type%>
<%
	// Clear the fields if someone copies the asset
	var fields = asset.GetContent();
	asset.DeleteContentFields(fields.Select(f => f.Key).ToList());
%>
