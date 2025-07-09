//function scrollToCurrentHitLocation(hitlocation) {
//    var anchorElement = null;

//    var found = $("span[id='"+hitlocation+"']:first");

//    if (found.length > 0) {
//        anchorElement = found[0];
//    }
//    if (anchorElement) {
//        anchorElement.scrollIntoView(true);
//        $('#' + hitlocation).addClass("highlight2");
//    }
//}

function scrollToCurrentHitLocation(prop) {
    //scroll to the top if the selector doesn't exists...
    if ($("#" + prop).length > 0) {

        $('html,body').animate({ scrollTop: $("#" + prop).offset().top - 50 }, 'slow');

        $('#' + prop).addClass("highlight2");
    }
}

function isNextVisible() {
    return currentHit < (hitcount - 1);
}

function isPrevVisible() {
    return currentHit > 0;
}

function goToNext() {
    $("#hitlocation" + currentHit).removeClass("highlight2");
    if (isNextVisible()) {
        currentHit = currentHit + 1;
    }
    scrollToCurrentHitLocation("hitlocation" + currentHit);
}

function goToPrevious() {
    $("#hitlocation" + currentHit).removeClass("highlight2");
    if (isPrevVisible()) {
        currentHit = currentHit - 1;
    }
    scrollToCurrentHitLocation("hitlocation" + currentHit);
}


var hitcount = 0;
var currentHit = 0;

$(document).ready(function () {

    var showButton = ($('#hhighlight').length != 0);

    if (showButton) {
        parent.showInnerSearchButtons();

        var words = $('#hhighlight').attr('value');
        var anchor = $('#hitanchor').attr('value');

        var wordsArray = new Array();
        var wordsArrayIndex = 0;
        $.each(words.split(" "), function (idx, val) {
            val = val.replace(/\+/g, "\\s+");
            val = $.trim(val);
            val = val.toLowerCase();
            if (val != "" && $.inArray(val, wordsArray) == -1) {
                wordsArray[wordsArrayIndex] = val;
                wordsArrayIndex = wordsArrayIndex + 1;
            }
        });


        $('body').removeHighlight(); // remove old highlights

        $.each(wordsArray, function (idx, val) {
            $('#leftcol').highlight(val);
        });

        //highlight
        var count = 0;
        $(".highlight").each(function (i, obj) {
            $(obj).attr("id", "hitlocation" + i);
            hitcount = hitcount + 1;
        });

        if (hitcount > 0) {
            //     $("<div id='header'><div id='prev'><a href='#'>Prev</a></div><div id='next'><a href='#first'>Next</a></div></div>").prependTo("body");
        }

        //Method for Next, Prev
        var anchorLoc = -1;
        if (anchor != "") {
            var anchortxt = "name=\"" + anchor + "\"";
            var anchorIndex = $('body').html().indexOf(anchortxt);
            // see http://api.jquery.com/html/  "This method uses the browser's innerHTML property. 
            //      Some browsers may not return HTML that exactly replicates the HTML source in an original document. 
            //      For example, Internet Explorer sometimes leaves off the quotes around attribute values if they contain only alphanumeric characters"
            if (anchorIndex == -1) {
                var tempAnchor = anchortxt.replace(/\"/gi, ""); //anchortxt.replace("\"", "");
                anchorIndex = $('body').html().indexOf(tempAnchor);
            }

            var index = 0;
            var pos = 0;
            while ((index < hitcount) && (pos < anchorIndex)) {
                pos = $('body').html().indexOf("hitlocation" + index);
                if (pos < anchorIndex) index++;
            }
            if (index < hitcount)
                anchorLoc = index;
        }

        if (anchorLoc > -1) {
            currentHit = anchorLoc;
        }


        scrollToCurrentHitLocation("hitlocation" + currentHit);

    } else {
        parent.hideInnerSearchButtons();
    }

});

$(document).click(function (obj) {
    var elementclicked = obj.srcElement;
//    if ((elementclicked.pathname != null && elementclicked.pathname.indexOf("GetDocument.ashx") != -1) ||
//        (elementclicked.parentElement != null && elementclicked.parentElement.pathname != null && elementclicked.parentElement.pathname.indexOf("GetDocument.ashx") != -1)) {
//        parent.hideInnerSearchButtons();
//    }
});

