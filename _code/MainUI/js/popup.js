var popupStatus = 0;

//loading popup with jQuery magic!
function loadPopup(){
	//loads popup only if it is disabled
    if (popupStatus == 0) {
        
		$("#backgroundPopup").css({
			"opacity": "0.7"
		});
		$("#backgroundPopup").fadeIn("slow");
		$("#popupContact").fadeIn("slow");
		popupStatus = 1;
	}
}

//disabling popup with jQuery magic!
function disablePopup(){
	//disables popup only if it is enabled
	if(popupStatus==1){
	    $("#printContent").empty();
        $("#backgroundPopup").fadeOut("slow");
        $("#popupContact").fadeOut("slow");

        $("#pdfPrint").attr('checked', false);
        $("#subpagesPrint").attr('checked', false);
        $("#sourcesPrint").attr('checked', false);

        $("#subpagesPrint").removeAttr('disabled');

        $("#sourcesPrint").removeAttr('disabled');

		popupStatus = 0;
	}
}

function removeJoinButtonsPrint() {
    $("#printContent").contents().find(".joinSectionsTopic").hide();
    $("#printContent").contents().find(".joinSectionsTopic").remove();
    $("#printContent").contents().find(".joinSectionsSubtopic").hide();
    $("#printContent").contents().find(".joinSectionsSubtopic").remove();
}

function finishLoadingContent() {
    removeJoinButtonsPrint();
}

//centering popup
function centerPopup2(){
	//request data for centering

	//centering
	$("#popupContact").css({
		"position": "absolute",
		"top": 0
	});

}

function updatePrintSourcesCheckbox() {
    callWebService("WS/DocumentTools.asmx/GetBookTools", "{id:" + getActiveDocumentId() + ", type:'" + getActiveDocumentType() + "'}", updatePrintSourcesCheckboxHandler, ajaxFailed);
}

function countSubDocuments() {
    if ($("#subpagesPrint:checked").length > 0) {
        callWebService("WS/Content.asmx/countSubDocuments", "{docId:" + getActiveDocumentId() + "}", subDocumentCountHandler, ajaxFailed);
    }
}


function subDocumentCountHandler(msg) {
    //this is the minimum number of documents before a popup will warn the user that joining them may take a minute.
    var DOC_WARN_LEVEL = 9;


    var count = msg;
    var test = 1;

    if (count > DOC_WARN_LEVEL) 
    {
        test = confirm("The application will have to put together "+count+" documents and may become unresponsive for a few minutes.");
    }
    if (test) {
        var printSubDocs = "true";

        var printPDF = "";
        var showCodificationSources = "";

        if ($("#sourcesPrint:checked").length > 0) {
            showCodificationSources = "true";

        }

        if ($("#pdfPrint:checked").length > 0)
            printPDF = "true";

        var windowUrl = "PrintDocument.ashx?id=" + getActiveDocumentId() + "&type=" + getActiveDocumentType() + "&printSubdocuments=" + printSubDocs + "&showCodificationSources=" + showCodificationSources + "&printToPDF=" + printPDF;

        if (g_currentView != null && (g_currentView.toolName == toolName_joinSections || g_currentView.toolName == toolName_joinChildren) && g_lastJoinSectionsUrl) {
            $("#subpagesPrint").attr('checked', true);
            $("#subpagesPrint").attr('disabled', true);
            $("#sourcesPrint").attr('disabled', true);
            if (showCodificationSources) {
                windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl + "%26show_sources";
            } else {
                if (g_lastJoinSectionsUrl.indexOf("%26show_sources") > 0)
                    windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl.replace("%26show_sources", "");
                else
                    windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl;
            }

        }
        $('#printContent').load(windowUrl, finishLoadingContent);


    }
    else {
        $("#subpagesPrint").attr('checked', false);
    }
    
}

function updatePrintSourcesCheckboxHandler(msg) {
    /*if (msg.ShowFafTools && msg.ViewSources) {
        setPrintSourcesCheckbox(true);
    } else {
        setPrintSourcesCheckbox(false);
    }*/
    var testTargetDoc = getMyScreens()[getActiveScreenIndex()].targetDoc;
    if (testTargetDoc.length > 2 && ((testTargetDoc.substring(0, 3).toUpperCase() == "FVS")||(testTargetDoc.substring(0, 3).toUpperCase() == "PFP"))) {
        $('#pdfPrintOuterDiv').hide();
        setPrintSourcesCheckbox(false);
    }
    else {
        $('#pdfPrintOuterDiv').show();
        if (testTargetDoc.substring(0, 3).toUpperCase() == "FAF") {
            setPrintSourcesCheckbox(true);
        }
        else {
            setPrintSourcesCheckbox(false);
        }
        
    }
}

function setPrintSourcesCheckbox(visible) {

    if (visible && (disableShowCodReferences == false) ) {
        $('#sourcesPrintDiv').show();
    } else {
        $('#sourcesPrintDiv').hide();
    }
}


//CONTROLLING EVENTS IN jQuery
$(document).ready(function () {
    //LOADING POPUP
    setPrintSourcesCheckbox(false);

    //Click the button event!


    $("#button").click(function () {
        if (hasActiveDocument()) {
            //centering with css
            centerPopup2();
            //load popup
            loadPopup();

            //update the print sources stuff
            updatePrintSourcesCheckbox();

            // Adding code to correctly populate popup
            loadPrintContent();
        }
    });

    $("#printBtn").click(function () {
        if (hasActiveDocument()) {
            //centering with css
            centerPopup2();
            //load popup
            loadPopup();

            //update the print sources stuff
            updatePrintSourcesCheckbox();

            // Adding code to correctly populate popup
            loadPrintContent();
        }
    });

    $("#addBookmarkbtn").click(function () {
        if (hasActiveDocument()) {
            addPageBookmark(getActiveDocumentId(), getActiveDocumentType());
        }
    });

    $("#deleteBookmarkbtn").click(function () {
        if (hasActiveDocument()) {
            deletePageBookmark(getActiveDocumentId(), getActiveDocumentType());
        }
    });

    //CLOSING POPUP
    //Click the x event!
    $("a.popupContactClose").click(function () {
        disablePopup();
    });
    //Click out event!
    $("#backgroundPopup").click(function () {
        disablePopup();
    });
    //Press Escape event!
    $(document).keypress(function (e) {
        if (e.keyCode == 27 && popupStatus == 1) {
            disablePopup();
        }
    });

});



function loadPrintContent() {
    var showCodificationSources = "";
    if ($("#sourcesPrint:checked").length > 0) {
        showCodificationSources = "true";

    }

    var joined = getActiveDocumentVCT();

    var printSubDocs = "";
    if (joined) {
        $("#subpagesPrint").attr('checked', 'checked');
    }
    if ($("#subpagesPrint:checked").length > 0) {
        printSubDocs = "true";
        countSubDocuments();

    }
    else {
        var printPDF = "";
        if ($("#pdfPrint:checked").length > 0)
            printPDF = "true";

        var windowUrl = "PrintDocument.ashx?id=" + getActiveDocumentId() + "&type=" + getActiveDocumentType() + "&printSubdocuments=" + printSubDocs + "&showCodificationSources=" + showCodificationSources + "&printToPDF=" + printPDF;

        if (g_currentView != null && (g_currentView.toolName == toolName_joinSections || g_currentView.toolName == toolName_joinChildren) && g_lastJoinSectionsUrl) {
            $("#subpagesPrint").attr('checked', true);
            $("#subpagesPrint").attr('disabled', true);
            $("#sourcesPrint").attr('disabled', true);
            if (showCodificationSources) {
                windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl + "%26show_sources";
            } else {
                if (g_lastJoinSectionsUrl.indexOf("%26show_sources") > 0)
                    windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl.replace("%26show_sources", "");
                else
                    windowUrl += "&joinSectionsUrl=" + g_lastJoinSectionsUrl;
            }

        }
        $('#printContent').load(windowUrl, finishLoadingContent);

    }

}