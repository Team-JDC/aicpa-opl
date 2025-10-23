// --- FAF Tools Integration ---

function loadFafTools() {
    const isDocView = hasActiveDocument() && isCurrentViewScreen();
    const params = isDocView ? `{id:${getActiveDocumentId()}, type:'${getActiveDocumentType()}'}` : `{id:-1, type:'Site'}`;
    loadTemplate("WS/DocumentTools.asmx/GetBookTools", params, "templates/fafToolbar.html", "Toolbar-Tools-Faf");
}

function setFafCopyright() {
    setCopyright(`Copyright © 2009-${getCurrentCopyrightYear()}, Financial Accounting Standards Board, Norwalk, Connecticut. All Rights Reserved.`);
}

function doFafGotoLink() {
    clearCurrentView();
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
    const params = `{standard:'${standard}', topic:'${topic}', subTopic:'${subtopic}'}`;
    loadTemplate("WS/DocumentTools.asmx/GetStandardsForCrossReference", params, "templates/toolsCrossReference.html", "document-container");
}

function getCrossReferenceResults(standard, number, topic, subtopic, section) {
    if (!standard && !topic) {
        $('#standardPrompt, #topicPrompt').css("color", "red");
        $('#crossRefInputError').show();
        return;
    }

    updateCurrentTool_crossRef(standard, number, topic, subtopic, section);
    $('#crossRefInputError').hide();

    const params = `{standard:'${standard}', number:'${number}', topic:'${topic}', subTopic:'${subtopic}', section:'${section}'}`;
    loadTemplate("WS/DocumentTools.asmx/GetCrossReferenceResults", params, "templates/resultsCrossReference.html", "cross-ref-results");
}

function loadCrossReferenceFromValues(toolParams) {
    setToolAsCurrentView(toolName_crossRef, toolParams);
    hideDocumentSpecificButtons();
    setFafCopyright();
    const params = `{standard:'${toolParams.standard}', topic:'${toolParams.topic}', subTopic:'${toolParams.subtopic}'}`;
    loadTemplate("WS/DocumentTools.asmx/GetStandardsForCrossReference", params, "templates/toolsCrossReference.html", "document-container", toolParams, loadCrossReferenceFromValuesCallback);
}

function loadCrossReferenceFromValuesCallback(toolParams) {
    if (toolParams.number) $('#number').val(toolParams.number);
    if (toolParams.section) $('#section').val(toolParams.section);

    const params = `{standard:'${toolParams.standard}', number:'${toolParams.number}', topic:'${toolParams.topic}', subTopic:'${toolParams.subtopic}', section:'${toolParams.section}'}`;
    loadTemplate("WS/DocumentTools.asmx/GetCrossReferenceResults", params, "templates/resultsCrossReference.html", "cross-ref-results");
}

function doFafGotoSubmit(topicNum, subNum, sectNum) {
    const targetDocMap = {
        '1': 'faf-generalprinciples',
        '2': 'faf-presentation',
        '3': 'faf-assets',
        '4': 'faf-liabilities',
        '5': 'faf-equity',
        '6': 'faf-revenue',
        '7': 'faf-expenses',
        '8': 'faf-broadTransactions',
        '9': 'faf-industry'
    };

    const targetDoc = targetDocMap[topicNum.charAt(0)] || 'faf-';
    let targetPtr = subNum ? `${topicNum}-${subNum}` : `topic_${topicNum}`;
    if (sectNum) targetPtr += `-${sectNum}`;

    doLink(targetDoc, targetPtr, true);
}

function doFafWhatLinksHereLink() {
    clearCurrentView();
    hideDocumentSpecificButtons();
    setFafCopyright();
    if (hasActiveDocument()) {
        fillContentPaneFromUrl(`Handlers/DownloadDocument.ashx?docid=${getActiveDocumentId()}&d_ft=17`);
    }
}

function doFafArchiveLink() {
    clearCurrentView();
    hideDocumentSpecificButtons();
    setFafCopyright();
    if (hasActiveDocument()) {
        fillContentPaneFromUrl(`Handlers/DownloadDocument.ashx?docid=${getActiveDocumentId()}&d_ft=18`);
    }
}

function doFafDocumentLink() {
    hasActiveDocument() ? doScreenLink(getActiveScreenIndex()) : doHomePageLink();
}

function doFafJoinSectionsLink() {
    setToolAsCurrentView(toolName_joinSections, null);
    hideDocumentSpecificButtons();
    setFafCopyright();
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'', content:'', includeSubtopics:false}", "templates/fafJoinSections.html", "document-container");
}

function doJoinSectionsQuery(topicNum, sectionNum, content, includeSubtopics) {
    updateCurrentTool_joinSections(topicNum, sectionNum, content, includeSubtopics);
    const params = `{topicNum:'${topicNum}', sectionNum:'${sectionNum}', includeSubtopics:${includeSubtopics}}`;
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsResults", params, "templates/fafJoinSectionsResults.html", "fafJoinSectionsResults");
}

function doJoinSectionsFromValues(toolParams) {
    setToolAsCurrentView(toolName_joinSections, toolParams);
    hideDocumentSpecificButtons();
    setFafCopyright();
    const params = `{topicNum:'${toolParams.topicNum}', content:'${toolParams.content}', includeSubtopics:${toolParams.includeSubtopics}}`;
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", params, "templates/fafJoinSections.html", "document-container", toolParams, doJoinSectionsFromValuesCallback);
}

function doJoinSectionsFromValuesCallback(toolParams) {
    if (toolParams.sectionNum) $('#section').val(toolParams.sectionNum);
    const params = `{topicNum:'${toolParams.topicNum}', sectionNum:'${toolParams.sectionNum}', includeSubtopics:${toolParams.includeSubtopics}}`;
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsResults", params, "templates/fafJoinSectionsResults.html", "fafJoinSectionsResults");
}

let g_lastJoinSectionsUrl = '';

function doJoinSections(showSources) {
    setShowSources(showSources);
    setPrintButton(true);

    const $checkboxes = $("#join-sections-results-table-div input.join-section-checkbox:checked");
    if ($checkboxes.length === 0) return alert("You must select at least one document to join");

    let queryString = '', title = '';
    $checkboxes.each(function () {
        const val = $(this).val();
        const firstChar = val.charAt(0);
        const targetDocMap = {
            '1': 'faf-',
            '2': 'faf-presentation',
            '3': 'faf-assets',
            '4': 'faf-liabilities',
            '5': 'faf-equity',
            '6': 'faf-revenue',
            '7': 'faf-expenses',
            '8': 'faf-broadTransactions',
            '9': 'faf-industry'
        };
        const targetDoc = targetDocMap[firstChar] || 'faf-';
        queryString += `&targetdoc=${targetDoc}&targetptr=${val}`;
        title += `${targetDoc} ${val}, `;
    });

    hideFAFTools(true);

    if (getShowSources()) {
        queryString += "&show_sources";
        $("#sourcesPrint").prop("checked", true);
    }
    if (getActiveScreen().showHighlight) queryString += "&hilite";

    const anchor = getActiveScreen().hitAnchor;
    if (anchor) queryString += `&hitanchor=${anchor}`;

    g_lastJoinSectionsUrl = encodeURIComponent(queryString);
    $("#sourcesPrint").prop("disabled", true);

    fillContentPaneFromUrl(`Handlers/GetDocuments.ashx?show_sources=${getShowSources()}${anchor ? `&hitanchor=${anchor}` : ''}&d_hh=${getActiveScreen().showHighlight}${queryString}`);
}

function doJoinChildren(targetDoc, targetPtr) {
    const toolParams = { targetDoc, targetPtr };
    setToolAsCurrentView(toolName_joinChildren, toolParams);
    hideDocumentSpecificButtons();
    setFafCopyright();
    const params = `{ targetDoc:'${targetDoc}', targetPointer:'${targetPtr}'}`;
    callWebService("WS/Content.asmx/GetNodeToGrandChildrenByTargetDocTargetPointer", params, doJoinChildrenResult, ajaxFailed);
}

function doJoinChildrenResult(breadcrumbNode) {
    setPrintButton(true);
    let queryString = `&id=${breadcrumbNode.SiteNode.Id}&type=${breadcrumbNode.SiteNode.Type}`;

    breadcrumbNode.Children.forEach(child => {
        queryString += `&id=${child.SiteNode.Id}&type=${child.SiteNode.Type}`;
    });

    hideFAFTools(true);

    if (getShowSources()) queryString += "&show_sources";
    if (getActiveScreen().showHighlight) queryString += "&hilite";
    const anchor = getActiveScreen().hitAnchor;
    if (anchor) queryString += `&hitanchor=${anchor}`;

    g_lastJoinSectionsUrl = encodeURIComponent(queryString);
    fillContentPaneFromUrl(`Handlers/GetDocuments.ashx?show_sources=${getShowSources()}${anchor ? `&hitanchor=${anchor}` : ''}&d_hh=${getActiveScreen().showHighlight}${queryString}`);
}

function toggleShowSources() {
    const show = !getShowSources();
    setShowSources(show);
    $("#sourcesPrint").prop("checked", show);

    if (hasActiveDocument()) {
        clearCurrentViewWithoutRecording();
        doScreenLink(getActiveScreenIndex());
    } else {
        doHomePageLink();
    }
}

function doFafGotoDropDownChange(topicNum, subNum) {
    const params = `{topicNum:'${topicNum}', subNum:'${subNum}'}`;
    loadTemplate("WS/DocumentTools.asmx/GetGotoInformation", params, "templates/fafGoto.html", "document-container");
}

function doFafJoinSectionsChange(topicNum, content, includeSubtopics) {
    const params = `{topicNum:'${topicNum}', content:'${content}', includeSubtopics:${includeSubtopics}}`;
    loadTemplate("WS/DocumentTools.asmx/GetJoinSectionsInformation", params, "templates/fafJoinSections.html", "document-container");
}
