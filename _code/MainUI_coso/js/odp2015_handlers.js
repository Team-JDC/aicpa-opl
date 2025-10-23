
function doTocLink(id, type) {
    console.log("doTocLink : " + id + " type:" + type);
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
    window.location.href = '/content/link/' + targetDoc + '/' + targetPtr;
}

// when someone clicks a link in the content
function doLinkRoute(targetDoc, targetPtr, useNewScreen, viewCompleteTopic, norecord) {


    if (viewCompleteTopic === undefined) {
        viewCompleteTopic = false;
    }


    
    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
    callWebService("/WS/Content.asmx/ResolveContentLink", params, loadContentBySiteNode, ajaxFailed);
  
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
    console.log("id: " + id + " type:" + type + " id type:" + typeof (id) + " type type:" + typeof (type));
    if (typeof (type) == 'function') {
        debugger;
    }
    callWebService("/WS/Content.asmx/GetPrimaryContent", params, loadContentBySiteNode, ajaxFailed, scrollbarPosition);
}


//#################### Used for Notes 
function addNote(targetDoc, targetPtr) {
    var addNoteTitleIdJQuery = "#modalTitle";
    var addNoteTextIdJQuery = "#modalTextArea";
    $(addNoteTitleIdJQuery).val("");
    $(addNoteTextIdJQuery).val("");
    $("#btnModalSave").click(function () {
        var tdoc = targetDoc;
        var tptr = targetPtr;
        saveNote(tdoc, tptr);
        $("#btnModalSave").unbind("click",this);
    });
    $("#myModal").modal("toggle");    
}

    function saveNote(targetDoc, targetPtr) {

        var addNoteTitleIdJQuery = "#modalTitle";
        var addNoteTextIdJQuery = "#modalTextArea";

        var titleText = $(addNoteTitleIdJQuery).val();
        var noteText = $(addNoteTextIdJQuery).val();
        
        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', noteText:'" + noteText + "', noteTitle:'" + titleText + "'}";
        callWebService("/WS/UserPreferences.asmx/SaveNote", paramsString, saveNoteResultHandler, ajaxFailed);
        return false;
    }

    function updateNoteById(id) {
        
        var addNoteTitleIdJQuery = "#noteTitle-" + id;
        var addNoteTextIdJQuery = "#noteTextArea-"+id;

        var noteText = $(addNoteTextIdJQuery).val();
        var titleText = $(addNoteTitleIdJQuery).val();
        var paramsString = "{id: '" + id + "', noteText:'" + noteText + "', noteTitle:'" + titleText + "'}";

        callWebService("/WS/UserPreferences.asmx/UpdateNote", paramsString, updateNotesHandler, ajaxFailed);
        return false;
    }

    function updateNotesHandler(note) {
     
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
        callWebService("/WS/UserPreferences.asmx/DeleteNote", "{id:" + id + "}", deleteMyNoteResultHandler, ajaxFailed);
    }

    function deleteMyNoteResultHandler() {
        loadNotes();
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
            "<button type=\"button\" id=\"btnModalSave-" + note.Id + "\" class=\"btn btn-primary\" data-dismiss=\"modal\"  onclick=\"updateNoteById(" + note.Id + ")\" >Save Changes</button>" +
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
html +="        <div class='modal fade' id='myNotes' tabindex='-1' role='dialog' aria-labelledby='myNotesLabel' aria-hidden='true'>";
html +="			 <div class='modal-dialog'>";
html +="				<div class='modal-content'>					        ";
html +="					<div id='notesDivHeader' class='modal-header'>";
html +="						<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>×</span></button>";
html +="						<h3 class='modal-title' id='H1'>Notes</h3>";
html +="					</div>					";
html +="										";




            for (var i = 0; i < notes.length; i++) { 
                var note = notes[i];
                
                var title = note.Title;
                if (title.length == 0) {
                    title = "[Empty]";
                }
                html += "<li><a href=\"#\" class=\"\" data-toggle=\"modal\" data-target=\"#myModal-" + note.Id + "\">" + title + "</a></li>";
                editors += buildNoteModal(note);
            }

html +="					<div class='modal-footer'>";
html +="						<button type='button' class='btn btn-default' data-dismiss='modal'>Cancel</button>					";
html +="					</div>";
html +="				</div>";
html +="		</div>";
html += editors;

html = "<div id='NoteInsert'>" + html + "</div>";
$("#document-container-left").append(html);
$("#myNotes").modal("toggle");   

        }
    }





    //#############################  BOOKMARKS

    function enableBookmarkButtons(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";

        callWebService("WS/UserPreferences.asmx/GetBookmarkByBookIdDocType", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);
    }

    function enableBookmarkButtonsResultHandler(bookmark) {
        if (bookmark == null) {
            $("#bookmarkId").hide();

        } else {
            $("#bookmarkId").hide();
        }
    }


    function deleteMyBookmarkResultHandler() {
        loadBookmarks();
    }

    function deleteMyBookmark(id) {
        callWebService("/WS/UserPreferences.asmx/DeleteBookmarkByID", "{id:" + id + "}", deleteMyBookmarkResultHandler, ajaxFailed);
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

    function addPageBookmark(id, type, bookmarkTitle) {
        if (typeof bookmarkTitle == 'undefined')
            bookmarkTitle = '';

        var paramsString = "{id:'" + id + "', type:'" + type + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("/WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", paramsString, addPageBookmarkResultHandler, ajaxFailed);

        //    loadTemplate("/WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "templates/mybookmarks.html", "document-container");

    }

    function deletePageBookmark(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";
        callWebService("/WS/UserPreferences.asmx/DeleteBookmarkByBookIdDocType", paramsString, deletePageBookmarkResultHandler, ajaxFailed);
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
        if (bookmark == null) {
            setDocBookmark(false);
            setAddBookmarkButton(true);
            setDeleteBookmarkButton(false);
        } else {
            setDocBookmark(true);
            setAddBookmarkButton(false);
            setDeleteBookmarkButton(true);
        }
    }


