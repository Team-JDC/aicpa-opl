/*
 * Async Treeview 0.1 - Lazy-loading extension for Treeview
 * 
 * http://bassistance.de/jquery-plugins/jquery-plugin-treeview/
 *
 * Copyright (c) 2007 Jörn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Revision: $Id$
 *
 */

;(function($) {


function loadInitialTree(url, id, type, ulToAppend, treeRootNode) {
    var params = "{id: '" + id +"', type:'" + type + "'}";

    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
		    function createNodeIntialLoad(parentUl, recursionCount) {
                var id = this.SiteNode.Id;
                var type = this.SiteNode.Type;
                var title = this.SiteNode.Title;

                var current = null;

                if (type == "Site") {
                    current = $("<li/>").attr("id", id).attr("siteNodeType", type).html("<span>" + title + "</span>").appendTo(parentUl);
                } else {
                    current = $("<li/>").attr("id", id).attr("siteNodeType", type).html("<a href=\"#\" onclick=\"doTocLink(" + id + ", '" + type + "')\"><img src='images/icon_" + type + ".gif' alt='" + type + " Icon' class='icon' border='0'/><span>" + title + "</span></a>").appendTo(parentUl);
                }
			    
			    if (this.classes) {
				    current.children("span").addClass(this.classes);
			    }
			    if (this.expanded) {
				    current.addClass("open");
			    }

                if (recursionCount == 0) {
                    current.addClass("calledWS");
                }

                var hasChildren = this.Children && this.Children.length;

                if (recursionCount == 1 && !hasChildren) {
                    current.addClass("isLeaf");
                }

                // our recursive case
                if (recursionCount < 2 && hasChildren) {
                    var ulForChildren = $("<ul/>").appendTo(current);
					$.each(this.Children, createNodeIntialLoad, [ulForChildren, recursionCount + 1]);
                    /*
                    for (var i=0; i < this.Children.length; i++)
                    {
                        createNodeIntialLoad.apply(this.Children[i], [ulForChildren, recursionCount + 1]);
                        //createNodeIntialLoad(ulForChildren, recursionCount + 1);
                    }
                    */
                }
            }

            // This basically does:
            // BreadcrumbNode b = WSCall();
            // b.createNodeInitialLoad(child, 0);
            createNodeIntialLoad.apply(response.d, [ulToAppend, 0]);
            $(treeRootNode).treeview({add: ulToAppend});
        },
        error: ajaxFailed
    });
}

function loadEverything(url, id, type, ulToAppend, treeRootNode) {
    var params = "{id: '" + id +"', type:'" + type + "'}";

    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
		    function createNodeLoadEverything(parentUl, recursionCount) {
                var id = this.SiteNode.Id;
                var type = this.SiteNode.Type;
                var title = this.SiteNode.Title;
                var expanded = this.Expanded;

                var current = null;

                if (type == "Site") {
                    current = $("<li/>").attr("id", id).attr("siteNodeType", type).html("<span>" + title + "</span>").appendTo(parentUl);
                } else {
                    current = $("<li/>").attr("id", id).attr("siteNodeType", type).html("<a href=\"#\" onclick=\"doTocLink(" + id + ", '" + type + "')\"><img src='images/icon_" + type + ".gif' alt='" + type + " Icon' class='icon' border='0'/><span>" + title + "</span></a>").appendTo(parentUl);
                }
			    
			    if (this.classes) {
				    current.children("span").addClass(this.classes);
			    }
			    if (this.expanded || expanded) {
				    current.addClass("open");
			    }

                // This assumption is simply that on the server side we only set Expanded if all
                // children and grandchildren were retrieved.
                if (recursionCount == 0 || expanded) {
                    current.addClass("calledWS");
                }

                var hasChildren = this.Children && this.Children.length;

                // TODO: This isn't right
                if (!hasChildren) {
                    current.addClass("isLeaf");
                }

                // our recursive case
                if (hasChildren) {
                    var ulForChildren = $("<ul/>").appendTo(current);
					$.each(this.Children, createNodeLoadEverything, [ulForChildren, recursionCount + 1]);
                    /*
                    for (var i=0; i < this.Children.length; i++)
                    {
                        createNodeLoadEverything.apply(this.Children[i], [ulForChildren, recursionCount + 1]);
                        //createNodeLoadEverything(ulForChildren, recursionCount + 1);
                    }
                    */
                }
            }

            // This basically does:
            // BreadcrumbNode b = WSCall();
            // b.createNodeInitialLoad(child, 0);
            createNodeLoadEverything.apply(response.d, [ulToAppend, 0]);
            $(treeRootNode).treeview({add: ulToAppend});
        },
        error: ajaxFailed
    });
}

function load(url, id, type, ulToAppend, currentLi, treeRootNode) {
    var params = "{id: '" + id +"', type:'" + type + "'}";

    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
		    function createNode(parentUl, currentLi, recursionCount) {

                var hasChildren = this.Children && this.Children.length;

                if (recursionCount == 0) {
                    $(currentLi).addClass("calledWS");

                    if (hasChildren) {
                        var ulForChildren = $(currentLi).children("ul");

                        if ($(currentLi).attr("containsDummyNode") != undefined) {
                            // remove dummy child node
                            $(currentLi).removeAttr("containsDummyNode");
                            $(ulForChildren).html("");

                            // create child nodes, rather than iterate
                            $.each(this.Children, createNode, [ulForChildren, null, 2]);
                        }

                        var childLiList = $(ulForChildren).children("li");

                        for (var i = 0; i < this.Children.length; i++) {
                            var childBreadcrumb = this.Children[i];
                            var childLi = childLiList[i];

                            createNode.apply(childBreadcrumb, [ulForChildren, childLi, recursionCount + 1]);
                        }
                    }
                } else if (recursionCount == 1) {
                    if (hasChildren) {
                        var numberOfChildren = this.Children.length;
                        var ulForChildren = $("<ul/>").appendTo(currentLi);

                        if (numberOfChildren > g_tocNodeChildLimit) {
                            $(currentLi).attr("containsDummyNode", "1");
        			        var dummyChildLi = $("<li/>").attr("attTwo", "true").appendTo(ulForChildren);

                        }
                        else {
					        $.each(this.Children, createNode, [ulForChildren, null, recursionCount + 1]);
							/*
                        	for (var i=0; i < this.Children.length; i++)
                        	{
                            	createNode.apply(this.Children[i], [ulForChildren, recursionCount + 1]);
                            	//createNode(ulForChildren, null, recursionCount + 1);
                        	}
                        	*/
                        }
                        
                    }
                    else {
                        $(currentLi).addClass("isLeaf");
                    }
                } else { // recursion count > 1
                    var id = this.SiteNode.Id;
                    var type = this.SiteNode.Type;
                    var title = this.SiteNode.Title;

			        currentLi = $("<li/>").attr("id", id).attr("siteNodeType", type).html("<a href=\"#\" onclick=\"doTocLink(" + id + ", '" + type + "')\"><img src='images/icon_" + type + ".gif' alt='" + type + " Icon' class='icon' border='0'/><span>" + title + "</span></a>").appendTo(parentUl);
			        if (this.classes) {
				        currentLi.children("span").addClass(this.classes);
			        }
			        if (this.expanded) {
				        currentLi.addClass("open");
			        }
                }
            }

            // This basically does:
            // BreadcrumbNode b = WSCall();
            // b.createNodeInitialLoad(child, 0);

            createNode.apply(response.d, [ulToAppend, currentLi, 0]);
            $(treeRootNode).treeview({add: ulToAppend});
        },
        error: ajaxFailed

    });
}

function loadByHtml(url, id, type, ulToAppend, currentLi, treeRootNode) {
    var params = "{id: '" + id +"', type:'" + type + "'}";

    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $(ulToAppend).html(response.d);
            $(treeRootNode).treeview({add: ulToAppend, prerendered: true});
        },
        error: ajaxFailed

    });
}


var proxied = $.fn.treeview;
$.fn.treeview = function(settings) {
	if (!settings.url) {
		return proxied.apply(this, arguments);
	}
	var container = this;
	//loadInitialTree(settings.url, -1, "Site", this, container);
    setLoading(true);
    if (settings.sync) {
        loadEverything("WS/Content.asmx/GetTocToNode", getActiveDocumentId(), getActiveDocumentType(), this, container);
    } else if (settings.lastState) {
        //loadInitialTree(settings.url, -1, "Site", this, container);
        loadEverything("WS/Content.asmx/GetTocToNode", getTocStateId(), getTocStateType(), this, container);
    } else {
        //loadInitialTree(settings.url, -1, "Site", this, container);
        loadByHtml("WS/Content.asmx/GetInitialTreeTocHtml", -1, "Site", this, null, container);
    }
    setLoading(false);
	var userToggle = settings.toggle;
	return proxied.call(this, $.extend({}, settings, {
		collapsed: true,
		toggle: function() {
            //setLoading(true);
			var $this = $(this);

            setTocStateId($this.attr("id"));
            setTocStateType($this.attr("siteNodeType"));

			if (!$this.hasClass("calledWS") && $this.hasClass("collapsable")) {
				var ulToAppend = $this.children("ul");
                $this.addClass("calledWS");

                //load(settings.url, $this.attr("id"), $this.attr("siteNodeType"), ulToAppend, this, container);
                loadByHtml("WS/Content.asmx/GetNodeToGrandChildrenHtml", $this.attr("id"), $this.attr("siteNodeType"), ulToAppend, this, container);
			}
			if (userToggle) {
				userToggle.apply(this, arguments);
			}
            //setLoading(false);
		}
	}));
};

})(jQuery);