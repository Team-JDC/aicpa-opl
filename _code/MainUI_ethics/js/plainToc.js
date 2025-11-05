function initialTocLoad(rootId) {
    var id = getTocStateId();
    var type = getTocStateType();

    loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
}

function syncTocLoad(rootId) {
    if (hasActiveDocument()) {
        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
    }
    else {
        loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", -1, "Site", $("#" + rootId), true);
    }
}

function TocLoadByIdType(rootId, id, type) {
    if ((id) && (type)) {
        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
    } else {
        loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", -1, "Site", $("#" + rootId), true);
    }
}


function loadPlainTocByHtml(url, id, type, ulToAppend, shouldExpandToNode) {
    var params = "{id: '" + id + "', type:'" + type + "'}";

    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            ulToAppend.html(response.d);
            if (shouldExpandToNode) {
                expandToNode(id, type);
            }
        },
        error: ajaxFailed

    });
}

function toggleTocNode(id, type, uniqueId) {
    var childUl = $("#childUl-" + uniqueId);
    var currentLi = $("#currentLi-" + uniqueId);
    var currentDiv = $("#currentDiv-" + uniqueId);

    if (!childUl.hasClass("calledWS")) {
        childUl.addClass("calledWS");

        loadPlainTocByHtml("WS/Content.asmx/GetNodeToGrandChildrenHtml", id, type, childUl, false);
    }

    toggleCurrentLiClass(currentLi);
    toggleCurrentDivClass(currentDiv);

    childUl.slideToggle();

    // save state
    setTocStateId(id);
    setTocStateType(type);
}

function toggleCurrentLiClass(currentLi) {
    if (currentLi.hasClass("expandable")) {
        currentLi.removeClass("expandable");
        currentLi.addClass("collapsable");

        if (currentLi.hasClass("lastExpandable")) {
            currentLi.removeClass("lastExpandable");
            currentLi.addClass("lastCollapsable");
        }
    }
    else {
        currentLi.removeClass("collapsable");
        currentLi.addClass("expandable");

        if (currentLi.hasClass("lastCollapsable")) {
            currentLi.removeClass("lastCollapsable");
            currentLi.addClass("lastExpandable");
        }
    }
}

function toggleCurrentDivClass(currentDiv) {
    if (currentDiv.hasClass("expandable-hitarea")) {
        currentDiv.removeClass("expandable-hitarea");
        currentDiv.addClass("collapsable-hitarea");

        if (currentDiv.hasClass("lastExpandable-hitarea")) {
            currentDiv.removeClass("lastExpandable-hitarea");
            currentDiv.addClass("lastCollapsable-hitarea");
        }
    }
    else {
        currentDiv.removeClass("collapsable-hitarea");
        currentDiv.addClass("expandable-hitarea");

        if (currentDiv.hasClass("lastCollapsable-hitarea")) {
            currentDiv.removeClass("lastCollapsable-hitarea");
            currentDiv.addClass("lastExpandable-hitarea");
        }
    }
}

function expandToNode(id, type) {
    // Handle "root" (-1) up front
    if (id === -1) {
        var $rootLi = $('#mainToc').children('li:first');
        if ($rootLi.length) {
            manualExpand.call($rootLi[0]);
            $rootLi[0].scrollIntoView(true);
        } else {
            logErrorToServer('expandToNode: root li not found under #mainToc');
        }
        return;
    }

    // ─────────────────────────────────────────────
    // 1. Find the li – use id only, ignore type
    //    (this avoids all the "Document" vs "document" problems)
    // ─────────────────────────────────────────────
    var $currentLi = $("#mainToc li[id^='currentLi-" + id + "-']");

    if (!$currentLi.length) {
        // Nothing found – *don’t* touch currentLi[0], just log and bail
        logErrorToServer('expandToNode: li not found for id=' + id + ', type=' + type);
        return;
    }

    var currentLi = $currentLi.first(); // just in case there are multiple

    // ─────────────────────────────────────────────
    // 2. Expand ancestors and the current node
    // ─────────────────────────────────────────────
    var ancestry = currentLi.parents("#mainToc li");
    ancestry.each(manualExpand);          // expand parents
    manualExpand.call(currentLi[0]);      // expand the node itself

    // ─────────────────────────────────────────────
    // 3. Scroll into view
    // ─────────────────────────────────────────────
    currentLi[0].scrollIntoView(true);

    // ─────────────────────────────────────────────
    // 4. Flash highlight on first span
    // ─────────────────────────────────────────────
    var $span = currentLi.find("span:first");
    if ($span.length) {
        $span.css("background", "#5CB3FF");
        setTimeout(function () {
            $span.animate({ backgroundColor: "#ffffff" }, 1500, "swing");
        }, 4000);
    }
}


function manualExpand() {
    var currentLi = $(this);
    var currentDiv = currentLi.children("div:first");
    var childUl = currentLi.children("ul:first");

    toggleCurrentLiClass(currentLi);
    toggleCurrentDivClass(currentDiv);

    childUl.show();

}
