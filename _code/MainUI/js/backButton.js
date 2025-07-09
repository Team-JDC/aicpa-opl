// 2010-08-04 sburton: The following deal with the new approach to the back button
// It is a work in progress at this point

function view() {
    this.isTool = false;
    this.isScreen = false;
    this.renderMe = (function () {
        alert("Render function should be overloaded in sub class");
    });
}

// TODO: turn this into an enum
var toolName_none = "none";
var toolName_quickFind = "quickFind";
var toolName_blankSearch = "blankSearch";
var toolName_whatsNew = "whatsNew";
var toolName_joinSections = "joinSections";
var toolName_ethicsJoinSections = "ethicsJoinSections";
var toolName_gotoCode = "gotoCode";
var toolName_crossRef = "crossRef";
var toolName_help = "help";
var toolName_userPrefs = "userPrefs";
var toolName_homePage = "homePage";
var toolName_toc = "toc";
var toolName_joinChildren = "joinChildren";

function toolState() {
    this.isTool = true;
    this.isScreen = false;
    this.toolName = toolName_none;
    this.toolParams = "";

    this.renderMe = (function () {
        renderTool(this.toolName, this.toolParams);
    });
}

function screenState() {
    this.isTool = false;
    this.isScreen = true;
    this.screenObj = null;
    this.screenIndex = -1;

    this.renderMe = (function () {
        renderScreen(this.screenObj, this.screenIndex);
    });
}

function renderTool(toolName, toolParams) {
    // right now we are ignoring parameters, but we are putting it in place for later
    switch (toolName) {
        case toolName_quickFind:
            loadQuickFind();
            break;

        case toolName_whatsNew:
            loadWhatsNew();
            break;

        case toolName_homePage:
            loadHomePage();
            break;

        case toolName_toc:
            if (toolParams)
                loadToc(false, toolParams.id, toolParams.type);
            else loadToc(false);
            break;

        case toolName_help:
            loadHelp();
            break;

        case toolName_gotoCode:
            doFafGotoLink();
            break;

        case toolName_crossRef:
            if (toolParams) {
                loadCrossReferenceFromValues(toolParams);
            }
            else {
                loadCrossReference();
            }
            break;

        case toolName_joinSections:
            if (toolParams) {
                doJoinSectionsFromValues(toolParams);
            }
            else {
                doFafJoinSectionsLink();
            }
            break;

        case toolName_ethicsJoinSections:
            if (toolParams) {
                ethicsJoinSectionsParams(toolParams);
            }
            break;

        case toolName_joinChildren:
            if (toolParams) {
                doJoinChildren(toolParams.targetDoc, toolParams.targetPtr);
            }
            else {
                // do nothing...we must have parameters
            }
            break;

        case toolName_blankSearch:
            loadBlankSearch();
            break;

        case toolName_none:
        default:
            // do nothing
            break;
    }
}

function renderScreen(screenObj, screenIndex) {
    setActiveScreenIndex(screenIndex);
    getActiveScreen().viewCompleteTopic = screenObj.viewCompleteTopic;
    if (screenObj.viewCompleteTopic) {        
        ethicsJoinSections(screenObj.siteNode);
    } else {
        loadContentBySiteNode(screenObj.siteNode, screenObj.scrollbarPosition);
    }
}

function backStack() {
    this.internalStack = Array();
    this.pushView = (function (view) {
        this.internalStack.push(view);
    });

    this.popView = (function () {
        return this.internalStack.pop();
    });

    this.topView = (function () {
        var count = this.internalStack.length;

        var returnView = null;
        if (count > 0) {
            returnView = this.internalStack[count - 1];
        }

        return returnView;
    });

    this.count = (function () {
        return this.internalStack.length;
    });

    this.handleScreenDelete = (function (deletedIndex) {
        for (var i = this.internalStack.length - 1; i >= 0; i--) {
            // iterate backwards through the list so we can remove an item when we encounter it if necessary

            var view = this.internalStack[i];

            if (view.isScreen) {
                if (view.screenIndex == deletedIndex) {
                    // delete this from the history
                    this.internalStack.splice(i, 1);
                }
                else if (view.screenIndex > deletedIndex) {
                    // decrement this index
                    view.screenIndex--;
                }
            }
        }

        // also update current view if necessary
        view = g_currentView;
        if (view && view.isScreen) {
            if (view.screenIndex == deletedIndex) {
                // delete this from the history
                g_currentView = null;
            }
            else if (view.screenIndex > deletedIndex) {
                // decrement this index
                view.screenIndex--;
            }
        }
    });
}

var g_backStack = new backStack();
var g_currentView = null;

function doBack() {
    if (g_backStack.count() > 0) {

        g_currentView = null;
        var v = g_backStack.popView();

        v.renderMe();
    }
    else {
        // for now if we don't have anywhere to go, let's just go back to the homepage
        // later we may want to gray out the button so they don't get here
        // but either way this is a good fallback
        loadHomePage();
    }
}

function setToolAsCurrentView(toolName, toolParams) {
    recordCurrentViewForBack();

    var tool = new toolState();
    tool.toolName = toolName;
    tool.toolParams = toolParams;

    g_currentView = tool;
}

function setScreenAsCurrentView(screenObj, screenIndex) {
    recordCurrentViewForBack();

    var state = new screenState();
    //state.siteNode = screenObj.siteNode;
    state.screenObj = screenObj.clone();
    state.screenIndex = screenIndex;

    g_currentView = state;
}

function clearCurrentView() {
    recordCurrentViewForBack();
    g_currentView = null;
}

// function: clearCurrentViewWithoutRecording
// description: this simply clears out the current view.  This should hardly ever be called
//   in most cases, we should call clearCurrentView().  The reason for this method would be if you
//   are "refreshing" the current page and don't want to enter something into the back stack
//   such as with view sources.
function clearCurrentViewWithoutRecording() {
   g_currentView = null;
}

function recordCurrentViewForBack() {
    updateCurrentView();

    if (g_currentView) {
        g_backStack.pushView(g_currentView);
    }
}

// function: updateCurrentView
// description: this function merely updates the current view (ie records the scrollbars) it does not otherwise affect
//   the state of the back button / back stack.
function updateCurrentView() {
    if (g_currentView) {
        if (g_currentView.isScreen) {
            updateCurrentScreen();
        }
    }
}

function updateCurrentScreen() {
    g_currentView.screenObj.recordScrollbarPosition();    
    // we also want to update the screen object in the myScreens list
    var currentIndex = getActiveScreenIndex();
    if (currentIndex >= 0 && currentIndex < getMyScreenCount()) {
        getMyScreens()[currentIndex].recordScrollbarPosition();    
    }
}

function updateCurrentTool_crossRef(standard, number, topic, subtopic, section) {
    var params =
    { standard: standard,
        number: number,
        topic: topic,
        subtopic: subtopic,
        section: section
    };

    g_currentView.toolParams = params;
}

function updateCurrentTool_joinSections(topicNum, sectionNum, content, includeSubtopics) {
    var params =
    { topicNum: topicNum,
        sectionNum: sectionNum,
        content: content,
        includeSubtopics: includeSubtopics
    };

    g_currentView.toolParams = params;
}

function updateBackForScreenClose(deletedIndex) {
    g_backStack.handleScreenDelete(deletedIndex);
}

function isCurrentViewScreen() {
    return g_currentView != null && g_currentView.isScreen;
}

//*******************************************************************
//Name: updateTocView
//Purpose: Store the current id/type that was just clicked on in the
//  g_currentView
//Input: id   - Document ID
//       type - Document Type
//Output: None
//*******************************************************************
function updateTocView(id, type) {
    if (g_currentView) {
        if (g_currentView.toolName == toolName_toc) {
            var params = { id: id, type: type };
            g_currentView.toolParams = params;
        }
    }
}

