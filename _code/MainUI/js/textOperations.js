/* 
    TextOperations.js is where i've placed all the code for the copy/paste functionality for aicpa 
    
*/
//http://stackoverflow.com/questions/6251937/how-to-get-selecteduser-highlighted-text-in-contenteditable-element-and-replac
//http://jsfiddle.net/dKaJ3/2/
(function($) {
    $.strRemove = function(theTarget, theString) {
        return $("<div/>").append(
            $(theTarget, theString).remove().end()
        ).html();
    };
})(jQuery);

//not used
(function ($) {
    $.extend($.fn, {
        makeCssInline: function () {
            this.each(function (idx, el) {
                var style = el.style;
                var properties = [];
                for (var property in style) {
                    if ($(this).css(property)) {
                        properties.push(property + ':' + $(this).css(property));
                    }
                }
                this.style.cssText = properties.join(';');
                $(this).children().makeCssInline();
            });
        }
    });
} (jQuery));

var pageLinks = new Array();
var pageJS = new Array();

function getReturnUrl() {
    return d_returnurl;
}

function getBaseUrl() {
    pathArray = window.location.href.split('/');
    protocol = pathArray[0];
    host = pathArray[2];
    url = protocol + '//' + host + '/';
    return url;
}

function pad(num, size) {
    var s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
}

function getSelectionHtml() {
    var html = "";
    if (typeof window.getSelection != "undefined") {
        var sel = window.getSelection();
        if (sel.rangeCount) {
            var container = document.createElement("div");
            for (var i = 0, len = sel.rangeCount; i < len; ++i) {
                container.appendChild(sel.getRangeAt(i).cloneContents());
            }
            html = container.innerHTML;
        }
    } else if (typeof document.selection != "undefined") {
        if (document.selection.type == "Text") {
            html = document.selection.createRange().htmlText;
        }
    }
    return html;
}

function GetHilightedContent() {
    var asHtml = sanitizeHtml(getSelectionHtml());
    asHtml = asHtml.replace(/[\r\n]+/g, ' ');
    return asHtml;
}

function parseSectionInner(obj) {
    var paraNumber = $(obj).find(".titlepage .ps_para_number").html()
    
    var paraHTML = $(obj).children("p.ps_para_number").html()
    paraHTML = paraNumber + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + paraHTML;
    
    return paraHTML;    
}

//Will remove the BookMark/Netes/Etc Code
function sanitizeHtml(htmlStr) {
    var result = $('<div>').append(htmlStr); // make valid string
    //Remove ToolTip links
    $(result).find("a.tooltip").each(function (i, obj) {
        $(this).find("span.popupSpan").each(function (i, obj) {
            $(this).remove();
        });
        //Remove <A...>
        this.outerHTML = this.innerHTML;
    });

    // Remove NotesButtons
    $(result).find("span.notesButtons").each(function (i, obj) {
        $(this).remove();
    });
        
    // Fix Conent Link
    $(result).find("span.contentLink a").each(function (i, obj) {
        //var onc = this.getAttribute("onclick");
        var onc = this.getAttributeNode("onclick").value;
        if (onc) {
            var params = null;
            var targetDoc = null;
            var targetPtr = null;
            
            try {
                params = onc.match(/dolink\('([^\']+)',\s*?'([^\']+)',\s*?false/i);
                if (params != null) {
                    targetDoc = params[1];
                    targetPtr = params[2];
                }
            } catch (err) {

            }
            var img = $(obj).parent().find("img[alt*='locked']:first");
            var classname = img.attr("class");
            var hidden = false; //don't show just in case
            //builds the class that is defined for the locked image and checks that
            //this is the only way I could figure out how to get :hidden to work
            if ((classname) && (classname.length > 0)) {
                classname = ".contentLink img." + classname;
                hidden = $(classname).is(":hidden");
            }
            var newLink = "";
            if ((targetDoc) && (targetPtr) && (hidden)) {
                newLink = "tdoc=" + targetDoc + "&tptr=" + targetPtr;
                newLink = '<a href="' + getReturnUrl() + '&' + newLink + '">' + $(this).text() + '</a>';
                $(this).parent().before(newLink);
                //$(this).empty();
                //$(this).parent().html("");
                $(this).parent().remove();
            } else if ((targetDoc) && (targetPtr) && (!hidden)) {
                // if the content is locked then just show the text
                $(this).parent().before($(this).text());
                $(this).parent().remove();
                //$(this).parent().html(""); // clear html           
            }
        }
    });
    $(result).find("span.contentLink").remove();
    $(result).find("span.noindex").remove();
    $(result).find("a[onclick*='doFootnoteLink']").each(function (i, obj) {
        //var onc = this.getAttribute("onclick");
        var onc = this.getAttributeNode("onclick").value;
        if (onc) {
            var params = onc.match(/doFootnoteLink\('([^\']+)', ?'([^\']+)'/i);
            var href = this.getAttribute("href");
            var newLink = "";
            if ((params) && (params.length >= 3)) {
                newLink = "tdoc=" + params[1] + "&tptr=" + params[2];
                newLink = getReturnUrl()+'&' + newLink;
                $(this).removeAttr("onclick");
                $(this).attr("href", newLink);
            } else {
                $(this).remove();
            }
        }    else {
            $(this).parent().html(this.innerHTML);
        }

    });
    $(result).find("a[href*='Handlers/GetResource.ashx?r_bn']").each(function (i, obj) {
        //var onc = this.getAttribute("href");
        var onc = this.getAttributeNode("href").value;
        if (onc) {
            var params = onc.match(/.*r_bn=([^&]+).*?r_rn=([^&]+)/i);
            var href = this.getAttribute("href");
            var newLink = "";
            if ((params) && (params.length >= 3)) {
                newLink = "r_bn=" + params[1] + "&r_rn=" + params[2];
                newLink = getReturnUrl() + '&' + newLink;
                $(this).attr("href", newLink);
            } else {
                $(this).remove();
            }
        } else {
            $(this).parent().html(this.innerHTML);
        }

    });
    $(result).find("img[class*='joinSections']").each(function (i, obj) {
        $(this).remove();
    });

    if ($(result).find(".section").length == 0) {
        if (($(result).find(".ps_para_number").length > 0) && ($(result).find(".titlepage .ps_para_number").length > 0)) {
            var paraHTML = parseSectionInner(result);
            $(result).find(".titlepage").remove();
            $(result).children("p.ps_para_number").html(paraHTML);
        }
    }


    $(result).find(".section").each(function (i, obj) {
        //.titlepage .ps_para_number

        if (($(this).children(".ps_para_number").length > 0) && ($(this).find(".titlepage .ps_para_number").length > 0)) {

            var paraHTML = parseSectionInner(this);
            $(obj).children(".titlepage").remove();

            $(this).children("p.ps_para_number").html(paraHTML);

            /*
            var paraNumber = $(this).find(".titlepage .ps_para_number").html()
            //alert('found' + paraNumber);
            $(this).children(".titlepage").remove();
            var paraHTML = $(this).children("p.ps_para_number").html()
            paraHTML = paraNumber + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + paraHTML;
            //alert("html" + paraHTML);
            $(this).children("p.ps_para_number").html(paraHTML);
            */
        }
        
                    /* save code

            var paraNumber = $(this).find(".titlepage .ps_para_number").html()
            //alert('found' + paraNumber);
            $(this).find(".titlepage .ps_para_number").remove();
            var paraHTML = $(this).find("p.ps_para_number").html()
            paraHTML = paraNumber + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + paraHTML;
            //alert("html"+paraHTML);
            $(this).find("p.ps_para_number").html(paraHTML);
            */
    });

//    $(result).find("span:hidden").each(function () {
//        $(this).remove();
//    });

    return $(result).html();
}



var now;
var scrollposx;
var scrollposy;
var scrolltotop;

function selectElementContents(el) {
    var body = document.body, range, sel;
    if (body.createTextRange) {
        range = body.createTextRange();
        range.moveToElementText(el);
        range.select();
    } else if (document.createRange && window.getSelection) {
        try {
            range = document.createRange();
            range.selectNodeContents(el.firstChild);
        } catch (NotFoundError) {
        }
        sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
    }
    var t = setTimeout("fix()", 2);
}

function fix() {
    jQuery('#ideal').html('');
//    var sel = window.getSelection();
//    sel.addRange(now);
    window.scrollTo(0, scrolltotop);
}

$(document).ready(function () {
    doTextOperationsReady();
});

function doTextOperationsReady() {
    function GetScrollTop() {
        var doc = document.documentElement
        var body = document.body;
        return ((doc && doc.scrollTop) || (body && body.scrollTop || 0)) - (doc.clientTop || 0);
    }

    $('body').bind('copy cut', function (e) {
        //now = window.getSelection().getRangeAt(0);
        var d = document.createDocumentFragment();
        var html = GetHilightedContent();
        var temp;

        //        if (document.body.createTextRange)
        //            temp = document.createElement('div');
        //        else
        //            temp = document.createElement('textarea');

        temp = document.createElement('div');
        temp.setAttribute("style", "clear:both");
        
        //<link type="text/css" href="resources/jquery.tooltip.css" rel="stylesheet">
        //        html = "<link type='text/css' href='" + getBaseUrl() + "Styles/main.css" + "' rel='stylesheet'>" + html;
        //        html = "<link type='text/css' href='" + getBaseUrl() + "Styles/ethics.css" + "' rel='stylesheet'>" + html;
        //        html = "<style> div.chapter, div.section {padding:0; margin:0; } </style>" + html;
        temp.innerHTML = html;
        d.appendChild(temp);

        scrolltotop = GetScrollTop();

        $('#ideal').css({ 'top': scrolltotop });
        $('#ideal').html(d);
        try {
            $('#ideal').makeCssInline();
        } catch(err) {
        // 
        }
        //  $('#ideal').html("<style> div.chapter, div.section {padding:0; margin:0; } </style>" + d);

        selectElementContents(document.getElementById('ideal'));
    });


    $("body").mouseup(function () {
        // Do something here 
        //        var selection = document.getSelection().getRangeAt(0).cloneContents();
        //        if (selection.childNodes.length > 0) {
        //            parent.showTextOpButtons();
        //        } else {
        //            parent.hideTextOpButtons();
        //        }


    });
    parent.hideTextOpButtons();
}