<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Coso.aspx.cs" Inherits="MainUI.Coso" %>
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
<link rel="stylesheet" type="text/css" href="Styles/coso.css" />

<script type="text/javascript" src="js/globalVariables.js"></script>
<script type="text/javascript" src="js/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="js/jquery.easing.1.3.js"></script>
<script type="text/javascript" src="js/detect.js"></script>
<script type="text/javascript" src="js/jQuery-jtemplates.min.js"></script>
<script type="text/javascript" src="js/accordian-javascript.js"> </script>
<script type="text/javascript" src="js/jquery.jBreadCrumb.1.1.js"></script>
<script type="text/javascript" src="js/jquery.ui.core.js"></script>
<script type="text/javascript" src="js/jquery.ui.widget.js"></script>
<script type="text/javascript" src="js/jquery.ui.position.js"></script>
<%--<script type="text/javascript" src="js/jquery.ui.autocomplete.js"></script>--%>

<!--<script src="js/jquery.tabs1.js" type="text/javascript"></script>-->
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
<script type="text/javascript" src="js/coso_code.js"></script>
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
    $.timeout(50 * 60 * 1000, timeoutHandler); // set default timeout to 50 mins
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
<script src="resources/jquery.cookie.js" type="text/javascript"></script>
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



</head>

<body scroll="no" id="application-body" class="coso">
<!--[if lte IE 7]><div id="ie"><![endif]-->
<!--[if lte IE 6]><div id="ie6"><![endif]-->
<form id="form1" runat="server" onsubmit="return false;">
<div id="loading-div" style="display: none;">
    <div id="loading-div-opacity">
        <img id="loading-spinner" src="images/ajax-loading.gif" alt="Loading - Your request will be fulfilled momentarily."/>
    </div>
</div>
<div class="clear"></div>
<div id="footer">
    <div class="logo"><a href="#" onclick="doHomePageLink()"><img src="images/logo-coso.png" alt="COSO Online Library" title="COSO Online Library" border="0"/></a></div>
    <div style="float:right;">
        <ul class="nav">
            <li class="active">
                <a href="#Toolbar-Library">Library</a>
                <div id="Toolbar-Library" class="navColumn"></div>
            </li>
            <li id="Tab-MyDocuments">
                <a href="#Toolbar-MyDocuments">Documents</a>                
                <script type="text/javascript">                        
                    document.write("<b id=\"documentcount\" class=\"multiDocsIcon\">" + getMyScreenCount() + "</b>");
                </script>                
                <div id="Toolbar-MyDocuments" class="navColumn"></div>
            </li>
            <li>
                <a href="#Toolbar-Tools">Tools</a>
                <div id="Toolbar-Tools" class="navColumn">
            	    <ul class="tools">
                        <li><a href="#" onclick="loadToc();">Table of Contents</a></li>
                        <%--<li><a href="#" onclick="loadQuickFind();">Quick Find</a></li>--%>
                	    <li id="Toolbar-Tools-Print"><a href="#" id="button">Print Document</a></li>
                	    <li id="Toolbar-Tools-FontPreferences"><a href="#" onclick="loadPreferences();">Font Preferences</a></li>
                        <% if (ShowSearch)
                            {%>
                                <li id="Toolbar-Tools-SavedSearches"><a href="#" onclick="loadSavedSearches();">Saved Searches</a></li>
                            <%} 
                        %>
                        <li id="Toolbar-Tools-MyNotes"><a href="#" onclick="doNotesLink();">My Notes</a></li>
                        <li id="Toolbar-Tools-MyBookmarks"><a href="#" onclick="doBookmarkLink();">My Bookmarks</a></li>
                    </ul>
                </div>
            </li>
            <li class="settings">
                <a href="#Toolbar-Help"><img src="images/icon-settings.png" alt="Settings Menu" /></a>
                <div id="Toolbar-Help" class="navColumn">
                    <ul class="tools">
                	    <li><a href="#" onclick="loadHelp();">Help</a></li>
                        <li><a href="#" onclick="logoutButton();">Logout</a></li>
                    </ul>
                </div>
            </li>
        </ul>
        <div id="searchForm">
            <% if (ShowSearch)
            { %>
            
            <input type="search" class="search float" id="searchTerms" onfocus="clearSearchBox();" value="New Search" onkeydown="doSearchCheck(event.keyCode);" autocomplete="off" title="For best results, use the advance search feature to search for a phrase."/>
            <a id="searchResultButtonLink" href="#" onclick="getResults()" title="View current search results"><img id="searchResultsButton" src="images/btn-results-grayout.gif" alt="See search results" title="See search results" border="0" /></a>                                   
            <a id="back-button" href="#" onclick="doBackButtonLink();"><img src="images/icon-back.png" alt="Go Back" title="Go Back" border="0" /></a> 
            <%} %>
            <!--<ul class="tabs2">-->
                <% if (ShowSearch)
                { %>
                <!--<li><a href="#" id="search" onclick="doAdvancedNavigationalSearch('', document.getElementById('searchTerms').value, 1, 100, 10, 0, 1, 1,0);" title="For best results, use the advance search feature to search for a phrase."><input id="btnSearch" type="image" src="images/search2.png" class="submit-search float" /></a></li>-->
                <!--<li><a class="loader" onclick="loadBlankSearch();" href="#"><img style="border: none;" id="searchAdvButton" alt="Advanced Search" src="images/adv.png"></a></li>
                <li><a href="#" id="searchResultButtonLink" onclick="getResults();" ><img id="searchResultsButton" src="images/results2.png" alt="See search results" title="See search results" border="0" /></a></li> 
                <%} %>-->
                <!-- <li><a href="#" onclick="doClearHitHighlighting();" ><img src="images/btn-clear.gif" alt="Clear Search" title="Clear Search" border="0" style="padding-right:3px;" /></a></li> -->
                   
                <!-- <li><a class="loader" href="#HOME" onclick="loadHomePage();"><img src="images/btn-home-icon.gif" alt="Home" border="0" /></a></li> -->        
            <!--</ul>-->
            
        </div>
      <div class="clear"></div>
      </div>
</div>
<div id="cright" class="clear cright">
    <p>©2017, Committee of Sponsoring Organizations of the Treadway Commission (COSO).</p>
</div>

<div id="main">
    <div class="header">
        <!-- <div class="logo2"><img src="images/searchbar-right.gif" alt="" border="0" /></div> -->
        <!-- <div class="top-nav">Navigate the Site:</div> -->
        <p id="copyright" class="copyright"><%-- 2010-12-31 sburton: fyi, this default copyright text will be replaced by the copyright for the appropriate page that loads --%>Copyright &copy; <script>                                                                                                                                                                                                     document.write(getCurrentCopyrightYear());</script>, American Institute of Certified Public Accountants, Inc. All Rights Reserved.</p>
        <div class="breadcrumbWrapper">
            <div id="breadcrumbContainer" class="breadcrumbs-container module"></div>
        </div>
        <div class="clear"></div>
        <div class="search-container">
            <div class="searchbar-mid">
                <div class="search-hit-wrapper" id="headerHit">
                    
                    <span class="search-hit-close"><a href="#" onclick="$('#headerHit').hide();" id="hhClose" title="Close"><img src="images/coso_search-hits-close.png" alt="Close" border="0" /></a></span>
	                <span class="search-hit-nextDoc" id="nextHitDoc"><a href="#" title="Find Next Document Hit"></a></span>
    	            <span class="search-hit-next" id="nextHit"><a href="#" title="Find Next Page Hit"></a></span>
    	            <span class="search-hit-prev" id="prevHit"><a href="#" title="Find Previous Page Hit"></a></span>
    	            <span class="search-hit-prevDoc" id="prevHitDoc"><a href="#" title="Find Previous Document Hit"></a></span>
    	            <span class="search-hit-label"></span>
    	            <div class="clear"></div>
                </div>
                <div class="navButtons-wrapper" id="navButtons">
                    <div class="nav-buttons">
                        <div id="nav-prev"><a href="#" onclick="gotoPreviousDocument();" title="Previous Document">< Prev</a></div>
                        <div id="nav-next"><a href="#" onclick="gotoNextDocument();" title="Next Document">Next ></a></div>
                    </div>
                   
                    <div class="doc-tools-wrapper">
                        <!-- <span id="nav-toc"><a href="#" onclick="loadTocDocToggle();" title="Table of Contents" >TOC</a></span> -->
                        <span id="Toolbar-Tools-SearchResults-Top" style="display:none;"><a href="#" id="searchResultsbtn" title="Search Results" onclick="getResults()">Search Results</a></span>
                        <span id="Toolbar-Tools-AddBookmark-Top" style="display:none;"><a href="#" id="addBookmarkbtn" title="Add Bookmark">Bookmark</a></span>
                        <span id="Toolbar-Tools-DeleteBookmark-Top" style="display:none;" ><a href="#" id="deleteBookmarkbtn" title="Delete Bookmark">Delete Bookmark</a></span>
                        <span id="Toolbar-Tools-Print-Top" style="display:none;"><a href="#" id="printBtn" title="Print">Print</a></span>
                    </div>
                    <div id="formatOptionsContainer"></div> 
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
</div>

<div id="Documents">



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
        <button onclick="saveBookmarkCoso()">Save</button>
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
        setContentSize();

        $(window).resize(function () {
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
               
        subtractAmount = 78;
        height = $(window).height() - subtractAmount;

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
