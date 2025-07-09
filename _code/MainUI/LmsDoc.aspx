<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LmsDoc.aspx.cs" Inherits="MainUI.LmsDoc" %>
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
<link rel="Stylesheet" type="text/css" href="Styles/jquery.cluetip.css" />
<link rel="stylesheet" type="text/css" href="Handlers/GetResource.ashx?type=subscription_access" />

<script type="text/javascript" src="js/globalVariables.js"></script>
<script type="text/javascript" src="js/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="js/jquery.easing.1.3.js"></script>
<script type="text/javascript" src="js/detect.js"></script>
<script type="text/javascript" src="js/jQuery-jtemplates.min.js"></script>
<script type="text/javascript" src="js/accordian-javascript.js"> </script>
<script type="text/javascript" src="js/jquery.jBreadCrumb.1.1.js"></script>

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
<%--<script src="js/jquery.highlight-3.js" type="text/javascript"></script>

<style type="text/css">
    .highlight 
    {
        background-color: #5CB3FF;
    }
</style>
--%>

<script type="text/javascript">
    $.timeout(50 * 60 * 1000, timeoutHandler); // set default timeout to 50 mins
</script>

<script src="js/popup.js" type="text/javascript"></script>
<script src="resources/jquery.cookie.js" type="text/javascript"></script>
<script src="resources/jquery.treeview.js" type="text/javascript"></script>
<script src="resources/jquery.treeview.async.js" type="text/javascript"></script>
<script src="js/plainToc.js" type="text/javascript"></script>

<script type="text/javascript">
    
    
        
</script>


</head>

<body scroll="no" id="application-body">
<form runat="server" onsubmit="return false;">
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


<div id="main">
    <div class="header">
    	<div class="logo"><img src="images/logo1.png" alt="AICPA Online Professional Library" title="AICPA Online Professional Library" border="0" height="61"/></div>
        <div style="padding-top:40px" class="upsell">
            <p style="font-size:15px"><b>Please visit the <a style="color:#ff6700;" href="http://www.cpa2biz.com/" target="_blank">AICPA Store</a> to purchase this publication.</b></p>
        </div>
        <div class="logo2"><img src="images/searchbar-right.gif" alt="" border="0" /></div>
        <div class="clear"></div>
        <div class="search-container">
        	<div class="searchbar-left"></div>
            <div class="searchbar-mid">
                <p id="copyright" class="copyright float"><%-- 2010-12-31 sburton: fyi, this default copyright text will be replaced by the copyright for the appropriate page that loads --%>Copyright &copy; <script>document.write(getCurrentCopyrightYear());</script>, American Institute of Certified Public Accountants, Inc. All Rights Reserved.</p>
            </div>
		</div>
        <div class="clear"></div>
        <div class="disclaimer-container">    
            <!--<div class="disclaimerbar-left"></div>    	-->
            <div class="disclaimerbar-mid">
                <p id="disclaimer" class="disclaimer float">Permission to use this content is strictly limited to the intended purpose of review and assistance in completion of this course. Republication of this material in print, on websites or in any other form, format or media without the express written permission of the AICPA is prohibited. Requests for permission to republish or redistribute this material should be directed to <a href="mailto:copyright@aicpa.org">copyright@aicpa.org</a>.</p>
            </div>
        </div>
	</div>
    <div class="clear"></div>
    <div id="content-container">
        <div id="document-container" class="document-container" ></div>
        <div id="backup-document-container" class="document-container" style="display:none;"></div>
        <iframe id="iframe-main" class="iframe-main" frameborder="0" ></iframe>
        <iframe id="backup-iframe-main" class="iframe-main" frameborder="0" style="display:none;"></iframe>
    </div>
    <div class="clear"></div>
</div>


</form>
<script type='text/javascript'>
    $(function () {
        $('#iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        $(window).resize(function () {
            $('#iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        });
        $('#backup-iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        $(window).resize(function () {
            $('#backup-iframe-main').css({ 'width': (($(document).width()) - 85) + 'px' });
        });
    });



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
//        $('#iframe-main').css({ 'width': getContentAreaWidth() + 'px' });
        //$('#iframe-main').css({ 'padding-left': 0 +'px' });       
    }

    function getContentAreaHeight() {
        var height;
        

        height = $(document).height() - 170;

        return height;
    }

    function getContentAreaWidth() {
        var height;
        height = $(document).width() - 78;
        return height;
    }

    
   

</script>
</body>
</html>
