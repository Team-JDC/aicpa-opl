
var g_TargetDoc = "";
var g_TargetPtr = "";


var getStackTrace = function () {
    var obj = {};
    Error.captureStackTrace(obj, getStackTrace);
    return obj.stack;
};



//copied from webservicehelpers.js
function afterLoadContentBySiteNode(siteNode) {
//    updateArrowsForDocument();
//    loadCopyright(siteNode.Id, siteNode.Type);
//    loadMyScreens();
//    setMyDocumentsTab(true);
//    setPrintButton(true);
//    setBookmarkButtons(true);    
//    loadBreadCrumbForActiveDocument();
//    loadFafTools();
//    loadFormatOptions();
//    doProcessFeatures();
//    setEmailPageButton(true);
    enableBookmarkButtonsByIdType(siteNode.Id, siteNode.Type);    

//    var hash = $.History.setState("#/content?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
}


/**
* Sets the Visibility of the loading div
* 
* @param visible: true if the loading div should be shown, false to hide
*/
function setLoading(visible) {
//    if (visible) {
//        $('.add-loading').click(function () {
//            target.loadingOverlay();
//        });
//    } else {
//        $('.remove-loading').click(function () {
//            target.loadingOverlay('remove');
//        });
//    }
//    


    if (visible) {
        //console.log('Visible: True');
        if (!$("#loading-div").is(":visible")) {
            //$("#document-container img").bind('load', allImagesLoaded); 
            $("#loading-div").fadeIn();
        } else {
           // console.log("Already visible");
        }
    } else {
      //  console.log('Visible: False');
        if ($("#loading-div").is(":visible")) {
            //http: //localhost:55868/content/link/emap-part1/emap-part1_102
            $("#loading-div").fadeOut();
        } else {
           // console.log("Already hidden");
        }
    }
    //console.log(getStackTrace());
}

function setLoading2(visible) {
    //    if (visible) {
    //        $('.add-loading').click(function () {
    //            target.loadingOverlay();
    //        });
    //    } else {
    //        $('.remove-loading').click(function () {
    //            target.loadingOverlay('remove');
    //        });
    //    }
    //    

    if (visible) {
        console.log('Visible: True');
        if (!$("#loading-div").is(":visible")) {
            $("#loading-div").fadeIn();
        } else {
            console.log("Already visible");
        }
    } else {
        console.log('Visible: False');
        if ($("#loading-div").is(":visible")) {
            //http: //localhost:55868/content/link/emap-part1/emap-part1_102
            $("#loading-div").fadeOut();
        } else {
            console.log("Already hidden");
        }
    }
    console.log(getStackTrace());
}


function showInnerSearchButtons() {
    $('#headerHit').show();
}

function hideInnerSearchButtons() {
    $('#headerHit').hide();
}

//copied from webservicehelpers.js
function fillContentPaneFromUrl(url) {
    //$('#iframe-main').hide();
    //$('#document-container').show();

    $//('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks

    //$('#backup-document-container').hide();
    $('#document-container').load(url, function () {
        $('#document-container').unbind('load');
        setLoading(false);
    });
}

function fillLeftContentPaneFromUrl(url) {
    //$('#iframe-main').hide();
    //$('#document-container').show();

    //$('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks

    //$('#backup-document-container').hide();
    $('#document-container-left').load(url, function () {
        $('#document-container-left').unbind('load');
        $('#document-container-left').show();
        setLoading(false);
    });
}


function gotoUpsellPage(id, type, pageType) {
    //var paramsForMsg = { pageType: pageType };
    var params = "{id:" + id + ", type:'" + type + "'}";
    loadTemplate("/WS/Content.asmx/GetUpsellData", params, "/templates/odp2015/restricted.html", "document-container");
}

// function: loadContentBySiteNode
// params: siteNode - the siteNode to load
//         scrollbarPosition (OPTIONAL) - the scrollbar position to goto, if present this will
//            take precedence over a document anchor / hit highlighting
function loadContentBySiteNode(siteNode, scrollbarPosition) {
    if (siteNode.Restricted) {
        gotoUpsellPage(siteNode.Id, siteNode.Type);
        //RemoveEmptyScreens();
    } else if (siteNode.Restricted && isLMSLink()) {
        fillContentPaneFromUrl("/templates/errordirect.html");
    } else { // in our subscription
        // set siteNode property of active screen to be the new siteNode
        //getActiveScreen().siteNode = siteNode;

        //Changes here
        //setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());

        //callWebService("WS/UserPreferences.asmx/GetBookDataByBookIdDocType", "{id:" + siteNode.Id + ",type:'" + siteNode.Type + "'}", GetScreenData, ajaxFailed);

        if (siteNode.Type == "SiteFolder") {
            //scrollToAnchor(); // scroll div to the top
            loadTemplate("/WS/Content.asmx/GetSiteFolderDetails", "{id:" + siteNode.Id + "}", "/templates/siteFolderTitlePage2.html", "document-container-left");
            $('#document-container-left').ready(function () {
                if (getShowHighlights()) {
                    showInnerSearchButtons();
                } else {
                    hideInnerSearchButtons();
                }
            })
        } else {
            var anchor = null;
           
            if (siteNode.Anchor) {
                anchor = siteNode.Anchor;
            }
            // could turn on load screen here (figure out where to turn off; in both scrollToXXX?)
            setLoading(true);

            var hitAnchor = "";
            var url = "/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getShowHighlights() + "&lms=false";


            if ($('#backup-document-container')[0]) {
                $('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks
            }
            $('#document-container-left').load(url, function () {
                $('#document-container-left').unbind('load');
                $('#document-container-left').show();


                $('#document-container-left').ready(function () {
                    //Show the bookmark if it is possible. 
                    enableBookmarkButtonsByIdType(siteNode.Id, siteNode.Type);
                    if (getShowHighlights()) {
                        updateHitDocButtons(siteNode.Id, siteNode.Type);
                    } else {
                        hideInnerSearchButtons();
                    }
                    if (anchor || scrollbarPosition) {
                        window.doScrollTimerCallback = function () {
                            clearInterval(theTimer);
                            theTimer = null;
                            if (scrollbarPosition) {
                            } else {
                                scrollToAnchor(anchor);
                            }
                            setLoading(false);
                        }; //window.doScrollTimerCallback

                        theTimer = setInterval(checkTimer, timerInterval);
                        checkTimerCount = 0;

                        checkTimer();
                    } // if (anchor...
                }); // $('#document-container').ready

                setLoading(false);
            });            
        }
        afterLoadContentBySiteNode(siteNode);
    }  // outer else

    // fade in
    //    $('#document-container').fadeIn('600');
}

var highlightName = "destroyer_hilite";
var theTimer = null;
var checkTimerCount = 0;
var timerInterval = 500; //ms
var maxTimerChecks = 6; //currently 3 secs

// Code below was taken from StackOverflow. It appears to work 
//http: //stackoverflow.com/questions/8922107/javascript-scrollintoview-middle-alignment
Element.prototype.documentOffsetTop = function () {
    return this.offsetTop + (this.offsetParent ? this.offsetParent.documentOffsetTop() : 0);
};


function scrollToAnchor(anchor) {
    var anchorElement = null;

    if (anchor) {
        if (anchor == highlightName) {
            //            var found = $("a[name='" + anchor + "']:first");
            var found = $('#document-container-left').contents().find("a[name='" + anchor + "']:first");

            if (found.length > 0) {
                anchorElement = found[0];
            }
        }
        else {
            //var anchors = document.getElementsByName(anchor);
            var anchors = $('#document-container-left').contents().find("a[name='" + anchor + "']");

            if (anchors.length > 0) {
                anchorElement = anchors[0];
            }
        }
    }

    //info about ScrollIntoView
    //https://developer.mozilla.org/en-US/docs/Web/API/Element.scrollIntoView
    if (anchorElement) {
        var top = anchorElement.documentOffsetTop() - (window.innerHeight / 2);
        window.scrollTo(0, top);
        //anchorElement.scrollIntoView(true); // if the above doesn't work then we should do something.. this is the original
    }
    else {        
        $("#document-container-left").contents().find("left-col").scrollTop(0);
    }
}

function checkTimer() {
    checkTimerCount++;

    if (checkAvailable() || checkTimerCount >= maxTimerChecks) {
        doScrollTimerCallback();
    }
}

function checkAvailable() {
    var available = true;

    try {
        if ($("#document-container-left").contents().find('#css-loaded img').height == 0) {
            available = false;
        }
    }
    catch (e) {
        available = false;
    }

    if (available) {
        var cssLoaded = $("#document-container-left").contents().find("#css-loaded");
        //if ($('#css-loaded').height() != 3 || $('#css-loaded').width() != 3) {
        if (cssLoaded.height() != 3 || cssLoaded.width() != 3) {
            available = false;
        }
    }

    return available;
}




function scrollToPosition(position) {
    //$("#content-container").scrollTop(position);
    $("#document-container-left").contents().find("body").scrollTop(position);
}




//used to unencode endeca snippet, or for anything else that's needed.
function replaceAll(stringToParse, search, replace) {
//catch the null case;
    if (stringToParse == null || stringToParse === false)
        return stringToParse;
    while (stringToParse.indexOf(search) > -1) {
        stringToParse = stringToParse.replace(search, replace);
    }
    return stringToParse;
}

function unencodeHtml(stringToParse) {
    parsedString1 = replaceAll(stringToParse, "&gt;", ">");
    parsedString2 = replaceAll(parsedString1, "&lt;", "<");
    parsedString3 = replaceAll(parsedString2, "&#39;endeca_term&#39;", "endeca_term");
    return parsedString3;
}

function showBookmarks(nodeType) {
    if (nodeType == null || nodeType === false) {
        return false;
    } else {
        if (nodeType != 'SiteFolder') {
            return true;
        } else {
            return false; 
        }
    }
    return false;
}

function clearPhoneNav(me) {
    $(me).attr("href", "");
    $(me).prop("disable", true);
    $(me).attr("disabled", "disabled");
}

function setPhoneNav(me, link) {
    $(me).attr("href", link);
    $(me).prop("disabled", false);
    $(me).removeAttr("disabled");
}

function clearBookmarkOnclick(me) {
    $(me).attr("onclick", "");
    $(me).hide(); 
}

function setBookmarkOnclick(me, link) {
    $(me).attr("onclick", link);    
}

function updateMobileNav() {
    //https://stackoverflow.com/questions/14144358/bootstrap-modal-getting-blacked-out-because-of-positioned-navbar/22443294#22443294
    //Moved this here, but it's really for the non-mobile
    $("#download_modal").appendTo("body");
    var printOnClick= $('a.print').attr('onclick');
    if (printOnClick) {
        $("#aPrintPhone").each(function () {
            $(this).attr("onclick", printOnClick);
            $(this).prop("disabled", false);
            $(this).removeAttr("disabled");
            $(this).show();    
        });
    } else {
    $("#aPrintPhone").each(function () {
        $(this).attr("onclick", "");
        $(this).prop("disabled", true);
        $(this).hide();
    });
    }

    var prevPhone = $('a.prev').attr('href');
    var nextPhone = $('a.next').attr('href');
    if (prevPhone) {
        $('a#phonePrev').each(function () {
            setPhoneNav(this, prevPhone);
        });        
    } else {
        $('a#phonePrev').each(function () {
            clearPhoneNav(this);

        });
    }
    if (nextPhone) {
        $('a#phoneNext').each(function () {
            setPhoneNav(this, nextPhone);
        });        
    } else {
        $('a#phoneNext').each(function () {
            clearPhoneNav(this);
        });
    }

    //copy href from main page to mobile nav
    
    var aBookmarkPhone = $("#Toolbar-Tools-AddBookmark-Widget > a").attr('onclick');
    var rBookmarkPhone = $("#Toolbar-Tools-DeleteBookmark-Widget > a").attr('onclick');

    if (aBookmarkPhone) {
        $('a#aBookmarkPhone').each(function () {
            setBookmarkOnclick(this, aBookmarkPhone);
        });        
    } else {
        $('a#aBookmarkPhone').each(function () {
            clearBookmarkOnclick(this);
        });
    }

    if (rBookmarkPhone) {
        $('a#rBookmarkPhone').each(function () {
            setBookmarkOnclick(this, rBookmarkPhone);
        });
    } else {
        $('a#rBookmarkPhone').each(function () {
            clearBookmarkOnclick(this);
        });
    }


}

function loadDocumentWidgetByIdType(id, type) {
    var params = "{ id:'" + id + "', type:'" + type + "'}";
    enableBookmarkButtonsByIdType(id, type);

//    loadTemplateDual('/WS/Content.asmx/ResolveContentLinkExtraByIdType', params, [{ 'templateUrl': '/templates/next_prev_widget.html', 'containerId': 'document-container-right' },
//                                                                            { 'templateUrl': '/templates/odp2015/bookmark_phone.html', 'containerId': 'phoneBookmarkli' }], {}, updateMobileNav);
    loadTemplate('/WS/Content.asmx/ResolveContentLinkExtraByIdType', params, '/templates/odp2015/next_prev_widget.html', 'document-container-right', {}, updateMobileNav,null, widgetLoadFailed);
    
}

function widgetLoadFailed() {
    //document-container-right
    //do nothing
}


function loadDocumentWidget(targetDoc, targetPtr) {
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    enableBookmarkButtons(targetDoc, targetPtr);
//    loadTemplateDual('/WS/Content.asmx/ResolveContentLinkExtra', params, [{ 'templateUrl': '/templates/next_prev_widget.html', 'containerId': 'document-container-right' },
//                                                                          { 'templateUrl': '/templates/odp2015/next_prev_widget_phone.html', 'containerId': 'phone_widget'}], {}, updateMobileNav);

    loadTemplate('/WS/Content.asmx/ResolveContentLinkExtra', params, '/templates/odp2015/next_prev_widget.html', 'document-container-right', {},updateMobileNav); 
}

function loadHistory() {
    loadTemplate('/WS/HomePage.asmx/GetRecentDocuments', '{}', '/templates/history.html', 'liHistory');
}

function loadWhatsNew() {    
    loadTemplate('WS/WhatsNew.asmx/GetWhatsNew', "{}", 'templates/whatsNew.html', 'document-container-right');
}

function loadHistory() {
    loadTemplate('/WS/HomePage.asmx/GetRecentDocuments', '{}', '/templates/historyPhone.html', 'liHistoryPhone');
}

function doPrint(id, type) {
    var showSources = (getShowSources() ? "&showCodificationSources=true" : "");
    var windowUrl = "/PrintDocument.ashx?id=" + id + "&type=" + type + "&doPrint=true" + showSources;
    var windowHandle = window.open(windowUrl, "Print", "width=800,height=600,left=0,top=0,screenX=0,screenY=0,scrollbars=yes");
}
/*
$("#printButton").click(function () {
    if (hasActiveDocument()) {
        var showCodificationSources = "";
        if ($("#sourcesPrint:checked").length > 0)
            showCodificationSources = "true";
        var printSubDocs = "";
        if ($("#subpagesPrint:checked").length > 0)
            printSubDocs = "true";
        var printPDF = "";
        if ($("#pdfPrint:checked").length > 0)
            printPDF = "true";

        var windowUrl = "PrintDocument.ashx?id=" + getActiveDocumentId() + "&type=" + getActiveDocumentType() + "&printSubdocuments=" + printSubDocs + "&showCodificationSources=" + showCodificationSources + "&printToPDF=" + printPDF + "&doPrint=true";
        if (g_currentView != null && (g_currentView.toolName == toolName_joinSections || g_currentView.toolName == toolName_joinChildren) && g_lastJoinSectionsUrl) {
            windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl;
        }
        var windowHandle = window.open(windowUrl, "Print", "width=600,height=250,left=0,top=0,screenX=0,screenY=0");

        disablePopup();
        $("#printContent").empty();
    }
});
*/


function loadToc(syncToc, id, type) {
    if (id == undefined)
        id = getTocStateId();
    if (type == undefined)
        type = getTocStateType();
    if (!syncToc) {
        if ((id) && (type)) {
            var params = { id: id, type: type };
            //setToolAsCurrentView(toolName_toc, params);
            //set TOC to last state
            setTocStateId(id);
            setTocStateType(type);
        } else {
            //setToolAsCurrentView(toolName_toc, null);
        }
    }

    //hideDocumentSpecificButtons();

    if ((id) && (type)) {
        if ((id != -1) && (type != "Site"))
            fillDocumentContainerFromUrl("/static/opl2015plainSyncTocReload.html");
        else fillDocumentContainerFromUrl("/static/opl2015plainToc.html");
    } else if (syncToc) {
        fillDocumentContainerFromUrl("/static/opl2015plainSyncToc.html");
    } else {
        fillDocumentContainerFromUrl("/static/opl2015plainToc.html");
    }
}

function fillDocumentContainerFromUrl(url) {    
    $('#document-container-left').show();
    $('#document-container-left').load(url, function () {
        $('#document-container-left').unbind('load');
        setLoading(false);
    });
}

function IsNull(aTextField) {
    if (aTextField == null) {
        return '';
    }
    else { return aTextField; }
}

function doNotesLink() {
    window.location = "/tools/notes";
}

function loadNotes() {
    loadTemplate("/WS/UserPreferences.asmx/GetAllMyNotes", "{}", "/templates/odp2015/notes.html", "document-container-left");
}


function loadBookmarks() {
    loadTemplate("/WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "/templates/odp2015/bookmarks.html", "document-container-left");
}

function loadArchiveContent(id) {
    var encodeStr = g_TargetDoc + "|" + g_TargetPtr + "|" + id;
    encodeStr = Base64.encode(encodeStr);
    window.location = "/tools/loadarchive/archive/" + encodeStr;
}

//id = filename
function doLoadArchiveContent(id) {
    fillDocumentContainerFromUrl("/Handlers/GetArchiveContent.ashx?id=" + id);
}

//fillContentPaneFromUrl("Handlers/DownloadDocument.ashx?docid=" + getActiveDocumentId() + "&d_ft=" + 18);
function loadArchive(documentid) {    
    fillContentPaneFromUrl("/Handlers/DownloadDocument.ashx?docid=" + documentid + "&d_ft=" + 18);
    //loadTemplate("/WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "/templates/odp2015/archive.html", "document-container-left");
}

function loadArchiveByDocAndPtr(targetDoc, targetPtr) {    
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    callWebService("/WS/Content.asmx/ResolveContentLink", params, function (sitenode) {
        //write these to the global variables g_TargetDoc and g_TargetPtr
        // these will be used by the loadArchiveContent
        g_TargetDoc = targetDoc;
        g_TargetPtr = targetPtr;
        fillContentPaneFromUrl("/Handlers/DownloadDocument.ashx?docid=" + sitenode.Id + "&d_ft=" + 18);
    }, ajaxFailed);
    //loadTemplate("/WS/Content.asmx/ResolveContentLink", data, "/templates/odp2015/archive.html", "document-container");
}



function loadSavedSearches() {
    loadTemplate('/WS/SearchServices.asmx/GetSavedSearches', '{}', '/templates/odp2015/savedSearches.html', 'document-container-left');
}

function loadPreferences() {
    loadTemplate("/WS/UserPreferences.asmx/GetUserPreferences", "{}", "/templates/odp2015/fonts.html", "document-container-left");
}

function loadGoTo() {
    loadTemplate("/WS/DocumentTools.asmx/GetGotoInformation", "{topicNum:'', subNum:''}", "/templates/odp2015/goto.html", "document-container-left");
}

function loadJoin() {
    loadTemplate("/WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'', content:'', includeSubtopics:false}", "/templates/odp2015/join.html", "document-container-left");
}

function loadGuide() {
    loadTemplate("/WS/SearchServices.asmx/GetSavedSearches", "{}", "/templates/odp2015/howToGuide.html", "document-container-left");
}

function loadXRef() {
    loadTemplate("/WS/DocumentTools.asmx/GetStandardsForCrossReference", "{standard: '', topic: '',subTopic: ''}", "/templates/odp2015/xref.html", "document-container-left");
}

function setDocBookmark(visible) {
    if (visible) {
        $(".docBookmark").show();
    } else {
        $(".docBookmark").hide();
    }
}

function setAddBookmarkButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-AddBookmark-Widget").show();
        $(".aBookmarkPhone").show();

    } else {
        $("#Toolbar-Tools-AddBookmark-Widget").hide();
        $(".aBookmarkPhone").hide();
    }
}

function setDeleteBookmarkButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-DeleteBookmark-Widget").show();
        $(".rBookmarkPhone").show();
        $(".docBookmark").show();
    } else {
        $("#Toolbar-Tools-DeleteBookmark-Widget").hide();
        $(".rBookmarkPhone").hide();
        $(".docBookmark").hide();
    }
}

function setBookmarkButtons(visible) {
    if (!visible) {
        setAddBookmarkButton(false);
        setDeleteBookmarkButton(false);
    } else {
        setAddBookmarkButton(true);
        setDeleteBookmarkButton(true);
//        //only handle 'Document' items.  We can't bookmark a site right now
//        if (getActiveDocumentType() == 'Document')
//            enableBookmarkButtons(getActiveDocumentId(), getActiveDocumentType());
        //
    }
}

/* Load the targetDoc for the current book */
function GetScreenData(data) {
    var reloadScreen = (getActiveScreen().targetDoc != data.TargetDoc);
    getActiveScreen().targetDoc = data.TargetDoc;
    if (reloadScreen)
        loadMyScreens();
}



function callWebService(serviceUrl, paramString, successFunction, errorFunction, paramForCallback) {
    $.ajax({
        type: "POST",
        url: serviceUrl,
        data: paramString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            successFunction(msg.d, paramForCallback)
        },        
        error: function (xhr, ajaxOptions, thrownError) {
            if (errorFunction && typeof errorFunction == "function") {
                errorFunction(xhr);
            }
        },
        complete: function (xhr, status) {
            if (status === 'error' || !xhr.responseText) {
                //handleError();
                //alert("Error:" + xhr.responseText);
                try {
                    logErrorToServer("CallWebService:AjaxFailed.. Status: " +result.status + "-- ResponseText: " +result.responseText);
                } catch (e) {
                    // nothing.
                }
                        
            } else {
                var data = xhr.responseText;
                //...
            }
        } 
    });
}

/***********************************************************************
 Purpose: With ODP 2015, we have multiple menu's, but we want to use the 
   same data but call two different templates.  This will call the service
   and use that data for calling multiple templates.


   input:
        serviceUrl - Service Call
        paramString - 
        templateUrls -
        containerId
        paramForMsg
        callback
        nonfilter


 ***********************************************************************/
function loadTemplateDual(serviceUrl, paramString, templateUrls, paramsForMsg, callback, nonFilter, failCallback) {
    if (nonFilter == null)
        nonFilter = false;
    try {
        $.ajax({
            type: "POST",
            url: serviceUrl,
            data: paramString,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {                
                if (!callback) {
                    msg.d.client = paramsForMsg;
                }
                //instantiate a template with data
                for (index = 0; index < templateUrls.length; index++) {
                    var url = templateUrls[index].templateUrl;
                    var containerId = templateUrls[index].containerId;
                    applyTemplate(msg, url, containerId, nonFilter);

                    if (callback) {
                        callback(paramsForMsg);
                    }
                }
            },            
            error: function (xhr, ajaxOptions, thrownError) {
                if (failCallback)
                {
                    failCallback();
                } else if (ajaxFailed) {
                    ajaxFailed(xhr);
                }
            }


        });
    }
    catch (e) {
        ajaxFailed();
    }
}

//Make use of method above and just recall that. 
function loadTemplate(serviceUrl, paramString, templateUrl, containerId, paramsForMsg, callback, nonfilter, failCallback) {
    var obj = [{ 'templateUrl': templateUrl, 'containerId': containerId }];
    loadTemplateDual(serviceUrl, paramString, obj, paramsForMsg, callback, nonfilter);
}

function loadExacct() {
    //setToolAsCurrentView(toolName_homePage, "");
    loadTemplate("/WS/HomePage.asmx/GetHomePageData", "{}", "/templates/loadexacct.html", "document-container");
}

function applyTemplate(msg, templateUrl, containerId, nonfilter) {
    if ((nonfilter != null) && (nonfilter == true)) {
        $('#' + containerId).setTemplateURL(templateUrl, [], { filter_data: false }); //http://stackoverflow.com/questions/1721603/jtemplates-html-in-variables
    }
    else {
        $('#' + containerId).setTemplateURL(templateUrl);
    }
    $('#' + containerId).processTemplate(msg);
    $('#' + containerId).ready(function () {
        setLoading(false);
        if (getShowHighlights()) {
            showInnerSearchButtons();
        } else {
            hideInnerSearchButtons();
        }
    })
}


function removeUnloadEvent() {
    window.onbeforeunload = null;
}

function ajaxFailed(result) {
    if (result) {
        if (result.status == 500 && result.responseText.indexOf("UserNotAuthenticated") != -1) {

            // redirect to SessionExpired Page
            logErrorToServer("AjaxFailed.. Status: " + result.status + "-- ResponseText: " + result.responseText);
            removeUnloadEvent();
            window.location = "/SessionExpired.aspx";
        } else {
            logErrorToServer(result.responseText);
            loadError();
            //        alert('failed' + result.status + ' ' + result.statusText);
            //        alert('reponse text ' + result.responseText);
        }
    } else {
        logErrorToServer("AjaxFailed:Result was null");
    }
    //setLoading(false);
}


function logErrorToServer(logString) {
    try {
        callWebService("/WS/Toolbars.asmx/LogError", "{logString: '" + logString + "'}", logErrorCallback, logErrorCallback);
    }
    catch (ex)
    { }
}

function loadError() {
//    hideDocumentSpecificButtons();
    fillContentPaneFromUrl("/templates/error.html");
}

function doLogout() {
    clearListCookies();
    callWebService("/WS/Toolbars.asmx/Logout", "{}", redirectToLogoutPage, ajaxFailed);
    //$.get(remoteLogoutUrl);
    //window.location.assign(remoteLogoutUrl);
}

function clearListCookies() {
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var spcook = cookies[i].split("=");
        deleteCookie(spcook[0]);
    }
    function deleteCookie(cookiename) {
        var d = new Date();
        d.setDate(d.getDate() - 1);
        var expires = ";expires=" + d;
        var name = cookiename;
        //alert(name);
        var value = "";
        document.cookie = name + "=" + value + expires + "; path=/";
        document.cookie = name + "=" + value + expires + "; path=/; domain=.odp.knowlysis.com";
    }
    //window.location = ""; // TO REFRESH THE PAGE
}

function redirectToLogoutButton() {
    removeUnloadEvent();
    document.location = "/Logout";
}

function redirectToLogoutPage() {
    //$.get(oktaLogoutUrl, function () {
    //});

    document.location = remoteLogoutUrl; //"/Logout";
    //$.ajax({
    //    url: oktaLogoutUrl,       
    //    success: function (data, textStatus) {
    //        console.log("OKTA Logout");
    //        console.log(data, textStatus);
    //        logErrorToServer("OKTA Logout..");
    //    }
    //}).always(function () {
    //    document.location = remoteLogoutUrl; //"/Logout";
    //});    
}


//Moved out to function so it can be called from the pagebeforehide event
function HideTocContent() {
    $('.internalContentTocTitle').toggleClass("internalContentTocTitleActive", false);    
    $('.mobileTocContent').slideUp();
    $('.mobileTocLoading').hide();
}

function ToggleInternalToc() {

    //tocContentHolder
    if ($('#tocContentHolder').is(':visible')) {
        $('#tocContentHolder').slideUp();
    } else {
        $('#tocContentHolder').slideDown();
        if (!$('#actualTocContentData').length) {
            loadMobileBreadcrumb();
        }
    }

    //if ($('.internalContentTocTitle').hasClass('internalContentTocTitleActive')) {
    //    HideTocContent();
    //}
    //else {
    //    $('.internalContentTocTitle').toggleClass("internalContentTocTitleActive", true);
    //    //var id = $.mobile.activePage[0].id;
    //    var docC = id + "Content";

    //    id = id.replace("document", "");
    //    var docnum = parseInt(id);
        

    //    var attr = $("#"+docC).hasClass("bloaded");
    //    if (!attr) {
    //        //internalContentTocTitle
    //        //internalContentTocTitle
    //        //internalContentToc 
    //        $("#document" + docnum + "Content #internalContentTocId").toggleClass("internalContentTocTitleInit", true);            
    //        loadMobileBreadcrumb(docnum)
    //        $("#"+docC).toggleClass("bloaded",true);
    //    }
        
    //    //$('.mobileTocLoading').slideDown();
    //    $('.mobileTocContent').slideDown();


    //}
}


function loadMobileBreadcrumb(docnum) {

    webserviceUrl = "/WS/Content.asmx/GetFullTocStrByTargetDocTargetPtr";
    parameters = "{ targetDoc: '" + routeTargetDoc + "', targetPtr: '" + routeTargetPtr + "', routeNodeType: '" + routeNodeType + "' }";


    $.ajax({
        type: "POST",
        url: webserviceUrl,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //$("#document" + docnum + "Content #breadcrumbContent").html(response.d);
            //console.log('loadMobileBreadcrumb: Trigger Create');
            //$("#document" + docnum + "Content #breadcrumbContent").trigger('create');
            //$("#document" + docnum + "Content #internalContentTocId").toggleClass('internalContentTocTitleInit', false);
            $('#tocContentHolder').html(response.d);
        },
        error: function (jqXHR, textStatus, errorThrown) {

            loadMobileBreadcrumbAjaxFailure(jqXHR, textStatus, errorThrown, docnum);
        }
    });

}

function loadMobileBreadcrumbAjaxFailure(jqXHR, textStatus, errorThrown, docnum) {
    var json = jqXHR.responseJSON;
    var error = "Error loading breadcrumb. If you believe this message was received in error, please contact support at service@aicpa.org.";
    if ((json != null) && (json.Message)) {
        error = "Error loading breadcrumb: "+json.Message;       
    }
    error = "<p>" + error + "</p>";
    //var desc = "errorThrown: " + errorThrown + " textStatus: " + textStatus + " docnum: " + docnum;
    $('#tocContentHolder').html(error);
}

function loadPFP() {
    fillLeftContentPaneFromUrl("/templates/loadpfptoolkit.htm");
}


