
/** 
* (06/15/10) Ben Bytheway: This function handles loading the template for the 
* FAF tools.
*/
function loadFafTools() {
    if (hasActiveDocument() && isCurrentViewScreen()) {
        loadTemplate("WS/DocumentTools.asmx/GetBookTools", "{id:" + getActiveDocumentId() + ", type:'" + getActiveDocumentType() + "'}", "templates/fafToolbar.html", "Toolbar-Tools-Faf");
    } else {
        loadTemplate("WS/DocumentTools.asmx/GetBookTools", "{id:-1, type:'Site'}", "templates/fafToolbar.html", "Toolbar-Tools-Faf");
    }
    //showFAFTools();
}

function setFafCopyright() {
    setCopyright("Copyright &copy; 2009-" + getCurrentCopyrightYear() + ", Financial Accounting Standards Board, Norwalk, Connecticut. All Rights Reserved.");
}

function doFafGotoLink() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_gotoCode, "");
    hideDocumentSpecificButtons();
    setFafCopyright();
    
    loadTemplate("WS/DocumentTools.asmx/GetGotoInformation", "{topicNum:'', subNum:''}", "templates/fafGoto.html", "document-container");
}

function loadCrossReference() {
    setToolAsCurrentView(toolName_crossRef, null);
    getCrossReferenceLink("", "", "");
}

function getCrossReferenceLink(standard, topic, subtopic) {

    hideDocumentSpecificButtons();
    setFafCopyright();
    loadTemplate("WS/DocumentTools.asmx/GetStandardsForCrossReference", "{standard:'" + standard + "', topic:'" + topic + "', subTopic:'" + subtopic + "'}", "templates/toolsCrossReference.html", "document-container");
}

function getCrossReferenceResults(standard, number, topic, subtopic, section) {
    if ((standard == '') && (topic == '')) {
        $('#standardPrompt').css("color", "red");
        $('#topicPrompt').css("color", "red");
        $('#crossRefInputError').show();
    } 
    else {
        updateCurrentTool_crossRef(standard, number, topic, subtopic, section); // save params in back button history

        $('#crossRefInputError').hide();
        loadTemplate("WS/DocumentTools.asmx/GetCrossReferenceResults", "{standard:'" + standard + "', number:'" + number + "', topic:'" + topic + "', subTopic:'" + subtopic + "', section:'" + section + "'}", "templates/resultsCrossReference.html", "cross-ref-results");
    }
}

function loadCrossReferenceFromValues(toolParams) {
    setToolAsCurrentView(toolName_crossRef, toolParams);

    hideDocumentSpecificButtons();
    setFafCopyright();
    loadTemplate("WS/DocumentTools.asmx/GetStandardsForCrossReference", "{standard:'" + toolParams.standard + "', topic:'" + toolParams.topic + "', subTopic:'" + toolParams.subtopic + "'}", "templates/toolsCrossReference.html", "document-container", toolParams, loadCrossReferenceFromValuesCallback);
}

function loadCrossReferenceFromValuesCallback(toolParams) {
    if (toolParams.number) {
        $("#number").val(toolParams.number);
    }

    if (toolParams.section) {
        $("#section").val(toolParams.section);
    }

    loadTemplate("WS/DocumentTools.asmx/GetCrossReferenceResults", "{standard:'" + toolParams.standard + "', number:'" + toolParams.number + "', topic:'" + toolParams.topic + "', subTopic:'" + toolParams.subtopic + "', section:'" + toolParams.section + "'}", "templates/resultsCrossReference.html", "cross-ref-results");
}

function doFafGotoSubmit(topicNum, subNum, sectNum) {
    var targetPtr = "";
    var targetDoc = "";
    var firstChar = topicNum.charAt(0);

    switch (firstChar) {
        case "1":
            targetDoc = "faf-generalprinciples";
            break;
        case "2":
            targetDoc = "faf-presentation";
            break;
        case "3":
            targetDoc = "faf-assets";
            break;
        case "4":
            targetDoc = "faf-liabilities";
            break;
        case "5":
            targetDoc = "faf-equity";
            break;
        case "6":
            targetDoc = "faf-revenue";
            break;
        case "7":
            targetDoc = "faf-expenses";
            break;
        case "8":
            targetDoc = "faf-broadTransactions";
            break;
        case "9":
            targetDoc = "faf-industry";
            break;
        default:
            targetDoc = "faf-";
            break;
    }

    //alert(targetDoc);

    if (subNum != "") {
        targetPtr = topicNum + "-" + subNum;
    }
    else {
        targetPtr = "topic_" + topicNum;
    }

    if (sectNum != "") {
        targetPtr = targetPtr + "-" + sectNum;
    }

    doLink(targetDoc, targetPtr, true);
}

function doFafWhatLinksHereLink() {
    clearCurrentView();
    hideDocumentSpecificButtons();
    setFafCopyright();
    if (hasActiveDocument()) {
        fillContentPaneFromUrl("Handlers/DownloadDocument.ashx?docid=" + getActiveDocumentId() + "&d_ft=" + 17);
    }
}

function doFafArchiveLink() {
    clearCurrentView();
    hideDocumentSpecificButtons();
    setFafCopyright();
    if (hasActiveDocument()) {
        fillContentPaneFromUrl("Handlers/DownloadDocument.ashx?docid=" + getActiveDocumentId() + "&d_ft=" + 18);
    }
}

/**
* This is the event handler for click on the "doc" button on the faf tools menu.
* The use case is that you have clicked something like archive, and want to go back to the doc
*/
function doFafDocumentLink() {
    if (hasActiveDocument()) {
        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        doScreenLink(getActiveScreenIndex());
    }
    else {
        doHomePageLink();
    }
}

function doFafJoinSectionsLink() {
    setToolAsCurrentView(toolName_joinSections, null);
    hideDocumentSpecificButtons();
    setFafCopyright();
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'', content:'', includeSubtopics:false}", "templates/fafJoinSections.html", "document-container");
}

function doJoinSectionsQuery(topicNum, sectionNum, content, includeSubtopics) {
    updateCurrentTool_joinSections(topicNum, sectionNum, content, includeSubtopics);

    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsResults", "{topicNum:'" + topicNum + "', sectionNum:'" + sectionNum + "', includeSubtopics:" + includeSubtopics + "}", "templates/fafJoinSectionsResults.html", "fafJoinSectionsResults");
}

function doJoinSectionsFromValues(toolParams) {
    setToolAsCurrentView(toolName_joinSections, toolParams);

    hideDocumentSpecificButtons();
    setFafCopyright();
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'" + toolParams.topicNum + "', content:'" + toolParams.content + "', includeSubtopics:" + toolParams.includeSubtopics + "}", "templates/fafJoinSections.html", "document-container", toolParams, doJoinSectionsFromValuesCallback);
}

function doJoinSectionsFromValuesCallback(toolParams) {
    if (toolParams.sectionNum) {
        $("#section").val(toolParams.sectionNum);
    }

    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsResults", "{topicNum:'" + toolParams.topicNum + "', sectionNum:'" + toolParams.sectionNum + "', includeSubtopics:" + toolParams.includeSubtopics + "}", "templates/fafJoinSectionsResults.html", "fafJoinSectionsResults");
}

var g_lastJoinSectionsUrl;

function doJoinSections(showSources) {
    setShowSources(showSources);
    setPrintButton(true);

    //get all of the document stuff ready
    //var checkBoxes = (".join-section-checkbox:checked");
    var checkBoxes = $("#join-sections-results-table-div input.join-section-checkbox:checked");

    if (checkBoxes.length < 1) {
        alert("You must select at least one document to join");
        return;
    }

    var queryString = "";
    var title = "";

    for (var i = 0; i < checkBoxes.length; i++) {
        
        var firstChar = $(checkBoxes[i]).val().charAt(0);
        var targetDoc = "";

        switch (firstChar) {
            case "1":
                targetDoc = "faf-";
                break;
            case "2":
                targetDoc = "faf-presentation";
                break;
            case "3":
                targetDoc = "faf-assets";
                break;
            case "4":
                targetDoc = "faf-liabilities";
                break;
            case "5":
                targetDoc = "faf-equity";
                break;
            case "6":
                targetDoc = "faf-revenue";
                break;
            case "7":
                targetDoc = "faf-expenses";
                break;
            case "8":
                targetDoc = "faf-broadTransactions";
                break;
            case "9":
                targetDoc = "faf-industry";
                break;
            default:
                targetDoc = "faf-";
                break;
        }

        queryString += "&targetdoc=" + targetDoc + "&targetptr=" + $(checkBoxes[i]).val() + "";
        title += targetDoc + " " + $(checkBoxes[i]).val() + ", ";
    }

    hideFAFTools(true);

    var joinSectionsUrl = queryString;

    if (getShowSources()) {
        joinSectionsUrl += "&show_sources";
        $("#sourcesPrint").attr('checked', true);
    }

    if (getActiveScreen().showHighlight) {
        joinSectionsUrl += "&hilite";
    }

    var hitAnchor = "";

    if (getActiveScreen().hitAnchor != null) {
        joinSectionsUrl += "&hitanchor=" + getActiveScreen().hitAnchor;
        hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
    }

    $("#sourcesPrint").attr('disabled', true);
    g_lastJoinSectionsUrl = escape(joinSectionsUrl);

    fillContentPaneFromUrl("Handlers/GetDocuments.ashx?show_sources=" + getShowSources() + hitAnchor +"&d_hh=" + getActiveScreen().showHighlight + queryString);
}

function doJoinChildren(targetDoc, targetPtr) {
    var toolParams = { targetDoc: targetDoc, targetPtr: targetPtr };
    setToolAsCurrentView(toolName_joinChildren, toolParams);

    hideDocumentSpecificButtons();
    setFafCopyright();
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    callWebService("WS/Content.asmx/GetNodeToGrandChildrenByTargetDocTargetPointer", params, doJoinChildrenResult, ajaxFailed);
}

function doJoinChildrenResult(breadcrumbNode) {
    setPrintButton(true);
    var queryString = "";

    queryString += "&id=" + breadcrumbNode.SiteNode.Id + "&type=" + breadcrumbNode.SiteNode.Type + "";

    for (var i = 0; i < breadcrumbNode.Children.length; i++) {
        queryString += "&id=" + breadcrumbNode.Children[i].SiteNode.Id + "&type=" + breadcrumbNode.Children[i].SiteNode.Type + "";
    }

    hideFAFTools(true);

    var joinSectionsUrl = queryString;
    if (getShowSources())
        joinSectionsUrl += "&show_sources";
    if (getActiveScreen().showHighlight)
        joinSectionsUrl += "&hilite";

    var hitAnchor = "";
    if (getActiveScreen().hitAnchor != null) {
        joinSectionsUrl += "&hitanchor=" + getActiveScreen().hitAnchor;
        hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
    }


    g_lastJoinSectionsUrl = escape(joinSectionsUrl);

    fillContentPaneFromUrl("Handlers/GetDocuments.ashx?show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getActiveScreen().showHighlight + queryString);
}

function toggleShowSources() {
    if (getShowSources()) {
        setShowSources(false);
        $("#sourcesPrint").attr('checked', false);
    } else {
        setShowSources(true);
        $("#sourcesPrint").attr('checked', true);
    }

    if (hasActiveDocument()) {
        // 2010-08-05 sburton: because we just redirect them back to the current document
        // we would end up with two entries in our history.  So we'll call a clear method now.
        clearCurrentViewWithoutRecording();

        var id = getActiveDocumentId();
        var type = getActiveDocumentType();

        doScreenLink(getActiveScreenIndex());
    }
    else {
        doHomePageLink();
    }
}

//***********************************************************************************
// onchange handlers
//***********************************************************************************
function doFafGotoDropDownChange(topicNum, subNum) {
    loadTemplate("WS/DocumentTools.asmx/GetGotoInformation", "{topicNum:'" + topicNum + "', subNum:'" + subNum + "'}", "templates/fafGoto.html", "document-container");
}

function doFafJoinSectionsChange(topicNum, content, includeSubtopics) {
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'" + topicNum + "', content:'" + content + "', includeSubtopics:" + includeSubtopics + "}", "templates/fafJoinSections.html", "document-container");
}