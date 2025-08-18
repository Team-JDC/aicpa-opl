/* Overrides */
/// <reference path="webServiceHelpers.js" />
// <reference path="backButton.js" />


var loginCallBack = null; // used to store the login call back
var disableShowCodReferences = true;


/* Used to override loadhelp located in webservicehelpers.js */
function loadHelp() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_help, "");

    hideDocumentSpecificButtons();
    doLink('opl-help', 'opl-help', true);
    //loadTemplate("WS/HomePage.asmx/GetHelpVisibility", "{}", "templates/help.html", "document-container");
}
function clearAndLoadTemplate(url, data, template, container, cb = '', cb2 = '') {
    clearCurrentView();
    hideDocumentSpecificButtons();
    loadTemplate(url, data, template, container, cb, cb2);
}
function loadPreferences() {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        doUserLogin(loadPreferences);
        return false;
    }

    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate('WS/UserPreferences.asmx/GetUserPreferences', '{}', 'templates/preferences.html', 'document-container');
}

function loadSavedSearches() {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        doUserLogin(loadSavedSearches);
        return false;
    }

    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate('WS/SearchServices.asmx/GetSavedSearches', '{}', 'templates/savedSearches.html', 'document-container');
}


//This function loads a blank search form when user links directly to Adv search without triggering basic search first.
function loadBlankSearch() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_blankSearch, ""); // update the back button status

    hideDocumentSpecificButtons();
    loadTemplate('WS/EndecaServices.asmx/DoBlankSearch', '{}', 'templates/ethicsblankSearch.htm', 'document-container', '', setUpSearchAutocomplete);

}
// I think this function missed
function doNotesLink(recordback) {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        doUserLogin(doNotesLink);
        return false;
    }

    if (recordback) {
     $.histoy
    }

    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate("WS/UserPreferences.asmx/GetAllMyNotes", "{}", "templates/notes.html", "document-container");
}

function doBookmarkLink() {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        doUserLogin(doBookmarkLink);
        return false;
    }

    clearCurrentView();

    hideDocumentSpecificButtons();
    loadTemplate("WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "templates/mybookmarks.html", "document-container");
}

function addNote(targetDoc, targetPtr) {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        doUserLogin(function () { addNote(targetDoc, targetPtr) });
        return false;
    }

    // prepare the ids for using jquery and such
    // escape all of the periods in the targetDoc-targetPtr string
    var containerId = targetDoc + "-" + targetPtr;
    var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

    var addDivId = "add-div-" + targetDoc + "-" + targetPtr;
    var addDivIdJQuery = "#" + addDivId.replace(/\./g, "\\.");

    var addTextAreaId = "add-textarea-" + targetDoc + "-" + targetPtr;
    var addTextInputId = "add-textinput-" + targetDoc + "-" + targetPtr;

    if ($("#iframe-main").contents().find(addDivIdJQuery).length > 0) {
        // Nothing
    } else {
        var html = "<div id='" + addDivId + "' class='addNoteDiv'>";
        html += "<div class='addNoteInnerDiv' >";
        html += "<label>Title:</label><input type='text' class='addNoteTextinput' id='" + addTextInputId + "'></input>";
        html += "<textarea id='" + addTextAreaId + "' class='addNoteTextarea' ></textarea>";
        html += "<div>";
        html += "<button onclick=\"top.saveNote('" + targetDoc + "', '" + targetPtr + "')\">Save</button>";
        html += "<button onclick=\"top.closeAdd('" + targetDoc + "', '" + targetPtr + "')\">Close</button>";
        html += "</div>";
        html += "</div>";
        html += "</div>";
        $("#iframe-main").contents().find(containerIdJQuery).append(html);
    }

    fixNote(containerIdJQuery, addDivIdJQuery);
    return false;
}

function addPageBookmark(id, type) {
    if (d_referringSite == "Ethics") {
        doUserLogin(function () { addPageBookmark(id, type); });
    } else {
        $('#popupBookmark').show();
    }
}

//Should be called when the iframe document is "ready".  Created for Ethics
function doDocumentReadyMethods(siteNode) {
    ethicsCheckDescendents("{ id: " + getActiveDocumentId() + ", type: '" + getActiveDocumentType() + "', ignoreAnchors: true }",
                function () { setJoinButtons(true) }, 
                function () { setJoinButtons(false) });
    //setJoinButtons(true);

    DisablePopupContent();
    makeSearchNextPrevDraggable();
}




var drag = {
    elem: null,
    state: false,
    indivx: 0,
    indivy: 0
};
var delta = {
    x: 0,
    y: 0
};

var iframeOffset;

function makeSearchNextPrevDraggable() {
    var box = $('#headerHit');

//    box.offset({
//        left: 100,
//        top: 75
    //    });

    box.on("mousedown", function (e) {
        if (!drag.state) {
            iframeOffset = $("#iframe-main").offset();
            drag.elem = this;
            drag.state = true;
            drag.indivx = e.pageX - $(this).offset().left;
            drag.indivy = e.pageY - $(this).offset().top;
        }
        return false;
    });
}


$(document).on("mousemove", function (e) {
    if (drag.state) {
        setBoxOffset(e.pageX - drag.indivx, e.pageY - drag.indivy);
    }
});

function LogTest(functionname, delta_x, e_pageX, drag_x, delta_y, e_pageY, drag_y) {
    $('#log').html(functionname + " " + delta_x + " = " + e_pageX + " - " + drag_x + " " + delta_y + " = " + e_pageY + " - " + drag.y);
}

function setBoxOffset(leftPosition, topPosition) {
    $(drag.elem).offset({
        left: leftPosition,
        top: topPosition
    });
}

$(document).on("mouseup", function () {
    if (drag.state) {
        drag.state = false;
    }
});


function afterLoadContentBySiteNode(siteNode) {
    updateArrowsForDocument();
    loadCopyright(siteNode.Id, siteNode.Type);
    loadMyScreens();
    setMyDocumentsTab(true);
    setPrintButton(true);
    setBookmarkButtons(true);
    loadBreadCrumbForActiveDocument();
    loadFafTools();
    loadFormatOptions();
    doProcessFeatures();
    setEmailPageButton(true);
    setJoinButtons(true);
    updateTocDoc();
    $('#iframe-main').show();
    $('#document-container').hide();

    if ($('#nav-toc').length > 0) {
        $('#nav-toc a img').attr('src', 'images/toc.png');
        $('#nav-toc a img').attr('alt', 'TOC');
    }
}

function hideDocumentSpecificButtons() {
    setArrows(false);
    setFormatOptions(false);
    setCopyright();
    setPrintButton(false);
    setBookmarkButtons(false);
    hideInnerSearchButtons();
    hideTextOpButtons();
    setEmailPageButton(false);
    updateTocDoc();
}



/* Ethics-only functions */

function loadTocDocToggle() {
    if ($('#nav-toc a img').attr('alt') == 'DOC' ||
    $('#nav-toc a img').attr('alt') == 'DOCCOMPLETE') {
        $('#nav-toc a img').attr('src', 'images/toc.png');
        $('#nav-toc a img').attr('alt', 'TOC');
        gotoActiveDocument();
        //doBack();
    } else {
        //        if (g_backStack.count() > 0 && g_currentView != null && !g_currentView.isTool && getActiveDocumentVCT()) {

        if (g_backStack.count() > 0 && g_currentView != null && !g_currentView.isTool) {
            $('#nav-toc a img').attr('src', 'images/doc.png');
            $('#nav-toc a img').attr('alt', 'DOC');
        } else {
            $('#nav-toc a img').attr('src', 'images/toc.png');
            $('#nav-toc a img').attr('alt', 'TOC');
        }
        loadToc();
    }

}

function updateTocDoc() {
    $('#nav-toc a img').show();
    if (g_currentView != null && g_currentView.isTool && g_currentView.toolName == "toc" && hasActiveDocument() && getActiveDocumentVCT()) {
        $('#nav-toc a img').attr('src', 'images/doc-complete.png');
        $('#nav-toc a img').attr('alt', 'DOCCOMPLETE');
    } else if (g_currentView != null && g_currentView.isTool && g_currentView.toolName == "toc" && !hasActiveDocument()) {
        $('#nav-toc a img').hide();
    } else if (g_currentView != null && g_currentView.isTool && g_currentView.toolName == "toc") {
        $('#nav-toc a img').attr('src', 'images/doc.png');
        $('#nav-toc a img').attr('alt', 'DOC');        
    } else {
        $('#nav-toc a img').attr('src', 'images/toc.png');
        $('#nav-toc a img').attr('alt', 'TOC');
    }
}

function gotoActiveDocument() {
    if (getActiveScreenIndex() == -1) {
        doHomePageLink();
    } else {
        doScreenLink(getActiveScreenIndex());
    }
}

function saveBookmarkEthics() {
    if (hasActiveDocument()) {
        var paramsString = "{id:'" + getActiveDocumentId() + "', type:'" + getActiveDocumentType() + "', bookmarkTitle:'" + $('#addBookmarkTitleInput').val() + "'}";
        callWebService("WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", paramsString, addPageBookmarkResultHandler, ajaxFailed);
        closeAddBookmark();
    } else {
        alert('Bookmarks are not allowed on this document');
    }
}

function closeAddBookmark() {
    $('#popupBookmark').hide();
    $('#addBookmarkTitleInput').val('');
}

function tryEthicsLogin() {
    var userName = $('#UserNameTextBox').val();
    var password = $('#PasswordTextBox').val();

    if (userName != '' && password != '') {
        $.ajax({
            type: "POST",
            url: "WS/User.asmx/LogUserIn",
            data: "{Username:'" + userName + "', Password:'" + password + "', Subscription:''}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (!msg.d.userId) {
                    $('#ErrorLabel').text(msg.d.message);
                } else {
                    d_referringSite = msg.d.referringSite;
                    disableUserLogin();
                    doLoginCallBack();
                }
            },
            error: function (msg) {
                $('#ErrorLabel').text('Error contacting login service.  Please try again.');
            }
        });
    } else {
        $('#ErrorLabel').text('Please enter both an email address and a password.');
        return false;
    }
}

function isUserLoggedIn() {
    if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
        return true;
    } else return false;
}

function doUserLogin(callback) {
    $("#backgroundPopup").css("opacity", "0.7").fadeIn("slow");
    $("#popupLogin").fadeIn("slow");
    loginCallBack = callback;
}

function showEthicsRegister() {
    $('#ethicsLogin').hide();
    $('#ethicsRegister').show();
}

function doLoginCallBack() {
  if (typeof loginCallBack === 'function') loginCallBack();
  loginCallBack = null;
}

function tryEthicsRegister() {
    var userName = $('#RegUserNameTextBox').val();
    var firstName = $('#FirstNameTextBox').val();
    var lastName = $('#LastNameTextBox').val();
    var password = $('#RegPasswordTextBox').val();
    var passwordConf = $('#ConfirmPasswordTextBox').val();

    if (password != passwordConf) {
        $('#RegErrorLabel').text('Password and password confirm do not match.  Please try again.');
        return false;
    }

    if (userName != '' && firstName != '' && lastName != '' && password != '' && passwordConf != '') {
        $.ajax({
            type: "POST",
            url: "WS/User.asmx/RegisterUser",
            data: "{Username:'" + userName + "', Password:'" + password + "', FirstName:'" + firstName + "', LastName:'" + lastName + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (!msg.d.userId) {
                    $('#RegErrorLabel').text(msg.d.message);
                } else {
                    d_referringSite = msg.d.referringSite;
                    disableUserLogin();
                    doLoginCallBack();
                }
            },
            error: function (msg) {
                $('#RegErrorLabel').text('Error contacting registration service.  Please try again.');
            }
        });
    } else {
        $('#RegErrorLabel').text('Please enter information in all the fields.');
        return false;
    }
}

function sendEthicsPassword() {
    var sendUserName = $('#UserNameTextBox').val();

    if (sendUserName == '') {
        $('#ErrorLabel').text('Please enter an email address.');
        return false;
    } else {
        $('#ErrorLabel').text('Sending password to ' + sendUserName);
        $.ajax({
            type: "POST",
            url: "WS/User.asmx/SendUserPassword",
            data: "{Username: '" + sendUserName + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $('#ErrorLabel').text('Password sent.');
            },
            error: function (msg) {
                $('#ErrorLabel').text('Error contacting password service.  Please try again.');
            }
        });
    }
}

function disableUserLogin() {
    $("#backgroundPopup, #popupLogin").fadeOut("slow");
    $("#UserNameTextBox, #PasswordTextBox, #RegUserNameTextBox, #FirstNameTextBox, #LastNameTextBox, #RegPasswordTextBox, #ConfirmPasswordTextBox").val('');
    $('#ErrorLabel, #RegErrorLabel').text('');
    $('#ethicsLogin').show();
    $('#ethicsRegister').hide();
}

//Press Escape event!
$(document).on("keypress", function (e) {
    if (e.key === "Escape") disableUserLogin();
});

function removeJoinButtons() {
    $("#iframe-main").contents().find(".joinSectionsTopic").hide();
    $("#iframe-main").contents().find(".joinSectionsTopic").remove();
    $("#iframe-main").contents().find(".joinSectionsSubtopic").hide();
    $("#iframe-main").contents().find(".joinSectionsSubtopic").remove();
    $(".joinSectionsTopic").hide();
    $(".joinSectionsSubtopic").hide();
    $(".joinSectionsTopic").remove();
    $(".joinSectionsSubtopic").remove();

}
function removeJoinTableButtons() {
    $("#iframe-main").contents().find(".joinSectionsTable").hide();
    $(".joinSectionsTable").hide();
    

}

function setJoinButtons(visible) {
    if (visible) {
        $("#iframe-main").contents().find(".joinSectionsTopic").show();
        $("#iframe-main").contents().find(".joinSectionsSubtopic").show();
        $(".joinSectionsTopic").show();
        $(".joinSectionsSubtopic").show();
    } else {
        $("#iframe-main").contents().find(".joinSectionsTopic").hide();
        $("#iframe-main").contents().find(".joinSectionsSubtopic").hide();
        $("#iframe-main").contents().find(".joinSectionsTopic").remove();
        $("#iframe-main").contents().find(".joinSectionsSubtopic").remove();
        $(".joinSectionsTopic").hide();
        $(".joinSectionsSubtopic").hide();
        $(".joinSectionsTopic").remove();
        $(".joinSectionsSubtopic").remove();

    }
}
function setJoinTableButtons(visible) {
    if (visible) {
        $("#iframe-main").contents().find(".joinSectionsTable").show();
        $(".joinSectionsTable").show();
    } else {
        $("#iframe-main").contents().find(".joinSectionsTable").hide();
        $(".joinSectionsTable").hide();
    }
}

function ethicsJoinSections(sitenode) {    
    getActiveScreen().viewCompleteTopic = true;    
    if (sitenode == null) {        
        ethicsJoinSectionsParams("{ id: " + getActiveDocumentId() + ", type: '" + getActiveDocumentType() + "', ignoreAnchors: true }");
    } else {
        getActiveScreen().siteNode = sitenode;
        ethicsJoinSectionsParams("{ id: " + sitenode.Id + ", type: '" + sitenode.Type + "', ignoreAnchors: true }");        
    }
}



function ethicsJoinSectionsParams(idTypeString) {
    //setToolAsCurrentView(toolName_ethicsJoinSections, idTypeString);
    setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());
    setLoading(true);

    $.ajax({
        type: "POST",
        url: "WS/Content.asmx/GetNodeAndAllDescendants",
        data: idTypeString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            ethicsJoinSectionsPart2(msg.d);
        },
        error: function (msg) {
            alert('Error joining sections.');
        }
    });
}

function ethicsJoinTables() {
    ethicsJoinTablesParams("{ id: " + getActiveDocumentId() + ", type: '" + getActiveDocumentType() + "' }");
}

function ethicsJoinTablesParams(idTypeString) {
    setToolAsCurrentView(toolName_ethicsJoinSections, idTypeString);
    setLoading(true);

    $.ajax({
        type: "POST",
        url: "WS/Content.asmx/GetNodeAndAllDescendants",
        data: idTypeString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            ethicsJoinTablesPart2(msg.d);
        },
        error: function (msg) {
            alert('Error joining sections.');
        }
    });
}


function ethicsCheckDescendents(idTypeString, hasDescendentsCB, hasNoDescendentsCB) {
    $.ajax({
        type: "POST",
        url: "WS/Content.asmx/HasDescendants",
        data: idTypeString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d) {
                // Has descendents to Join 
                if (hasDescendentsCB)
                    hasDescendentsCB();

            } else {
                // doesn't have descendents
                if (hasNoDescendentsCB)
                    hasNoDescendentsCB();
            }
        },
        error: function (msg) {
            alert('Error joining sections.');
        }
    });
}

function ethicsJoinSectionsPart2(listOfNodes) {
    var queryString = "";
    var count = 0;
    
    for (i = 0; i < listOfNodes.length; i++) {
        var currentId = listOfNodes[i].SiteNode.Id;
        var currentType = convertDocTypeToEnumVal(listOfNodes[i].SiteNode.Type);
        //GetDocuments will skip DocumentAnchors, so I am pulling them out here to make the 
        // string smaller. I also convert the string to the enum value
        if (currentType != 6) {
            queryString += "&id=" + currentId + "&type=" + currentType;
            count++;
        }
    }

    

    $('#backup-document-container')[0].innerHTML = "";
    $('#backup-document-container').hide();

    if (count > 1) {
        queryString += "&joinsecs=true";
        filliFrameFromUrl("GetDocuments.ashx?" + queryString.substring(1), function () {
            setLoading(false);
            removeJoinButtons(); //deletes them from the page
//            loadMyScreens();
            
//            setJoinButtons(false);
//            setJoinTableButtons(true);
//            setArrows(true);
            afterLoadContentBySiteNode(getActiveScreen().siteNode);
        });
    } else if (count == 1) {
        filliFrameFromUrl("GetDocument.ashx?" + queryString.substring(1), function () {
            setLoading(false);
            removeJoinButtons();
            getActiveScreen().viewCompleteTopic = false;    
//            loadMyScreens();

//            setJoinButtons(false);
//            setJoinTableButtons(true);
//            setArrows(true);
            afterLoadContentBySiteNode(getActiveScreen().siteNode);
        });
//        $('#document-container').load("Handlers/GetDocument.ashx?" + queryString.substring(1), function () {
//            $('#iframe-main').hide();
//            $('#document-container').show();
//            setLoading(false);
//            setJoinButtons(false);
//            setJoinTableButtons(true);
//        });
    } else {
        alert('No child documents found.');
        setLoading(false);
    }
}

function convertDocTypeToEnumVal(docType) {
    if (docType == "Site")
        return 1;
    if (docType == "Book")
        return 2;
    if (docType == "Document")
        return 3;
    if (docType == "Format")
        return 4;
    if (docType == "BookFolder")
        return 5;
    if (docType == "DocumentAnchor")
        return 6;
    if (docType == "SiteFolder")
        return 7;
    return 0;
}

function ethicsJoinTablesPart2(listOfNodes) {
    var queryString = "";

    for (i = 0; i < listOfNodes.length; i++) {
        var currentId = listOfNodes[i].SiteNode.Id;
        var currentType = convertDocTypeToEnumVal(listOfNodes[i].SiteNode.Type);                
        //GetDocuments will skip DocumentAnchors, so I am pulling them out here to make the 
        // string smaller. I also convert the string to the enum value
        if (currentType != 6) {
            queryString += "&id=" + currentId + "&type=" + currentType + "&tableStyle=ethics_revision_history";            
        }       
        
    }



    $('#backup-document-container')[0].innerHTML = "";
    $('#backup-document-container').hide();

    if (listOfNodes.length > 1) {
        queryString += "&joinsecs=true";
        filliFrameFromUrl("GetDocuments.ashx?" + queryString.substring(1), function () {
            setLoading(false);
            removeJoinTableButtons();
            setJoinButtons(true);
            setJoinTableButtons(false);
            setArrows(false);
        });
    } else if (listOfNodes.length == 1) {
    $('#document-container').load("GetDocument.ashx?" + queryString.substring(1), function () {
        $('#iframe-main').hide();
        $('#document-container').show();
        setLoading(false);
        setJoinButtons(true);
        setJoinTableButtons(false);
    });
    } else {
        alert('No child documents found.');
        setLoading(false);
    }
}

function setEmailPageButton(visible) {
    if (visible) {
        $("#Toolbar-Tools-EmailPage-Top").show();
        $("#Toolbar-Tools-Link-Top").show();
    } else {
        $("#Toolbar-Tools-EmailPage-Top").hide();
        $("#Toolbar-Tools-Link-Top").hide();
    }
}

function loadHomePage() {
    setToolAsCurrentView(toolName_homePage, "");
    loadTemplate("WS/HomePage.asmx/GetHomePageData", "{}", "templates/ethicshomePage.htm", "document-container");
}

function loadQuickFind() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_quickFind, "");


    hideDocumentSpecificButtons();
    fillDocumentContainerFromUrl("static/ethicsquickFind.htm");
}

function loadPdf() {
    fillContentPaneFromUrl("ethicsresources/et-cod.pdf");
}

function OnReadyEvent() {
    SetDisableEnableDefinitionPopupCheckbox();

    $("#disableDefPopupId").on("change", function () {
        if (this.checked) {
            $.cookie("disablepopup", "true", { expires: 30 });
        } else {
            $.cookie("disablepopup", "", { expires: 30 });
        }

        DisablePopupContent();
    });

}

function DisablePopupContent() {
    var disablepopup = $.cookie("disablepopup");

    if (disablepopup) {
        $("#iframe-main").contents().find(".tooltip .popupSpan").addClass("popupSpanDisabled").removeClass("popupSpan");
    }
    else {
        $("#iframe-main").contents().find(".tooltip .popupSpanDisabled").addClass("popupSpan").removeClass("popupSpanDisabled");
    }
}

function SetDisableEnableDefinitionPopupCheckbox() {

    var disablepopup = $.cookie("disablepopup");

    if (disablepopup) {
        //        $("#disableDefPopupId").prop("checked", true);
        $("#disableDefPopupId").attr("checked", "checked")
    }
    else {
        //        $("#disableDefPopupId").prop("checked"), false);
        $("#disableDefPopupId").attr("checked", "")
    }
}


function gotoPreviousDocument() {
    var result = prepareActiveScreen(false, false);

    if (result) {        
        // Load new page in dc
        callWebService("WS/Content.asmx/GetPreviousDocumentEthics", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "', vct:" + getActiveScreen().viewCompleteTopic + "}", loadContentBySiteNodePrevious, ajaxFailed);
        getActiveScreen().viewCompleteTopic = false;
    }
}

function gotoNextDocument() {
    var result = prepareActiveScreen(false, false);

    if (result) {        
        // Load new page in dc
        callWebService("WS/Content.asmx/GetNextDocumentEthics", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "', vct:" + getActiveScreen().viewCompleteTopic + "}", loadContentBySiteNodeNext, ajaxFailed);
        getActiveScreen().viewCompleteTopic = false;
        
    }
}
function doToolbarHomeNavLink(siteFolderName) {
    if (siteFolderName == '') {
        doLink("et-cod", "et-cod", "true")
    }
    else {
        doSiteFolderLink(siteFolderName, true);
    }
}
function doEmailLink() {
    var title = "AICPA Code of Conduct Content Link";
    // getActiveScreen().siteNode.Title
    var link = getEmailLink();
    var href = "mailto:?subject=" + encodeURIComponent(title) + "&body=" + encodeURIComponent(link);
    window.open(href,'_blank');
}

function loadFormatOptions() {
    if (hasActiveDocument() && getActiveDocumentType() == "Document") {
        loadTemplate("WS/Content.asmx/GetDocumentFormats", "{id:" + getActiveDocumentId() + "}", "templates/ethicsformatOptions.html", "formatOptionsContainer");
        $("#formatOptionsContainer").show();
    }
    else {
        $("#formatOptionsContainer").hide();
    }

}

