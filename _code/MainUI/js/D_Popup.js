/**
 * D_Popup Tooltip Handler
 * Originally by Scott Burton, Knowlysis
 * Modernized for jQuery 3.7.1
 */

$(function () {
    $("a.fasb-tooltip, a.xbrlReference")
        .on('mouseenter', function () { showPopup(this); })
        .on('mouseleave', function () { hidePopup(this); });
});


function showPopup(link) {
    const $popup = $(link).find("span.popupSpan");
    const popupWidth = 350;
    const headerOffset = 350;

    const windowWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    const linkLeft = getAbsoluteLeft(link);
    const linkTop = getAbsoluteTop(link) - headerOffset;

    const hasRightSpace = windowWidth - linkLeft;
    const popupLeft = hasRightSpace > (popupWidth + 75)
        ? linkLeft - 450
        : linkLeft - popupWidth + 15;

    $popup.css({
        left: `${popupLeft}px`,
        top: `${linkTop}px`,
        width: popupWidth
    }).show();
}

function hidePopup(link) {
    $(link).find("span.popupSpan").hide();
}

function getAbsoluteLeft(element) {
    let left = element.offsetLeft;
    while (element.offsetParent) {
        element = element.offsetParent;
        left += element.offsetLeft;
    }
    return left;
}

function getAbsoluteTop(element) {
    let top = element.offsetTop;
    while (element.offsetParent) {
        element = element.offsetParent;
        top += element.offsetTop;
    }
    return top;
}
