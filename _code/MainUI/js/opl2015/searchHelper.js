///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015
///** This is for ODP 2015


function deleteSavedSearch(id, name) {    
    var params = "{name:'" + name + "'}";
    loadTemplate('/WS/SearchServices.asmx/DeleteUserSavedSearch', params, '/templates/odp2015/savedSearches.html', 'document-container-left', null, function () {
        //$("#deleteModal-" + id).modal("toggle");
        //Here because of a fade bug. 
        $("#deleteModal" + id).modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();

    });

}

function renameSavedSearch(id, oldname) {    
    var newName = $("#savedSearchTitle-" + id).val();    
    var params = "{name:'" + oldname + "', newName:'" + newName + "'}";
    loadTemplate('/WS/SearchServices.asmx/RenameUserSavedSearch', params, '/templates/odp2015/savedSearches.html', 'document-container-left', null, function () {
        //$("#renameModal-" + id).modal("toggle");  
        //Here because of a fade bug. 
        $("#renameModal" + id).modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();

    });

}


/********************************************************

 ********************************************************/

function doAdvancedNavigationalSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed, nonauthoritative, callback) {
    var url = "";    
    url = "/search?did="+dimensionId+"&q=" + keywords + "&sm=" + searchMode + "&mh=" + maxHits + "&ps=" + pageSize + "&po=" + pageOffset + "&se=" + showExcerpts + "&su=" + showUnsubscribed + "&nu=" + nonauthoritative;
    window.location = url;
}

function getResults() {

    window.location = "/search?redo=0";

    //    var params = "{dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + "}";
    //    loadTemplate('WS/EndecaServices.asmx/EndecaAdvancedSearch', params, 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);
}

function doSearchWithCurrentCriteria() {
    setLoading(true);
    loadTemplateDual('/WS/EndecaServices.asmx/EndecaSearchWithCurrentCriteria', '{}', [{ 'templateUrl': '/templates/odp2015/searchResults.html', 'containerId': 'divLeftColInner' },
                                                                              { 'templateUrl': '/templates/odp2015/searchQuery.html', 'containerId': 'searchWidgetId'}], {}, setUpSearchAutocomplete);
}

function doAdvancedNavigationalSearchInt(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed, nonauthoritative, callback) {
//    clearCurrentView(); // update the back button status
    //content-container

    //alert('Dimension ID = ' + dimensionId.toString());

//    var txt = keywords;
//    var tbox = document.getElementById('searchTerms');
//    if (tbox) {
//        tbox.value = txt;
//    }

//    clearMyScreensHitHighlighting();
//    hideDocumentSpecificButtons();
    setLoading(true);
    if (keywords == "") {
        alert("Please Enter Search Terms");
        return false;
    } 

    var temp = keywords;
    keywords = temp.replace(/'/g, '\\\''); //escape single quotes so they can be passed to Endeca without breaking the javascript

    var filterUnsubscribed = 1; // We don't want to see the options that they don't have subscriptions to. per AICPA

    if (dimensionId == "null") {
        dimensionId = '';
    }

    var params = "{dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + ", nonauthoritative: " + nonauthoritative + "}";
    //console.log(params);


    loadTemplateDual('/WS/EndecaServices.asmx/EndecaAdvancedSearch', params, [{ 'templateUrl': '/templates/odp2015/searchResults.html', 'containerId': 'divLeftColInner' },
                                                                              { 'templateUrl': '/templates/odp2015/searchQuery.html', 'containerId': 'searchWidgetId'}], {}, setUpSearchAutocomplete);

//    loadTemplate('/WS/EndecaServices.asmx/EndecaAdvancedSearch', params, '/templates/odp2015/searchResults.html', 'divLeftColInner', '', setUpSearchAutocomplete);
//    loadTemplate('/WS/EndecaServices.asmx/EndecaAdvancedSearch', params, '/templates/odp2015/searchQuery.html', 'searchWidgetId', '', setUpSearchAutocomplete)
//    if (document.images) {
//        var resultTrue = new Image();
//        resultTrue.src = "images/btn-results.gif";

//        var searchResultsButton = document.images["searchResultsButton"];
//        var source = "";
//        if (searchResultsButton) {
//            source = searchResultsButton.src;
//        }
//        var ethicsImg = "images/results2.png";
//        if (source.indexOf(ethicsImg) == -1)
//            searchResultsButton.src = resultTrue.src;
//    }
//    if (callback) {
//        callback();
//    }
    //$('#content-container').scrollTop();
    $("#divLeftColInner").animate({ "scrollTop": 0 });
}

function setUpSearchAutocomplete() {
    setLoading(false);
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


/**********************************************************/


function saveSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed) {
    var filterUnsubscribed = 0;
//    if ($('#showUnsubscribed').attr('checked') == true) filterUnsubscribed = 0;
    //    else filterUnsubscribed = 1;
    $("#mySaveNoteModal").modal('hide');    
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    var searchName = $('#searchTitle').val();
    var params = "{searchName:'" + searchName + "', dimensionId:'" + dimensionId + "', keywords:'" + keywords + "', searchMode:" + searchMode + ", maxHits:" + maxHits + ", pageSize:" + pageSize + ", pageOffset:" + pageOffset + ", showExcerpts:" + showExcerpts + ", filterUnsubscribed:" + filterUnsubscribed + "}";
    loadTemplate('/WS/SearchServices.asmx/SaveUserSearch', params, '/templates/odp2015/savedSearches.html', 'document-container-left');
    $("#document-container-right").empty();
}

function updateHitDocButtons(id, type) {
    doNextHitDoc(id, type);
    doPrevHitDoc(id, type);
}

function doNextHitDoc(id, type) {
    if (id && type)
        $.ajax({
            type: "POST",
            url: "/WS/EndecaServices.asmx/EndecaNextHitDoc",
            dataType: "json",
            data: "{id:" + id + ", type: '" + type + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d.Id != -1) {
                    $('#nextHitDoc').removeClass("disabled");
                    $('#nextHitDoc').off("click");
                    $('#nextHitDoc').on("click", function () {
                        doSearchLink(data.d.Id, data.d.Type, false);
                    });
                } else {
                    $('#nextHitDoc').addClass("disabled");
                    $('#nextHitDoc').off("click");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(textStatus);
                logErrorToServer("doNextHitDoc..Error: " + textStatus + "-- Error Thrown: " + errorThrown);
                
            }
        });
}

function doPrevHitDoc(id, type) {
    if (id && type)
        $.ajax({
            type: "POST",
            url: "/WS/EndecaServices.asmx/EndecaPrevHitDoc",
            dataType: "json",
            data: "{id:" + id + ", type: '" + type + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d.Id != -1) {
                    $('#prevHitDoc').removeClass("disabled");
                    $('#prevHitDoc').off("click");
                    $('#prevHitDoc').on("click", function () {
                        doSearchLink(data.d.Id, data.d.Type, false);
                    });
                } else {
                    $('#prevHitDoc').addClass("disabled");
                    $('#prevHitDoc').off("click");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                logErrorToServer("DoPrevHitDoc..Error: " + textStatus + "-- Error Thrown: " + errorThrown);
               // alert(textStatus);
            }
        });
}