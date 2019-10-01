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
<div class="home-page">
	<div class="home-banner-option-one">
		<div class="container-fluid">
			<div class="row">
				<div class="col-sm-6 nopadding banner-left">
					<div class="aspect-ratio aspect-ratio--4to3">
						<img src="<%= asset["top_left_background"] %>" class="img-responsive">
					</div>
					<div class="banner-text">
						<h4 class="banner-title"><%= asset["top_left_title"] %></h4>
						<p class="banner-paragraph"><%= asset["top_left_paragraph"] %></p>
						<a href="<%= new Href().ComponentOutput(asset, context, "top_left_cta_link") %>" class="btn banner-btn"><%= asset["top_left_cta_text"] %></a>
					</div>
				</div>
				<div class="col-sm-6 nopadding banner-right">
					<div class="aspect-ratio aspect-ratio--4to3">
						<img src="<%= asset["top_right_background"] %>" class="img-responsive">
					</div>
					<div class="banner-text">
						<h4 class="banner-title"><%= asset["top_right_title"] %></h4>
						<p class="banner-paragraph"><%= asset["top_right_paragraph"] %></p>
						<a href="<%= new Href().ComponentOutput(asset, context, "top_right_cta_link") %>" class="btn banner-btn"><%= asset["top_right_cta_text"] %></a>
					</div>
				</div>
			</div>
		</div>
	</div>
	<div class="home-banner-option-two">
		<div class="container-fluid">
			<div class="row">
				<div class="col-sm-12 nopadding">
					<div class="aspect-ratio aspect-ratio--8to3">
						<img src="<%= asset["middle_background"] %>" class="img-responsive">
					</div>
					<div class="banner-text">
						<h4 class="banner-title">
							<img src="<%= asset["middle_top"] %>" class="hidden-xs"><br>
							<br>
							<%= asset["middle_title"] %>
						</h4>
						<p class="banner-subtitle"><%= asset["middle_subtitle"] %></p>
						<p class="hidden-xs hidden-sm"><%= asset["middle_paragraph"] %></p>
						<a href="<%= new Href().ComponentOutput(asset, context, "middle_cta_link") %>" class="btn banner-btn"><%= asset["middle_cta_text"] %></a>
					</div>
				</div>
			</div>
		</div>
	</div>
	<div class="home-banner-option-three">
		<div class="container-fluid">
			<div class="row">
				<div class="col-sm-6 nopadding banner-left">
					<div class="aspect-ratio aspect-ratio--4to3">
						<img src="<%= asset["bottom_left_background"] %>" class="img-responsive">
					</div>
					<div class="banner-text">
						<h4 class="banner-title"><%= asset["bottom_left_title"] %></h4>
						<p class="banner-paragraph"><%= asset["bottom_left_paragraph"] %></p>
						<a href="<%= new Href().ComponentOutput(asset, context, "bottom_left_cta_link") %>" class="btn banner-btn"><%= asset["bottom_left_cta_text"] %></a>
					</div>
				</div>
				<div class="col-sm-6 noppading banner-right">
					<div class="row">
						<div class="col-sm-6 nopadding">
							<div class="banner-xs-one">
								<img src="<%= asset["bottom_right_top_left_background"] %>" class="img-responsive">
								<div class="banner-text">
									<h4 class="banner-title">
										<img src="<%= asset["bottom_right_top_left_top"] %>"><br>
										<br>
										<%= asset["bottom_right_top_left_title"] %>
									</h4>
								</div>
							</div>
						</div>
						<div class="col-sm-6 nopadding">
							<div class="banner-xs-two">
								<div class="aspect-ratio aspect-ratio--4to3">
									<img src="<%= asset["bottom_right_top_right_background"] %>" class="img-responsive">
								</div>
								<div class="banner-text">
									<h4 class="banner-title"><%= asset["bottom_right_top_right_title"] %></h4>
									<p class="banner-paragraph"><%= asset["bottom_right_top_right_paragraph"] %></p>
									<a href="<%= new Href().ComponentOutput(asset, context, "bottom_right_top_right_cta_link") %>" class="btn banner-btn"><%= asset["bottom_right_top_right_cta_text"] %></a>
								</div>
							</div>
						</div>
					</div>
					<div class="row">
						<div class="col-sm-12 nopadding">
							<div class="banner-sm">
								<div class="aspect-ratio aspect-ratio--8to3">
									<img src="<%= asset["bottom_right_bottom_background"] %>" class="img-responsive">
								</div>
								<div class="banner-text">
									<h4 class="banner-title"><%= asset["bottom_right_bottom_title"] %></h4>
									<p class="banner-paragraph"><%= asset["bottom_right_bottom_paragraph"] %></p>
									<a href="<%= new Href().ComponentOutput(asset, context, "bottom_right_bottom_cta_link") %>" class="btn banner-btn"><%= asset["bottom_right_bottom_cta_text"] %></a>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
