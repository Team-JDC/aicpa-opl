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

function getShowSources() {
    var value = $.cookie(SHOWSOURCES_COOKIENAME);
    return (value == null ? false : value == "true");
}

function setShowSources(show) {
    $.cookie(SHOWSOURCES_COOKIENAME, show);    
}

function getShowHighlights() {
    var value = $.cookie(SHOWHIGHLIGHTS_COOKIENAME);
    return (value == null ? false : value == "true");
}

function setShowHighlights(show) {
    $.cookie(SHOWHIGHLIGHTS_COOKIENAME, show);
}
function setActiveDocument(sitenode) {
    $.cookie(ACTIVEDOCUMENT_COOKIENAME, sitenode);
}

function hasActiveDocument() {
    var hasDoc = false;
    var siteNode = $.cookie(ACTIVEDOCUMENT_COOKIENAME);
    if (siteNode && siteNode.Id != -1) {
        hasDoc = true;
    }

    return hasDoc;
}

// Gets the 0 (ZERO!) based index of the active screen
function getActiveScreenIndex() {
//    console.log(g_activeScreenIndex);
//    return g_activeScreenIndex;
}

// Sets the 0 (ZERO!) based index of the active screen
function setActiveScreenIndex(newIndex) {
//    if (newIndex < 0 || newIndex > getMyScreenCount() - 1) {
//        throw "new screen index is not valid";
//    }

//    g_activeScreenIndex = newIndex;
}

// resets the activeScreenIndex to -1 -- so it doesn't have one
function clearActiveScreenIndex() {
    g_activeScreenIndex = -1;
}

function getActiveDocumentId() {

    var siteNode = $.cookie(ACTIVEDOCUMENT_COOKIENAME);
    if (siteNode) {
        return siteNode.Id;
    } else { return -1 }
}

function getActiveDocumentType() {
    var siteNode = $.cookie(ACTIVEDOCUMENT_COOKIENAME);
    if (siteNode) {
        return siteNode.Type;
    } else {
        return -1 
    }
}

function setActiveDocumentType(type) {
    alert("setActiveDocumentType is deprecated");
}



//=========================================
// TOC STATE
//=========================================

var g_tocStateId = -1;
var g_tocStateType = "Site";

function setTocStateId(id) {
    g_tocStateId = id;
}

function getTocStateId() {
    return g_tocStateId;
}

function setTocStateType(type) {
    g_tocStateType = type;
}

function getTocStateType() {
    return g_tocStateType;
}

