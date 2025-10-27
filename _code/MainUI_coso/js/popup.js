let popupStatus = 0;

// Show the print popup (if not already open)
function loadPopup() {
    if (popupStatus === 0) {
        $("#backgroundPopup").css("opacity", "0.7").fadeIn("slow");
        $("#popupContact").fadeIn("slow");
        popupStatus = 1;
    }
}

// Hide the print popup (if open)
function disablePopup() {
    if (popupStatus === 1) {
        $("#printContent").empty();
        $("#backgroundPopup, #popupContact").fadeOut("slow");

        $("#pdfPrint, #subpagesPrint, #sourcesPrint").prop("checked", false).prop("disabled", false);

        popupStatus = 0;
    }
}

// Remove any joined section buttons before printing
function removeJoinButtonsPrint() {
    const $frameContents = $("#printContent").contents();
    $frameContents.find(".joinSectionsTopic, .joinSectionsSubtopic").remove();
}

// Called after print content is loaded
function finishLoadingContent() {
    removeJoinButtonsPrint();
}

// Position the popup on screen
function centerPopup2() {
    $("#popupContact").css({
        position: "absolute",
        top: 0
    });
}

// Load print tools status and update the "show sources" checkbox
function updatePrintSourcesCheckbox() {
    const id = getActiveDocumentId();
    const type = getActiveDocumentType();

    callWebService(
        "WS/DocumentTools.asmx/GetBookTools",
        `{id:${id}, type:'${type}'}`,
        updatePrintSourcesCheckboxHandler,
        ajaxFailed
    );
}

// Count the number of subdocuments to determine if a warning is needed
function countSubDocuments() {
    if ($("#subpagesPrint").is(":checked")) {
        callWebService(
            "WS/Content.asmx/countSubDocuments",
            `{docId:${getActiveDocumentId()}}`,
            subDocumentCountHandler,
            ajaxFailed
        );
    }
}

// Handles document count response for subpages
function subDocumentCountHandler(count) {
    const DOC_WARN_LEVEL = 9;
    let proceed = true;

    if (count > DOC_WARN_LEVEL) {
        proceed = confirm(`The application will have to put together ${count} documents and may become unresponsive for a few minutes.`);
    }

    if (proceed) {
        const printPDF = $("#pdfPrint").is(":checked") ? "true" : "";
        const showSources = $("#sourcesPrint").is(":checked") ? "true" : "";

        let url = `PrintDocument.ashx?id=${getActiveDocumentId()}&type=${getActiveDocumentType()}&printSubdocuments=true&showCodificationSources=${showSources}&printToPDF=${printPDF}`;

        if (g_currentView?.toolName === toolName_joinSections || g_currentView?.toolName === toolName_joinChildren) {
            if (g_lastJoinSectionsUrl) {
                $("#subpagesPrint").prop("checked", true).prop("disabled", true);
                $("#sourcesPrint").prop("disabled", true);

                const showSrcTag = "%26show_sources";
                url += `&joinSectionsUrl=${showSources ? g_lastJoinSectionsUrl + showSrcTag :
                    g_lastJoinSectionsUrl.replace(showSrcTag, "")}`;
            }
        }

        $("#printContent").load(url, finishLoadingContent);
    } else {
        $("#subpagesPrint").prop("checked", false);
    }
}

// Determine visibility of source checkbox
function updatePrintSourcesCheckboxHandler(msg) {
    const docPrefix = getMyScreens()[getActiveScreenIndex()].targetDoc?.substring(0, 3).toUpperCase();

    if (["FVS", "PFP"].includes(docPrefix)) {
        $("#pdfPrintOuterDiv").hide();
        setPrintSourcesCheckbox(false);
    } else {
        $("#pdfPrintOuterDiv").show();
        setPrintSourcesCheckbox(docPrefix === "FAF");
    }
}

// Show or hide the source checkbox
function setPrintSourcesCheckbox(visible) {
    if (visible && disableShowCodReferences === false) {
        $("#sourcesPrintDiv").show();
    } else {
        $("#sourcesPrintDiv").hide();
    }
}

// Load print content with appropriate flags and checkboxes
function loadPrintContent() {
    const showSources = $("#sourcesPrint").is(":checked") ? "true" : "";
    const isJoined = getActiveDocumentVCT();

    if (isJoined) {
        $("#subpagesPrint").prop("checked", true);
    }

    if ($("#subpagesPrint").is(":checked")) {
        countSubDocuments();
    } else {
        const printPDF = $("#pdfPrint").is(":checked") ? "true" : "";

        let url = `PrintDocument.ashx?id=${getActiveDocumentId()}&type=${getActiveDocumentType()}&printSubdocuments=false&showCodificationSources=${showSources}&printToPDF=${printPDF}`;

        if (g_currentView?.toolName === toolName_joinSections || g_currentView?.toolName === toolName_joinChildren) {
            if (g_lastJoinSectionsUrl) {
                $("#subpagesPrint").prop("checked", true).prop("disabled", true);
                $("#sourcesPrint").prop("disabled", true);

                const showSrcTag = "%26show_sources";
                url += `&joinSectionsUrl=${showSources ? g_lastJoinSectionsUrl + showSrcTag :
                    g_lastJoinSectionsUrl.replace(showSrcTag, "")}`;
            }
        }

        $("#printContent").load(url, finishLoadingContent);
    }
}

// Attach event handlers
$(function () {
    setPrintSourcesCheckbox(false);

    $("#button, #printBtn").on("click", function () {
        if (hasActiveDocument()) {
            centerPopup2();
            loadPopup();
            updatePrintSourcesCheckbox();
            loadPrintContent();
        }
    });

    $("#addBookmarkbtn").on("click", function () {
        if (hasActiveDocument()) {
            addPageBookmark(getActiveDocumentId(), getActiveDocumentType());
        }
    });

    $("#deleteBookmarkbtn").on("click", function () {
        if (hasActiveDocument()) {
            deletePageBookmark(getActiveDocumentId(), getActiveDocumentType());
        }
    });

    $("a.popupContactClose, #backgroundPopup").on("click", disablePopup);

    $(document).on("keydown", function (e) {
        if (e.key === "Escape" && popupStatus === 1) {
            disablePopup();
        }
    });
});
