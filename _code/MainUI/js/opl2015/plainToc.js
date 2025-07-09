function initialTocLoad(rootId) {
    var id = getTocStateId();
    var type = getTocStateType();

    //loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
    loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
}

function syncTocLoad(rootId) {
    if (hasActiveDocument()) {
        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
    }
    else {
        loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", -1, "Site", $("#" + rootId), true);
    }
}

function TocLoadByIdType(rootId, id, type) {
    if ((id) && (type)) {
        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
    }  else {
        loadPlainTocByHtml("/WS/Content.asmx/GetInitialTreeTocHtml", -1, "Site", $("#" + rootId), true);
    }
}


function loadPlainTocByHtml(url, id, type, ulToAppend, shouldExpandToNode, level, callback) {
    if (!level) {
        level = 0;
    }    
    var params = "{id: '" + id + "', type:'" + type + "', level: '"+level+"'}";

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
            if (callback) {
                callback();
            }
        },
        error: ajaxFailed

    });
}

function toggleTocNode(id, type, uniqueId, level) {
    if (!level) {
        level = 0;
    }
    var childUl = $("#childUl-" + uniqueId);
    var currentLi = $("#currentLi-" + uniqueId);
    var currentDiv = $("#currentDiv-" + uniqueId);
    
    //var me = $(event.target).closest('.acc_head');
    
    if (!childUl.hasClass("calledWS")) {
        childUl.addClass("calledWS");
        //$(childUl).html("<div id='tocContentHolder' style='display: block;'><img src='/images/loading-spinner.gif'></img></div>");
        //childUl.slideToggle();
        //<div id='tocContentHolder' style='display: none;'><img src='/images/loading-spinner.gif'></img></div>
        loadPlainTocByHtml("/WS/Content.asmx/GetNodeToGrandChildrenHtml", id, type, childUl, false, level, function () {
            childUl.slideToggle();
        });
    } else {
        //acc_headClick(me);
        childUl.slideToggle();
    }

    toggleCurrentLiClass(currentLi);
    toggleCurrentDivClass(currentDiv);

    //childUl.slideToggle();

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
    var liIdString = "#currentLi-" + id + "-" + type;

    if (id != -1) {
        var currentLi = $(liIdString);
        var ancestry = currentLi.parents("#mainToc li");
        ancestry.each(manualExpand);

        // do something with currentLi
        manualExpand.call(currentLi[0]);

        //scroll to the node
        var anchorElement = currentLi[0];
        if (anchorElement)
            anchorElement.scrollIntoView(true);
        else logErrorToServe('Anchor Empty: '+ lidIdString + ':'+currentLi[0]);        

        var span = $(liIdString + " span:first");

        span.css("background", "#5CB3FF");

        setTimeout(function () {
            //span.css("background", "#ffffff");
            // if we had jquery-ui, this animation would work.  But this is the only place we need jquery-ui
            span.animate({ backgroundColor: "#ffffff" }, 1500, "swing");
        }, 4000);
    }
    else {
        manualExpand.call($('#mainToc').children("li:first")[0]);
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
