<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PostInputInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%
    if (!context.InputForm.HasField("page_title"))
        context.ValidationError = "Please enter a page title.";
    else if (!asset.Label.Equals(context.InputForm["page_title"]))
        asset.Rename(context.InputForm["page_title"]);

    if (string.IsNullOrEmpty(context.InputForm["folder_root_path"]))
        context.ValidationErrorFields.Add("folder_root_path", "Please select a root folder.");

    context.InputForm["folder_root"] = Asset.Load(context.InputForm["folder_root_path"]).AssetPath.ToString();
%>