// toc-loader.js (jQuery 3.7.1 compatible with cleaned structure and comments)

/**
 * Loads the initial Table of Contents (TOC) based on stored state.
 * @param {string} rootId - The ID of the root UL element where TOC will load.
 */
function initialTocLoad(rootId) {
  loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", getTocStateId(), getTocStateType(), $("#" + rootId), true);
}

/**
 * Loads the TOC synchronously based on the active document if present.
 * Falls back to site-level TOC if no active document.
 */
function syncTocLoad(rootId) {
  const id = hasActiveDocument() ? getActiveDocumentId() : -1;
  const type = hasActiveDocument() ? getActiveDocumentType() : "Site";
  loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", id, type, $("#" + rootId), true);
}

/**
 * Loads the TOC by specific document ID and type. Falls back to active document if both are not provided.
 */
function TocLoadByIdType(rootId, id, type) {
  const validId = id || getActiveDocumentId();
  const validType = type || getActiveDocumentType();
  loadPlainTocByHtml("WS/Content.asmx/GetInitialTreeTocHtml", validId, validType, $("#" + rootId), true);
}

/**
 * Common function to fetch and load TOC HTML into the given UL container.
 */
function loadPlainTocByHtml(url, id, type, ulToAppend, shouldExpandToNode) {
  const params = JSON.stringify({ id: id.toString(), type });

  $.ajax({
    type: "POST",
    url,
    data: params,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      ulToAppend.html(response.d);
      if (shouldExpandToNode) expandToNode(id, type);
    },
    error: ajaxFailed
  });
}

/**
 * Toggles a TOC node to expand/collapse and loads children via web service if needed.
 */
function toggleTocNode(id, type, uniqueId) {
  const childUl = $("#childUl-" + uniqueId);
  const currentLi = $("#currentLi-" + uniqueId);
  const currentDiv = $("#currentDiv-" + uniqueId);

  if (!childUl.hasClass("calledWS")) {
    childUl.addClass("calledWS");
    loadPlainTocByHtml("WS/Content.asmx/GetNodeToGrandChildrenHtml", id, type, childUl, false);
  }

  toggleCurrentLiClass(currentLi);
  toggleCurrentDivClass(currentDiv);
  childUl.slideToggle();

  setTocStateId(id);
  setTocStateType(type);
}

/**
 * Toggles the list item CSS classes between expandable and collapsable.
 */
function toggleCurrentLiClass(currentLi) {
  const isExpandable = currentLi.hasClass("expandable");
  currentLi.toggleClass("expandable", !isExpandable);
  currentLi.toggleClass("collapsable", isExpandable);
  currentLi.toggleClass("lastExpandable", !isExpandable && currentLi.hasClass("lastCollapsable"));
  currentLi.toggleClass("lastCollapsable", isExpandable && currentLi.hasClass("lastExpandable"));
}

/**
 * Toggles the div hitarea CSS classes between expandable and collapsable.
 */
function toggleCurrentDivClass(currentDiv) {
  const isExpandable = currentDiv.hasClass("expandable-hitarea");
  currentDiv.toggleClass("expandable-hitarea", !isExpandable);
  currentDiv.toggleClass("collapsable-hitarea", isExpandable);
  currentDiv.toggleClass("lastExpandable-hitarea", !isExpandable && currentDiv.hasClass("lastCollapsable-hitarea"));
  currentDiv.toggleClass("lastCollapsable-hitarea", isExpandable && currentDiv.hasClass("lastExpandable-hitarea"));
}

/**
 * Expands the tree to the specified node by ID and highlights it.
 */
function expandToNode(id, type) {
  if (id === -1) {
    manualExpand.call($('#mainToc').children("li:first")[0]);
    return;
  }

  const liIdString = `#currentLi-${id}-${type}`;
  const currentLi = $(liIdString);
  const ancestry = currentLi.parents("#mainToc li");

  ancestry.each(manualExpand);
  manualExpand.call(currentLi[0]);

  const anchorElement = currentLi[0];
  if (anchorElement) {
    anchorElement.scrollIntoView(true);
  } else {
    logErrorToServe(`Anchor Empty: ${liIdString}`);
  }

  const span = $(`${liIdString} span:first`).css("background", "#5CB3FF");
  setTimeout(() => {
    span.animate({ backgroundColor: "#ffffff" }, 1500, "swing");
  }, 4000);
}

/**
 * Manually expands a given TOC node (used during tree initialization).
 */
function manualExpand() {
  const currentLi = $(this);
  const currentDiv = currentLi.children("div:first");
  const childUl = currentLi.children("ul:first");

  toggleCurrentLiClass(currentLi);
  toggleCurrentDivClass(currentDiv);
  childUl.show();
}
