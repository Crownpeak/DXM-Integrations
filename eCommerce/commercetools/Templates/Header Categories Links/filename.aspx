<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.FilenameInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<%
	context.PublishPath = NameHelper.MakeFilename(context, asset);
%>
