// Make navigation drop down function in IE 6

sfHover = function() {
	var sfEls = document.getElementById("navigation").getElementsByTagName("LI");
	for (var i=0; i<sfEls.length; i++) {
		sfEls[i].onmouseover=function() {
			this.className+=" navigation-hover";
		}
		sfEls[i].onmouseout=function() {
			this.className=this.className.replace(new RegExp(" navigation-hover\\b"), "");
		}
	}
}
if (window.attachEvent) window.attachEvent("onload", sfHover);