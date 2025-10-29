/*
* D_Popup
* Scott Burton, Knowlysis
* Based on some methods from JTip (By Cody Lindley (http://www.codylindley.com))
*
* Requires JQuery
*/

//on page load (as soon as its ready) call JT_init
$(function () {  DPopup_init() });

function DPopup_init() {
    
    $("a.tooltip").hover(function () { DPopup_show(this) }, function () { DPopup_hide(this) });
    $("a.xbrlReference").hover(function () { DPopup_show(this) }, function () { DPopup_hide(this) });
}

function DPopup_show(link) {
    console.log("show popup");
    var de = document.documentElement;
    var w = self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
    //var windowheight = $(window).innerHeight();
    var windowheight = $(window)[0].innerHeight; // I use this ghetto notation because the normal notation wasn't working in IE.
    var h = $(document).height();
    if (windowheight != undefined) {
        h = windowheight + $(window)[0].pageYOffset;

    }


    var hasArea = w - getAbsoluteLeft(link);
    var hasHeight = h - getAbsoluteTop(link);
    var clickElementy = getAbsoluteTop(link); //set y position 


    var popupWidth = 370;
    var popupHeight = 200;

    if (hasArea > (popupWidth + 75)) {
        var clickElementx = getAbsoluteLeft(link); //set x position
    }
    else {
        var clickElementx = getAbsoluteLeft(link) - popupWidth + 15; //set x position
    }
    if (hasHeight < popupHeight + 25) {
        clickElementy = h - popupHeight - 25;
    }

    $(link).find("span.popupSpan").css({ left: clickElementx + "px", top: clickElementy + "px", width: popupWidth, height: popupHeight + "px", overflow: "auto" });
    $(link).find("span.glossaryheader").css({ width: popupWidth - 30 });
    $(link).find("span.glossarybody").css({ width: popupWidth - 30 });
    $(link).find("span.popupSpan").show();
}

function DPopup_hide(link) {
    $(link).find("span.popupSpan").hide();
}


function getElementWidth(o) {
    return o.offsetWidth;
}

function getAbsoluteLeft(o) {
    // Get an object left position from the upper left viewport corner
    oLeft = o.offsetLeft            // Get left position from the parent object
    while (o.offsetParent != null) {   // Parse the parent hierarchy up to the document element
        oParent = o.offsetParent    // Get parent object reference
        oLeft += oParent.offsetLeft // Add parent left position
        o = oParent
    }
    return oLeft
}

function getAbsoluteTop(o) {
    // Get an object top position from the upper left viewport corner
    oTop = o.offsetTop            // Get top position from the parent object
    while (o.offsetParent != null) { // Parse the parent hierarchy up to the document element
        oParent = o.offsetParent  // Get parent object reference
        oTop += oParent.offsetTop // Add parent top position
        o = oParent
    }
    return oTop
}

