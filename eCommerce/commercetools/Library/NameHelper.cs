using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CrownPeak.CMSAPI;
using CrownPeak.CMSAPI.Services;
/* Some Namespaces are not allowed. */
namespace Component_Library.Component_Project.Components
{
	public class NameHelper
	{
		// Paths in here will be untouched for publishing
		private static string[] _excludedPaths =
		{
			//"/Analyst Relations/Gartner/DXP MQ 2019/Surety/Corporate Site/_System",
			//"/Analyst Relations/Gartner/DXP MQ 2019/Surety/Customer Portal/_System"
		};

		public static string MakeFilename(OutputContext context, Asset asset)
		{
			context.PublishPath = FormatFilename(context.PublishPath, context, asset);
			return context.PublishPath;
		}

		public static string MakeAssetFilename(OutputContext context, Asset asset)
		{
			context.PublishPath = FormatFilename(context.PublishPath, context, asset, true);
			return context.PublishPath;
		}

		public static string MakeUrl(OutputContext context, Asset asset, bool removeFileExtension = true)
		{
			context.PublishUrl = FormatUrl(context.PublishUrl, context, asset, removeFileExtension);
			return context.PublishUrl;
		}

		public static string MakeAssetUrl(OutputContext context, Asset asset)
		{
			context.PublishUrl = FormatUrl(context.PublishUrl, context, asset, true);
			return context.PublishUrl;
		}

		public static string PrepareWrappedFilename(OutputContext context, Asset asset)
		{
			var pubname = asset.Label.ToLower();
			var ext = context.RemotePath.Extension;

			if (pubname.Contains("." + ext.ToLower()))
			{
				var tempExt = pubname.Substring(pubname.LastIndexOf("."));
				pubname = pubname.Replace(tempExt, "");
			}
			else if (!pubname.Contains("."))
			{
				context.PublishPath = context.PublishPath.Replace("." + ext, "");
			}

			pubname = ReplaceCharactersInWrappedFiles(pubname);
			context.PublishPath = context.PublishPath.Replace(context.RemotePath.Label, pubname);
			return context.PublishPath;
		}

		public static string PrepareWrappedUrl(OutputContext context, Asset asset)
		{
			var pubname = asset.Label.ToLower();
			var ext = context.RemotePath.Extension;

			if (pubname.Contains("." + ext.ToLower()))
			{
				var tempExt = pubname.Substring(pubname.LastIndexOf("."));
				pubname = pubname.Replace(tempExt, "");
			}
			else if (!pubname.Contains("."))
			{
				context.PublishUrl = context.PublishUrl.Replace("." + ext, "");
			}

			pubname = ReplaceCharactersInWrappedFiles(pubname);
			context.PublishUrl = context.PublishUrl.Replace(context.RemotePath.Label, pubname);
			return context.PublishUrl;
		}

		public static string ReplaceCharactersInWrappedFiles(string label)
		{
			return label.ToLower()
					.Replace(" ", "-")
					.Replace("%20", "-")
					.Replace("---", "-")
					.Replace("--", "-")
					.Replace("#47;", "")
					.Replace("%", "")
					.Replace("&", "")
					.Replace(":", "")
					.Replace("?", "")
					.Replace(@"/", "-")
					.Replace(".", "-");
		}

		/// <summary>
		/// Shared functions for filename formatting
		/// </summary>
		/// <param name="path"></param>
		/// <param name="context"></param>
		/// <param name="asset"></param>
		/// <returns></returns>
		public static string FormatFilename(string path, OutputContext context, Asset asset, bool isAttachment = false)
		{
			var assetPath = asset.AssetPath.ToString();
			path = path.Replace("/_System/", "/");
			if (_excludedPaths.Any(p => assetPath.StartsWith(p))) return path;

			string ext = context.RemotePath.Extension;
			string szPath1 = "", szPath2 = "";
			try { szPath1 = asset.AssetPath[1]; }
			catch { }
			try { szPath2 = asset.AssetPath[2]; }
			catch { }

			if (!isAttachment)
			{
				if (asset.AssetPath.Count > 0 && (szPath1.Equals("_Master", StringComparison.OrdinalIgnoreCase) || szPath2.Equals("_Master", StringComparison.OrdinalIgnoreCase)))
				{
					path = path.Replace(ext, "master");
				}
				if (context.LayoutName.Equals("rss_output.aspx") ||
								context.LayoutName.Equals("rss_output.asp") ||
								context.LayoutName.Equals("xml_output.aspx") ||
								context.LayoutName.Equals("xml_output.asp") ||
								context.LayoutName.Equals("output_rss.aspx") ||
								context.LayoutName.Equals("output_xml.aspx"))
				{
					path = path.Replace(ext, "xml");
				}

				if (context.LayoutName.Equals("output_exacttarget.aspx"))
				{
					path = path.Replace(context.RemotePath.Label, context.RemotePath.Label + "_et");
				}
				if (asset.TemplateLabel != null && asset.TemplateLabel.StartsWith("SPA "))
				{
					path = path.Replace(ext, "html");
				}

				if (string.Equals(asset.AssetPath[1], "RSS Feeds"))
					path = path.Replace(ext, "xml");
			}

			// Convert accented characters to latin alphabet (doing our best!)
			// See http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
			var encoding = Encoding.GetEncoding("ISO-8859-8");
			path = encoding.GetString(encoding.GetBytes(path));
			// Now replace anything that's not one of our allowed characters
			path = new Regex("[^A-Za-z0-9\\-_\\.\\/]+").Replace(path, "-");
			//if template is not a "static" file - js, css, png, jpg etc
			if (!string.IsNullOrWhiteSpace(asset.TemplateLabel) && !asset.TemplateLabel.Equals("DeveloperCS"))
			{
				// Replace all but the last . before extension
				path = new Regex("\\.(?=[^.]*\\.)").Replace(path, "-");
			}
			path = new Regex("[\\-]{2,}").Replace(path, "-");

			//path = path.Replace("/_assets/", "/");
			path = path.Replace("/_master/", "/master/");
			path = path.Replace("/_system/", "/");

			if (context.LayoutName.Equals("output_mobile.aspx"))
				path = path.Replace(context.RemotePath.Label, context.RemotePath.Label + "_mobile");

			// They can override the published filename if they need to
			var fn = asset["filename"];
			if (!string.IsNullOrWhiteSpace(fn))
			{
				// Strip any folders - we don't support that
				fn = fn.Split("/".ToCharArray()).Last();
				// Replace any characters that need it
				fn = ReplaceCharactersInWrappedFiles(fn);
				// Get the new filename and the extension if they provided it
				var fnext = "";
				if (fn.IndexOf(".") >= 0)
				{
					fnext = fn.Split(".".ToCharArray()).Last();
					fn = fn.Substring(0, fn.IndexOf("."));
				}
				// Find the existing filename and extension
				var existingfn = path.Split("/".ToCharArray()).Last();
				var existingext = existingfn.Split(".".ToCharArray()).Last();
				if (!string.IsNullOrWhiteSpace(existingext))
					existingfn = existingfn.Substring(0, existingfn.IndexOf("."));
				// Now replace existing with new
				if (string.IsNullOrWhiteSpace(fnext))
					path = path.Replace("/" + existingfn + "." + existingext, "/" + fn + "." + existingext);
				else
					path = path.Replace("/" + existingfn + "." + existingext, "/" + fn + "." + fnext);
			}

			if (path.IndexOf("/WEB-INF/") < 0 && path.IndexOf("/META-INF/") < 0)
				path = path.ToLower().Replace("#47;", "");

			// Folder and file names shouldn't begin with a hyphen
			path = new Regex("[/][-]+").Replace(path, "/");

			// Master pages (using tag library)
			if (asset.TemplateId > 0)
			{
				if (asset.TemplateLabel.Equals("Corporate Site Wrapper"))
					path = "/WEB-INF/tags/tag" + asset.BranchId + ".tag";
				else if (asset.TemplateLabel.Equals("Customer Portal Site Wrapper"))
					path = context.RemotePath.Folder.TrimEnd('/') + "/" + context.RemotePath.Label.ToLower() + ".master";
			}

			// Replace any paths that we want to strip/modify
			if (path.Contains("/_system/"))
			{
				path = path.Replace("/_system/", "/");
			}

			// Special things we don't want to append .aspx to
			path = new Regex("\\.htaccess\\.aspx$").Replace(path, ".htaccess");
			path = new Regex("\\.xml\\.aspx$").Replace(path, ".xml");
			path = new Regex("\\.config\\.aspx$").Replace(path, ".config");

			// Special things that must go in the root
			path = new Regex("^(.+)/WEB-INF/", RegexOptions.IgnoreCase).Replace(path, "/WEB-INF/");
			path = new Regex("^(.+)/META-INF/", RegexOptions.IgnoreCase).Replace(path, "/META-INF/");
			path = new Regex("^(.+)/bin/", RegexOptions.IgnoreCase).Replace(path, "/bin/");

			// Make sure classes don't get two extensions
			path = path.Replace(".class.aspx", ".class");

			return path;
		}

		/// <summary>
		/// Shared functions for url formatting
		/// </summary>
		/// <param name="path"></param>
		/// <param name="context"></param>
		/// <param name="asset"></param>
		/// <returns></returns>
		public static string FormatUrl(string path, OutputContext context, Asset asset, bool isAttachment = false, bool removeFileExtension = true)
		{
			var url = FormatFilename(path, context, asset, isAttachment);

			//// Extensionless URLs
			//if (url.EndsWith("/default.aspx") || url.EndsWith("/index.jsp"))
			//{
			//	url = new Regex("[/]default\\.aspx$").Replace(url, "/");
			//	url = new Regex("[/]index\\.jsp$").Replace(url, "/");
			//}

			if (asset.TemplateLabel != null && asset.TemplateLabel.StartsWith("SPA ") && url.Contains("/content/"))
			{
				url = url.Replace("/content/", "/#/");
				url = url.Replace("#/home", "#/");
			}

			if (removeFileExtension)
			{
				if (url.EndsWith(".aspx") || url.EndsWith(".jsp"))
				{
					url = new Regex("\\.aspx$").Replace(url, "");
					url = new Regex("\\.jsp$").Replace(url, "");
				}
			}

			return url;
		}

	}
}
