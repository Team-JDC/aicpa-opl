
// Dev

//This function is to do an advanced search.
function doAdvancedNavigationalSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed, nonauthoritative, callback) {
    clearCurrentView(); // update the back button status
    //content-container

    //alert('Dimension ID = ' + dimensionId.toString());




    var txt = keywords;
    var tbox = document.getElementById('searchTerms');
    if (tbox) {
        tbox.value = txt;
    }

    clearMyScreensHitHighlighting();
    hideDocumentSpecificButtons();

    if (keywords == "") {
        alert("Please Enter Search Terms");
        return false;
    }

    var temp = keywords;
    keywords = temp.replace(/'/g, '\\\''); //escape single quotes so they can be passed to Endeca without breaking the javascript

    var filterUnsubscribed;

    if ($('#showUnsubscribed').attr('checked') == true) filterUnsubscribed = 0;
    else filterUnsubscribed = 1;

    var params = "{dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + ", nonauthoritative: " + nonauthoritative+"}";

    //alert(params);

    loadTemplate('WS/EndecaServices.asmx/EndecaAdvancedSearch', params, 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);
    if (document.images) {
        var resultTrue = new Image();
        resultTrue.src = "images/btn-results.gif";

        var searchResultsButton = document.images["searchResultsButton"];
        var source = "";
        if (searchResultsButton) {
            source = searchResultsButton.src;
        }
        var ethicsImg = "images/results2.png";
        if (source.indexOf(ethicsImg) == -1)
            searchResultsButton.src = resultTrue.src;
    }
    if (callback) {
        callback();
    }
    //$('#content-container').scrollTop();
    $("#content-container").animate({ "scrollTop": 0 });
}

function doHitResult(id, type, keywords, searchMode, container) {   
    var hideContainer = $('#' + container).is(':visible');
    $('#' + container).slideToggle();

    if (hideContainer) return;
    
    var params = "{id:" + id + ", type:'" + type + "', keywords:'"+keywords+"', searchMode:" + searchMode + "}";
    loadTemplate('WS/DocumentTools.asmx/GetHitDetail', params, 'templates/hitDetail.html', container, '',null, true);

}



function getResults() {
    clearCurrentView(); // update the back button status

    hideDocumentSpecificButtons();
    loadTemplate('WS/EndecaServices.asmx/EndecaSearchWithCurrentCriteria', '{}', 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);

//    var params = "{dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + "}";
//    loadTemplate('WS/EndecaServices.asmx/EndecaAdvancedSearch', params, 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);
}

//This function loads a blank search form when user links directly to Adv search without triggering basic search first.
function loadBlankSearch() {
    clearCurrentView(); // update the back button status
    //setToolAsCurrentView(toolName_blankSearch, ""); // update the back button status

    hideDocumentSpecificButtons();
    loadTemplate('WS/EndecaServices.asmx/DoBlankSearch', '{}', 'templates/blankSearch.html', 'document-container', '', setUpSearchAutocomplete);
}


// clears bacic search textbox is default search term is still there, otherwise it's left alone.
// will need to change function if default search term is changed.
function clearSearchBox() {
    if ($('#searchTerms').val() == "New Search") {
        $('#searchTerms[value]').val("");
    }
    else {
        $('#searchTerms').select();
    }
}

function saveSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed) {
    var filterUnsubscribed;
    if ($('#showUnsubscribed').attr('checked') == true) filterUnsubscribed = 0;
    else filterUnsubscribed = 1;
    var searchName = $('#searchNameBox').val();
    if (searchName == "") {
        alert("Please Enter Search Name");
        document.getElementById('searchNameBox').focus();
    }
    var params = "{searchName:'" + searchName + "', dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + "}";
    loadTemplate('WS/SearchServices.asmx/SaveUserSearch', params, 'templates/savedSearches.html', 'document-container');
}

function deleteSavedSearch(name) {
    var params = "{name:'" + name + "'}";
    loadTemplate('WS/SearchServices.asmx/DeleteUserSavedSearch', params, 'templates/savedSearches.html', 'document-container');
}

function renameSavedSearchPrompt(name) {
    $('#nameSearchWindow').show();
    $('#oldName').val(name);
    $('#searchNameBox').val(name);
    document.getElementById('searchNameBox').focus();
}

function renameSavedSearch() {
    var newName = $('#searchNameBox').val()
    if (newName == "") {
        alert("Please Enter New Search Name");
        document.getElementById('searchNameBox').focus();
        return false;
    }
    var params = "{name:'" + $('#oldName').val() + "', newName:'" + newName + "'}";
    loadTemplate('WS/SearchServices.asmx/RenameUserSavedSearch', params, 'templates/savedSearches.html', 'document-container');
}

function nameSearch(keywords) {
    if (keywords == "") {
        alert("Please Enter Search Terms");
    } else {
        $('#nameSearchWindow').show();
    }
}

function cancelNameSearch() {
    $('#nameSearchWindow').hide();
}

function setUpSearchAutocomplete() {
//    if ($('#searchTermsAdvanced').length > 0) {
//        $.ajax({
//            type: "POST",
//            url: "WS/SearchServices.asmx/GetSuggestedSearchTerms",
//            dataType: "json",
//            data: "{}",
//            contentType: "application/json; charset=utf-8",
//            success: function (data) {
//                $("#searchTermsAdvanced").autocomplete({
//                    source: data.d,
//                    minlength: 3,
//                    maxResults: 30
//                });
//            },
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//                alert(textStatus);
//            }
//        });
//    }
}