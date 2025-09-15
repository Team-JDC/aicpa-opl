<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ethics.aspx.cs" Inherits="MainUI.Ethics" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title>AICPA Online Professional Library</title>
<link rel="stylesheet" type="text/css" href="Styles/detect1280.css"/>
<link rel="stylesheet" type="text/css" href="Styles/main.css" />
<link rel="stylesheet" type="text/css" href="Styles/Base.css" />
<link rel="stylesheet" type="text/css" href="Styles/BreadCrumb.css" />
<link rel="stylesheet" type="text/css" href="Styles/pop.css" />
<link rel="stylesheet" type="text/css" href="Styles/notes.css" />
<link rel="stylesheet" type="text/css" href="Styles/loading.css" />
<link rel="stylesheet" type="text/css" href="resources/jquery.treeview.css" />
<link rel="Stylesheet" type="text/css" href="Styles/jquery.ui.core.css" />
<link rel="Stylesheet" type="text/css" href="Styles/jquery.ui.theme.css" />
<%--<link rel="Stylesheet" type="text/css" href="Styles/jquery.ui.autocomplete.css" />--%>
<link rel="Stylesheet" type="text/css" href="Styles/jquery.cluetip.css" />
<link rel="stylesheet" type="text/css" href="Handlers/GetResource.ashx?type=subscription_access" />
<link rel="stylesheet" type="text/css" href="Styles/ethics.css" />

<script type="text/javascript" src="js/globalVariables.js"></script>
<script type="text/javascript" src="js/jquery-3.7.1.min.js"></script>
<script type="text/javascript" src="js/jquery.easing.1.3.js"></script>
<script type="text/javascript" src="js/detect.js"></script>
<script type="text/javascript" src="js/jQuery-jtemplates.min.js"></script>
<script type="text/javascript" src="js/accordian-javascript.js"> </script>
<script type="text/javascript" src="js/jquery.jBreadCrumb.1.1.js"></script>
<script type="text/javascript" src="js/jquery.ui.core.js"></script>
<script type="text/javascript" src="js/jquery.ui.widget.js"></script>
<script type="text/javascript" src="js/jquery.ui.position.js"></script>
<%--<script type="text/javascript" src="js/jquery.ui.autocomplete.js"></script>--%>

<script src="js/jquery.tabs1.js" type="text/javascript"></script>
<!-- <script src="js/jquery-slide-panel.js" type="text/javascript"></script> -->
<script src="js/backButton.js" type="text/javascript"></script>
<script src="js/webServiceHelpers.js" type="text/javascript"></script>
<script src="js/jquery.timeout.js" type="text/javascript"></script>
<script type="text/javascript" src="js/jquery.cluetip.js"></script>
<script src="js/searchHelper.js" type="text/javascript"></script>
<script src="js/handlers.js" type="text/javascript"></script>
<script src="js/fafTools.js" type="text/javascript"></script>
<script src="js/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
<script type="text/javascript" src="js/processFeatures.js"></script>
<script type="text/javascript" src="js/ethics_code.js"></script>
<%--
<script type="text/javascript" src="js/jquery.zclip.min.js"></script>
<script type="text/javascript" src="js/ZeroClipboard.js"></script>
--%>
<%--<script src="js/jquery.highlight-3.js" type="text/javascript"></script>

<style type="text/css">
    .highlight 
    {
        background-color: #5CB3FF;
    }
</style>


</script>  
--%>
    <script type="text/javascript">
        var d_features = [<asp:Literal runat="server" id="JSVars" text="<%$ Session: D_FEATURES%>"/>];
        var d_returnurl = '<asp:Literal runat="server" id="ReturnUrl" text="<%$ Session: D_RETURNURL%>"/>';
        var d_referringSite = '<asp:Literal runat="server" id="ReferringSiteVar" />';
    </script>  

<script type="text/javascript">
    $.timeout(600 * 60 * 1000, timeoutHandler); // set default timeout to 50 mins
</script>
<script type="text/javascript">
    jQuery(document).ready(function () {
        jQuery("#breadCrumb2").jBreadCrumb();
    });
</script>

<%--<% if (ShowSearch)
   { %>
<script type="text/javascript">
    jQuery(document).ready(function () {
        if ($('#searchTerms').length > 0) {
            $.ajax({
                type: "POST",
                url: "WS/SearchServices.asmx/GetSuggestedSearchTerms",
                dataType: "json",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $("#searchTerms").autocomplete({
                        source: data.d,
                        minlength: 3,
                        position: { my: "right top", at: "right bottom" },
                        maxResults: 30
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        }
    });
</script>
<%} %>--%>

<script src="js/popup.js" type="text/javascript"></script>
<script src="resources/jquery.cookie.min.js" type="text/javascript"></script>
<script src="resources/jquery.treeview.js" type="text/javascript"></script>
<script src="resources/jquery.treeview.async.js" type="text/javascript"></script>
<script src="js/plainToc.js" type="text/javascript"></script>

<script type="text/javascript" src="media/swfobject.js"></script>
<script type="text/javascript">
    swfobject.registerObject("csSWF", "9.0.28", "expressInstall.swf");


   if (window.screen)
   {
       top.window.moveTo(0, 0);
       if (document.all)
       {
           top.window.resizeTo(screen.availWidth, screen.availHeight);
       }
       else if (document.layers || document.getElementById)
       {
           if (top.window.outerHeight < screen.availHeight || top.window.outerWidth < screen.availWidth)
           {
               top.window.outerHeight = screen.availHeight;
               top.window.outerWidth = screen.availWidth;
           }
       }
   }
  
        
</script>

<!-- sburton 2010-06-08: commenting out because we're not using "demo" for our toc 
<script src="resources/demo.js" type="text/javascript"></script>
-->
</head>

<body scroll="no" id="application-body">
<!--[if lte IE 7]><div id="ie"><![endif]-->
<!--[if lte IE 6]><div id="ie6"><![endif]-->
<form id="form1" runat="server" onsubmit="return false;">
<div id="loading-div" style="display: none;">
    <div id="loading-div-opacity">
        <img id="loading-spinner" src="images/ajax-loading.gif" alt="Loading - Your request will be fulfilled momentarily."/>
    </div>
    <!-- <div id="loading-sub-div-container">
        <div id="loading-sub-div">
            Loading - Your request will be fulfilled momentarily.<br />
            Return to the <a href="#" onclick="loadHomePage();">home page</a> instead.
        </div> 
    </div>-->
</div>
<div id="footer">
	<div class="prefooter">
    	<ul class="tabs">
            <div class="homeIcon"><a href="#Toolbar-Home" onclick="doHomePageLink()"></a></div>
            <li class="active"><a href="#Toolbar-Home">My Library</a></li>            
            <li id="Tab-MyDocuments"><a href="#Toolbar-MyDocuments">Documents</a></li>
            <li><a href="#Toolbar-Tools">Tools</a></li>
            <!-- <li><a href="#Toolbar-Preferences">Preferences</a></li> -->
            <li><a href="#Toolbar-Help">Help</a></li>
        </ul>
        <div id="formatOptionsContainer"></div>
        <div class="right" style="right:68px;">
            <span class="feedbackLink float"><a href="mailto:<%= FeedbackEmailAddress %>?subject=Code%20of%20Conduct%20Feedback">Feedback</a></span>
            <span class="hide top5 float"><a id="toolbarToggleButton" href="#" class="btn-slide" onclick="doToolbarToggle();"></a></span>
            <a href="#" onclick="logoutButton();"><img class="float" style="margin:5px 0 0 5px;" src="images/btn-logout.gif" alt="Log Out" border="0" /></a>
        </div>
    </div>
    <div class="clear"></div>
    <div>
        <div id="panel" class="postfooter">
        	<div id="Toolbar-Home" class="tab_content">
            </div>
            <div id="Toolbar-MyDocuments" class="tab_content">
            </div>
            <div id="Toolbar-Tools" class="tab_content">
            	<ul class="tools">
                    <li><a href="#" onclick="loadToc();"><img class="tools" src="images/tools-toc.gif" alt="Table of Contents" title="Table of Contents"  /></a></li>
                    <li><a href="#" onclick="loadQuickFind();"><img class="tools" src="images/tools-find.gif" alt="Quick Find" title="Quick Find"  /></a></li>
                	<li id="Toolbar-Tools-Print"><a href="#" id="button"><img class="tools" src="images/tools-print.gif" alt="Print Document" title="Print Document"  /></a></li>
                	<li id="Toolbar-Tools-FontPreferences"><a href="#" onclick="loadPreferences();"><img src="images/pref-fonts.gif" alt="Font Preferences" title="Font Preferences" /></a></li>
                    <% if (ShowSearch)
                       { %>
                    <li id="Toolbar-Tools-SavedSearches"><a href="#" onclick="loadSavedSearches();"><img src="images/pref-searches.gif" alt="Saved Searches" title="Saved Searches"  /></a></li>
                    <%} %>
                    <li id="Toolbar-Tools-MyNotes"><a href="#" onclick="doNotesLink();"><img src="images/pref-notes.gif" alt="My Notes" title="My Notes" /></a></li>
                    <li id="Toolbar-Tools-MyBookmarks"><a href="#" onclick="doBookmarkLink();"><img src="images/pref-bookmarks.gif" alt="My Bookmarks" title="My Bookmarks" /></a></li>
                    <!-- <li id="Toolbar-Tools-MyPrefs"><a href="#" onclick="loadPreferences();"><img src="images/pref-pref.gif" alt="My Preferences" title="My Preferences" /></a></li> -->
                </ul>
                <input type="checkbox" name="disableDefPopup" value="DefPopupDisabled" id="disableDefPopupId" /><label for="disableDefPopupId" class="disableDefPopupLabel">Disable Definition Popup</label>
                <ul id="Toolbar-Tools-Faf" class="faf"></ul>
            </div>
            <div id="Toolbar-Help" class="tab_content">
                <ul class="tools">
                	<li><a href="#" onclick="loadHelpEthics();"><img class="tools" src="images/tools-help.gif" alt="Help" title="Help"  /></a></li>
                </ul>
            </div>
        </div>
    </div>
</div>

<div id="main">
    <div class="header">
    	<div class="logo"><a href="#" onclick="doHomePageLink()"><img src="images/logo1.png" alt="AICPA Online Professional Library" title="AICPA Online Professional Library" border="0" height="61"/></a></div>
        <div class="logo2"><img src="images/searchbar-right.gif" alt="" border="0" /></div>
        <div class="top-nav">Navigate the Site:</div>
        <p id="copyright" class="copyright"><%-- 2010-12-31 sburton: fyi, this default copyright text will be replaced by the copyright for the appropriate page that loads --%>Copyright &copy; <script>                                                                                                                                                                                                     document.write(getCurrentCopyrightYear());</script>, American Institute of Certified Public Accountants, Inc. All Rights Reserved.</p>
        <div class="breadcrumbWrapper">
            <div class="breadcrumbs-start"></div>
            <div id="breadcrumbContainer" class="breadcrumbs-container module"></div>
            <div class="breadcrumbs-end"></div>
        </div>
        <div class="clear"></div>
        <div class="search-container">
        	<div class="searchbar-left"></div>
            <div class="searchbar-mid">
                <div class="search-hit-wrapper" id="headerHit">
                    
                    <span class="search-hit-close"><a href="#" onclick="$('#headerHit').hide();" title="Close"><img src="images/search-hits-close.png" alt="Close" border="0" /></a></span>
	                <span class="search-hit-nextDoc" id="nextHitDoc"><a href="#" title="Find Next Document Hit"></a></span>
    	            <span class="search-hit-next" id="nextHit"><a href="#" title="Find Next Page Hit"></a></span>
    	            <span class="search-hit-prev" id="prevHit"><a href="#" title="Find Previous Page Hit"></a></span>
    	            <span class="search-hit-prevDoc" id="prevHitDoc"><a href="#" title="Find Previous Document Hit"></a></span>
    	            <span class="search-hit-label"></span>
    	            <div class="clear"></div>
                </div>
                <div class="navButtons-wrapper" id="navButtons">
                    <div id="nav-prev"><a href="#" onclick="gotoPreviousDocument();" title="Previous Document"><img src="images/prev.png" alt="< Prev" border="0" /></a></div>
                    <div id="nav-next"><a href="#" onclick="gotoNextDocument();" title="Next Document"><img src="images/next.png" alt="Next >" border="0" /></a></div>
                    <div id="nav-toc"><a href="#" onclick="loadTocDocToggle();" title="Table of Contents" ><img src="images/toc.png" alt="TOC" border="0" /></a></div>
                    <div class="textOp-wrapper" id="textOpButtons">
    	                <span class="textOp-copy" id="textCopy"><a href="#" title="Copy hilighted text"></a></span>
                    </div>
                    <span id="Toolbar-Tools-AddBookmark-Top" style="display:none;"><a href="#" id="addBookmarkbtn" title="Add Bookmark"><img src="images/bookmark2.png" alt="Bookmark" border="0" /></a></span>
                    <span id="Toolbar-Tools-DeleteBookmark-Top" style="display:none;" ><a href="#" id="deleteBookmarkbtn" title="Delete Bookmark"><img src="images/bookmark2-delete.png" alt="Bookmark" border="0" /></a></span>
                    <span id="Toolbar-Tools-Print-Top" style="display:none;"><a href="#" id="printBtn" title="Print"><img src="images/print2.png" alt="Print" border="0" /></a></span>
                    <span id="Toolbar-Tools-EmailPage-Top" style="display:none;" ><a href="#" id="emailPagebtn" onclick="doEmailLink()" title="Email Link"><img src="images/email2.png" alt="Email" border="0" /></a></span>
                    <span id="Toolbar-Tools-Link-Top" class="nav-toc" style="display:none;"><a href="#" onclick="openLinkDialog()" title="Link"><img src="images/link2.png" alt="Link" border="0" /></a></span>
                </div>          
                <div id="searchForm" class="right" >
                    <% if (ShowSearch)
                       { %>
                    <input type="text" class="search float" id="searchTerms" onfocus="clearSearchBox();" value="New Search" onkeydown="doSearchCheck(event.keyCode);" autocomplete="off" title="For best results, use the advance search feature to search for a phrase."/>
                    <%} %>
                    <ul class="tabs2 float">
                    <% if (ShowSearch)
                       { %>
                        <li><a href="#" id="search" onclick="doAdvancedNavigationalSearch('', document.getElementById('searchTerms').value, 1, 100, 10, 0, 1, 1,0);" title="For best results, use the advance search feature to search for a phrase."><input id="btnSearch" type="image" src="images/search2.png" class="submit-search float" /></a></li>
                        <li><a class="loader" onclick="loadBlankSearch();" href="#"><img style="border: none;" id="searchAdvButton" alt="Advanced Search" src="images/adv.png"></a></li>
                        <li><a href="#" id="searchResultButtonLink" onclick="getResults();" ><img id="searchResultsButton" src="images/results2.png" alt="See search results" title="See search results" border="0" /></a></li> 
                        <%} %>
                        <!-- <li><a href="#" onclick="doClearHitHighlighting();" ><img src="images/btn-clear.gif" alt="Clear Search" title="Clear Search" border="0" style="padding-right:3px;" /></a></li> -->
                        <li id="back-button"><a href="#" onclick="doBackButtonLink();"><img src="images/back2.png" alt="Go Back" title="Go Back" border="0" style="padding-right:5px;" /></a></li>    
                	    <!-- <li><a class="loader" href="#HOME" onclick="loadHomePage();"><img src="images/btn-home-icon.gif" alt="Home" border="0" /></a></li> -->
                        
                    </ul>
                </div>
            </div>
		</div>
	</div>
    <div id="content-container">
        <div id="document-container" class="document-container" ></div>
        <div id="backup-document-container" class="document-container" style="display:none;"></div>
        <iframe id="iframe-main" class="iframe-main" frameborder="0"></iframe>
        <iframe id="backup-iframe-main" class="iframe-main" frameborder="0" style="display:none;"></iframe>
    </div>
    <div class="clear"></div>
    <div class="content-container-bottom ">
    	<div class="bottom-left"></div>
        <div class="bottom-mid"></div>
        <div class="bottom-right"></div>
    </div>
</div>

<div id="popupContact">  
        <div class="print-header" style="width: 650px;">
        	<div class="float" style="margin-top:-8px;">
            	<a href="#" id="printButton"><img src="images/btn-print.gif" alt="Print the document" border="0" /></a>
            </div>
            <div class="float" style="margin:-6px 0 0 8px; font:11px Arial, Helvetica, sans-serif; color:#000;">
                <div class="float" id="pdfPrintOuterDiv" ><input class="checkbox" type="checkbox" id="pdfPrint" /> Print PDF</div>
                <div id="subpagesPrintDiv" class="float"><input class="checkbox" type="checkbox" id="subpagesPrint" style="margin-left:10px;" onchange="loadPrintContent()" /> Print related subpages</div>
                <div id="sourcesPrintDiv" class="float"><input class="checkbox" type="checkbox" id="sourcesPrint" style="margin-left:10px;" onchange="loadPrintContent()" /> Show codification sources</div>
            </div>
        	<div class="float" style="position:absolute; right:20px; top:8px; z-index: 9999;">
            	<a href="#" class="popupContactClose"><img src="images/btn-close.gif" alt="Print the document" border="0" /></a>
            </div>
        </div>  
        <div id="areaToPrint" style="width: 650px;">
            <div class="clear" id="printContent"></div>
        </div>
</div>

<div id="popupLogin">  
    <div>
        <div class="float" style="position:absolute; right:20px; top:8px; z-index: 9999;">
            <a href="#" class="popupLoginClose" onclick="disableUserLogin();"><img src="images/btn-close.gif" alt="Close" border="0" /></a>
        </div>
    </div>
    <div class="tool-container3" id="ethicsLogin">
        <h2 class="borderbot" style="padding-left:0; padding-top: 0;">Please enter your username and password.</h2><br />
        <table cellpadding="0" cellspacing="7">
            <asp:RadioButtonList ID="rblUsers" runat="server" />
            <asp:Label ID="LoginMessageLabel" runat="server"></asp:Label>
            <tr>
                <td><asp:Label ID="UserNameLabel" runat="server" Text="Email:"></asp:Label></td>
                <td><asp:TextBox ID="UserNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="PassWordLabel" runat="server" Text="Password:"></asp:Label></td>
                <td><asp:TextBox ID="PasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><input id="btnLogin" type="button" value="Login" onclick="tryEthicsLogin()" /></td>
            </tr>
            <tr>
                <td colspan="2"><a onclick="sendEthicsPassword();">Forgotten Password?</a></td>
            </tr>
            <tr>
                <td colspan="2"><a onclick="showEthicsRegister();">Register if you don't have an account</a></td>
            </tr>
        </table>    
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
        <br />
    </div>
    <div class="tool-container3" id="ethicsRegister" style="display:none">
        <h2 class="borderbot" style="padding-left:0; padding-top: 0;">Please enter user information below.</h2>
        <table cellpadding="0" cellspacing="7">
            <tr>
                <td><asp:Label ID="RegUserNameLabel" runat="server" Text="Email address (Username):"></asp:Label></td>
                <td><asp:TextBox ID="RegUserNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label></td>
                <td><asp:TextBox ID="FirstNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label></td>
                <td><asp:TextBox ID="LastNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="RegPasswordLabel" runat="server" Text="Password:"></asp:Label></td>
                <td><asp:TextBox ID="RegPasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm password:"></asp:Label></td>
                <td><asp:TextBox ID="ConfirmPasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="RegErrorLabel" runat="server" ForeColor="Red"></asp:Label>
                </td>	
            </tr>
            <tr>
                <td colspan="2">
                    <input id="ethicsRegisterBtn" type="button" value="Register" onclick="tryEthicsRegister()" />
                </td>
            </tr>
        </table>    
    </div>
</div>

<div id="popupBookmark" class="addNoteDiv" style="display:none; left: 165px; top: 120px; z-index: 9999;"> 
    <label>Title:</label>
    <input id="addBookmarkTitleInput" class="addNoteTextinput" type="text" />
    <div>
        <button onclick="saveBookmarkEthics()">Save</button>
        <button onclick="closeAddBookmark()">Close</button>
    </div>
</div>

<div id="genericPopup" class="addNoteDiv" style="display:none; left: 250px; top: 200px; z-index: 999999; padding: 10px;">
    <div id="genericPopupTitle"></div>
    <hr />
    <div id="genericPopupContent" style="word-wrap: break-word;"></div>
    <input id="genericPopupCloseButton" type="button" value="OK" onclick="closeGenericPopup();" style="float:right; margin: 10px 20px 5px 20px; width: 50px;" />
</div>

<div id="backgroundPopup" onclick="disableUserLogin();"></div>

<script type='text/javascript'>
    //$(function () {
        //$('#iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        //$(window).resize(function () {
            //$('#iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
       // });
        //$('#backup-iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        //$(window).resize(function () {
           // $('#backup-iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
       // });
   // });
    
    $(function () {
        // $('#content-container').css({ 'height': (($(document).height()) - 235) + 'px' });
        //$('#content-container').css({ 'height': getContentAreaHeight() + 'px' });
        setContentSize();

        $(window).resize(function () {
            //$('#content-container').css({ 'height': (($(document).height()) - 235) + 'px' });
            //$('#content-container').css({ 'height': getContentAreaHeight() + 'px' });
            setContentSize();

        });
    });

    function setContentSize() {
        $('#content-container').css({ 'height': getContentAreaHeight() + 'px' });
        centerPopup();
    }

    function getContentAreaHeight() {
        var height;
        var subtractAmount;

        if (isToolbarHidden()) {
            subtractAmount = 170;
        }
        else {
            subtractAmount = 235;
        }

        height = $(document).height() - subtractAmount;

        return height;
    }

    //centering popup
    function centerPopup() {
        //request data for centering
        var windowWidth = document.documentElement.clientWidth;
        var windowHeight = document.documentElement.clientHeight;
        var popupHeight = $("#popupContact").height();
        var popupWidth = $("#popupContact").width();

        var topHeight = windowHeight / 2 - popupHeight / 2;
        if (topHeight < 0) {
            topHeight = 0;
        }

        var leftWidth = windowWidth / 2 - popupWidth / 2;
        if (leftWidth < 0) {
            leftWidth = 0;
        }

        //centering
        $("#popupContact").css({
            "left": leftWidth
            });
        //only need force for IE6
        $("#popupContact").height(getContentAreaHeight() + 200);
        $("#backgroundPopup").css({
            "height": windowHeight
        });

    }

</script>
</form>
</body>
</html>
