(function(window, $, undefined) {

	var b = document.getElementById("bynder-compactview");
	if (!b) {
		b = document.createElement("div");
		b.id = "bynder-compactview";
		b.setAttribute("data-assetTypes", "image");
		b.setAttribute("data-autoload", "false");
		b.setAttribute("data-button", "Open Compact View");
		b.setAttribute("data-collections", "true");
		b.setAttribute("data-defaultEnvironment", "");
		b.setAttribute("data-fullScreen", "false");
		b.setAttribute("data-header", "true");
		b.setAttribute("data-language", "en_US");
		b.setAttribute("data-mode", "single");
		b.setAttribute("data-zindex", "300");
		b.setAttribute("data-shadowDom", "false");
		b.style.height = "1px";
		b.style.width = "1px";
		document.body.appendChild(b);

		var s = document.createElement("script");
		s.type = "text/javascript";
		s.src = "https://d8ejoa1fys2rk.cloudfront.net/modules/compactview/includes/js/client-1.5.0.min.js";
		document.body.appendChild(s);
	}

	var bynderImage = null;

	findBynderImages();
	window.setInterval(findBynderImages, 1000);

	function findBynderImages() {
		var img = cp$('img[data-cp-integration="bynder"]');
		if (img.length > 0) {
			img.attr("data-cp-integration", "bynder processed");
			img.css("cursor", cpInlineEnabled ? "pointer" : "default");

			img.each(function (index) {
				var spans = $(this).closest("ng-component").find("span");
				var url = spans.eq(1).text();
				if (url && url.indexOf("http") === 0) {
					$(this).attr("src", url).attr("alt", spans.eq(3).text()).css("visibility", "visible");
				} else if (cpInlineEnabled) {
					$(this).attr("src", "../V3/images/src-placeholder.png").css("visibility", "visible");
				}
			});

			img.click(function (event) {
				if (cpInlineEnabled) {
					bynderImage = event.srcElement;
					event.preventDefault();
					event.cancelBubble = true;
					event.stopPropagation();
					$("#bynder-compactview > button").click();
				}
				return false;
			});
		}
	}

	document.addEventListener('BynderAddMedia', function (e) {
		var selectedAssets = e.detail;

		var asset;
		for (var i = 0; i < selectedAssets.length; i++) {
			asset = selectedAssets[i];
			switch (asset.type) {
				case 'image':

					var component = $(bynderImage).closest("ng-component");
					var debugs = component.find("span.cpDebugText");
					var spans = component.find("span[contenteditable]");
					spans.eq(0).text(asset.thumbnails['webimage']).blur();
					var field = debugs.eq(0).text().split(" ")[0];
					cpNotify('dropImage|' + cpCmsId + '|' + field + '|' + asset.thumbnails['webimage']);
					bynderImage.src = asset.thumbnails['webimage'];

					spans.eq(1).text(asset.name);
					field = debugs.eq(1).text().split(" ")[0];
					cpNotify('dropImage|' + cpCmsId + '|' + field + '|' + asset.name);
					bynderImage.alt = asset.name;
					break;
			}
		}
	});

}(window, cp$));