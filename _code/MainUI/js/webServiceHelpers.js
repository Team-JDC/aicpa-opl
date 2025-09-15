// Note this function has to come first because it is used during some of the jTemplate rendering.
function convertToFileName(input) {
    return input.replace(/ /g, "_");
}

function function_exists(function_name) {
    // Checks if the function exists  
    // 
    // version: 1006.1915
    // discuss at: http://phpjs.org/functions/function_exists    
    // +   original by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // +   improved by: Steve Clay
    // +   improved by: Legaev Andrey
    // +   improved by: Benjamin Bytheway (not really, but I wanted my name in this)
    // *     example 1: function_exists('isFinite');
    // *     returns 1: true    
    if (typeof function_name == 'string') {
        return (typeof this.window[function_name] == 'function');
    } else {
        return (function_name instanceof Function);
    }
}

//$(document).click(function (obj) {

//    var elementclicked = obj.srcElement;

//    if (elementclicked.outerHTML.indexOf("doHitLink") > -1 ||
//        elementclicked.outerHTML.indexOf("doSearchLink") > -1 ||
//        (elementclicked.parentElement != null && elementclicked.parentElement.className == "search-hit-wrapper") ||
//        (elementclicked.parentElement != null && elementclicked.parentElement.parentElement != null && elementclicked.parentElement.parentElement.className == "search-hit-wrapper")) {

//    }
//    else {
//        hideInnerSearchButtons();
//    }
//});



$(function () {
    addUnloadEvent();
    loadHomeNavToolbar();
    hideInnerSearchButtons();
    hideTextOpButtons();
    OnReadyEvent();

    parameters = getUrlVars();
    var targetdoc = parameters["targetdoc"];
    var targetptr = parameters["targetptr"];
    var id = parameters["id"];
    var type = parameters["type"];
    var viewCompleteTopic = (parameters["vct"] != null && parameters["vct"] == '1' ? true : false);


    if (targetdoc != null && targetptr != null) {
        doLink(targetdoc, targetptr, true, viewCompleteTopic);
    }
    else if (id != null && type != null) {
        doHomePageContentLink(id, type, viewCompleteTopic);
    }
    else {
        //loadHomePage(); DW
        loadMyScreens();
        loadLibraryBooks();
        hideDocumentSpecificButtons();
        // loadRoot = true
        loadBreadCrumbForActiveDocument(true);
        loadFafTools();
    }
});

function OnReadyEvent() { }

function getUrlVars() {
    var vars = [], hash;
    var urlMinusAnchor = window.location.href;
    if (window.location.href.indexOf('#') > 0)
        urlMinusAnchor = urlMinusAnchor.substring(0, window.location.href.indexOf('#'));
    var hashes = urlMinusAnchor.slice(urlMinusAnchor.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function loadMyScreens() {
    var screens = getMyScreens();

    var activeIndex = -1;
    if (hasActiveScreen()) {
        activeIndex = getActiveScreenIndex();
    }

    var message = { d: screens, activeIndex: activeIndex };

    applyTemplate(message, "templates/myDocuments.html", "Toolbar-MyDocuments");
}

function loadHomeNavToolbar() {
    loadTemplate('WS/HomePage.asmx/GetHomePageFirstLevelData', '{}', 'templates/homeNavToolbar.html', 'Toolbar-Home');
}

function loadHomePage() {
    setToolAsCurrentView(toolName_homePage, "");
    loadTemplate("WS/HomePage.asmx/GetHomePageData", "{}", "templates/homePage.html", "document-container");
}

function loadExacct() {
    //setToolAsCurrentView(toolName_homePage, "");
    loadTemplate("WS/HomePage.asmx/GetHomePageData", "{}", "templates/loadexacct.html", "document-container");
}

function loadBreadCrumbForActiveDocument(loadRoot) {
    if (loadRoot) {
        loadBreadCrumb(-1, "Site");
    } else {
        if (hasActiveDocument()) {
            loadBreadCrumb(getActiveDocumentId(), getActiveDocumentType());
        }
        else {
            // if no active doc, load the home list
            loadBreadCrumb(-1, "Site");
        }
    }
}

function loadCopyright(id, type) {
    if (id && type) {
        callWebService("WS/Content.asmx/GetCopyrightNotice", "{id:" + id + ",type:'" + type + "'}", loadCopyrightResultHandler, ajaxFailed);
    }
}

function loadCopyrightResultHandler(data) {
    setCopyright(data);
}

/**
* Sets the Visibility of the next/prev arrow buttons
* 
* @param visible: true if the arrows should be shown, false to hide
*/
function setCopyright(text) {
    if (text) {
        $("#copyright").html(text);
    } else {

        var defaultText = "Copyright &copy; " + getCurrentCopyrightYear() + ", American Institute of Certified Public Accountants, Inc. All Rights Reserved.";
        $("#copyright").html(defaultText);
    }
}

function loadBreadCrumb(id, type) {
    loadTemplate("WS/Content.asmx/GetFullBreadcrumb", "{id:" + id + ",type:'" + type + "'}", 'templates/fullBreadcrumb.html', 'breadcrumbContainer');
}

function setFormatOptions(show) {
    if (show) {
        loadFormatOptions();
    }
    else {
        hideFormatOptions();
    }
}

function loadFormatOptions() {
    if (hasActiveDocument() && getActiveDocumentType() == "Document") {
        loadTemplate("WS/Content.asmx/GetDocumentFormats", "{id:" + getActiveDocumentId() + "}", "templates/formatOptions.html", "formatOptionsContainer");
        $("#formatOptionsContainer").show();
    }
    else {
        $("#formatOptionsContainer").hide();
    }

}

function hideFormatOptions() {
    $("#formatOptionsContainer").hide();
}

/** 
* This function handles hiding and showing the "Next"
* and previous arrow buttons when the content is a document (proper) ie, not a siteFolder, etc.
*/
function updateArrowsForDocument() {
    if (hasActiveScreen() && getActiveScreen().siteNode.Type == 'Document') {
        setPreviousArrow(getActiveScreen().siteNode.hasPrevious);
        setNextArrow(getActiveScreen().siteNode.hasNext);
    }
    else {
        setArrows(false);
    }
}

/**
* Sets the Visibility of the previous arrow button
* 
* @param visible: true if the previous arrow should be shown, false to hide
*/
function setPreviousArrow(visible) {
    if (visible) {
        $('#nav-prev').show();
    }
    else {
        $('#nav-prev').hide();
    }
}

/**
* Sets the Visibility of the next arrow button
* 
* @param visible: true if the next arrow should be shown, false to hide
*/
function setNextArrow(visible) {
    if (visible) {
        $('#nav-next').show();
    }
    else {
        $('#nav-next').hide();
    }
}

/**
* Sets the Visibility of the next/prev arrow buttons
* 
* @param visible: true if the arrows should be shown, false to hide
*/
function setArrows(visible) {
    setPreviousArrow(visible);
    setNextArrow(visible);
}

/**
* Sets the Visibility of the loading div
* 
* @param visible: true if the loading div should be shown, false to hide
*/
function setLoading(visible) {
    if (visible) {
        $("#loading-div").fadeIn();
    } else {
        $("#loading-div").fadeOut();
    }
}

/**
* Sets the Visibility of the Print Button
* 
* @param visible: true if the Print Button should be shown, false to hide
*/
function setPrintButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-Print").show();
        $("#Toolbar-Tools-Print-Top").show();
    } else {
        $("#Toolbar-Tools-Print").hide();
        $("#Toolbar-Tools-Print-Top").hide();
    }
}

/**
* Sets the Visibility of the Bookmark Button
* 
* @param visible: true if the Print Button should be shown, false to hide
*/
function setAddBookmarkButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-AddBookmark-Top").show();
    } else {
        $("#Toolbar-Tools-AddBookmark-Top").hide();
    }
}

function setDeleteBookmarkButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-DeleteBookmark-Top").show();
    } else {
        $("#Toolbar-Tools-DeleteBookmark-Top").hide();
    }
}

function setEmailPageButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-EmailPage-Top").show();
        $("#Toolbar-Tools-Link-Top").show();
    } else {
        $("#Toolbar-Tools-EmailPage-Top").hide();
        $("#Toolbar-Tools-Link-Top").hide();
    }
}

function setSearchResultsButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-SearchResults-Top").show();
    } else {
        $("#Toolbar-Tools-SearchResults-Top").hide();
    }
}

function setBookmarkButtons(visible) {
    if (!visible)
    {
        setAddBookmarkButton(false);
        setDeleteBookmarkButton(false);
    } else {
        //only handle 'Document' items.  We can't bookmark a site right now
        if (getActiveDocumentType() == 'Document')
            enableBookmarkButtons(getActiveDocumentId(), getActiveDocumentType());
    }
}

function copyText() {
    var iframe = document.getElementById('iframe-main');
    var idoc = iframe.contentDocument || iframe.contentWindow.document; // ie compatibility
        
    if (document.getElementById('iframe-main').contentWindow.GetHilightedContent) {
        var content = document.getElementById('iframe-main').contentWindow.GetHilightedContent();
        content = content.replace(/\|INSERT LINK HERE\|/gi, d_returnurl);
        return content;
    }
    return '';
}

function openLinkDialog() {
    var boxTitle = "To link directly to this page, copy and paste the link below into a document or into your browser:";
    var deepLink = getEmailLink();

    openGenericPopup(boxTitle, deepLink)
}

function openGenericPopup(title, content) {
    $('#genericPopupTitle').text(title);    
    $('#genericPopupContent').text(content);
    $("#genericPopup").fadeIn("slow");
}

function closeGenericPopup() {
    $("#genericPopup").fadeOut("slow");
}

function getEmailLinkError(result) {
    alert("Error generating email link");
}

function getEmailLink() {    
    var id = getActiveDocumentId();
    var type = getActiveDocumentType();

    var paramstr = "{id:" + id + ",type:'" + type + "'}";
    var targetPtr = undefined;
    var targetDoc = undefined;    
    $.ajax({
        type: "POST",
        url: "WS/Content.asmx/GetTargetDocPtrByBookIdDocType",
        data: paramstr,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            targetDoc = msg.d.TargetDoc;
            targetPtr = msg.d.TargetPointer;
        },
        error: getEmailLinkError,
        complete: function (xhr, status) {
            if (status === 'error' || !xhr.responseText) {
                //handleError();
                alert("Error:" + xhr.responseText);
            } else {
                var data = xhr.responseText;
                //...
            }
        }
    });

    var vct = "";
    if (getActiveDocumentVCT()) {
        vct = "&vct=1";
    } else if (hasActiveScreen() && (getActiveScreen().siteNode.Anchor != null) && (getActiveScreen().siteNode.Anchor != "")) {
        targetPtr = getActiveScreen().siteNode.Anchor;
    }

    var title = document.getElementById('iframe-main').contentWindow.document.title;
    return d_returnurl + '&tdoc=' + targetDoc + '&tptr=' + targetPtr + vct;
}

function GetTargetPtrAndDocByIdType(id, type) {
   
}


//GetTargetDocPtrByBookIdDocType

function openEmailLink() {
    var title = getActiveScreen().siteNode.Title;
    var link = title + ' - ' + getEmailLink();
    var href = "mailto:?subject=" + encodeURIComponent(title) + "&body=" + encodeURIComponent(link);
    window.open(href);
}

function updateEmailLink() {
    // no longer valid
}

function showTextOpButtons() {
    $("#textOpButtons").show();
   
}

function hideTextOpButtons() {
    $("#textOpButtons").hide();
}

function hideInnerSearchButtons() {
    $('#headerHit').hide();
}

function showInnerSearchButtons() {
    $('#headerHit').show();
}

function isInnerSearchButtons() {
    return $('#headerHit').is(':visible');
}

function showHideNextPrevButtons() {
    var showNext = false;
    var showPrev = false;

    if (document.getElementById('iframe-main').contentWindow.isNextVisible && document.getElementById('iframe-main').contentWindow.isNextVisible()) {
        showNext = true;
    }

    if (document.getElementById('iframe-main').contentWindow.isPrevVisible && document.getElementById('iframe-main').contentWindow.isPrevVisible()) {
        showPrev = true;
    }

    if (showNext) {
        //$("#nextHit").show();
        $("#nextHit").removeClass("disabled");
        //$("#nextHit").attr("href", "#");
        $("#nextHit").off("click");
        $("#nextHit").on('click', function () {
            if (document.getElementById('iframe-main').contentWindow.goToNext) {
                document.getElementById('iframe-main').contentWindow.goToNext();
                showHideNextPrevButtons();
            }
        });
    }
    else {
        //$("#nextHit").hide();
        $("#nextHit").addClass("disabled");
        $("#nextHit").off("click");
        //$("#nextHit").attr("href", "");
    }

    if (showPrev) {
        //$("#prevHit").show();
        $("#prevHit").removeClass("disabled");
        //$("#nextHit").attr("href", "#");
        $("#prevHit").off("click");
        $("#prevHit").on('click',function () {
            if (document.getElementById('iframe-main').contentWindow.goToPrevious) {
                document.getElementById('iframe-main').contentWindow.goToPrevious();

                showHideNextPrevButtons();
            }
        });
    }
    else {
        //$("#prevHit").hide();
        $("#prevHit").addClass("disabled");
        $("#prevHit").off("click");
        //$("#nextHit").attr("href", "");
    }
}

function hideDocumentSpecificButtons() {
    setArrows(false);
    setFormatOptions(false);
    setCopyright();
    setPrintButton(false);
    setBookmarkButtons(false);
    hideInnerSearchButtons();
    hideTextOpButtons();
    setEmailPageButton(false);
}

function hideFAFTools(turnOffOnlyDocumentSpecificFafToolButtons) {
    if (turnOffOnlyDocumentSpecificFafToolButtons) {
        $("#fafWhatLinksHere").hide();
        $("#fafArchive").hide();
        $("#fafViewSources").hide();
    } else {
        $("#Toolbar-Tools-Faf").hide();
    }
}

function showFAFTools(showOnlyDocumentSpecificFafToolButtons) {
    if (showOnlyDocumentSpecificFafToolButtons) {
        $("#fafWhatLinksHere").show();
        $("#fafArchive").show();
        $("#fafViewSources").show();
    } else {
        $("#Toolbar-Tools-Faf").show();
    }
}

function loadTemplate(serviceUrl, paramString, templateUrl, containerId, paramsForMsg, callback, nonfilter) {
    if (nonfilter == null)
        nonfilter = false;
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
                applyTemplate(msg, templateUrl, containerId, nonfilter);

                if (callback) {
                    callback(paramsForMsg);
                }
            },
            error: ajaxFailed

        });
    }
    catch (e) {
        ajaxFailed();
    }
}

function applyTemplate(msg, templateUrl, containerId, nonfilter) {
    if ((nonfilter != null) && (nonfilter == true))
        $('#' + containerId).setTemplateURL(templateUrl, [], { filter_data: false }); //http://stackoverflow.com/questions/1721603/jtemplates-html-in-variables
    else $('#' + containerId).setTemplateURL(templateUrl); 
    $('#' + containerId).processTemplate(msg);

    if (containerId == "document-container") {
        $('#iframe-main').hide();
        $('#document-container').show();
        doProcessFeatures();
    }
}

function ajaxFailed(result) {
    if (isLMSLink()) {
        logErrorToServer("LMSLink - "+result.responseText);
        removeUnloadEvent();
        fillContentPaneFromUrl("templates/errordirect.html");
    } else if (result.status == 500 && result.responseText.indexOf("UserNotAuthenticated") != -1) {
        // UserNotAuthenticated Exception
        // redirect to SessionExpired Page
        removeUnloadEvent();
        window.location = "SessionExpired.aspx";
    } else {
        logErrorToServer(result.responseText);
        loadError();
//        alert('failed' + result.status + ' ' + result.statusText);
//        alert('reponse text ' + result.responseText);
    }
    //setLoading(false);
}

function addUnloadEvent() {
    var url = window.location.toString();
    
    if (url.indexOf("lmsdoc") < 0) {
        //window.onbeforeunload = confirmBrowseAway;
    }
}

function removeUnloadEvent() {
    window.onbeforeunload = null;
}

function confirmBrowseAway() {
    return "You are leaving the AICPA Online Professional Library! Click \"Cancel\" to remain on the site and then use the site's navigation to continue your research.";
}


// function: closeScreen
// params: index of the screen to close
// purpose: closes the screen with the given index and updates the UI.
//          if the active screen is closed the next one is loaded
//          if the last screen is closed it redirects to the home page
function closeScreen(index) {
    var originalScreenCount = getMyScreenCount();
    
    removeScreen(index);
    updateBackForScreenClose(index);


    var activeScreenIndex = getActiveScreenIndex();
    var newActiveIndex = activeScreenIndex;
    var refreshContent = false;

    if (activeScreenIndex == index) {
        // we are closing the active screen
        refreshContent = true;

        if (index == originalScreenCount - 1) {
            // we are closing the last screen
            newActiveIndex--;
        }
    }
    else if (activeScreenIndex > index) {
        // we are closing a screen before our active screen
        newActiveIndex--;
    }

    if (newActiveIndex < 0) {
        // we have closed the last screen
        clearActiveScreenIndex();

        // refresh everything and redirect to home page
        loadBreadCrumbForActiveDocument();
        loadMyScreens();
        loadLibraryBooks();
        hideDocumentSpecificButtons();
        loadFafTools();
        loadHomePage();
    }
    else {
        setActiveScreenIndex(newActiveIndex);

        if (refreshContent) {
            if (hasActiveScreen()) {
                var activeScreen = getActiveScreen();
                doScreenLink(newActiveIndex);
            }
        }
        else {
            // just refresh the myScreens list
            loadMyScreens();
        }
    }
}

function loadNotReady() {
    fillContentPaneFromUrl("templates/notReady.html");
}

function loadNewDocumentLanding() {
    clearCurrentView();
    fillContentPaneFromUrl("static/newDocument.html");
}

function loadHelp() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_help, "");

    hideDocumentSpecificButtons();
    loadTemplate("WS/HomePage.asmx/GetHelpVisibility", "{}", "templates/help.html", "document-container");
}

function loadPFP() {
    clearCurrentView();
    fillContentPaneFromUrl("templates/loadpfptoolkit.htm");
}

function loadHelpEthics() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_help, "");

    hideDocumentSpecificButtons();
    loadTemplate("WS/HomePage.asmx/GetHelpVisibility", "{}", "templates/help-ethic.html", "document-container");
}

function loadError() {
    hideDocumentSpecificButtons();
    fillContentPaneFromUrl("templates/error.html");
}

function loadPreferences() {
    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate('WS/UserPreferences.asmx/GetUserPreferences', '{}', 'templates/preferences.html', 'document-container');
}

function loadSavedSearches() {
    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate('WS/SearchServices.asmx/GetSavedSearches', '{}', 'templates/savedSearches.html', 'document-container');
}

function doNotesLink() {
    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate("WS/UserPreferences.asmx/GetAllMyNotes", "{}", "templates/notes.html", "document-container");
}

function doBookmarkLink() {
    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate("WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "templates/mybookmarks.html", "document-container");
}

function loadToc(syncToc, id, type) {
    if (id == undefined)
        id = getTocStateId();
    if (type == undefined)
        type = getTocStateType();
    if (!syncToc) {
        if ((id) && (type)) {
            var params = { id: id, type: type };
            setToolAsCurrentView(toolName_toc, params);
            //set TOC to last state
            setTocStateId(id);
            setTocStateType(type);
        } else {
            setToolAsCurrentView(toolName_toc, null);
        }
    }

    hideDocumentSpecificButtons();

    if ((id) && (type)) {
        if ((id != -1) && (type != "Site"))
            fillDocumentContainerFromUrl("static/plainSyncTocReload.html");
        else fillDocumentContainerFromUrl("static/plainToc.html");
    } else if (hasActiveScreen() && syncToc) {
        fillDocumentContainerFromUrl("static/plainSyncToc.html");
    } else {
        fillDocumentContainerFromUrl("static/plainToc.html");
    }
}

function loadChildren(id, type, dropdownContainer) {
    $("#" + dropdownContainer).slideDown("2000");
}

var highlightName = "destroyer_hilite";
var theTimer = null;
var checkTimerCount = 0;
var timerInterval = 500; //ms
var maxTimerChecks = 6; //currently 3 secs

function scrollToAnchor(anchor) {
    var anchorElement = null;

    if (anchor) {
        if (anchor == highlightName) {
//            var found = $("a[name='" + anchor + "']:first");
            var found = $('#iframe-main').contents().find("a[name='" + anchor + "']:first");

            if (found.length > 0) {
                anchorElement = found[0];
            }
        }
        else {
            //var anchors = document.getElementsByName(anchor);
            var iframe = document.getElementById("iframe-main");
//            var anchors = iframe.document.getElementsByName(anchor);
            var anchors = $('#iframe-main').contents().find("a[name='" + anchor + "']");

            if (anchors.length > 0) {
                anchorElement = anchors[0];
            }
        }
    }

    if (anchorElement) {
        anchorElement.scrollIntoView(true);
    }
    else {
        //$("#content-container").scrollTop(0);
        $("#iframe-main").contents().find("body").scrollTop(0);
    }
}

function scrollToPosition(position) {
    //$("#content-container").scrollTop(position);
    $("#iframe-main").contents().find("body").scrollTop(position);
}

function gotoUpsellPage(id, type, pageType) {
    clearCurrentView();

    var paramsForMsg = { pageType: pageType };
    var params = "{id:" + id + ", type:'" + type + "'}";
    loadTemplate("WS/Content.asmx/GetUpsellData", params, "templates/restricted.html", "document-container", paramsForMsg);
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
        error: errorFunction,
        complete: function (xhr, status) {
            if (status === 'error' || !xhr.responseText) {
                //handleError();
                alert("Error:"+xhr.responseText);
            }   else {
                var data = xhr.responseText;
                //...
            }
        }});
}

function checkTimer() {
    checkTimerCount++;

    if (checkAvailable() || checkTimerCount >= maxTimerChecks)
    {
        doScrollTimerCallback();
    }
}

function checkAvailable() {
    var available = true;

    try {
        if ($("#iframe-main").contents().find('#css-loaded img').height == 0) {
            available = false;
        }
    }
    catch (e) {
        available = false;
    }

    if (available) {
        var cssLoaded = $("#iframe-main").contents().find("#css-loaded");
        //if ($('#css-loaded').height() != 3 || $('#css-loaded').width() != 3) {
        if (cssLoaded.height() != 3 || cssLoaded.width() != 3) {
            available = false;
        }
    }

    return available;
}

// function: loadContentBySiteNode
// params: siteNode - the siteNode to load
//         scrollbarPosition (OPTIONAL) - the scrollbar position to goto, if present this will
//            take precedence over a document anchor / hit highlighting
function loadContentBySiteNode(siteNode, scrollbarPosition) {
    if (siteNode.Restricted && !isLMSLink()) {
        gotoUpsellPage(siteNode.Id, siteNode.Type, g_pageTypes.SCREEN);
        RemoveEmptyScreens();
    } else if (siteNode.Restricted &&isLMSLink()) {
        fillContentPaneFromUrl("templates/errordirect.html");
    }
    else { // in our subscription
        // set siteNode property of active screen to be the new siteNode
        getActiveScreen().siteNode = siteNode;

        //Changes here
        setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());

        callWebService("WS/UserPreferences.asmx/GetBookDataByBookIdDocType", "{id:" + siteNode.Id + ",type:'" + siteNode.Type + "'}", GetScreenData, ajaxFailed);

        if (siteNode.Type == "SiteFolder") {
            scrollToAnchor(); // scroll div to the top
            loadTemplate("WS/Content.asmx/GetSiteFolderDetails", "{id:" + siteNode.Id + "}", "templates/siteFolderTitlePage.html", "document-container");
        }
        else {
            var anchor = null;

            if (siteNode.Anchor) {
                anchor = siteNode.Anchor;
            }
            else if (hasActiveScreen() && getActiveScreen().showHighlight) {
                anchor = highlightName;
            }

            // could turn on load screen here (figure out where to turn off; in both scrollToXXX?)
            setLoading(true);

            if (theTimer) {
                clearInterval(theTimer);
                theTimer = null;
            }

            var hitAnchor = "";
            if (getActiveScreen().hitAnchor != null) {
                hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
                // We need to reset the hitAnchor
                getActiveScreen().hitAnchor = null;
            }

            hideTextOpButtons();
            if (!getActiveScreen().showHighlight)
                hideInnerSearchButtons();

            //var url = "Handlers/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + "&d_hh=" + getActiveScreen().showHighlight;
            var url = "GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getActiveScreen().showHighlight + "&lms=" + isLMSLink();
            

            if ($('#backup-document-container')[0]) {
                $('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks
            }

            $('#backup-document-container').hide();

            $('#document-container').hide();
            //$('#iframe-main').load(url);

            var iframe = document.getElementById("iframe-main");
            //iframe.src = "/static/loadingPage.html";
            if (iframe != null)
                iframe.src = url;

            //sburton: test code
            //$('#iframe-main').attr("src", url);
            //            $('#iframe-main').load(function () {
            //                alert("loaded");
            //            });

            // sburton: Begin section for scrollbar binding
            $('#iframe-main').off("load");
            $('#iframe-main').on("load", function () {
                $('#iframe-main').off("load"); // Only run once

                doProcessFeatures();
                doDocumentReadyMethods();

                if (anchor || scrollbarPosition) {
                    window.doScrollTimerCallback = function () {
                        clearInterval(theTimer);
                        theTimer = null;
                        showHideNextPrevButtons();

                        const $content = $("#iframe-main").contents();
                        if ($content.find("#hitlocation0").length > 0) {
                            // Already handled
                        } else if (scrollbarPosition) {
                            scrollToPosition(scrollbarPosition);
                        } else {
                            scrollToAnchor(anchor);
                        }

                        setLoading(false);
                    };

                    theTimer = setInterval(checkTimer, timerInterval);
                    checkTimerCount = 0;

                    showHideNextPrevButtons();
                    checkTimer();
                } else {
                    scrollToAnchor();
                    setLoading(false);
                }
            });
        // end of load call back function

            // sburton: End section for scrollbar binding


        } // else (Not SiteFolder)

        afterLoadContentBySiteNode(siteNode);
    }  // outer else

    // fade in
    //    $('#document-container').fadeIn('600');
    $('#iframe-main').fadeIn('600');
}

/* Load the targetDoc for the current book */
function GetScreenData(data) {
    var reloadScreen = (getActiveScreen().targetDoc != data.TargetDoc);
    getActiveScreen().targetDoc = data.TargetDoc;
    if (reloadScreen)
        loadMyScreens();
}


/* NEXT BUTTON */
function loadContentBySiteNodeNext(siteNode) {
    loadContentBySiteNodeNextOrPrevious(siteNode, true);
}

/* PREVIOUS BUTTON */
function loadContentBySiteNodePrevious(siteNode) {
    loadContentBySiteNodeNextOrPrevious(siteNode, false);
}

function loadContentBySiteNodeNextOrPrevious(siteNode, isNext) {
    //setLoading(true);    

    // set siteNode property of active screen to be the new siteNode
    getActiveScreen().siteNode = siteNode;
    setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());

    $('#backup-document-container').attr('id', 'document-container-temp');
    $('#document-container').attr('id', 'backup-document-container');
    $('#document-container-temp').attr('id', 'document-container');

    $('#backup-iframe-main').attr('id', 'iframe-main-temp');
    $('#iframe-main').attr('id', 'backup-iframe-main');
    $('#iframe-main-temp').attr('id', 'iframe-main');


    scrollToAnchor(); // scroll container to top first

    if (isNext) {
        //fillDocumentContainerFromUrlNext("Handlers/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources());
        fillDocumentContainerFromUrlNext("GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources());
    }
    else {
        //fillDocumentContainerFromUrlPrevious("Handlers/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources());
        fillDocumentContainerFromUrlPrevious("GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources());
    }

    afterLoadContentBySiteNode(siteNode);
}

//Should be called when the iframe document is "ready".  Created for Ethics
function doDocumentReadyMethods() {
 
}

function afterLoadContentBySiteNode(siteNode) {
    updateArrowsForDocument();
    loadCopyright(siteNode.Id, siteNode.Type);
    loadMyScreens();
    setMyDocumentsTab(true);
    setPrintButton(true);
    setBookmarkButtons(true);    
    loadBreadCrumbForActiveDocument();
    loadFafTools();
    loadFormatOptions();
    doProcessFeatures();
    setEmailPageButton(true);

//    var hash = $.History.setState("#/content?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
//    console.log("hash set as: " + hash);
}

function animateNext() {
    stopRunningAnimations();
    $('#document-container').css({ marginLeft: $('#document-container').width() + 100 });
    $('#document-container').show();
    $('#backup-document-container').animate({ marginLeft: $('#document-container').width() * -1 - 100 }, 1500, "swing", refreshBackupContainer);
    $('#document-container').animate({ marginLeft: 0 }, 1500, "swing");

    setLoading(false);
}

function animateNextIframe() {
    stopRunningAnimations();
    $('#iframe-main').css({ marginLeft: $('#iframe-main').width() + 100 });
    $('#iframe-main').show();
    $('#backup-iframe-main').animate({ marginLeft: $('#iframe-main').width() * -1 - 100 }, 1500, "swing", refreshBackupContainerIframe);
    $('#iframe-main').animate({ marginLeft: 0 }, 1500, "swing");

    setLoading(false);
}

function animatePrevious() {
    stopRunningAnimations();
    $('#document-container').css({ marginLeft: $('#document-container').width() * -1 - 100 });
    $('#document-container').show();
    $('#backup-document-container').animate({ marginLeft: $('#document-container').width() + 100 }, 1500, "swing", refreshBackupContainer);
    $('#document-container').animate({ marginLeft: 0 }, 1500, "swing");

    setLoading(false);
}

function animatePreviousIframe() {
    stopRunningAnimations();
    $('#iframe-main').css({ marginLeft: $('#iframe-main').width() * -1 - 100 });
    $('#iframe-main').show();
    $('#backup-iframe-main').animate({ marginLeft: $('#iframe-main').width() + 100 }, 1500, "swing", refreshBackupContainerIframe);
    $('#iframe-main').animate({ marginLeft: 0 }, 1500, "swing");

    setLoading(false);
}

function stopRunningAnimations() {
    $('#iframe-main').stop(true, true);
    $('#backup-iframe-main').stop(true, true);
    $('#document-container').stop(true, true);
    $('#backup-document-container').stop(true, true);
}

/* BACKUP CONTENT DIV */
function loadBackupContentBySiteNode(siteNode) {

    var hitAnchor = "";
    if (getActiveScreen().hitAnchor != null) {        
        hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
    }
    
    fillBackupContainerFromUrl("Handlers/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getActiveScreen().showHighlight);
}

function refreshBackupContainer() {
    $('#backup-document-container')[0].innerHTML = "";

    $('#backup-document-container').hide();
    $('#backup-document-container').css({ marginLeft: 0 });
}

function refreshBackupContainerIframe() {
    $('#backup-iframe-main').innerHTML = "";

    $('#backup-iframe-main').hide();
    $('#backup-iframe-main').css({ marginLeft: 0 });
}


/* FILL DOCUMENTCONTAINER */
function fillDocumentContainerFromUrlPrevious(url) {
    //$('#document-container').load(url, {}, animatePrevious);

    $('#document-container').hide();
    $('#backup-document-container').hide();
    var iframe = document.getElementById("iframe-main");
    setLoading(true);
    iframe.src = url;
    
    // sburton: I think this jquery way would work too
//    $('#iframe-main').attr("src", url);
//    $('#iframe-main').load(function () {
//        alert("loaded");
//    });
    $('#iframe-main').load(function () {
        $('#iframe-main').off('load');
        setLoading(false);
        doDocumentReadyMethods();
        animatePreviousIframe();
        $('#iframe-main').fadeIn('600');
    });

}

function fillDocumentContainerFromUrlNext(url) {
    //$('#document-container').load(url, {}, animateNext);

    $('#document-container').hide();
    $('#backup-document-container').hide();
    var iframe = document.getElementById("iframe-main");
    setLoading(true);
    iframe.src = url;

    // sburton: I think this jquery way would work too
//    $('#iframe-main').attr("src", url);
//    $('#iframe-main').load(function () {
//        alert("loaded");
//    });


    // sburton: firefox doesn't like this way
    // window.frames["iframe-main"].location = url;
    $('#iframe-main').load(function () {
        $('#iframe-main').off('load');
        setLoading(false);
        doDocumentReadyMethods();
        animateNextIframe();
        $('#iframe-main').fadeIn('600');
    });
}

//Fill the iFrame container with a specific url
function filliFrameFromUrl(url, callback) {
    if (arguments.length <= 1)
        callback = null;
    $('#document-container').hide();
    $('#backup-document-container').hide();
    var iframe = document.getElementById("iframe-main");
    setLoading(true);
    iframe.src = url;

    $('#iframe-main').on('load', function handler() {
        // Unbind this handler after the first execution
        $('#iframe-main').off('load', handler);

        setLoading(false);

        if (callback) {
            callback();
        }

        $('#iframe-main').fadeIn(600); // no quotes around duration
    });

}


function fillDocumentContainerFromUrl(url) {
    $('#iframe-main').hide();
    $('#document-container').show();
    $('#document-container').load(url, function () {
        $('#iframe-main').off('load');
        setLoading(false);
    });
}

function fillBackupContainerFromUrl(url) {
    $('#iframe-main').hide();
    $('#backup-document-container').show();
    $('#backup-document-container').load(url, function () {
        $('#iframe-main').off('load');
        setLoading(false);
    });
}

function fillContentPaneFromUrl(url) {
    $('#iframe-main').hide();
    $('#document-container').show();

    $('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks
    
    $('#backup-document-container').hide();
    $('#document-container').load(url, function () {
        $('#iframe-main').off('load');
        setLoading(false);
    });
}

function fillContentPane(html) {
    $('#iframe-main').hide();
    $('#document-container').show();

    $('#document-container').html(html);
}

//used to unencode endeca snippet, or for anything else that's needed.
function replaceAll(stringToParse, search, replace) {
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

function changeCheckBoxValue(id) {
    var id = id;
    var val = $("#" + id + "[value]").val();
    if (val == 0) {
        $("#" + id + "[value]").val(1);
    }
    else {
        $("#" + id + "[value]").val(0);
    }
}

var timerTime = 500;
var dropdownCloseTimers = new Array(10);

function setDropdownCloseTimer(id) {
    dropdownCloseTimers[id] = window.setTimeout(function () { closeDropdown(id) }, timerTime);
}

function cancelDropdownCloseTimer(id) {
    if (dropdownCloseTimers[id]) {
        window.clearTimeout(dropdownCloseTimers[id]);
        dropdownCloseTimers[id] = null;
    }
}

function closeDropdown(id) {
    $("#drop" + id).slideUp("2000");
}

function closeAllDropdowns() {
    $("#breadCrumbDrops > div").slideUp("2000");
}

function clearMyScreensHitHighlighting() {
    for (var i = 0; i < getMyScreenCount(); i++) {
        getMyScreens()[i].showHighlight = false;
        getMyScreens()[i].hitAnchor = null;
    }
}

//Left and Right functions for javascript.
function Left(str, n) {
    if (n <= 0)
        return "";
    else if (n > String(str).length)
        return str;
    else
        return (String(str).substring(0, n) + "...");
}

function Right(str, n) {
    if (n <= 0)
        return "";
    else if (n > String(str).length)
        return str;
    else {
        var iLen = String(str).length;
        return (String(str).substring(iLen, iLen - n) + "...");
    }
}
function IsNull(aTextField) {
    if (aTextField == null) {
        return '';
    }
    else { return aTextField; }
}

function loadFafToc(pageName) {
    $('.navigation').treeview({
        collapsed: true,
        prerendered: true,
        unique: true
    });

}

function doToc(pageName) {
    $('.navigation').treeview({
        collapsed: true,
        prerendered: true,
        unique: true
    });
}

function loadQuickFind() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_quickFind, "");


    hideDocumentSpecificButtons();
    fillDocumentContainerFromUrl("static/quickFind.html");
}



function loadVideo() {
    clearCurrentView();

    hideDocumentSpecificButtons();
    fillDocumentContainerFromUrl("video.html");
}

function loadWhatsNew() {
    setToolAsCurrentView(toolName_whatsNew, "");

    hideDocumentSpecificButtons();
    loadTemplate('WS/WhatsNew.asmx/GetWhatsNew', "{}", 'templates/whatsNew.html', 'document-container');
}

function loadArchiveContent(id) {
    // hideDocumentSpecificButtons();
    // We still want the print button to show in these Archives, so here are the commands
    // in hideDocumentSpecificButtons() minus hiding the print button
    setArrows(false);
    setFormatOptions(false);
    setCopyright();

    fillDocumentContainerFromUrl("Handlers/GetArchiveContent.ashx?id=" + id);
}

function keepSessionAlive() {
    callWebService("WS/Toolbars.asmx/KeepSessionAlive", "{}", keepSessionAliveCallback, ajaxFailed);
}

function keepSessionAliveCallback() {
    // any code to do after refreshing the session could go here
    // right now we don't have anything that needs to happen
}

$(function () {
    $("#printButton").on('click',function () {
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

    $("#printContent").ajaxComplete(function () {
        $("#printContent div[id$='-large']").css("display", "block");
        $("#printContent div[id$='-small']").css("display", "none");
        $("#printContent img[src='resources/table-minus.gif']").css("display", "none");
        $('#printContent #docNotCurrentHeader').css("top", "36");
    });

    
});

function savePreferences() {
    var params = "{preferenceString:'";

    $("select option:selected").each(function () {
        params += $(this).parent().attr("id") + "*" + $(this).attr("value") + "-";
    });

    params += "'}";

    loadTemplate('WS/UserPreferences.asmx/SaveUserPreferences', params, 'templates/preferences.html', 'document-container');

    setTimeout(function () {
        $("#flashBox").fadeOut();
    }, 3000);

    //alert("Preferences saved.");
}

// sburton: This function probably needs to move to a different file.
// it is used to handle redirection to C2B for an upsell
function sendToCatalog(domain, userGuid, destDoc, destPtr) {
    var fromUrl;
    var toUrl;

    var sourceId;
    var sourceType;

    if (hasActiveDocument()) {
        sourceId = getActiveDocumentId();
        sourceType = getActiveDocumentType();
    }

    var currentUrl = window.location.href;
    var aspxIndex = currentUrl.indexOf(".aspx");

    if (aspxIndex == -1) {
        alert("Current page address '" + currentUrl + "' was unexpected");
    }
    else {
        // base is everything up through (and including) .aspx)
        var baseUrl = currentUrl.substring(0, aspxIndex + 5);
        fromUrl = baseUrl;
        toUrl = baseUrl;

        if (sourceId && sourceType) {
            fromUrl = fromUrl + "?id=" + sourceId + "&type=" + sourceType;
        }

        if (destDoc && destPtr) {
            toUrl = toUrl + "?targetdoc=" + destDoc + "&targetptr=" + destPtr;
        }
    }

    fromUrl = encodeURIComponent(fromUrl).replace(/\./g, "%2E");
    toUrl = encodeURIComponent(toUrl).replace(/\./g, "%2E");
    if (typeof (window.opener) != "undefined" && window.opener.closed == false) {
        var url = window.location.protocol + "//" + "stagingenv.cpa2biz.com/myaccount/myonlinesubscription_resourceredirect.jsp?Domain=" + domain + "&returnurl=" + fromUrl + "&linkurl=" + toUrl;
        logout();
        window.opener.location = url;
        window.opener.focus();
        windows.close;
    }
    else {
        // the original c2b window was not found or is not still open
        var url = "CatalogRedirect.aspx?Domain=" + domain + "&Guid=" + userGuid + "&returnurl=" + fromUrl + "&linkurl=" + toUrl;
        //alert(url);
        logout();
        window.open(url);
        windows.close;        
    }

    logout();
}

function logout() {
    callWebService("WS/Toolbars.asmx/Logout", "{}", redirectToLogoutPage, ajaxFailed);
}

function logoutButton() {
    callWebService("WS/Toolbars.asmx/Logout", "{}", redirectToLogoutButton, ajaxFailed);
}

function redirectToLogoutButton() {
    removeUnloadEvent();
    document.location = ("Logout.aspx");
}


function redirectToLogoutPage() {
    document.location = ("Logout.aspx");
    
}

function logErrorToServer(logString) {
    try {
        callWebService("WS/Toolbars.asmx/LogError", "{logString: '" + logString + "'}", logErrorCallback, logErrorCallback);
    }
    catch (ex)
    { }
}

function logErrorCallback() {
    // we do nothing after successful or unsuccesful logging
}

// sburton 2010-11-06: We will probably want this, but lets wait until we really need it to put it in
//function setMainIframeContent(html) {
//    $("#iframe-main").html(html);
//}

function loadMainIframeContentByUrl(url) {
    $("#iframe-main").load(url).show();
    $("#document-container").hide();
}

function getCurrentCopyrightYear() {
    var date = new Date();
    var currentYear = date.getFullYear();

    return currentYear;
}

function isToolbarHidden() {
    return $("#toolbarToggleButton").hasClass("activex");
}

function isLMSLink() {
    if (window.location.href.toLowerCase().indexOf(LMS_DOC_PAGE) > 0)
        return true;
    else return false;
}

function LMSTargetDoc() {
    if (isLMSLink()) {
        return parameters["targetdoc"];
    } else return "";
}

function LMSTargetPtr() {
    if (isLMSLink()) {
        return parameters["targetptr"];
    } else return "";
}

function loadLibraryBooks() {
    loadTemplate('WS/Content.asmx/GetLibraryBooks', '{}', 'templates/LibraryBooks.html', 'Toolbar-Library');
}