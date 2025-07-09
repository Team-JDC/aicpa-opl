function loadBreadCrumb(id, type) {
    loadTemplate("WS/Content.asmx/GetFullBreadcrumb", "{id:" + id + ",type:'" + type + "'}", 'templates/cosofullBreadcrumb.html', 'breadcrumbContainer');
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
            fillDocumentContainerFromUrl("static/cosoPlainSyncTocReload.html");
        else fillDocumentContainerFromUrl("static/cosoPlainToc.html");
    } else if (hasActiveScreen() && syncToc) {
        fillDocumentContainerFromUrl("static/cosoPlainSyncToc.html");
    } else {
        fillDocumentContainerFromUrl("static/cosoPlainToc.html");
    }
}

// Public global variables
(function ($) {
jQuery.fn.jBreadCrumb.defaults =
    {
        maxFinalElementLength: 400,
        minFinalElementLength: 200,
        minimumCompressionElements: 4,
        endElementsToLeaveOpen: 0,
        beginingElementsToLeaveOpen: 0,
        timeExpansionAnimation: 800,
        timeCompressionAnimation: 1500,
        timeInitialCollapse: 500,
        overlayClass: 'chevronOverlay',
        previewWidth: 50
    };
})(jQuery);

function loadMyScreens() {
    var screens = getMyScreens();

    var activeIndex = -1;
    if (hasActiveScreen()) {
        activeIndex = getActiveScreenIndex();
    }

    var message = { d: screens, activeIndex: activeIndex };

    applyTemplate(message, "templates/cosoDocuments.html", "Toolbar-MyDocuments");    
    $("#documentcount").html(getMyScreenCount());
}

function setMyDocumentsTab(visible) {
    if (visible) {
        if (isToolbarHidden()) {
            doToolbarToggle();
        }

        $(".tabs li").removeClass("active"); //Remove any "active" class
        //$("#Tab-MyDocuments").addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        //$("#Toolbar-MyDocuments").fadeIn(); //Fade in the active ID content
    } else {
        // currently there is no reason to implement this side of things
    }
}
//this is probably obsolete and needs to be deleted.
function doNextGenerationLink(guid, domain, referringSite) {
    attngWindow = window.open("/coso/ResourceSeamlessLogin.aspx?hidEncPersGUID=" + guid + "&hidSourceSiteCode=" + referringSite + "&hidDomain=" + domain + "&hidURL=coso    .aspx&prod=coso", "coso");
    attngWindow.focus();
}

function loadHomePage() {
    setToolAsCurrentView(toolName_homePage, "");
    // Parameters for loadTemplate...
    //           serviceUrl, paramString, templateUrl, containerId, paramsForMsg, callback, nonfilter
    loadTemplate("WS/HomePage.asmx/GetHomePageData", "{}", "static/cosoPlainToc.html", "document-container");
}

//No search button autosubmit when the enter button is clicked
function doSearchCheck(keyInfo) {
    if (keyInfo == 13) {
        //document.getElementById('search').focus();
        //document.getElementById('search').click();
        doAdvancedNavigationalSearch('', document.getElementById('searchTerms').value, 1, 100, 10, 0, 1, 1, 0,
        function () {
            $('#searchResultButtonLink').show();
        });
    }
}

/*****************************************************
    This is a prototype for the doscscreen that is used
    in the cosoDocuments template.  It will limit truncate
    a string and add a '...' if the len is provided

    input: len (optional) - length to limit the string to
    output: (string) - original string that is truncated
 *****************************************************/
docscreen.prototype.titleLimit = function (len) {
    var result = "";
    if (this.targetDoc == "") {
        result = this.siteNode.Title;
    } else {
        result = this.targetDoc + " - " + this.siteNode.Title;
    }

    
    if (len) {
        if (result.length > len) {
            result = result.substr(0, len - 1) + '&hellip;';
        }        
    }
    return result;
};

function saveBookmarkCoso() {
    if (hasActiveDocument()) {
        var paramsString = "{id:'" + getActiveDocumentId() + "', type:'" + getActiveDocumentType() + "', bookmarkTitle:'" + $('#addBookmarkTitleInput').val() + "'}";
        callWebService("WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", paramsString, addPageBookmarkResultHandler, ajaxFailed);
        closeAddBookmark();
    } else {
        alert('Bookmarks are not allowed on this document');
    }
}

function loadHelp() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_help, "");
    hideDocumentSpecificButtons();
    loadTemplate("WS/HomePage.asmx/GetHelpVisibility", "{}", "templates/COSOhelp.html", "document-container");
}



