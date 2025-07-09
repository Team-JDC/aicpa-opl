



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
    console.log("afterLoadContent");
}


/**
* Sets the Visibility of the loading div
* 
* @param visible: true if the loading div should be shown, false to hide
*/
function setLoading(visible) {
//    if (visible) {
//        $("#loading-div").fadeIn();
//    } else {
//        $("#loading-div").fadeOut();
//    }
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

    $//('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks

    //$('#backup-document-container').hide();
    $('#document-container-left').load(url, function () {
        $('#document-container-left').unbind('load');
        $('#document-container-left').show();
        setLoading(false);
    });
}

// function: loadContentBySiteNode
// params: siteNode - the siteNode to load
//         scrollbarPosition (OPTIONAL) - the scrollbar position to goto, if present this will
//            take precedence over a document anchor / hit highlighting
function loadContentBySiteNode(siteNode, scrollbarPosition) {
    if (siteNode.Restricted) {
        gotoUpsellPage(siteNode.Id, siteNode.Type, g_pageTypes.SCREEN);
        //RemoveEmptyScreens();
    } else if (siteNode.Restricted && isLMSLink()) {
        fillContentPaneFromUrl("templates/errordirect.html");
    }
    else { // in our subscription
        // set siteNode property of active screen to be the new siteNode
        //getActiveScreen().siteNode = siteNode;

        //Changes here
        //setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());

        //callWebService("WS/UserPreferences.asmx/GetBookDataByBookIdDocType", "{id:" + siteNode.Id + ",type:'" + siteNode.Type + "'}", GetScreenData, ajaxFailed);

        if (siteNode.Type == "SiteFolder") {
            //scrollToAnchor(); // scroll div to the top
            loadTemplate("/WS/Content.asmx/GetSiteFolderDetails", "{id:" + siteNode.Id + "}", "/templates/siteFolderTitlePage2.html", "document-container-left");
        }
        else {
            var anchor = null;
            /*dw
            if (siteNode.Anchor) {
            anchor = siteNode.Anchor;
            }
            else if (hasActiveScreen() && getActiveScreen().showHighlight) {
            anchor = highlightName;
            }
            */
            // could turn on load screen here (figure out where to turn off; in both scrollToXXX?)
            setLoading(true);
            /*
            if (theTimer) {
            clearInterval(theTimer);
            theTimer = null;
            }
            */
            var hitAnchor = "";
            /*
            if (getActiveScreen().hitAnchor != null) {
            hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
            // We need to reset the hitAnchor
            getActiveScreen().hitAnchor = null;
            }
            */
            //hideTextOpButtons();
            //if (!getActiveScreen().showHighlight)
            //    hideInnerSearchButtons();

            
            //var url = "GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getActiveScreen().showHighlight + "&lms=" + isLMSLink();
            var url = "/GetDocument.ashx?id=" + siteNode.Id + "&type=" + siteNode.Type + "&show_sources=" + getShowSources() + hitAnchor + "&d_hh=false&lms=false";


            if ($('#backup-document-container')[0]) {
                $('#backup-document-container')[0].innerHTML = ""; // most effecient...potential for jQuery memory leaks
            }

            //DW$('#backup-document-container').hide();

            //$('#document-container-left').hide();


            //var iframe = document.getElementById("iframe-main");

            //if (iframe != null)
            ///iframe.src = url;
            fillLeftContentPaneFromUrl(url);
        }
        afterLoadContentBySiteNode(siteNode);
    }  // outer else

    // fade in
    //    $('#document-container').fadeIn('600');
}

function loadDocumentWidgetByIdType(id, type) {
    var params = "{ id:'" + id + "', type:'" + type + "'}";
    enableBookmarkButtonsByIdType(id, type);
    loadTemplate('/WS/Content.asmx/ResolveContentLinkExtraByIdType', params, '/templates/next_prev_widget.html', 'document-container-right');
}


function loadDocumentWidget(targetDoc, targetPtr) {
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    enableBookmarkButtons(targetDoc, targetPtr);
    loadTemplate('/WS/Content.asmx/ResolveContentLinkExtra', params, '/templates/next_prev_widget.html', 'document-container-right'); 
}

function loadHistory() {
    loadTemplate('/WS/HomePage.asmx/GetRecentDocuments', '{}', '/templates/history.html', 'liHistory');
}

function loadWhatsNew() {    
    loadTemplate('WS/WhatsNew.asmx/GetWhatsNew', "{}", 'templates/whatsNew.html', 'document-container-right');
}

function loadMyLibrary() {
    loadTemplate('/WS/Content.asmx/GetLibraryBooks', '{}', '/templates/mylibrary.html', 'liLibrary');
}

function loadHistory() {
    loadTemplate('/WS/HomePage.asmx/GetRecentDocuments', '{}', '/templates/historyPhone.html', 'liHistoryPhone');
}

function loadMyLibrary() {
    loadTemplate('/WS/Content.asmx/GetLibraryBooks', '{}', '/templates/mylibraryPhone.html', 'liLibraryPhone');
}

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
        //setLoading(false);
    });
}



function doNotesLink() {
    window.location = "/tools/notes";
}


function loadNotes() {
    //clearCurrentView();

    //hideDocumentSpecificButtons();
    loadTemplate("/WS/UserPreferences.asmx/GetAllMyNotes", "{}", "/templates/odp2015/notes.html", "document-container-left");
}


function loadBookmarks() {
    loadTemplate("/WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "/templates/odp2015/bookmarks.html", "document-container-left");
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
    } else {
        $("#Toolbar-Tools-AddBookmark-Widget").hide();
    }
}

function setDeleteBookmarkButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-DeleteBookmark-Widget").show();
    } else {
        $("#Toolbar-Tools-DeleteBookmark-Widget").hide();
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
        error: errorFunction,
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
    if ((nonfilter != null) && (nonfilter == true)) {
        $('#' + containerId).setTemplateURL(templateUrl, [], { filter_data: false }); //http://stackoverflow.com/questions/1721603/jtemplates-html-in-variables
    }
    else {
        $('#' + containerId).setTemplateURL(templateUrl);
    }
    $('#' + containerId).processTemplate(msg);

//    if (containerId == "document-container") {
//        $('#iframe-main').hide();
//        $('#document-container').show();
//        doProcessFeatures();
//    }
}


function ajaxFailed(result) {
    if (result.status == 500 && result.responseText.indexOf("UserNotAuthenticated") != -1) {
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


function logErrorToServer(logString) {
    try {
        callWebService("/WS/Toolbars.asmx/LogError", "{logString: '" + logString + "'}", logErrorCallback, logErrorCallback);
    }
    catch (ex)
    { }
}

function loadError() {
//    hideDocumentSpecificButtons();
    fillContentPaneFromUrl("templates/error.html");
}
