

function timeoutHandler() {
    var shouldKeepAlive = confirm("Your session is about to expire.\n\nClick 'OK' if you wish to stay logged in, or click 'Cancel' to log out.\n\nFor your security reasons, if you are unable to respond you will be logged out automatically.");

    if (shouldKeepAlive) {
        keepSessionAlive();
    }
    else {
        document.location.href = "Logout.aspx";
    }
}

function doFootnoteLink(targetDoc, anchorName) {
    window.location.href = '/content/link/' + targetDoc + '/' + anchorName;
}

function googleAnalytics(event, searchTerm, searchType, typeCategory, filterName) {
    dataLayer.push({
        event: event,
        'Search Term': searchTerm,
        'Search Type': searchType,
        'Search TypeCategory': typeCategory,
        'Search Filter Name': filterName
    });
}

function showDocumentToolbar(show) {
    if (show) {
        $("#liFafDocument").show();
    } else {
        $("#liFafDocument").hide();
    }
}

function doTocLink(id, type) {
    //console.log("doTocLink : " + id + " type:" + type);
    //setLoading(true); 

    //open in new window = true
    //showHighlight = false
    //scrollBarPosition can be passed as an optional third parameter
    //updateTocView(id, type);
    //var result = prepareActiveScreen(true, false);

    //if (result) {
        loadPrimaryContent(id, type);
    //}

}

function doLink(targetDoc, targetPtr, useNewScreen, viewCompleteTopic, norecord) {
    //targetPtr = targetPtr.replace(/\./g, "=+=");
    
    window.location.href = '/content/link/' + targetDoc + '/' + targetPtr;
}

// when someone clicks a link in the content
function doLinkRoute(targetDoc, targetPtr, useNewScreen, viewCompleteTopic, norecord) {
    if (viewCompleteTopic === undefined) {
        viewCompleteTopic = false;
    }

    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";    
    callWebService("/WS/Content.asmx/ResolveContentLink", params, loadContentBySiteNode, doLinkRouteFailed);
  
}

function doLinkRouteFailed() {    
    fillContentPaneFromUrl("/templates/odp2015/contentError.html");    
}


// when someone clicks on an item in a drop down breadcrumb list
function doBreadcrumbLink(id, type) {
    //setLoading(true);
    if (id != -1) {
        loadPrimaryContent(id, type);
    }
}


function loadPrimaryContent(id, type, scrollbarPosition) {
    var params = "{ id:" + id + ", type:'" + type + "'}";
    //console.log("id: " + id + " type:" + type + " id type:" + typeof (id) + " type type:" + typeof (type));
    if (typeof (type) == 'function') {
        debugger;
    }
    callWebService("/WS/Content.asmx/GetPrimaryContent", params, loadContentBySiteNode, ajaxFailed, scrollbarPosition);
}


//#################### Used for Notes 

function selectAll() {
    $("[name^=cb-]").each(function () {
        $(this).attr("checked", true);
    });
}

function deselectAll() {
    $("[name^=cb-]").each(function () {
        $(this).attr("checked", false);
    });
}

function bulkDelete() {
                
    var noteList = [];

    $("[name^=cb-]").each(function () {
        if ($(this).is(":checked")) {
            noteList.push($(this).val());            
        }
    });

    if (noteList.length > 0) {
        deleteNoteSelection(noteList.join());
    }
}

function deleteNoteSelection(selection) {    
    callWebService("/WS/UserPreferences.asmx/DeleteNotes", "{noteIds:'" + selection + "'}", deleteMyNoteResultHandler, ajaxFailed);
}

function deleteMyNoteResultHandler(note) {
    loadNotes();
}

function addNote(targetDoc, targetPtr) {
    var addNoteTitleIdJQuery = "#modalTitle";
    var addNoteTextIdJQuery = "#modalTextArea";
    $(addNoteTitleIdJQuery).val("");
    $(addNoteTextIdJQuery).val("");
    $("#btnModalSave").off("click").on("click", function (e) {
        const tdoc = targetDoc;
        const tptr = targetPtr;
        saveNote(tdoc, tptr);
    });
    $("#myModal").modal("toggle");
}

function noteEscape(value) {

    //escape quotes and such so that there the request doesn't break
    value = value.replace(/\\/g, "\\\\");
    value = value.replace(/"/g, "\\\"");
    value = value.replace(/'/g, "\\\'");
    return value;
}

    function saveNote(targetDoc, targetPtr) {
        //console.log( -- TargetDoc: " + targetDoc + "TargetPtr: " + targetPtr);
        var addNoteTitleIdJQuery = "#modalTitle";
        var addNoteTextIdJQuery = "#modalTextArea";

        var titleText = $(addNoteTitleIdJQuery).val();
        var noteText = $(addNoteTextIdJQuery).val();

        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', noteText:'" + noteEscape(noteText) + "', noteTitle:'" + noteEscape(titleText) + "'}";
        callWebService("/WS/UserPreferences.asmx/SaveNote", paramsString, saveNoteResultHandler, ajaxFailed);
        return false;
    }

    function updateNoteById(id) {
        
        var addNoteTitleIdJQuery = "#noteTitle-" + id;
        var addNoteTextIdJQuery = "#noteTextArea-"+id;

        var noteText = $(addNoteTextIdJQuery).val();
        var titleText = $(addNoteTitleIdJQuery).val();
        var paramsString = "{id: '" + id + "', noteText:'" + noteEscape(noteText) + "', noteTitle:'" + noteEscape(titleText) + "'}";

        dataLayer.push({ 'event': 'notes' })

        callWebService("/WS/UserPreferences.asmx/UpdateNote", paramsString, updateNotesHandler, ajaxFailed);
        return false;
    }

    function updateNotesHandler(note) {
        var addNoteHeaderIdJQuery = "#editHeader-" + note.Id;
        var addNoteTitleIdJQuery = "#editTitle-" + note.Id;
        
        var addNoteTextIdJQuery = "#editText-" + note.Id;

        $(addNoteHeaderIdJQuery).text(note.Title);
        $(addNoteTitleIdJQuery).text(note.Title);
        $(addNoteTextIdJQuery).text(note.Text);
    }

    function saveNoteResultHandler(note) {
        var containerId = note.TargetDoc + "-" + note.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        //        $(containerIdJQuery + " span.editNote").show();
        //        $(containerIdJQuery + " span.addNote").hide();
        $("#leftcol").contents().find(containerIdJQuery + " span.editNote").show();
        //$("#leftcol").contents().find(containerIdJQuery + " span.addNote").hide();

    }

    function deleteMyNote(id) {
        callWebService("/WS/UserPreferences.asmx/DeleteNote", "{id:" + id + "}", function () {  $("#liNote-" + id).remove(); }, ajaxFailed);
    }

    function deleteNote(targetDoc, targetPtr, id, index) {
        var editDivId = "edit-div-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var editDivIdJQuery = "#" + editDivId.replace(/\./g, "\\.");

        var paramsForCallback = {};
        paramsForCallback.TargetDoc = targetDoc;
        paramsForCallback.TargetPtr = targetPtr;

        callWebService("/WS/UserPreferences.asmx/DeleteNote", "{id:" + id + "}", deleteNoteResultHandler, ajaxFailed, paramsForCallback);

        $("#leftcol").contents().find(editDivIdJQuery).fadeOut();
        $("#leftcol").contents().find(editDivIdJQuery).remove();
    }

    function deleteNoteResultHandler(returnFromWS, note) {
        var containerId = note.TargetDoc + "-" + note.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        callWebService("/WS/UserPreferences.asmx/GetNotes", "{targetDoc:'" + note.TargetDoc + "', targetPtr:'" + note.TargetPtr + "'}", updateEditNoteButton, ajaxFailed, note);

        $("#leftcol").contents().find(containerIdJQuery + " span.addNote").show();
    }

    function updateEditNoteButton(notes, params) {
        var containerIdJQuery = "#" + (params.TargetDoc + "-" + params.TargetPtr).replace(/\./g, "\\.");
        if (notes.length == 0)
            $("#leftcol").contents().find(containerIdJQuery + " span.editNote").hide();
    }

    //***************************************
    // My Notes
    //***************************************

    function editNote(targetDoc, targetPtr) {
        callWebService("/WS/UserPreferences.asmx/GetNotes", "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "'}", getNoteResultHandler, ajaxFailed);

    }

    function getCopyrightYear() {
        var year = new Date().getFullYear();
        $('#copyright').html( year );
    }

    function buildNoteModal(note){
        
        
        var html = "<div class=\"modal fade\" id=\"myModal-"+note.Id+"\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel-"+note.Id+"\" aria-hidden=\"true\">";
        html += "<div class=\"modal-dialog\">" +
            "<div class=\"modal-content\">" +
            "<div class=\"modal-header\">" +
            "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
            "<h3 class=\"modal-title\" id=\"myModalLabel\">Edit Note</h3>" +
            "</div>" +
            "<div class=\"modal-body\">" +
            "<label>Title*</label>" +
            "<br>" +
            "<input id=\"noteTitle-"+note.Id+"\" type=\"text\" placeholder=\"Title is required\" value=\""+note.Title+"\">" +
            "<br>" +
            "<label>Note Contents</label>" +
            "<br>" +
            "<textarea id=\"noteTextArea-"+note.Id+"\">"+note.Text+"</textarea>" +
            "</div>" +
            "<div class=\"modal-footer\">" +
            "<button type=\"button\" id=\"btnModalSave-" + note.Id + "\" class=\"btn btn-primary\" data-dismiss=\"modal\"  onclick=\"updateNoteById(" + note.Id + ")\" >Save</button>" +
            "<button type=\"button\" id=\"btnModalClose-"+note.Id+"\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>" +
            "</div>" +
            "</div>" +
            "</div>" +
            "</div>";
        return html;
    }

    function getNoteResultHandler(notes) {
        if ($("#NoteInsert").length) {
            $("#NoteInsert").remove();
        }
        if (notes.length == 1) {
            
            var note = notes[0];
            html = buildNoteModal(note);
    
            html = "<div id='NoteInsert'>" + html + "</div>";
            $("#document-container-left").append(html);
            $("#myModal-"+note.Id).modal("toggle");   
        } else {



            var html = "";
            var editors = "";
            html += "<div class='modal fade' id='myNotes' tabindex='-1' role='dialog' aria-labelledby='myNotesLabel' aria-hidden='true'>";
            html += "<div class='modal-dialog'>";
            html += "<div class='modal-content'>";
            html += "<div id='notesDivHeader' class='modal-header'>";
            html += "<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
            html += "<h3 class='modal-title' id='H1'>Notes</h3>";
            html += "</div>";





            for (var i = 0; i < notes.length; i++) { 
                var note = notes[i];
                
                var title = note.Title;
                if (title.length == 0) {
                    title = "[Empty]";
                }
                html += "<li><a href=\"#\" class=\"\" data-toggle=\"modal\" data-target=\"#myModal-" + note.Id + "\">" + title + "</a></li>";
                editors += buildNoteModal(note);
            }

            html += "<div class='modal-footer'>";
            html += "<button type='button' class='btn btn-default' data-dismiss='modal'>Cancel</button>";
            html += "</div>";
            html += "</div>";
            html += "</div>";
            html += editors;

            html = "<div id='NoteInsert'>" + html + "</div>";
            $("#document-container-left").append(html);
            $("#myNotes").modal("toggle");   

        }
    }





    //#############################  BOOKMARKS

    function deleteMyBookmarkResultHandler() {
        loadBookmarks();
    }

    function deleteMyBookmark(id) {
        callWebService("/WS/UserPreferences.asmx/DeleteBookmarkByID", "{id:" + id + "}",
        function () {
            $("#liBookmark-" + id).remove();
        }, ajaxFailed);
    }

    function addPageBookmarkResultHandler(bookmark) {
        //Update bookmark buttons
        setBookmarkButtons(true);
    }

    function deletePageBookmarkResultHandler(bookmark) {
        //Update bookmark butons
        setBookmarkButtons(true);
    }

    function addBookmarkResultHandler(bookmark) {
        var containerId = bookmark.TargetDoc + "-" + bookmark.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        $("#leftcol").contents().find(containerIdJQuery + " span.deleteBookmark").show();
        $("#leftcol").contents().find(containerIdJQuery + " span.addBookmark").hide();
    }


    function closeAdd(targetDoc, targetPtr) {
        var addDivId = "add-div-" + targetDoc + "-" + targetPtr;
        var addDivIdJQuery = "#" + addDivId.replace(/\./g, "\\.");

        var addTextAreaId = "add-textarea-" + targetDoc + "-" + targetPtr;
        var addTextAreaIdJQuery = "#" + addTextAreaId.replace(/\./g, "\\.");

        var addTextInputId = "add-textinput-" + targetDoc + "-" + targetPtr;
        var addTextInputIdJQuery = "#" + addTextInputId.replace(/\./g, "\\.");

        //        $(addTextAreaIdJQuery).val("");
        //        $(addDivIdJQuery).fadeOut();
        $("#leftcol").contents().find(addTextAreaIdJQuery).val("");
        $("#leftcol").contents().find(addTextInputIdJQuery).val("");

        $("#leftcol").contents().find(addDivIdJQuery).fadeOut();

        return false;
    }

    function addBookmark(targetDoc, targetPtr, bookmarkTitle) {
        if (typeof bookmarkTitle == 'undefined')
            bookmarkTitle = '';

        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("/WS/UserPreferences.asmx/SaveBookmark", paramsString, addBookmarkResultHandler, ajaxFailed);
    }


    function deleteBookmark(targetDoc, targetPtr) {
        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("/WS/UserPreferences.asmx/SaveBookmark", paramsString, addBookmarkResultHandler, ajaxFailed);
    }



    function addPageBookmark(id, type, bookmarkTitle) {
        if (typeof bookmarkTitle == 'undefined')
            bookmarkTitle = '';

        var paramsString = "{id:'" + id + "', type:'" + type + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("/WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);

        //    loadTemplate("/WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "templates/mybookmarks.html", "document-container");

    }

    function deletePageBookmark(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";
        callWebService("/WS/UserPreferences.asmx/DeleteBookmarkByBookIdDocType", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);
    }




    function enableBookmarkButtonsByIdType(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";
        callWebService("/WS/UserPreferences.asmx/GetBookmarkByBookIdDocType", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);
    }

    function enableBookmarkButtons(targetDoc, targetPtr) {
        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "'}";
        callWebService("/WS/UserPreferences.asmx/GetBookmark", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);
    }


    function enableBookmarkButtonsResultHandler(bookmark) {
        if ((bookmark == null) || 
            (bookmark != null && (bookmark.Id == 0))) {
            setDocBookmark(false);
            setAddBookmarkButton(true);
            setDeleteBookmarkButton(false);
        } else {
            setDocBookmark(true);
            setAddBookmarkButton(false);
            setDeleteBookmarkButton(true);
        }
    }


    //#############################  PREFERENCES

    function addFontSize() {
        var key = 'FontSize';
        var value = $('input[name="font-size"]:checked').val();
        var paramsString = "{preferenceKey:'" + key + "', preferenceValue:'" + value + "'}";
        callWebService("/WS/UserPreferences.asmx/AddUserPreference", paramsString, displaySavedPreferenceMessage, ajaxFailed);
    }

    function saveFontSize() {
        var value = $('input[name="font-size"]:checked').val();
        var paramsString = '{preferenceString: "FontSize*' + value + '"}';
        callWebService("/WS/UserPreferences.asmx/SaveUserPreferences", paramsString, displaySavedPreferenceMessage, ajaxFailed);
    }

    function displaySavedPreferenceMessage() {
        //loadPreferences();
        console.log('saved');
        $('#fontSaved').fadeIn( 500 ).delay( 2000 ).fadeOut( 500 );
    }

    //#############################  GO TO

    function doFafGotoDropDownChange(topicNum, subNum) {
        loadTemplate("/WS/DocumentTools.asmx/GetGotoInformation", "{topicNum:'" + topicNum + "', subNum:'" + subNum + "'}", "/templates/odp2015/goto.html", "document-container-left");
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

    //########################### Join

    function doFafJoinSectionsChange(topicNum, content, includeSubtopics) {
        loadTemplate("/WS/DocumentTools.asmx/GetJoinSectionsInformation", "{topicNum:'" + topicNum + "', content:'" + content + "', includeSubtopics:" + includeSubtopics + "}", "/templates/odp2015/join.html", "document-container-left");
    }

    function doJoinSectionsQuery(topicNum, sectionNum, content, includeSubtopics) {
        //updateCurrentTool_joinSections(topicNum, sectionNum, content, includeSubtopics);

        loadTemplate("/WS/DocumentTools.asmx/GetJoinSectionsResults", "{topicNum:'" + topicNum + "', sectionNum:'" + sectionNum + "', includeSubtopics:" + includeSubtopics + "}", "/templates/odp2015/joinResults.html", "joinSectionResults");
    }

    function doJoinSections(showSources) {
        setShowSources(showSources);
        //setPrintButton(true);

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

        //hideFAFTools(true);

        var joinSectionsUrl = queryString;

        if (getShowSources()) {
            joinSectionsUrl += "&show_sources";
            $("#sourcesPrint").attr('checked', true);
        }

        if (getShowHighlights()) {
            joinSectionsUrl += "&hilite";
        }

        var hitAnchor = "";

//        if (getActiveScreen().hitAnchor != null) {
//            joinSectionsUrl += "&hitanchor=" + getActiveScreen().hitAnchor;
//            hitAnchor = "&hitanchor=" + getActiveScreen().hitAnchor;
//        }

        $("#sourcesPrint").attr('disabled', true);
        g_lastJoinSectionsUrl = encodeURIComponent(joinSectionsUrl);

        fillContentPaneFromUrl("/Handlers/GetDocuments.ashx?show_sources=" + getShowSources() + hitAnchor + "&d_hh=" + getShowHighlights() + queryString);
    }

    // ########################## CROSS REFERENCE

    function getCrossReferenceLink(standard, topic, subtopic) {

       // hideDocumentSpecificButtons();
       // setFafCopyright();
         loadTemplate("/WS/DocumentTools.asmx/GetStandardsForCrossReference", "{standard:'" + standard + "', topic:'" + topic + "', subTopic:'" + subtopic + "'}", "/templates/odp2015/xref.html", "document-container-left");
       
    }

    function check() { }

    function getCrossReferenceResults(standard, number, topic, subtopic, section) {
//        if ((standard == '') && (topic == '')) {
//            $('#standardPrompt').css("color", "red");
//            $('#topicPrompt').css("color", "red");
//            $('#crossRefInputError'). show();
//        }
//        else {
//            updateCurrentTool_crossRef(standard, number, topic, subtopic, section); // save params in back button history

//            $('#crossRefInputError').hide();
            loadTemplate("/WS/DocumentTools.asmx/GetCrossReferenceResults", "{standard:'" + standard + "', number:'" + number + "', topic:'" + topic + "', subTopic:'" + subtopic + "', section:'" + section + "'}", "/templates/odp2015/xrefResults.html", "resultsTable");
//          }
        }

        //################ ARCHIVE

        function doFafArchiveLink() {
//            clearCurrentView();
//            hideDocumentSpecificButtons();
//            setFafCopyright();
            if (hasActiveDocument()) {
                fillContentPaneFromUrl("/Handlers/DownloadDocument.ashx?docid=" + getActiveDocumentId() + "&d_ft=" + 18);
            }
        }

        //##### WHAT LINKS HERE

        function doFafWhatLinksHereLink() {
//            clearCurrentView();
//            hideDocumentSpecificButtons();
//            setFafCopyright();            
              fillContentPaneFromUrl("/Handlers/DownloadDocument.ashx?docid=" + getActiveDocumentId() + "&d_ft=" + 17);
          }

        function doFafWhatLinksHereLink(targetDoc, targetPtr) {
            var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
            callWebService("/WS/Content.asmx/ResolveContentLink", params, function (sitenode) {
                //write these to the global variables g_TargetDoc and g_TargetPtr
                // these will be used by the loadArchiveContent
                g_TargetDoc = targetDoc;
                g_TargetPtr = targetPtr;                  
                fillContentPaneFromUrl("/Handlers/DownloadDocument.ashx?docid=" + sitenode.Id + "&d_ft=" + 17);
            }, ajaxFailed);
              
        }

//remove this
$("#printBtn").on("click", function () {
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


//remove this
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

// when you click on a search link or on the search link icon
function doSearchLink(id, type, useNewScreen) {
    setShowHighlights(true);
    window.location = "/content/" + type + "/" + id + "?search=true";
    ////setLoading(true);

    //open in new window = useNewScreen (whatever was passed to us)
    //showHighlight = true
    //scrollBarPosition can be passed as an optional third parameter
    //var result = prepareActiveScreen(useNewScreen, true, null, null, false);

    //if (result) {

        //loadPrimaryContent(id, type);
    //}
    //showInnerSearchButtons();
}




function doCosoLink(guid, domain, referringSite, cosopath) {
    //cosocollection,coso-comp,cosoframework
    
    var newDomain = [];
    var validDomains = validCosoDomains.split('~');
    for (index = 0; index < validDomains.length; index++)
    {
        if (domain.indexOf(validDomains[index]) >= 0) {
            newDomain.push(validDomains[index]);
        }
    }
    var domainstr = newDomain.join("~");
    var url = cosopath + "?hidEncPersGUID=" + guid + "&hidSourceSiteCode=" + referringSite + "&hidDomain=" + domainstr + "&hidURL=Default.aspx&prod=coso";
    var cosoWindow = window.open(url, "coso");
    cosoWindow.focus();
}

function doNextGenerationLink(guid, domain, referringSite, nextGenerationPath) {
    attngWindow = window.open(nextGenerationPath+"?hidEncPersGUID=" + guid + "&hidSourceSiteCode=" + referringSite + "&hidDomain=" + domain + "&hidURL=Default.aspx", "exacct");
    attngWindow.focus();
}

function doPreviousLocationLink(pageType) {
    if (pageType && pageType == g_pageTypes.SEARCH_RESULTS) {
        getResults();
    } else {        
        window.history.back()
    }
}

// when someone got license agreement, agreed to it, and are now trying to navigate to the original document
function doLALink(id, type) {
    //setLoading(true); 

    //open in new window = false
    //showHighlight can be passed as an optional second parameter
    //scrollBarPosition can be passed as an optional third parameter
//    var result = prepareActiveScreen(false);

    //    if (result) {
    window.location.href = '/content/'+type+'/'+id;
    //loadPrimaryContent(id, type);
        //loadPrimaryContent(id, type);
    //}
}
