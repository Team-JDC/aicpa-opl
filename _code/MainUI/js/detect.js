/*
* 
* Part of article How to detect screen size and apply a CSS style
* http://www.ilovecolors.com.ar/detect-screen-size-css-style/
*
*/

$(document).ready(function () {

    if ((screen.width >= 1280) && (screen.height >= 960)) {

        $("link[rel=stylesheet]:first").attr({ href: "Styles/detect1280.css" });
    }


    else {

        $("link[rel=stylesheet]:first").attr({ href: "Styles/detect1024.css" });
    }
});



