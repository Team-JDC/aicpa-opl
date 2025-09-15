/*
* 
* Part of article How to detect screen size and apply a CSS style
* http://www.ilovecolors.com.ar/detect-screen-size-css-style/
*
*/

$(function () {
    const isLargeScreen = screen.width >= 1280 && screen.height >= 960;
    const stylesheet = isLargeScreen ? "Styles/detect1280.css" : "Styles/detect1024.css";

    $("link[rel='stylesheet']").first().attr("href", stylesheet);
});




