//***********************************************************************************
// Currently the convention is to create a handler function with a unique name that
// will handle- an html event (an html event being onclick, onchange, onfocus).
// that function should be located here.  So, as you can see below, there are
// multiple functions that handle various events that we handle here.  
//***********************************************************************************

//***********************************************************************************
// onclick handlers
//***********************************************************************************

    //*******************************************************************************
    // backable clicks
    //*******************************************************************************

    // when you click on a search link or on the search link icon
function doSearchLink(id, type, useNewScreen) {
    console.log(id, type)
        //setLoading(true);
        updateHitDocButtons(id, type);
        
        //open in new window = useNewScreen (whatever was passed to us)
        //showHighlight = true
        //scrollBarPosition can be passed as an optional third parameter
            var result = prepareActiveScreen(useNewScreen, true, null, null, false);

        if (result) {

            loadPrimaryContent(id, type);
        }
        showInnerSearchButtons();
    }

    function doHitLink(id, type, useNewScreen, hitAnchor) {
        //setLoading(true);
        updateHitDocButtons(id, type);

        //open in new window = useNewScreen (whatever was passed to us)
        //showHighlight = true
        //scrollBarPosition can be passed as an optional third parameter
        //hitAnchor 
        var result = prepareActiveScreen(useNewScreen, true, null, hitAnchor, false);

        if (result) {

            loadPrimaryContent(id, type);
        }
        showInnerSearchButtons();
    }

    function updateHitDocButtons(id,type) {
        doNextHitDoc(id, type);
        doPrevHitDoc(id, type);        
    }

    //function doAdvancedNavigationalSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed) {
    function doNextHitDoc(id, type) {
        if (id && type)
            $.ajax({
                type: "POST",
                url: "WS/EndecaServices.asmx/EndecaNextHitDoc",
                dataType: "json",
                data: JSON.stringify({ id: id, type: type }),
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d.Id != -1) {
                        $('#nextHitDoc').removeClass("disabled");
                        //$('#nextHitDoc').unbind("click");
                        //$('#nextHitDoc').click(function () {
                        //    doSearchLink(data.d.Id, data.d.Type, false);
                        //});
                        $('#nextHitDoc').off("click").on("click", function () {
                            doSearchLink(data.d.Id, data.d.Type, false);
                        });
                    } else {
                        $('#nextHitDoc').addClass("disabled");
                        $('#nextHitDoc').off("click");
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
    }

    function doPrevHitDoc(id,type) {
        if (id && type)
            $.ajax({
                type: "POST",
                url: "WS/EndecaServices.asmx/EndecaPrevHitDoc",
                dataType: "json",
                data: JSON.stringify({ id: id, type: type }),
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d.Id != -1) {
                        $('#prevHitDoc').removeClass("disabled");
                        //$('#prevHitDoc').unbind("click");
                        //$('#prevHitDoc').click(function () {
                        //    doSearchLink(data.d.Id, data.d.Type, false);
                        //});
                        $('#prevHitDoc').off("click").on("click", function () {
                            doSearchLink(data.d.Id, data.d.Type, false);
                        });

                    } else {
                        $('#prevHitDoc').addClass("disabled");
                        $('#prevHitDoc').off("click");
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
    }



    // when someone clicks on the current document icon after clicking the "+" button
    // on the my documents panel
    function openCurrentAsNewDocument() {
        //setLoading(true); 

        if (hasActiveDocument()) {
            var id = getActiveDocumentId();
            var type = getActiveDocumentType();
            var highlight = getActiveScreen().showHighlight;
            var hitAnchor = getActiveScreen().hitAnchor;
            var viewCompleteTopic = getActiveScreen().viewCompleteTopic;
            var siteNode = getActiveScreen().siteNode;
            //open in new window = true
            //showHighlight = highlight (use the value of the previous screen)
            //scrollBarPosition can be passed as an optional third parameter
            var result = prepareActiveScreen(true, highlight);

            if (result) {
                if (viewCompleteTopic) {
                    // create bogus SiteNode so it can be passed to the joinSections
                    ethicsJoinSections(siteNode);
                } else {
                    loadPrimaryContent(id, type);
                    
                }
            }
        }
        else {
            // we should preven this link from showing in the first place
            // but in case they get here, we can just redirect to the home page
            loadHomePage();
        }
    }

    // shows the next document
    function gotoNextDocument() {
        /* WITHOUT BACKUP CONTAINER
        $('#document-container').animate({ marginLeft: $('#document-container').width() * -1 - 100 }, 1500, "swing", function () {
            //open in new window = false
            //showHighlight = false
            //scrollBarPosition can be passed as an optional third parameter
            prepareActiveScreen(false, false);

            // Load new page in dc
            callWebService("WS/Content.asmx/GetNextDocument", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "'}", loadContentBySiteNodeNext, ajaxFailed);
        });

        /* WITH BACKUP CONTAINER
        */
        // Hide current dc and show backupdc
        //$('#backup-document-container').html($('#document-container > *'));
        //$('#document-container').hide();
        //$('#backup-document-container').show();

// 2010-08-06 sburton taken out
//        $('#backup-document-container').attr('id', 'document-container-temp');
//        $('#document-container').attr('id', 'backup-document-container');
//        $('#document-container-temp').attr('id', 'document-container');

        //open in new window = false
        //showHighlight = false
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(false, false);

        if (result)
        {
            // Load new page in dc
            callWebService("WS/Content.asmx/GetNextDocument", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "'}", loadContentBySiteNodeNext, ajaxFailed);
        }
    }

    // shows the previous document
    function gotoPreviousDocument() {
        /* WITHOUT BACKUP CONTAINER
        $('#document-container').animate({ marginLeft: $('#document-container').width() + 100 }, 1500, "swing", function () {
            //open in new window = false
            //showHighlight = false
            //scrollBarPosition can be passed as an optional third parameter
            prepareActiveScreen(false, false);

            // Load new page in dc
            callWebService("WS/Content.asmx/GetPreviousDocument", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "'}", loadContentBySiteNodePrevious, ajaxFailed);
        });
        */

        /* WITH BACKUP CONTAINER
        */
        // Hide current dc and show backupdc
        //$('#backup-document-container').html($('#document-container > *'));
        //$('#document-container').hide();
        //$('#backup-document-container').show();

        // 2010-08-06 sburton taken out
//        $('#backup-document-container').attr('id', 'document-container-temp');
//        $('#document-container').attr('id', 'backup-document-container');
//        $('#document-container-temp').attr('id', 'document-container');

        //open in new window = false
        //showHighlight = false
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(false, false);

        if (result)
        {
            // Load new page in dc
            callWebService("WS/Content.asmx/GetPreviousDocument", "{id:" + getActiveDocumentId() + ",type:'" + getActiveDocumentType() + "'}", loadContentBySiteNodePrevious, ajaxFailed);
        }
    }

    // when someone clicks on an item in a drop down breadcrumb list
    function doBreadcrumbLink(id, type) {
        //setLoading(true); 

        if (id != -1) {
            //open in new window = false
            //showHighlight = false
            //scrollBarPosition can be passed as an optional third parameter
            //hitanchor = null
            //ViewCompleteTopic = false
            var result = prepareActiveScreen(false, false, null, null, false);

            if (result)
            {
                loadPrimaryContent(id, type);
            }
        }
    }

    //when someone clicks on a link from the home page
    function doHomePageContentLink(id, type, viewCompleteTopic) {
        if (viewCompleteTopic === undefined) {
            viewCompleteTopic = false;
        }
        //setLoading(true); 

        //open in new window = true
        //showHighlight = false
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(true, false);

        if (result) {
            if (viewCompleteTopic) {
                getActiveScreen().viewCompleteTopic = true;    
                ethicsJoinSectionsParams("{ id: " + id + ", type: '" + type + "' }");                
            } else {
                loadPrimaryContent(id, type);
            }
        }
    }

    function doSiteFolderTitlePageLink(id, type) {
        var hash = $.History.setHash("#/sitefolder?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
        //var hash = $.History.setState("#/sitefolder?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
        console.log("hash set as: " + hash);
    }

    function doSiteFolderTitlePageLinkRoute(id, type) {
        //setLoading(true);
        
        //open in new window = false
        //showHighlight = false
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(false, false);

        if (result)
        {
            loadPrimaryContent(id, type);
        }
    }

    function doNextGenerationLink(guid, domain, referringSite) {
        attngWindow = window.open("/exacct/ResourceSeamlessLogin.aspx?hidEncPersGUID=" + guid + "&hidSourceSiteCode=" + referringSite + "&hidDomain=" + domain + "&hidURL=Default.aspx", "exacct");
        attngWindow.focus();
    }

    function doCosoLink(guid, domain, referringSite) {
    //cosocollection,coso-comp,cosoframework
        var newDomain = [];
        if (domain.indexOf("cosocollection") >= 0) {
            newDomain.push("cosocollection");
        } else if (domain.indexOf("coso-comp") >= 0) {
            newDomain.push("coso-comp");
        } else if (domain.indexOf("cosoframework")>=0) {
            newDomain.push("cosoframework");
        }
        var domainstr = newDomain.join(";");

        var url = "/coso/ResourceSeamlessLogin.aspx?hidEncPersGUID=" + guid + "&hidSourceSiteCode=" + referringSite + "&hidDomain=" + domainstr + "&hidURL=Default.aspx&prod=coso";
        var cosoWindow = window.open(url, "coso");
        cosoWindow.focus();
    }


    //when someone clicks one of the links on the bottom home nav toolbar
    function doToolbarHomeNavLink(siteFolderName) {
        doSiteFolderLink(siteFolderName, true);
    }

    function doTocLink(id, type) {
        var hash = $.History.setHash("#/content?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
        //var hash = $.History.setState("#/content?id=" + siteNode.Id + "&type='" + siteNode.Type + "'");
        console.log("doTocLinkRoute:hash set as: " + hash);        
    }

    // when someone clicks a link from the table of contents
    function doTocLinkRoute(id, type) {
        //setLoading(true); 

        //open in new window = true
        //showHighlight = false
        //scrollBarPosition can be passed as an optional third parameter
        updateTocView(id, type);
        var result = prepareActiveScreen(true, false);

        if (result)
        {
            loadPrimaryContent(id, type);
        }
    }

    // when someone got license agreement, agreed to it, and are now trying to navigate to the original document
    function doLALink(id, type) {
        //setLoading(true); 

        //open in new window = false
        //showHighlight can be passed as an optional second parameter
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(false);

        if (result)
        {
            loadPrimaryContent(id, type);
        }
    }

    //a special kind of link that exist in a small set of content
    //this link will check to see if a tab exists for the book it is linking to,
    //if so it will use that tab rather than open a new one.
    function doAratLink(targetDoc, targetPtr) {
        //setLoading(true);
        var foundScreen = false;
        var foundIndex = -1;
        for (var index = 0; index < getMyScreenCount(); index++) {
            var testTargetDoc = getMyScreens()[index].targetDoc;
            if (testTargetDoc == targetDoc) {
                foundScreen = true;
                foundIndex = index;
            }
        }
        if (foundScreen) {
            setActiveScreenIndex(foundIndex);
            doLink(targetDoc, targetPtr, false);
            //var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
            //callWebService("WS/Content.asmx/ResolveContentLink", params, loadContentBySiteNode, ajaxFailed);
        }
        else {
            doLink(targetDoc, targetPtr, true);
        }
        
        //var result = prepareActiveScreen(useNewScreen);

        //if (result) {
        //    var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
        //    callWebService("WS/Content.asmx/ResolveContentLink", params, loadContentBySiteNode, ajaxFailed);
       // }
    }

    function doLink(targetDoc, targetPtr, useNewScreen, viewCompleteTopic, norecord) {
        var hash = $.History.setHash("#/content?td='" + targetDoc + "'&tp='" + targetPtr + "'");
        //var hash = $.History.setState("#/content?td='" + targetDoc + "'&tp='" + targetPtr + "'");
        console.log("doLinkRoute: hash set as: " + hash);
    }
    
    // when someone clicks a link in the content
    function doLinkRoute(targetDoc, targetPtr, useNewScreen, viewCompleteTopic, norecord) {
        useNewScreen = false;
        var result = prepareActiveScreen(useNewScreen);
        getActiveScreen().targetDoc = targetDoc;

        //setLoading(true);
        if (!useNewScreen) {
            getActiveScreen().showHighlight = false;
        }

        //getActiveScreen().viewCompleteTopic = false;
        if (!useNewScreen) {
            getActiveScreen().viewCompleteTopic = false;
        }

        if (viewCompleteTopic === undefined) {
            viewCompleteTopic = false;
        }


        if (result) {
            getActiveScreen().viewCompleteTopic = viewCompleteTopic;
            var params = "{ targetDoc:'" + targetDoc + "', targetPointer:'" + targetPtr + "'}";
            if (viewCompleteTopic) {
                callWebService("WS/Content.asmx/ResolveContentLink", params, ethicsJoinSections, ajaxFailed);
            } else {                
                callWebService("WS/Content.asmx/ResolveContentLink", params, loadContentBySiteNode, ajaxFailed);
            }
        }
    }

    function doSiteFolderLink(siteFolderName, useNewScreen) {
        //var hash = $.History.setHash("#/folder?fn=" + encodeURIComponent("'" + siteFolderName + "'"));
        //var hash = $.History.setState("#/folder?fn=" + encodeURIComponent("'" + siteFolderName + "'"));        
        var hash = "/folder?fn=" + encodeURIComponent(siteFolderName);
        console.log("hash set as: " + hash);
        $.History.go(hash);
        //$.History.trigger(hash);
        //window.location.href = "default.aspx#"+hash;
        //useNewScreen = false;
        //doSiteFolderLinkRoute(siteFolderName, useNewScreen);
    }

    function doSiteFolderLinkRoute(siteFolderName, useNewScreen) {
        //setLoading(true);
        
        var result = prepareActiveScreen(useNewScreen);

        if (result)
        {
            var params = "{ folderName:\"" + siteFolderName + "\"}";
            callWebService("WS/Content.asmx/ResolveSiteFolder", params, loadContentBySiteNode, ajaxFailed);
        }
    }

    // when you click on a link that goes to the homepage
    function doHomePageLink() {
        loadMyScreens();
        loadLibraryBooks();        
        loadHomeNavToolbar();
        hideDocumentSpecificButtons();
        loadHomePage();
        // loadRoot = true
        loadBreadCrumbForActiveDocument(true);
        loadFafTools();
    }

    // when you click the clear button next to the back button and the results and search buttons.
    function doClearHitHighlighting() {
        // sburton 2010-08-06: removed unused functionality
    }

    // when you click on one of the screens in the myScreens (myDocuments) panel
    function doScreenLink(screenIndex) {
        //setLoading(true);

        var oldIndex = getActiveScreenIndex();

        //open in new window = false
        //showHighlight can be passed as an optional second parameter
        //scrollBarPosition can be passed as an optional third parameter
        var result = prepareActiveScreen(false);

        //set the index of the ActiveScreen
        setActiveScreenIndex(screenIndex);

        

        if (result) {
            if (getActiveDocumentVCT()) {
                ethicsJoinSections(getActiveScreen().siteNode);
            } else {
                loadPrimaryContent(getActiveDocumentId(), getActiveDocumentType(), getActiveScreen().scrollbarPosition);
            }
        }
        else
        {
            // Undo the setting of the active screen index
            setActiveScreenIndex(oldIndex);
        }
    }

    function doPreviousLocationLink(pageType) {
        if (pageType && pageType == g_pageTypes.SEARCH_RESULTS) {
            getResults();
        } else {
            doBackButtonLink();
        }
    }
    
    // when you click the back button
    function doBackButtonLink() {
        doBack();
    }

    function doLinkToForum() {
        callWebService("WS/Toolbars.asmx/GetForumData", "{}", forumDataCallback, ajaxFailed);
    }

    function forumDataCallback(forumData) {
        $("#forumForm")[0].action = forumData.PageUrl;
        $("#forumFormGuid")[0].value = forumData.UserGuid;

        $("#forumForm")[0].submit();
    }

    //***************************************
    // Add Note and Bookmarks
    //***************************************

    function addNote(targetDoc, targetPtr) {
        //var iframe = document.getElementById("iframe-main");

        // Ethics uses this to check if a user is logged in when clicking an add note link.
        if (typeof d_referringSite != 'undefined' && d_referringSite == "Ethics") {
            doUserLogin();
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

//        if (iframe.document.getElementById(addDivId)) {
        if ($("#iframe-main").contents().find(addDivIdJQuery).length > 0) {
           // $("#iframe-main").contents().find(addDivIdJQuery).fadeIn();
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
            //$("#iframe-main").contents().find(addDivIdJQuery).fadeIn();

            

//            $(containerIdJQuery).append(html);
//            $(addDivIdJQuery).fadeIn();
        }

        fixNote(containerIdJQuery, addDivIdJQuery);
        return false;
    }

    function fixMyNote(noteID) {
      
        var editDivId = "edit-div-" + noteID;

        var addDiv = $("#"+editDivId);
        //Get Container Top
        var x = addDiv.parent().offset().left;
        var y = addDiv.parent().offset().top;

        //get parent height 
        var parenth = $("#content-container").outerHeight();
        var parentw = $("#content-container").outerWidth();

        //Get scroll position of iFrame
        var scrollTop = $("#content-container").scrollTop(); //getCurrentScrollbarPosition();
        var scrollLeft = $("#content-container").scrollLeft(); //getCurrentScrollbarPosition();

        //Calculate the bottom of the note and view area.. 
        var noteBottom = y + addDiv.outerHeight();
        var viewBottom = scrollTop + parenth;
        var noteLeft = x + addDiv.outerWidth();
        var viewLeft = scrollLeft + parentw;

        //. If it is > than the view window move it up
        if (y < scrollTop) {
            y = scrollTop + 5;
            addDiv.css({ 'top': y });
        } else if (noteBottom > viewBottom) {
            y = y + (viewBottom - noteBottom - 25);
            addDiv.css({'top': y });
            //$("#iframe-main").contents().find(addDivID).css({ 'top': y });
        }

        if (x < scrollLeft) {
            x = scrollLeft + 5;
            addDiv.css({ 'left': x });
        } else if (noteLeft > viewLeft) {
            x = x + (viewLeft - noteLeft - 25);
            addDiv.css({ 'left': x });
        }

//        //Display Div.
        addDiv.fadeIn();
    }


    function fixNote(containerID, addDivID) {
        var container = $("#iframe-main").contents().find(containerID);
        

        var addDiv = $("#iframe-main").contents().find(addDivID);
        //Get Container Top
        var x = container.offset().left;
        var y = container.offset().top;

        //get parent height
        var parenth = $("#iframe-main").outerHeight();
        var parentw = $("#iframe-main").outerWidth();

        //Get scroll position of iFrame
        var iframeTop = getCurrentScrollbarPosition();
        var iframeLeft = $("#iframe-main").contents().find("body").scrollLeft();
        //Calculate the bottom of the note and view area.. 
        var noteBottom = y + addDiv.outerHeight();
        var viewbottom = iframeTop + parenth;
        var noteLeft = x + addDiv.outerWidth();
        var viewLeft = iframeLeft + parentw;

        //. If it is > than the view window move it up
        if (y < iframeTop) {
            y = iframeTop + 5;            
            $("#iframe-main").contents().find(addDivID).css({ 'top': y });
        } else if (noteBottom > viewbottom) {
            y = y + (viewbottom - noteBottom - 25);
            $("#iframe-main").contents().find(addDivID).css({ 'top': y });
        }
        if (x < iframeLeft) {
            x = iframeLeft + 5;
            $("#iframe-main").contents().find(addDivID).css({ 'left': x });
        } else if (noteLeft > viewLeft) {
            x = x + (viewLeft - noteLeft - 25);
            $("#iframe-main").contents().find(addDivID).css({ 'left': x });
        }

        //Display Div.
        addDiv.fadeIn();
    }

    function saveNote(targetDoc, targetPtr) {
        var addTextAreaId = "add-textarea-" + targetDoc + "-" + targetPtr;
        var addTextInputId = "add-textinput-" + targetDoc + "-" + targetPtr;

        var addTextAreaIdJQuery = "#" + addTextAreaId.replace(/\./g, "\\.");
        var addTextInputIdJQuery = "#" + addTextInputId.replace(/\./g, "\\.");

        //var noteText = $(addTextAreaIdJQuery).val();
        var noteText = $("#iframe-main").contents().find(addTextAreaIdJQuery).val();
        var titleText = $("#iframe-main").contents().find(addTextInputIdJQuery).val();

        if (titleText.trim() === "") {
            alert("Please enter some text for the title");
        }

        if (titleText.trim() === "") {
            alert("Please enter some text for the note");
            return;
        }
        
        //escape quotes and such so that there the request doesn't break
        noteText = noteText.replace(/\\/g, "\\\\");
        noteText = noteText.replace(/"/g, "\\\"");
        noteText = noteText.replace(/'/g, "\\\'");
        titleText = titleText.replace(/\\/g, "\\\\");
        titleText = titleText.replace(/"/g, "\\\"");
        titleText = titleText.replace(/'/g, "\\\'");

        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', noteText:'" + noteText + "', noteTitle:'" + titleText + "'}";

        callWebService("WS/UserPreferences.asmx/SaveNote", paramsString, saveNoteResultHandler, ajaxFailed);
        closeAdd(targetDoc, targetPtr);

        return false;
    }

    function saveNoteResultHandler(note) {
        var containerId = note.TargetDoc + "-" + note.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        //        $(containerIdJQuery + " span.editNote").show();
        //        $(containerIdJQuery + " span.addNote").hide();
        $("#iframe-main").contents().find(containerIdJQuery + " span.editNote").show();
        //$("#iframe-main").contents().find(containerIdJQuery + " span.addNote").hide();

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

        $("#iframe-main").contents().find(containerIdJQuery + " span.deleteBookmark").show();
        $("#iframe-main").contents().find(containerIdJQuery + " span.addBookmark").hide();
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
        $("#iframe-main").contents().find(addTextAreaIdJQuery).val("");
        $("#iframe-main").contents().find(addTextInputIdJQuery).val("");

        $("#iframe-main").contents().find(addDivIdJQuery).fadeOut();

        return false;
    }

    function addBookmark(targetDoc, targetPtr, bookmarkTitle) {
        if (typeof bookmarkTitle == 'undefined')
            bookmarkTitle = '';

        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("WS/UserPreferences.asmx/SaveBookmark", paramsString, addBookmarkResultHandler, ajaxFailed);
    }

    function addPageBookmark(id, type, bookmarkTitle) {
        if (typeof bookmarkTitle == 'undefined')
            bookmarkTitle = '';

        var paramsString = "{id:'" + id + "', type:'" + type + "', bookmarkTitle:'" + bookmarkTitle + "'}";

        callWebService("WS/UserPreferences.asmx/SaveBookmarkByBookIdDocType", paramsString, addPageBookmarkResultHandler, ajaxFailed);

        //    loadTemplate("WS/UserPreferences.asmx/GetAllMyBookmarks", "{}", "templates/mybookmarks.html", "document-container");

    }

    function deletePageBookmark(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";


        callWebService("WS/UserPreferences.asmx/DeleteBookmarkByBookIdDocType", paramsString, deletePageBookmarkResultHandler, ajaxFailed);

    }

    function enableBookmarkButtons(id, type) {
        var paramsString = "{id:'" + id + "', type:'" + type + "'}";

        callWebService("WS/UserPreferences.asmx/GetBookmarkByBookIdDocType", paramsString, enableBookmarkButtonsResultHandler, ajaxFailed);
    }

    function enableBookmarkButtonsResultHandler(bookmark) {
        if (bookmark == null) {
            setAddBookmarkButton(true);
            setDeleteBookmarkButton(false);
        } else {
            setAddBookmarkButton(false);
            setDeleteBookmarkButton(true);
        }
    }


    //***************************************
    // Edit Note
    //***************************************

    function editNote(targetDoc, targetPtr) {
        callWebService("WS/UserPreferences.asmx/GetNotes", "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "'}", getNoteResultHandler, ajaxFailed);

        return false;
    }

    //==================
    // Cluetip Setup
    //==================
    $.cluetip.setup({
        insertionType: 'appendTo',
        insertionElement: '#content-container'
    });

    function getNoteResultHandler(notes) {

        if (notes.length == 1) {
            var note = notes[0];

            var containerId = note.TargetDoc + "-" + note.TargetPtr;
            var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

            var editDivId = "edit-div-" + note.TargetDoc + "-" + note.TargetPtr;
            var editDivIdJQuery = "#" + editDivId.replace(/\./g, "\\.");

            var editTextAreaId = "edit-textarea-" + note.TargetDoc + "-" + note.TargetPtr;
            var editTextAreaIdJQuery = "#" + editTextAreaId.replace(/\./g, "\\.");

            var editTextInputId = "edit-textinput-" + note.TargetDoc + "-" + note.TargetPtr;
            var editTextInputJQuery = "#" + editTextInputId.replace(/\./g, "\\.");
            //        if (document.getElementById(editDivId)) {
            if ($("#iframe-main").contents().find(editDivIdJQuery).length > 0) {
                $("#iframe-main").contents().find(editTextAreaIdJQuery).val(note.Text);
                $("#iframe-main").contents().find(editTextInputJQuery).val(note.Title);
                //$("#iframe-main").contents().find(editDivIdJQuery).fadeIn();
            } else {
                var html = "<div id='edit-div-" + note.TargetDoc + "-" + note.TargetPtr + "' class='editNoteDiv' >";
                html += "<div class='editNoteInnerDiv'>";
                html += "<label>Title:</label><input type='text' class='editNoteTextinput' id='" + editTextInputId + "' value='" + note.Title + "'/>";
                html += "<textarea id='edit-textarea-" + note.TargetDoc + "-" + note.TargetPtr + "' class='editNoteTextarea' >" + note.Text + "</textarea>";
                html += "<div>";
                //html += "<a href='#' onclick=\"updateNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\"><img src='images/btn-save.gif' alt='Save Changes' border='0'></a>";
                html += "<button onclick=\"top.updateNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\">Save</button>";
                //html += "<a href='#' onclick=\"deleteNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\"><img src='images/btn-delete.gif' alt='Delete' border='0'></a>";
                html += "<button onclick=\"top.deleteNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\">Delete</button>";
                html += "<button onclick=\"top.closeEdit('" + note.TargetDoc + "', '" + note.TargetPtr + "')\">Close</button>";
                html += "</div>";
                html += "</div>";
                html += "</div>";
                //            $(containerIdJQuery).append(html);
                //            $(editDivIdJQuery).fadeIn();
                $("#iframe-main").contents().find(containerIdJQuery).append(html);
                //$("#iframe-main").contents().find(editDivIdJQuery).fadeIn();

            }
            fixNote(containerIdJQuery, editDivIdJQuery);
        }
        else {

            var tmpNote = notes[0];

             var containerId = tmpNote.TargetDoc + "-" + tmpNote.TargetPtr;
            var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

            var clueTipDivId = "notes-" + tmpNote.TargetDoc + "-" + tmpNote.TargetPtr;
            var clueTipDivStart = "<div id='" + clueTipDivId + "' style='display:none;'>";
            var clueTipDivEnd = "</div>";

            var noteList = "<ul>";
            var editDivs = "";
            for (var i = 0; i < notes.length; i++) {
                var note = notes[i];

                var editDivId = "edit-div-" + note.TargetDoc + "-" + note.TargetPtr;
                var editDivIdJQuery = "#" + editDivId.replace(/\./g, "\\.");

                var editTextAreaId = "edit-textarea-" + i + "-" + note.TargetDoc + "-" + note.TargetPtr;
                var editTextAreaIdJQuery = "#" + editTextAreaId.replace(/\./g, "\\.");

                var editTextInputId = "edit-textinput-" + i + "-" + note.TargetDoc + "-" + note.TargetPtr;
                var editTextInputJQuery = "#" + editTextInputId.replace(/\./g, "\\.");

                var html = "<div id='edit-div-" + i + "-" + note.TargetDoc + "-" + note.TargetPtr + "' class='editNoteDiv' >";
                html += "<div class='editNoteInnerDiv'>";
                html += "<label>Title:</label><input type='text' class='editNoteTextinput' id='" + editTextInputId + "' value='" + note.Title + "'/>";
                html += "<textarea id='" + editTextAreaId + "' class='editNoteTextarea' >" + note.Text + "</textarea>";
                html += "<div>";
                //html += "<a href='#' onclick=\"updateNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\"><img src='images/btn-save.gif' alt='Save Changes' border='0'></a>";
                html += "<button onclick=\"top.updateNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "', '" + i + "')\">Save</button>";
                //html += "<a href='#' onclick=\"deleteNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "')\"><img src='images/btn-delete.gif' alt='Delete' border='0'></a>";
                html += "<button onclick=\"top.deleteNote('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + note.Id + "', '" + i + "')\">Delete</button>";
                html += "<button onclick=\"top.closeEdit('" + note.TargetDoc + "', '" + note.TargetPtr + "', '" + i + "')\">Close</button>";
                html += "</div>";
                html += "</div>";
                html += "</div>";

                editDivs += html;

                noteList += "<li><a rel='#edit-div-" + i + "-" + note.TargetDoc + "-" + note.TargetPtr + "' class='cluetip' href='#' >" + note.Title + "</a>" + "</li>";
            }
            noteList += "</ul>";

            var anchorId = "anchor-" + tmpNote.TargetDoc + "-" + tmpNote.TargetPtr;
            var anchor = "<a id='" + anchorId + "' rel='#" + clueTipDivId.replace(/\./g, "\\.") + "' href='#' style='display:none;' >ClueTip</a>";

            var hoverDiv = clueTipDivStart + noteList + clueTipDivEnd;

            $("#" + clueTipDivId.replace(/\./g, "\\.")).remove();
            $("#content-container").append(hoverDiv);
            
            $("#iframe-main").contents().find("div.editNoteDiv").remove();
            $("#iframe-main").contents().find(containerIdJQuery).append(editDivs);
            $("#iframe-main").contents().find("div.editNoteDiv").hide();
            $("#content-container a.cluetip").on("click", function () {
                let divId = $(this).attr("rel");
                const containerID = divId.replace(/#edit-div-[0-9]+-/, "#").replace(/\./g, "\\.");
                divId = divId.replace(/\./g, "\\.");
                fixNote(containerID, divId);
            });

            $("#iframe-main").contents().find(containerIdJQuery).find("#" + anchorId.replace(/\./g, "\\.")).remove();
            $("#iframe-main").contents().find(containerIdJQuery).append(anchor);
            $("a.cluetip").on("click", function () {
                $(document).trigger('hideCluetip');
            });

            $("#cluetip").remove();
            $("#iframe-main").contents().find("#" + anchorId.replace(/\./g, "\\.")).cluetip({ activation: 'click', sticky: true, local: true, hideLocal: true, clickThrough: true, topOffset: 0, leftOffset: 25, closePosition: 'title', cluezIndex: 99
                
            });

            var anchorPos = $("#iframe-main").contents().find(containerIdJQuery + " span.editNote").position();

            var divH = 100 + (notes.length * 25);
            var containerH = $('#content-container').outerHeight();

            var event = jQuery.Event("click");
            event.pageX = anchorPos.left;
            event.pageY = anchorPos.top - getCurrentScrollbarPosition();
            if ((event.pageY + divH) > containerH)
                event.pageY = (containerH - divH);
            

            $("#iframe-main").contents().find("#" + anchorId.replace(/\./g, "\\.")).trigger(event);
            /*$("#iframe-main").contents().find(containerIdJQuery).cluetip({ activation: 'click', sticky: true, local: true, hideLocal: true, clickThrough: true, topOffset: 0, leftOffset: 0, closePosition: 'title',
                onShow: function () {
                    $(document).one('click', function () {
                        $(document).trigger('hideCluetip');
                    });
                }
            });*/
        }

        return false;
    }

    function updateNote(targetDoc, targetPtr, id, index) {
        var editTextAreaId = "edit-textarea-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var editTextAreaIdJQuery = "#" + editTextAreaId.replace(/\./g, "\\.");

        var addTextInputId = "edit-textinput-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var addTextInputIdJQuery = "#" + addTextInputId.replace(/\./g, "\\.");

        var noteText = $("#iframe-main").contents().find(editTextAreaIdJQuery).val();
        var titleText = $("#iframe-main").contents().find(addTextInputIdJQuery).val();

        if (titleText.trim() === "") {
            alert("Please enter some text for the title");
        }

        if (titleText.trim() === "") {
            alert("Please enter some text for the note.\nIf you are trying to remove the note, please click the delete button.");
            return;
        }

        //escape quotes and such so that there the request doesn't break
        noteText = noteText.replace(/\\/g, "\\\\");
        noteText = noteText.replace(/"/g, "\\\"");
        noteText = noteText.replace(/'/g, "\\\'");
        titleText = titleText.replace(/\\/g, "\\\\");
        titleText = titleText.replace(/"/g, "\\\"");
        titleText = titleText.replace(/'/g, "\\\'");

        var paramsString = "{id: '" + id + "', noteText:'" + noteText + "', noteTitle:'" + titleText + "'}";

        callWebService("WS/UserPreferences.asmx/UpdateNote", paramsString, saveNoteResultHandler, ajaxFailed);
        closeEdit(targetDoc, targetPtr, index);
    }

    function closeEdit(targetDoc, targetPtr, index) {
        var editDivId = "edit-div-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var editDivIdJQuery = "#" + editDivId.replace(/\./g, "\\.");

        var editTextAreaId = "edit-textarea-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var editTextAreaIdJQuery = "#" + editTextAreaId.replace(/\./g, "\\.");

        var addTextInputId = "edit-textinput-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var addTextInputIdJQuery = "#" + addTextInputId.replace(/\./g, "\\.");

        $("#iframe-main").contents().find(editTextAreaIdJQuery).val("");
        $("#iframe-main").contents().find(addTextInputIdJQuery).val("");
        $("#iframe-main").contents().find(editDivIdJQuery).fadeOut();
    }

    function deleteNote(targetDoc, targetPtr, id, index) {        
        var editDivId = "edit-div-" + (index != null ? (index + "-") : "") + targetDoc + "-" + targetPtr;
        var editDivIdJQuery = "#" + editDivId.replace(/\./g, "\\.");
        
        var paramsForCallback = {};
        paramsForCallback.TargetDoc = targetDoc;
        paramsForCallback.TargetPtr = targetPtr;

        callWebService("WS/UserPreferences.asmx/DeleteNote", "{id:" + id + "}", deleteNoteResultHandler, ajaxFailed, paramsForCallback);

        $("#iframe-main").contents().find(editDivIdJQuery).fadeOut();
        $("#iframe-main").contents().find(editDivIdJQuery).remove();
    }

    function deleteNoteResultHandler(returnFromWS, note) {
        var containerId = note.TargetDoc + "-" + note.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        callWebService("WS/UserPreferences.asmx/GetNotes", "{targetDoc:'" + note.TargetDoc + "', targetPtr:'" + note.TargetPtr + "'}", updateEditNoteButton, ajaxFailed, note);

        $("#iframe-main").contents().find(containerIdJQuery + " span.addNote").show();
    }

    function updateEditNoteButton(notes,params) {
        var containerIdJQuery = "#"+(params.TargetDoc + "-" + params.TargetPtr).replace(/\./g, "\\.");
        if (notes.length == 0)
            $("#iframe-main").contents().find(containerIdJQuery + " span.editNote").hide();
    }

    //***************************************
    // My Notes
    //***************************************
    function editMyNote(id) {
        $(".edit-div").fadeOut();
        fixMyNote(id);
        //$("#edit-div-" + id).fadeIn();

    }

    /* Use UpdateNote instead */
    function updateMyNote(id) {
        var noteText = $("#edit-textarea-" + id).val();
        var noteTitle = $("#edit-title-" + id).val();
        callWebService("WS/UserPreferences.asmx/UpdateNote", "{id: '" + id + "', noteText:'" + noteText + "', noteTitle:'" + noteTitle + "'}", saveMyNoteResultHandler, ajaxFailed);
        doNotesLink();
    }

    function closeMyEdit(id) {
        $("#edit-div-" + id).fadeOut();
    }

    function saveMyNoteResultHandler(note) {
        $("#edit-div-" + note.Id).fadeOut();
        $("#edit-textarea-" + note.Id).val(note.Text);
    }

    function executeBulkNoteAction() {
        var action = $('#note-bulk-action').val();
        if (action == "delete") { deleteSelectedNotes(); }
        if (action == "print") { printSelectedNotes(); }
    }

    function printSelectedNotes() {
        var printNoteIds = "";
        $('.note-checkbox').each(function () {
            if (this.checked)
                printNoteIds += "," + this.id;
        })

        if (printNoteIds.length > 1) {
            printNoteIds = printNoteIds.substring(1);
            var printNoteUrl = "PrintNotes.ashx?noteIds=" + printNoteIds;
            window.open(printNoteUrl, "Print Notes", "menubar=0,location=0,toolbar=0");
        }
    }

    function deleteMyNote(id) {
        callWebService("WS/UserPreferences.asmx/DeleteNote", "{id:" + id + "}", deleteMyNoteResultHandler, ajaxFailed);
    }

    function deleteSelectedNotes() {
        var deleteNoteIds = "";
        $('.note-checkbox').each(function () {
            if (this.checked)
                deleteNoteIds += "," + this.id;
        })

        if (deleteNoteIds.length > 1) {
            deleteNoteIds = deleteNoteIds.substring(1);
            callWebService("WS/UserPreferences.asmx/DeleteNotes", "{noteIds:'" + deleteNoteIds + "'}", deleteMyNoteResultHandler, ajaxFailed);
        }
    }

    function deleteMyNoteResultHandler() {
        doNotesLink();
    }

    function deleteMyBookmarkResultHandler() {
        doBookmarkLink();
    }

    function deleteMyBookmark(id) {
        callWebService("WS/UserPreferences.asmx/DeleteBookmarkByID", "{id:" + id + "}", deleteMyBookmarkResultHandler, ajaxFailed);
    }

    //Delete a book mark by TargetDoc and TargetPtr in book
    function deleteBookmark(targetDoc, targetPtr) {
        var paramsString = "{targetDoc:'" + targetDoc + "', targetPtr:'" + targetPtr + "'}";

        callWebService("WS/UserPreferences.asmx/DeleteBookmark", paramsString, deleteBookmarkResultHandler, ajaxFailed);
    }

    function deleteBookmarkResultHandler(bookmark) {
        if (bookmark == null) return;
        var containerId = bookmark.TargetDoc + "-" + bookmark.TargetPtr;
        var containerIdJQuery = "#" + containerId.replace(/\./g, "\\.");

        $("#iframe-main").contents().find(containerIdJQuery + " span.deleteBookmark").hide();
        $("#iframe-main").contents().find(containerIdJQuery + " span.addBookmark").show();
    }
//***********************************************************************************
// onchange handlers
//***********************************************************************************
function doQuickFindDropDownOnChange(sender, selectedOption)
{
    if (sender == 'documentType')
    {
        /*$('#subjectMatter').val('').attr('selected', 'selected');*/
        $('#subjectMatter').val('').prop('selected', true);
    }
    else
    {
        /*$('#documentType').val('').attr('selected', 'selected');*/
        $('#documentType').val('').prop('selected', true);
    }

    hideDocumentSpecificButtons();
    loadTemplate('WS/QuickFind.asmx/GetQuickFindResults', "{selectedTaxonomy:'" + selectedOption + "'}", 'templates/quickFindResults.html', 'quickfindRightCol');
}

//**********************************************************
// common functions used by these handlers
//**********************************************************
// params:
//  id
//  type
//  scrollbarPosition (OPTIONAL)
function loadPrimaryContent(id, type, scrollbarPosition) {
    var params = "{ id:" + id + ", type:'" + type + "'}";
    console.log("id: " + id + " type:" + type + " id type:" + typeof (id) + " type type:" + typeof (type));
    if (typeof (type) == 'function') {
        debugger;
    }
    callWebService("WS/Content.asmx/GetPrimaryContent", params, loadContentBySiteNode, ajaxFailed, scrollbarPosition);
}

function prepareActiveScreen(useNewScreen, showHighlight, scrollbarPosition, hitAnchor, viewCompleteTopic) {
    if (useNewScreen && getMyScreenCount() >= 10)
    {
        alert("You have reached the maximum number of open documents. Please close a document and try again.");

        setMyDocumentsTab(true);

        return false;
    }

    if (hasActiveScreen()) {
        getActiveScreen().recordScrollbarPosition();
    }

    createNewScreenIfRequestedOrNoActiveScreen(useNewScreen);

    //set showHighlight for the active screen if the parameter is not null
    if (showHighlight != null) {
        getActiveScreen().showHighlight = showHighlight;
    }

    //then set the scrollbarPosition if not null
    if (scrollbarPosition != null) {
        getActiveScreen().scrollbarPosition = scrollbarPosition;
    }

    if (hitAnchor != null) {
        getActiveScreen().hitAnchor = hitAnchor;
    }

    if (viewCompleteTopic != null) {
        getActiveScreen().viewCompleteTopic = viewCompleteTopic;
//    } else {
//        getActiveScreen().viewCompleteTopic = false;
    }

    return true;
}

function createNewScreenIfRequestedOrNoActiveScreen(useNewScreen) {
    if (useNewScreen) {
        // add new screen in MyDocuments list
        var screenInstance = new docscreen();
        var index = addNewScreen(screenInstance);

        // make new screen the active screen
        setActiveScreenIndex(index);
    }
    else {
        if (!hasActiveScreen()) {
            // no active screen AKA the very first one
            createNewScreenIfRequestedOrNoActiveScreen(true);
        }
    }
}

function doAdvSearchCheck(keyInfo) {
    console.log(keyInfo)
    if (keyInfo == 13) {
        document.getElementById('advancedsearch').focus();
        document.getElementById('advancedsearch').click(); 
    }
}

function doSearchCheck(keyInfo) {
    if (keyInfo == 13) {
        document.getElementById('search').focus();
        document.getElementById('search').click();
    }
}

function doSavedSearchCheck(keyInfo) {
    if (keyInfo == 13) {
        document.getElementById('savedsearch').focus();
        document.getElementById('savedsearch').click();
    }
}

function doRenameSearchCheck(keyInfo) {
    if (keyInfo == 13) {
        document.getElementById('renamesearch').focus();
        document.getElementById('renamesearch').click();
    }
}

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
    setScreenAsCurrentView(getActiveScreen(), getActiveScreenIndex());
}

function doLoadingCancel() {
    setLoading(false);
    loadHomePage();
}

function doToolbarToggle() {
    $("#panel").slideToggle("slow");
    $("#toolbarToggleButton").toggleClass("activex");
    toggleContentAreaSize();
}

function toggleContentAreaSize() {

    setContentSize();

//    if (isToolbarHidden()) {
//        makeContentAreaLarge();
//    }
//    else {
//        makeContentAreaSmall();
//    }
}

//function makeContentAreaSmall() {
//    setContentSize();
//}

//function makeContentAreaLarge() {
//    setContentSize();
//}


/**
* Sets the Visibility of the My Documents Tab
* 
* @param visible: true if the My Documents Tab div should be shown, false to hide
*/
function setMyDocumentsTab(visible) {
    if (visible) {
        if (isToolbarHidden()) {
            doToolbarToggle();
        }

        $(".tabs li").removeClass("active"); //Remove any "active" class
        $("#Tab-MyDocuments").addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        $("#Toolbar-MyDocuments").fadeIn(); //Fade in the active ID content
    } else {
        // currently there is no reason to implement this side of things
    }
}

$(function () {
    $(".noteTip").cluetip({
        activation: 'click',
        sticky: true,
        local: true,
        hideLocal: true,
        topOffset: 0,
        leftOffset: 0,
        closePosition: 'title',
        onShow: function () {
            $(document).one('mousedown', function () {
                $(document).trigger('hideCluetip');
            });
        }
    });
});

