/*******************************************************************************
file: globalVariables.js
This file defines any global variables necessary to maintain state information on the client.
*******************************************************************************/

var SHOWSOURCES_COOKIENAME = "showsources";
var SHOWHIGHLIGHTS_COOKIENAME = "showhighlights";
var ACTIVEDOCUMENT_COOKIENAME = "activedocument";
var LMS_DOC_PAGE = "lmsdoc.aspx";
//used to disable the "show codifications references"
var disableShowCodReferences = false;

var g_tocNodeChildLimit = 30;
var g_activeScreenIndex = -1;
var g_myScreens = null;
var g_showSources = false;
var g_NodeTypeArray = ['site', 'book', 'document', 'format', 'bookfolder', 'documentanchor', 'namedanchor', 'sitefolder'];

var g_pageTypes = {
    HOME_PAGE: "home_page",
    SCREEN: "screen",
    SEARCH_RESULTS: "search_results"
}


function getCurrentScrollbarPosition() {
//    var position = $("#document-container-left").contents().find("body").scrollTop();

        var documentContainerPosition = $("#content-container-left").scrollTop();
    //    //var contentInnerPosition = $("#content-inner").scrollTop();

    //    //var position = (documentContainerPosition > contentInnerPosition) ? documentContainerPosition : contentInnerPosition;
        var position = documentContainerPosition;

    return position;
}

// OLD
// var value = $.cookie(SHOWSOURCES_COOKIENAME);
// $.cookie(SHOWSOURCES_COOKIENAME, show);

// ✅ NEW
function getShowSources() {
    return Cookies.get(SHOWSOURCES_COOKIENAME) === "true";
}

function setShowSources(show) {
    Cookies.set(SHOWSOURCES_COOKIENAME, show, { expires: 30 });
}

function getShowHighlights() {
    return Cookies.get(SHOWHIGHLIGHTS_COOKIENAME) === "true";
}

function setShowHighlights(show) {
    Cookies.set(SHOWHIGHLIGHTS_COOKIENAME, show, { expires: 30 });
}

function setActiveDocument(sitenode) {
    Cookies.set(ACTIVEDOCUMENT_COOKIENAME, JSON.stringify(sitenode), { expires: 30 });
}

function hasActiveDocument() {
    let siteNode = Cookies.get(ACTIVEDOCUMENT_COOKIENAME);
    try {
        siteNode = siteNode ? JSON.parse(siteNode) : null;
    } catch (e) {
        return false;
    }
    return siteNode && siteNode.Id !== -1;
}

function getActiveDocumentId() {
    try {
        const siteNode = JSON.parse(Cookies.get(ACTIVEDOCUMENT_COOKIENAME) || '{}');
        return siteNode.Id || -1;
    } catch {
        return -1;
    }
}

function getActiveDocumentType() {
    try {
        const siteNode = JSON.parse(Cookies.get(ACTIVEDOCUMENT_COOKIENAME) || '{}');
        return siteNode.Type || -1;
    } catch {
        return -1;
    }
}

