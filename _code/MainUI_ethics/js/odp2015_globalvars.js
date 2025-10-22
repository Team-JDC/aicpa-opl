/*******************************************************************************
file: globalVariables.js
This file defines any global variables necessary to maintain state information on the client.
*******************************************************************************/

var LMS_DOC_PAGE = "lmsdoc.aspx";
//used to disable the "show codifications references"
var disableShowCodReferences = false;

var g_tocNodeChildLimit = 30;
var g_activeScreenIndex = -1;
var g_myScreens = null;
var g_showSources = false;


function getCurrentScrollbarPosition() {
//    var position = $("#document-container-left").contents().find("body").scrollTop();

        var documentContainerPosition = $("#content-container-left").scrollTop();
    //    //var contentInnerPosition = $("#content-inner").scrollTop();

    //    //var position = (documentContainerPosition > contentInnerPosition) ? documentContainerPosition : contentInnerPosition;
        var position = documentContainerPosition;

    return position;
}


function getShowSources() {
    return g_showSources;
}

function setShowSources(show) {
    g_showSources = show;
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

