(function (window, $, undefined) {

        var bynderOptions = {
		language: "en_US",
		assetTypes: ["image"],
		theme: {
			colorButtonPrimary: "#3380FF"
		},
		mode: "SingleSelectFile",
		onSuccess: function (assets, additionalInfo) {
			var url = (additionalInfo && additionalInfo.selectedFile)
				? additionalInfo.selectedFile.url
				: "";
			for (var i = 0; i < assets.length; i++) {
				var asset = assets[i];
				switch (asset.type) {
					case 'IMAGE':
						if (!url) url = asset.files['webimage'];
						var component = $(bynderImage).closest(".cp-dnd-component-wrapper");
						var spans = component.find("cpinline");
						spans.eq(0).text(url).blur();
						var field = spans.eq(0).attr("data-cp-field-name");
						top.onCpNotify('dropImage|' + cpCmsId + '|' + field + '|' + url);
						bynderImage.src = url;

						var alt = asset.name;
						spans.eq(1).text(alt);
						field = spans.eq(1).attr("data-cp-field-name");
						top.onCpNotify('dropImage|' + cpCmsId + '|' + field + '|' + alt);
						bynderImage.alt = alt;
						break;
				}
			}
		}
	};

	if (!window.BynderContactView) {
		var s = document.createElement("script");
		s.type = "text/javascript";
		s.src = "https://d8ejoa1fys2rk.cloudfront.net/5.0.5/modules/compactview/bynder-compactview-2-latest.js";
		document.body.appendChild(s);
	}

	function getInlineEnabled() {
		var inlineButton = window && window.top && window.top.document && window.top.document.getElementById("contentModuleInline");
		window.cpInlineEnabled = inlineButton && inlineButton.className.indexOf("view-active") >= 0;
	}
	getInlineEnabled();
	window.setInterval(getInlineEnabled, 1000);

	var bynderImage = null;
	findBynderImages();
	window.setInterval(findBynderImages, 1000);

	$("body").on("click", "img[data-cp-integration='bynder processed']", function (event) {
		if (cpInlineEnabled) {
			bynderImage = event.target || event.srcElement;
			event.preventDefault();
			event.cancelBubble = true;
			event.stopPropagation();
			BynderCompactView.open(bynderOptions);
		}
		return false;
	});

	setInterval(function() {
		if (!cpInlineEnabled) return;
		var images = $("img[data-cp-integration='bynder processed']");
		images.each(function(index) {
			var $this = $(this);
			var src = $this.attr("src");
			var alt = $this.attr("alt");
			var spans = $(this).closest(".cp-dnd-component-wrapper, .cp-dnd-saved-component-wrapper").find("cpinline");
			var url = spans.eq(0).text();
			if (url && src != url && url.indexOf("http") === 0) {
				$this.attr("src", url).attr("alt", spans.eq(1).text()).css("visibility", "visible");
			} else if (src != url && src !== "../../../../../v3/assets/images/UI-DragAndDrop/src-placeholder.png") {
				$this.attr("src", "../../../../../v3/assets/images/UI-DragAndDrop/src-placeholder.png").css("visibility", "visible");
			}
		});
	}, 500);

	function findBynderImages() {
		var img = cp$('img[data-cp-integration="bynder"]');
		if (img.length > 0) {
			img.attr("data-cp-integration", "bynder processed");
			img.css("cursor", cpInlineEnabled ? "pointer" : "default");

			img.each(function (index) {
				var spans = $(this).closest(".cp-dnd-component-wrapper, .cp-dnd-saved-component-wrapper").find("cpinline");
				var url = spans.eq(0).text();
				if (url && url.indexOf("http") === 0) {
					$(this).attr("src", url).attr("alt", spans.eq(1).text()).css("visibility", "visible");
				} else if (cpInlineEnabled) {
					$(this).attr("src", "../../../../../v3/assets/images/UI-DragAndDrop/src-placeholder.png").css("visibility", "visible");
				}
			});
		}
	}

}(window, cp$));