function loadFafToc(pageName) {
    $('.navigation').treeview({
        collapsed: true,
        prerendered: true,
        unique: true
    });

}


function AfterFafToolsLoad() {
    restickMenu();
}

function loadFafToolsByDocAndPtr(tdoc, tptr) {
    if (!tdoc || tdoc === "")
        tdoc = null;
    if (!tptr || tptr === "")
        tptr = null;

    if (tdoc && tptr) {
        //console.log("Loading FafTools:" + tdoc + ":" + tptr);
        loadTemplate("/WS/DocumentTools.asmx/GetBookToolsByDocAndPtr", "{targetDoc:'" + tdoc + "', targetPtr:'" + tptr + "'}", "/templates/odp2015/fafMenu.html", "liFasbAsc",null, AfterFafToolsLoad);
    } else {
        //console.log("Loading FafTools: -1:site");
        loadTemplate("/WS/DocumentTools.asmx/GetBookTools", "{id:-1, type:'Site'}", "/templates/odp2015/fafMenu.html", "liFasbAsc", null, AfterFafToolsLoad);
    }
    //showFAFTools();
}

function loadFafTools(id, type){
    if (!id || id === "")
        id = null;
    if (!type || type === "")
        type = null;

    if (id && type) {
        loadTemplate("/WS/DocumentTools.asmx/GetBookTools", "{id:" + id + ", type:'" + type + "'}", "/templates/odp2015/fafMenu.html", "liFasbAsc", null, AfterFafToolsLoad);
    } else {
        loadTemplate("/WS/DocumentTools.asmx/GetBookTools", "{id:-1, type:'Site'}", "/templates/odp2015/fafMenu.html", "liFasbAsc", null, AfterFafToolsLoad);
    }
    //showFAFTools();
}


function toggleShowSources() {
    if (getShowSources()) {
        setShowSources(false);
        $("#sourcesPrint").attr('checked', false);
    } else {
        setShowSources(true);
        $("#sourcesPrint").attr('checked', true);
    }

//    if (hasActiveDocument()) {
//        // 2010-08-05 sburton: because we just redirect them back to the current document
//        // we would end up with two entries in our history.  So we'll call a clear method now.
//        clearCurrentViewWithoutRecording();

//        var id = getActiveDocumentId();
//        var type = getActiveDocumentType();

//        doScreenLink(getActiveScreenIndex());
//    }
//    else {
//        doHomePageLink();
//    }
}

function doJoinChildren(targetDoc, targetPtr) {
    window.location = '/content/join/' + targetDoc + '/' + targetPtr;
}

function doJoinInternal(targetDoc, targetPtr) {
    var toolParams = { targetDoc: targetDoc, targetPtr: targetPtr };
    //setToolAsCurrentView(toolName_joinChildren, toolParams);

    //hideDocumentSpecificButtons();
    //setFafCopyright();
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    callWebService("/WS/Content.asmx/GetNodeToGrandChildrenByTargetDocTargetPointer", params, doJoinChildrenResult, ajaxFailed, params);
}

function doJoinChildrenResult(breadcrumbNode, params) {
    //    setPrintButton(true);
    //console.log(params);
    var queryString = "";

    queryString += "&id=" + breadcrumbNode.SiteNode.Id + "&type=" + breadcrumbNode.SiteNode.Type + "";

    for (var i = 0; i < breadcrumbNode.Children.length; i++) {
        queryString += "&id=" + breadcrumbNode.Children[i].SiteNode.Id + "&type=" + breadcrumbNode.Children[i].SiteNode.Type + "";
    }

//    hideFAFTools(true);

    var joinSectionsUrl = queryString;
    if (getShowSources())
        joinSectionsUrl += "&show_sources";
    if (getShowHighlights())
        joinSectionsUrl += "&hilite";

    var hitAnchor = "";
//    if (getActiveScreen().hitAnchor != null) {
//        joinSectionsUrl += "&hitanchor=" + getActiveScreen().hitAnchor;
//        hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
//    }


    g_lastJoinSectionsUrl = escape(joinSectionsUrl);

    fillLeftContentPaneFromUrl("/Handlers/GetDocuments.ashx?show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getShowHighlights() + queryString);
}