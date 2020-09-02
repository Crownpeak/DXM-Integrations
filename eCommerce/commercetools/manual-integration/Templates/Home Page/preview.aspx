<%@ Page Language="C#" Inherits="CrownPeak.Internal.Debug.PreviewInit" %>
<%@ Import Namespace="CrownPeak.CMSAPI" %>
<%@ Import Namespace="CrownPeak.CMSAPI.Services" %>
<!--DO NOT MODIFY CODE ABOVE THIS LINE-->
<%@ Import Namespace="Component_Library.Component_Project.Components" %>
<% //MODIFY or ADD Import Statements to Define Namespaces Used by the Template %>
<%//This plugin uses OutputContext as its context class type%>
<%
	var root = Asset.GetSiteRoot(asset).AssetPath + "/";
	var css = root + "css/";
	var img = root + "img/";
	var js = root + "js/";

	var fp = new FilterParams {Limit = 1};
	fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "Footer Links");
	var footer = asset.Parent.GetFilterList(fp).FirstOrDefault() ?? Asset.Load(-1);

	fp = new FilterParams {Limit = 1};
	fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "Header Links");
	var header = asset.Parent.GetFilterList(fp).FirstOrDefault() ?? Asset.Load(-1);

	fp = new FilterParams {Limit = 1};
	fp.Add(AssetPropertyNames.TemplateLabel, Comparison.Equals, "Header Categories Links");
	var headerCategories = asset.Parent.GetFilterList(fp).FirstOrDefault() ?? Asset.Load(-1);
%>
<!doctype html>
<html>
<head>
	<head>
		<meta charset="utf-8">
		<meta http-equiv="X-UA-Compatible" content="IE=edge">
		<meta name="viewport" content="width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no">
		<meta name="description" content="">
		<meta name="author" content="">
		<meta name="generator" content="commercetools">
		<!--[if IE]><link rel="icon" href="/favicon.ico"><![endif]-->
		<link href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,700" rel="stylesheet" type="text/css">
		<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">
		<link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/themes/smoothness/jquery-ui.min.css">
		<link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/jquery.slick/1.5.8/slick.css">
		<link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/jquery.slick/1.5.8/slick-theme.css">
		<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/3.2.0/animate.min.css">
		<link rel="stylesheet" href="https://cdn.jsdelivr.net/jquery.mcustomscrollbar/3.0.6/jquery.mCustomScrollbar.min.css">
		<link rel="stylesheet" href="<%= Asset.Load(css + "selectboxit.min.css").GetLink(LinkType.Include) %>">
		<base href="/">
		<title>SUNRISE</title>
		<link href="<%= Asset.Load(css + "app.776f3c24.css").GetLink(LinkType.Include) %>" rel="stylesheet">
		<link href="<%= Asset.Load(css + "app.776f3c24.css").GetLink(LinkType.Include) %>" rel="preload" as="style">
		<link href="<%= Asset.Load(js + "app.74bf44c3.js").GetLink(LinkType.Include) %>" rel="preload" as="script">
		<link href="<%= Asset.Load(js + "chunk-vendors.27b20d67.js").GetLink(LinkType.Include) %>" rel="preload" as="script">
		<link href="<%= Asset.Load(css + "app.776f3c24.css").GetLink(LinkType.Include) %>" rel="preload" as="stylesheet">
		<link rel="manifest" href="/manifest.json">
		<meta name="theme-color" content="#4DBA87">
		<meta name="apple-mobile-web-app-capable" content="no">
		<meta name="apple-mobile-web-app-status-bar-style" content="default">
		<meta name="apple-mobile-web-app-title" content="sunrise">
		<style type="text/css">
			.ps {
				-ms-touch-action: auto;
				touch-action: auto;
				overflow: hidden !important;
				-ms-overflow-style: none
			}

			@supports (-ms-overflow-style:none) {
				.ps {
					overflow: auto !important
				}
			}

			@media (-ms-high-contrast:none),screen and (-ms-high-contrast:active) {
				.ps {
					overflow: auto !important
				}
			}

			.ps.ps--active-x > .ps__scrollbar-x-rail, .ps.ps--active-y > .ps__scrollbar-y-rail {
				display: block;
				background-color: transparent
			}

			.ps.ps--in-scrolling.ps--x > .ps__scrollbar-x-rail {
				background-color: #eee;
				opacity: .9
			}

				.ps.ps--in-scrolling.ps--x > .ps__scrollbar-x-rail > .ps__scrollbar-x {
					background-color: #999;
					height: 11px
				}

			.ps.ps--in-scrolling.ps--y > .ps__scrollbar-y-rail {
				background-color: #eee;
				opacity: .9
			}

				.ps.ps--in-scrolling.ps--y > .ps__scrollbar-y-rail > .ps__scrollbar-y {
					background-color: #999;
					width: 11px
				}

			.ps > .ps__scrollbar-x-rail {
				display: none;
				position: absolute;
				opacity: 0;
				transition: background-color .2s linear,opacity .2s linear;
				bottom: 0;
				height: 15px
			}

				.ps > .ps__scrollbar-x-rail > .ps__scrollbar-x {
					position: absolute;
					background-color: #aaa;
					border-radius: 6px;
					transition: background-color .2s linear,height .2s linear,width .2s ease-in-out,border-radius .2s ease-in-out;
					bottom: 2px;
					height: 6px
				}

				.ps > .ps__scrollbar-x-rail:active > .ps__scrollbar-x, .ps > .ps__scrollbar-x-rail:hover > .ps__scrollbar-x {
					height: 11px
				}

			.ps > .ps__scrollbar-y-rail {
				display: none;
				position: absolute;
				opacity: 0;
				transition: background-color .2s linear,opacity .2s linear;
				right: 0;
				width: 15px
			}

				.ps > .ps__scrollbar-y-rail > .ps__scrollbar-y {
					position: absolute;
					background-color: #aaa;
					border-radius: 6px;
					transition: background-color .2s linear,height .2s linear,width .2s ease-in-out,border-radius .2s ease-in-out;
					right: 2px;
					width: 6px
				}

				.ps > .ps__scrollbar-y-rail:active > .ps__scrollbar-y, .ps > .ps__scrollbar-y-rail:hover > .ps__scrollbar-y {
					width: 11px
				}

			.ps:hover.ps--in-scrolling.ps--x > .ps__scrollbar-x-rail {
				background-color: #eee;
				opacity: .9
			}

				.ps:hover.ps--in-scrolling.ps--x > .ps__scrollbar-x-rail > .ps__scrollbar-x {
					background-color: #999;
					height: 11px
				}

			.ps:hover.ps--in-scrolling.ps--y > .ps__scrollbar-y-rail {
				background-color: #eee;
				opacity: .9
			}

				.ps:hover.ps--in-scrolling.ps--y > .ps__scrollbar-y-rail > .ps__scrollbar-y {
					background-color: #999;
					width: 11px
				}

			.ps:hover > .ps__scrollbar-x-rail, .ps:hover > .ps__scrollbar-y-rail {
				opacity: .6
			}

				.ps:hover > .ps__scrollbar-x-rail:hover {
					background-color: #eee;
					opacity: .9
				}

					.ps:hover > .ps__scrollbar-x-rail:hover > .ps__scrollbar-x {
						background-color: #999
					}

				.ps:hover > .ps__scrollbar-y-rail:hover {
					background-color: #eee;
					opacity: .9
				}

					.ps:hover > .ps__scrollbar-y-rail:hover > .ps__scrollbar-y {
						background-color: #999
					}

			.ps-container {
				position: relative
			}
		</style>
	</head>
</head>
<body>
	<div id="app">
		<header>
			<div class="navbar navbar-fixed-top">
				<div class="header">
					<div class="container">
						<div class="row">
							<div class="col-xs-12">
								<ul class="nav-list">
									<li class="list-item-nav-toggle">
										<button type="button" data-toggle="collapse" data-target=".navbar-collapse" class="navbar-toggle"><span class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span></button>
									</li>
									<li class="list-item-search">
										<button type="button" class="search-toggle icon-magnifying-glass"><span class="sr-only">Search</span></button></li>
									<li data-test="stores-link" class="list-item-store"><a href="/stores" class="store-select icon-marker"><span class="hidden-xs">Stores</span></a></li>
									<li class="list-item-help"><a href="#" class="link-help">Help</a></li>
									<%= header.Show() %>
									<li class="list-item-bag">
										<button data-test="mini-cart-open-button" class="not-empty link-your-bag icon-hand-bag"><span class="hidden-xs hidden-sm">Cart</span><span class="cart-item-number">0</span></button><!----></li>
									<li class="list-item-user">
										<ul class="nav-list">
											<!---->
											<li data-test="login-button" class="list-item-user"><a href="/login" class="link-user icon-user"><span class="hidden-xs hidden-sm">Sign In
											</span></a></li>
										</ul>
									</li>
									<li data-test="location-selector" class="list-item-location clearfix">
										<button data-test="location-selector-open-button">
											<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAABClJREFUeNrsW42VmzAMBhZoNiid4OgExwZlg9IN2KBswAh0A7qBrxOQTEA6AekEqWntPEUxYEs2l5c7v+d395LY1s8nWZblOArUzudzLv9M/Un2VPZsZciL7EfZD9P/cRzvo3tuksGd7KXs3dlPG2VvZS/ujdFUEWZqgxJAhxjJVR/A5/p348w89STUe2S0U5pOgeZ7wGwG5sgAg71mSH1egXFQWPVrQLc2EWLSABJKYfi+gMIyfJ8ZBDsoHxGc2QzBcFHiiJlm4XdQgKUDoppgMFcwtV5MIeECV4v5BRDi0rzY9nttPj6ZbbHTsRjTgd+nlj5hnIO2QZgNoikLwWxvAyGlBd1qh7UqMC53RB2facSssLUXAM+esGbvMhZ5ejrTSNot0dZzwrr5mgOzYDrlLNq7eELgUAQDWRohg+MOMjrTjLzr4MhsztHuzDwFEV0NxW4zR0I7rnYNWhaO4xproVO9K9hWnLViGbSkRMe3jFBoO64RDHByQ+SpAX9QMUziSnExOr+K6PHaSfZP8nw9/Y0S8MX36DHbhNRqyf5KAvS0dxwDxPADg67WaGbAs41EolrXAGWLudXefOVINaS1V/1JpEtvX78CQFLP6byvq7yYzo198badUPdtgpZ2hPHVFXphdMKNigImHjixOVRonqg0qk6TUr1gBKATomnaUgKsjyr9+88sEmB/e6b9njbYYqiZDc3bE2T4N5OYkBrmOsODRmPiEZJ/NtDwR6Yy8sQDEc8bRk1USF/MLYneWHtzDMch9893Dd9D4+agwDWJCEhjR8nCmKLBBEYhRHqOG51pOcFNCiGtCf7AZDgPyDA3GtQMvySc4xcmwvul1v85d0DDVDTpWOGYAIIzyvFL5YpOSBMhtHtSBwHOHIcEnZK4Wg7BcM6Bszqj7y6QVhq6zgrQg/sQYeYz8wDxVcP5Uhl0kxVguH2fN/Lq2oe7bQ43OTFu1lLNMfq6dQBzFr6yMTcC49zpouCg9chwa1MRYEHT4CYNu8lLmxoNImpKwth11FLuZWfsrfTAbMnMVhp5ibGWo8e7X/omvfMPI8NaMoHDxC3bXjL72Qb7I/UEZNwG6M5qYDgqe1+ECloaxwUr6kW2wRdUjLVrqpSdnBCqEaFcgNUUb4+cnIiIku6JTNcULSNzqh3GZdRCHC9MU7UMUDU6FMGVXgrTFpiuCVrOLbXkukYVqt4SM91Z1lxaF6m5FNQoejpUP+33aGpYZFw7KKDSo8rS4azNicuHxTnk0wBDNbxYki6qg05X7F2sODSB1q6jLZqyt97w1iF3DWTW6qqVRvFLmX6TJwAzUBxmXqBkM86lmoFyjQTaGOYefRxMQjGuCeyUAODWUag+okciYuEZT/mqz3gWHEo7QzT1oZZX6MYh7Ty6foq3RviUPZ3SsEGf4v0VYAAFoSuZ64HJQAAAAABJRU5ErkJggg==" alt="Location" class="pull-right"></button><!----></li>
								</ul>
							</div>
						</div>
					</div>
				</div>
				<div id="navigation" class="container">
					<div class="row logo-search">
						<div class="col-sm-8"><a href="/" class="brand-logo router-link-exact-active router-link-active">
							<img src="<%= Asset.Load(img + "logo.b7e3d87c.svg").GetLink() %>" alt="SUNRISE" class="img-responsive"></a></div>
						<div class="col-sm-4"></div>
					</div>
					<div class="row">
						<div class="dropdown-megamenu navbar navbar-default">
							<div class="navbar-collapse collapse">
								<ul class="nav navbar-nav">
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/new" class="dropdown-toggle" data-test="category-1st-level-link">New
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/women" class="dropdown-toggle" data-test="category-1st-level-link">Women
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/men" class="dropdown-toggle" data-test="category-1st-level-link">Men
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/accessories" class="dropdown-toggle" data-test="category-1st-level-link">Accessories
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/brands" class="dropdown-toggle" data-test="category-1st-level-link">Brands
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<li data-test="category-1st-level" class="dropdown menu-large"><a href="/products/sale" class="dropdown-toggle icon-ribbon sale" data-test="category-1st-level-link">Sale
        <img alt="More" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABAAgMAAADXB5lNAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAFzAAABcwHEdCJ9AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAAlQTFRF////AAAAAAAAflGpXQAAAAJ0Uk5TAPjF9bcWAAAAI0lEQVQ4y2NggADWAAZUwLVgVGD4C6xCA+QIjIbpyBOAFxcAjtpRr38pE40AAAAASUVORK5CYII=" class="mobile-plus-content visible-xs"></a><!----></li>
									<%= headerCategories.Show() %>
								</ul>
							</div>
						</div>
					</div>
				</div>
			</div>
		</header>

		<%= asset.Show() %>
		
		<footer>
			<div>
				<div class="footer">
					<div id="footer" class="container">
						<div class="row text-uppercase">
							<div>
								<%= footer.Show() %>
							</div>
							<div class="col-sm-6 text-center newsletter-box">
								<div class="row">
									<div class="col-sm-5 text-left footer-title-yellow">
										<p>Join our newsletter</p>
									</div>
									<div class="col-sm-7">
										<form action="#" method="POST" id="form-newsletter" class="email-submit">
											<input name="email" id="newsletter-input" type="email" placeholder="Your email..." required="required" class="email-box">
											<button id="newsletter-button" type="submit" class="search-button"><span class="icon-font icon-next"></span><span class="sr-only">Subscribe</span></button></form>
									</div>
								</div>
								<div class="row">
									<div class="col-sm-8 text-left card-info footer-title">
										<p>Pay securely with these payment methods</p>
										<img src="<%= Asset.Load(img + "cards.2dfae4e6.png").GetLink() %>" alt="payment types" class="img-responsive payment-cards"></div>
									<div class="col-sm-4 text-left card-info footer-title">
										<p>Follow us</p>
										<ul class="social-icons">
											<li><a href="#">
												<img src="<%= Asset.Load(img + "Facebook.783b7a48.png").GetLink() %>" alt="Facebook" class="social-icon"></a></li>
											<li><a href="#">
												<img src="<%= Asset.Load(img + "Pinterest.07e271de.png").GetLink() %>" alt="Pinterest" class="social-icon"></a></li>
											<li><a href="#">
												<img src="<%= Asset.Load(img + "Google.309a5e1d.png").GetLink() %>" alt="Google+" class="social-icon"></a></li>
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div class="sub-footer">
					<div class="container">
						<div class="row text-uppercase">
							<div class="col-sm-12">
								<ul class="footer-title imprint-row">
									<li class="hidden-xs">? 2019 Sunrise</li>
									<li><a href="#">Imprint</a></li>
									<li><a href="#">Privacy policy</a></li>
									<li><a href="#">Terms of use</a></li>
									<li class="visible-xs">? 2019 Sunrise</li>
								</ul>
							</div>
						</div>
					</div>
				</div>
			</div>
		</footer>
	</div>
</body>
</html>