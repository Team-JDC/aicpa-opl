// globalVariables.js (Refactored with Modern Syntax and Comments)

/**
 * Global state and helper utilities for tracking active screens, documents, and TOC state.
 */

const LMS_DOC_PAGE = "lmsdoc.aspx";
let disableShowCodReferences = false;

const g_tocNodeChildLimit = 30;
let g_activeScreenIndex = -1;
let g_myScreens = [];
let g_showSources = false;

const g_pageTypes = {
    HOME_PAGE: "home_page",
    SCREEN: "screen",
    SEARCH_RESULTS: "search_results"
};

// --- Screens Management ---
function getMyScreens() {
    return g_myScreens;
}

function setMyScreens(screenCollection) {
    g_myScreens = screenCollection;
}

function getMyScreenCount() {
    return g_myScreens.length;
}

function addNewScreen(screen) {
    g_myScreens.push(screen);
    return g_myScreens.length - 1;
}

function removeScreen(index) {
    g_myScreens.splice(index, 1);
}

function RemoveEmptyScreens() {
    g_myScreens = g_myScreens.filter(screen => screen.siteNode);
}

// --- Active Screen Handling ---
function getActiveScreenIndex() {
    return g_activeScreenIndex;
}

function setActiveScreenIndex(newIndex) {
    if (newIndex < 0 || newIndex >= getMyScreenCount()) {
        throw new Error("Invalid screen index");
    }
    g_activeScreenIndex = newIndex;
}

function clearActiveScreenIndex() {
    g_activeScreenIndex = -1;
}

function hasActiveScreen() {
    return g_activeScreenIndex !== -1;
}

function getActiveScreen() {
    if (!hasActiveScreen()) throw new Error("No active screen.");
    return g_myScreens[g_activeScreenIndex];
}

// --- Active Document Helpers ---
function hasActiveDocument() {
    try {
        const screen = getActiveScreen();
        return screen?.siteNode?.Id != null;
    } catch {
        return false;
    }
}

function getActiveDocumentId() {
    if (!hasActiveDocument()) throw new Error("No active document.");
    return getActiveScreen().siteNode.Id;
}

function getActiveDocumentType() {
    if (!hasActiveDocument()) throw new Error("No active document.");
    return getActiveScreen().siteNode.Type;
}

function getActiveDocumentVCT() {
    if (!hasActiveDocument()) throw new Error("No active document.");
    return getActiveScreen().viewCompleteTopic;
}

function setActiveDocumentId() {
    console.warn("setActiveDocumentId is deprecated.");
}

function setActiveDocumentType() {
    console.warn("setActiveDocumentType is deprecated.");
}

// --- Scrollbar Position ---
function getCurrentScrollbarPosition() {
    return $("#iframe-main").contents().find("body").scrollTop();
}

// --- docscreen Object Constructor ---
function docscreen() {
    this.scrollbarPosition = null;
    this.showHighlight = false;
    this.siteNode = null;
    this.targetDoc = "";
    this.hitAnchor = null;
    this.viewCompleteTopic = false;

    this.clone = () => {
        const newScreen = new docscreen();
        newScreen.siteNode = this.siteNode;
        newScreen.showHighlight = this.showHighlight;
        newScreen.hitAnchor = this.hitAnchor;
        newScreen.scrollbarPosition = this.scrollbarPosition;
        newScreen.viewCompleteTopic = this.viewCompleteTopic;
        return newScreen;
    };

    this.recordScrollbarPosition = () => {
        this.scrollbarPosition = getCurrentScrollbarPosition();
    };
}

// --- Source Toggle State ---
function getShowSources() {
    return g_showSources;
}

function setShowSources(show) {
    g_showSources = show;
}

// --- TOC State ---
let g_tocStateId = -1;
let g_tocStateType = "Site";

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
