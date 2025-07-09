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
var g_pageTypes = {
    HOME_PAGE: "home_page",
    SCREEN: "screen",
    SEARCH_RESULTS: "search_results"
}

function getMyScreens() {
    if (g_myScreens == null) {
        g_myScreens = new Array();
    }

    return g_myScreens;
}

function RemoveEmptyScreens() {
    for (var i = 0, len = g_myScreens.length; i < len; ++i) {
        if (!g_myScreens[i].siteNode) {
            g_myScreens.splice(i, 1);
            //delete (g_myScreens[i]);
            len--;
            i--;
        }
    }
}

function setMyScreens(screenCollection) {
    g_myScreens = screenCollection;
}

function getActiveDocumentId() {
    if (!hasActiveDocument()) {
        throw "No active document.";
    }
    return getActiveScreen().siteNode.Id;
}

function setActiveDocumentId(docId) {
    alert("setActiveDocumentId is deprecated.");
}

function hasActiveDocument() {
    var hasDoc = false;

    if (hasActiveScreen()) {
        var screen = getActiveScreen();

        if (screen.siteNode != null && screen.siteNode.Id != null) {
            hasDoc = true;
        }
    }

    return hasDoc;
}

function getActiveDocumentVCT() {
    if (!hasActiveDocument()) {
        throw "No active document.";
    }

    return getActiveScreen().viewCompleteTopic;
}

function getActiveDocumentType() {
    if (!hasActiveDocument()) {
        throw "No active document.";
    }

    return getActiveScreen().siteNode.Type;
}

function setActiveDocumentType(type) {
    alert("setActiveDocumentType is deprecated");
}

// Gets the 0 (ZERO!) based index of the active screen
function getActiveScreenIndex() {
    return g_activeScreenIndex;
}

// Sets the 0 (ZERO!) based index of the active screen
function setActiveScreenIndex(newIndex) {
    if (newIndex < 0 || newIndex > getMyScreenCount() - 1) {
        throw "new screen index is not valid";
    }

    g_activeScreenIndex = newIndex;
}

// resets the activeScreenIndex to -1 -- so it doesn't have one
function clearActiveScreenIndex() {
    g_activeScreenIndex = -1;
}

function hasActiveScreen() {
    return !(getActiveScreenIndex() == -1);
}

function getActiveScreen() {
    if (!hasActiveScreen()) {
        throw "No active screen.";
    }

    return getMyScreens()[getActiveScreenIndex()];
}

// adds a new screen to the end of the myScreens collection
// does NOT automatically make this the active screen
// returns the index of the newly added screen
function addNewScreen(screen) {
    var newIndex = getMyScreenCount();
    getMyScreens()[newIndex] = screen;
    return newIndex;
}

// removes screen at the given index and moves all others up in the list.
// if the screen to be removed is the active screen, the active screen will be set null
function removeScreen(index) {
    getMyScreens().splice(index, 1);
}

function getMyScreenCount() {
    return getMyScreens().length;
}

function docscreen() {
    this.scrollbarPosition = null;
    this.showHighlight = false;
    this.siteNode = null;
    this.targetDoc = "";
    this.hitAnchor = null;
    this.viewCompleteTopic = false; //    

    this.clone = (function () {
        var newScreen = new docscreen();
        newScreen.siteNode = this.siteNode;
        newScreen.showHighlight = this.showHighlight;
        newScreen.hitAnchor = this.hitAnchor;
        newScreen.scrollbarPosition = this.scrollbarPosition;
        newScreen.viewCompleteTopic = this.viewCompleteTopic;        

        return newScreen;
    });

    this.recordScrollbarPosition = (function () {
        this.scrollbarPosition = getCurrentScrollbarPosition();
    });
}

function getCurrentScrollbarPosition() {
    var position = $("#iframe-main").contents().find("body").scrollTop();

    //    var documentContainerPosition = $("#content-container").scrollTop();
    //    //var contentInnerPosition = $("#content-inner").scrollTop();

    //    //var position = (documentContainerPosition > contentInnerPosition) ? documentContainerPosition : contentInnerPosition;
    //    var position = documentContainerPosition;

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