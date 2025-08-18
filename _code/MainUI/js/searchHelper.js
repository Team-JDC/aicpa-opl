// Perform an advanced search using the provided parameters
function doAdvancedNavigationalSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed, nonauthoritative, callback) {
    clearCurrentView();
    clearMyScreensHitHighlighting();
    hideDocumentSpecificButtons();

    if (!keywords) {
        alert("Please Enter Search Terms");
        return false;
    }

    // Set textbox value if present
    const $searchBox = $('#searchTerms');
    if ($searchBox.length) $searchBox.val(keywords);

    // Escape single quotes for safe transmission
    const safeKeywords = keywords.replace(/'/g, "\\'");

    const filterUnsubscribed = $('#showUnsubscribed').is(':checked') ? 0 : 1;

    const params = JSON.stringify({
        dimensionId,
        keywords: safeKeywords,
        searchMode,
        maxHits,
        pageSize,
        pageOffset,
        showExcerpts,
        filterUnsubscribed,
        nonauthoritative
    });

    loadTemplate('WS/EndecaServices.asmx/EndecaAdvancedSearch', params, 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);

    // Update button image if needed
    const searchResultsButton = document.images["searchResultsButton"];
    if (searchResultsButton && !searchResultsButton.src.includes("images/results2.png")) {
        searchResultsButton.src = "images/btn-results.gif";
    }

    if (callback) callback();

    $("#content-container").animate({ scrollTop: 0 });
}

// Toggle and optionally load detailed search hit result
function doHitResult(id, type, keywords, searchMode, container) {
    const $container = $(`#${container}`);
    const isVisible = $container.is(':visible');

    $container.slideToggle();

    if (!isVisible) {
        const params = JSON.stringify({ id, type, keywords, searchMode });
        loadTemplate('WS/DocumentTools.asmx/GetHitDetail', params, 'templates/hitDetail.html', container, '', null, true);
    }
}

// Load search results based on current criteria
function getResults() {
    clearCurrentView();
    hideDocumentSpecificButtons();

    loadTemplate('WS/EndecaServices.asmx/EndecaSearchWithCurrentCriteria', '{}', 'templates/searchResults.html', 'document-container', '', setUpSearchAutocomplete);
}

// Load the advanced search form (blank)
function loadBlankSearch() {
    clearCurrentView();
    hideDocumentSpecificButtons();

    loadTemplate('WS/EndecaServices.asmx/DoBlankSearch', '{}', 'templates/blankSearch.html', 'document-container', '', setUpSearchAutocomplete);
}

// Clear or focus the search box depending on its value
function clearSearchBox() {
    const $searchBox = $('#searchTerms');
    if ($searchBox.val() === "New Search") {
        $searchBox.val("");
    } else {
        $searchBox.select();
}

// Save current search parameters under a user-defined name
function saveSearch(dimensionId, keywords, searchMode, maxHits, pageSize, pageOffset, showExcerpts, showUnsubscribed) {
    const filterUnsubscribed = $('#showUnsubscribed').is(':checked') ? 0 : 1;
    const searchName = $('#searchNameBox').val();

    if (!searchName) {
        alert("Please Enter Search Name");
        $('#searchNameBox').focus();
        return;
    }

    const params = JSON.stringify({
        searchName,
        dimensionId,
        keywords,
        searchMode,
        maxHits,
        pageSize,
        pageOffset,
        showExcerpts,
        filterUnsubscribed
    });

    loadTemplate('WS/SearchServices.asmx/SaveUserSearch', params, 'templates/savedSearches.html', 'document-container');
}

// Delete a saved search by name
function deleteSavedSearch(name) {
    const params = JSON.stringify({ name });
    loadTemplate('WS/SearchServices.asmx/DeleteUserSavedSearch', params, 'templates/savedSearches.html', 'document-container');
}

// Show rename dialog for a saved search
function renameSavedSearchPrompt(name) {
    $('#nameSearchWindow').show();
    $('#oldName').val(name);
    $('#searchNameBox').val(name).focus();
}

// Rename a saved search
function renameSavedSearch() {
    const newName = $('#searchNameBox').val();

    if (!newName) {
        alert("Please Enter New Search Name");
        $('#searchNameBox').focus();
        return false;
    }

    const params = JSON.stringify({
        name: $('#oldName').val(),
        newName
    });

    loadTemplate('WS/SearchServices.asmx/RenameUserSavedSearch', params, 'templates/savedSearches.html', 'document-container');
}

// Trigger name prompt if keywords exist
function nameSearch(keywords) {
    if (!keywords) {
        alert("Please Enter Search Terms");
    } else {
        $('#nameSearchWindow').show();
    }
}

// Hide the name search modal
function cancelNameSearch() {
    $('#nameSearchWindow').hide();
}

// Setup autocomplete for the advanced search terms input (placeholder, not active)
function setUpSearchAutocomplete() {
    // To be implemented if needed
    // Uncomment and update endpoint to enable jQuery UI autocomplete
}
