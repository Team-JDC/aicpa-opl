/**
 * COSO UI Utilities (jQuery 3.7.1 Compatible)
 * ------------------------------------------
 * Breadcrumb, TOC loading, bookmarks, search and toolbar helpers.
 */

// -------------------------
// Breadcrumb & TOC Loaders
// -------------------------

function loadBreadCrumb(id, type) {
    const params = JSON.stringify({ id, type });
    loadTemplate("WS/Content.asmx/GetFullBreadcrumb", params, "templates/cosofullBreadcrumb.html", "breadcrumbContainer");
}

function loadToc(syncToc = false, id = getTocStateId(), type = getTocStateType()) {
    if (!syncToc) {
        if (id && type) {
            const params = { id, type };
            setToolAsCurrentView(toolName_toc, params);
            setTocStateId(id);
            setTocStateType(type);
        } else {
            setToolAsCurrentView(toolName_toc, null);
        }
    }

    hideDocumentSpecificButtons();

    const isSiteRoot = (id === -1 || type === "Site");
    if (id && type) {
        fillDocumentContainerFromUrl(isSiteRoot ? "static/cosoPlainToc.html" : "static/cosoPlainSyncTocReload.html");
    } else if (hasActiveScreen() && syncToc) {
        fillDocumentContainerFromUrl("static/cosoPlainSyncToc.html");
    } else {
        fillDocumentContainerFromUrl("static/cosoPlainToc.html");
    }
}

// -------------------------
// My Screens (Toolbar)
// -------------------------

function loadMyScreens() {
    const screens = getMyScreens();
    const activeIndex = hasActiveScreen() ? getActiveScreenIndex() : -1;

    const message = { d: screens, activeIndex };
    applyTemplate(message, "templates/cosoDocuments.html", "Toolbar-MyDocuments");
    $("#documentcount").text(getMyScreenCount());
}

function setMyDocumentsTab(visible) {
    if (!visible) return;

    if (isToolbarHidden()) doToolbarToggle();

    $(".tabs li").removeClass("active");
    $(".tab_content").hide();
    //$("#Tab-MyDocuments").addClass("active");
    //$("#Toolbar-MyDocuments").fadeIn();
}

// -------------------------
// Legacy Launch (if used)
// -------------------------

function doNextGenerationLink(guid, domain, referringSite) {
    const url = `/coso/ResourceSeamlessLogin.aspx?hidEncPersGUID=${guid}&hidSourceSiteCode=${referringSite}&hidDomain=${domain}&hidURL=coso.aspx&prod=coso`;
    const attngWindow = window.open(url, "coso");
    attngWindow?.focus();
}

// -------------------------
// Home Page Loader
// -------------------------

function loadHomePage() {
    setToolAsCurrentView(toolName_homePage, "");
    loadTemplate("WS/HomePage.asmx/GetHomePageData", "{}", "static/cosoPlainToc.html", "document-container");
}

// -------------------------
// Search (Enter Key Handler)
// -------------------------

function doSearchCheck(keyCode) {
    if (keyCode === 13) {
        const searchTerms = document.getElementById('searchTerms')?.value || "";
        doAdvancedNavigationalSearch('', searchTerms, 1, 100, 10, 0, 1, 1, 0, () => {
            $('#searchResultButtonLink').show();
        });
    }
}

// -------------------------
// Title Shortening Utility
// -------------------------

docscreen.prototype.titleLimit = function (len) {
    let result = this.targetDoc
        ? `${this.targetDoc} - ${this.siteNode.Title}`
        : this.siteNode.Title;

    if (len && result.length > len) {
        result = `${result.substring(0, len - 1)}&hellip;`;
    }
    return result;
};

// -------------------------
// Bookmarks
// -------------------------

function saveBookmarkCoso() {
    if (!hasActiveDocument()) {
        alert("Bookmarks are not allowed on this document");
        return;
    }

    const title = $('#addBookmarkTitleInput').val();
    const params = JSON.stringify({
        id: getActiveDocumentId(),
        type: getActiveDocumentType(),
        bookmarkTitle: title
    });

    callWebService("WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", params, addPageBookmarkResultHandler, ajaxFailed);
    closeAddBookmark();
}

// -------------------------
// Help
// -------------------------

function loadHelp() {
    clearCurrentView();
    hideDocumentSpecificButtons();
    loadTemplate("WS/HomePage.asmx/GetHelpVisibility", "{}", "templates/COSOhelp.html", "document-container");
}

// -------------------------
// jBreadCrumb Defaults Init
// -------------------------

(($) => {
    $.fn.jBreadCrumb.defaults = {
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
