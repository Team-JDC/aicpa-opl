<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx" %>
<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_Document"
    CodeBehind="D_Document.ascx.cs" AutoEventWireup="True" EnableViewState="True" %>

<script type="text/javascript">

    function printPrompt(x, y) {

        var oPointer = document.getElementById("dhtmlpointer").style;
        var oBox = document.getElementById("dhtmltooltip").style;

        oPointer.top = y;
        oPointer.left = x - 15;
        oPointer.visibility = "visible";

        oBox.top = y + 14;
        oBox.left = x - 200;
        oBox.visibility = "visible";

        var isCodification = getIsCodification();
        var oIncludeSources = document.getElementById("divIncludeSources").style;

        if (isCodification) {
            oBox.height = 140;
            oIncludeSources.display = "block";
        }
        else {
            oIncludeSources.display = "none";
        }

        return;
    }

    function printPromptClose() {

        var oPointer = document.getElementById("dhtmlpointer").style;
        var oBox = document.getElementById("dhtmltooltip").style;

        oPointer.visibility = "hidden";
        oBox.visibility = "hidden";
    }

    function print(printSubDocs, printSourceFragments) {
        var isCodification = getIsCodification();
        // Add for Print to pdf, djf 2/23/10
        var windowUrl = "D_PrintDocument.aspx?p_pd=" + printSubDocs + "&isCodification=" + isCodification + "&showCodificationSources=" + printSourceFragments + "&printToPDF=" + "false";
        var windowHandle = window.open(windowUrl, "Print", "width=600,height=250,left=0,top=0,screenX=0,screenY=0");
        //var oPrintFrame = document.getElementById("PrintFrame");
        //oPrintFrame.src = windowUrl;
        printPromptClose();
    }

    function createPDF(printSubDocs, printSourceFragments) {
        var isCodification = getIsCodification();
        // Modified for Print to pdf, djf 2/23/10
        var windowUrl = "D_PrintDocument.aspx?p_pd=" + printSubDocs + "&isCodification=" + isCodification + "&showCodificationSources=" + printSourceFragments + "&printToPDF=" + "true";
        var windowHandle = window.open(windowUrl, "Print", "width=600,height=250,left=0,top=0,screenX=0,screenY=0");
        //var oPrintFrame = document.getElementById("PrintFrame");
        //oPrintFrame.src = windowUrl;
        printPromptClose();
    }

    function getPrintSubDocs() {
        return document.getElementById("printTypeSubDocs").checked;
    }

    function getPrintSourceFragments() {
        return document.getElementById("chkPrintSourceFragments").checked;
    }
    // Add for Print to pdf, djf 2/23/10
    function getPrintToPDF() {
        return document.getElementById("chkPrintToPDF").checked;
    }

    function getIsCodification() {
        var strIsCodification = "<%= IsCodificationDocument %>";
        return (strIsCodification == "True");
    }

    function tocInteraction() {
        keepScrollBarPos();
        loc = window.location.href;
        loc = loc.replace("tabindex=-1&", "tabindex=1&");
        if (loc.indexOf("tabid=3") > 0) {
            loc = loc.indexOf("&toc=false") > 0 ? loc.replace("&toc=false", "&toc=true") : loc + "&toc=true";
        } else if (loc.indexOf("tabid=2") > 0) {
            loc = loc.indexOf("&toc=true") > 0 ? loc.replace("&toc=true", "&toc=false") : loc + "&toc=false";
        }
        window.location.href = loc.indexOf("tabid=3") > 0 ? loc.replace("tabid=3", "tabid=2") : loc.replace("tabid=2", "tabid=3");
        return;
    }

    function tocImageToggler() {
        oImg = document.getElementById("tocToggler");
        var tmp = window.location.href;
        oImg.src = tmp.indexOf("tabid=2") > 0 ? "images/portal/hide_toc2.gif" : "images/portal/show_toc2.gif";
        oImg.title = tmp.indexOf("tabid=2") > 0 ? "hide table of contents" : "show table of contents";
        return;
    }
    function docInteraction(tab) {
        //If the enduser clicks on the document_selectable image
        /// get all the imageages because we will need to manipulate each of them every time

        dImg = document.getElementById("docToggler");
        lImg = document.getElementById("linkToggler");
        aImg = document.getElementById("archiveToggler");

        if (tab == 'doc') {
            var pos = dImg.src.lastIndexOf("/");
            var image = dImg.src.substring(pos + 1);
            if (image == 'document_selectable.jpg') {
                dImg.src = "images/portal/document_active.jpg";
                lImg.src = "images/portal/links_selectable.jpg";
                aImg.src = "images/portal/archive_selectable.jpg";
                //todo replace the body of the document 
            }
        }
        // If the end users clicks on the links selectable image
        if (tab == 'link') {
            var pos = lImg.src.lastIndexOf("/");
            var image = lImg.src.substring(pos + 1);
            if (image == 'links_selectable.jpg') {
                //dImg.src = url + "images/portal/document_selectable.jpg";
                dImg.src = "images/portal/document_selectable.jpg";
                lImg.src = "images/portal/links_active.jpg";
                aImg.src = "images/portal/archive_selectable.jpg";
                //todo replace the body of the document 
            }
        }
        // If the end users clicks on the links selectable image
        if (tab == 'archive') {
            var pos = aImg.src.lastIndexOf("/");
            var image = aImg.src.substring(pos + 1);
            if (image == 'archive_selectable.jpg') {
                dImg.src = "images/portal/document_selectable.jpg";
                lImg.src = "images/portal/links_selectable.jpg";
                aImg.src = "images/portal/archive_active.jpg";
                //todo replace the body of the document 
            }
        }

    }

    function printNonDoc() {
        var isIE = navigator.appName.toUpperCase() == 'MICROSOFT INTERNET EXPLORER' ? true : false;

        if (isIE) {
            document.docframe.focus();
            document.docframe.print();
        }
        else {
            window.frames['docframe'].focus();
            window.frames['docframe'].print();
        }
    }
</script>

<img id="dhtmlpointer" src="images/portal/printTop.gif">
<div id="dhtmltooltip">
    <table class="printText" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td align="center" colspan="2">
                Document Print<br>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <hr>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                What would you like to print?
            </td>
        </tr>
        <tr>
            <td valign="middle">
                <input id="printTypeNoSubDocs" type="radio" checked name="printType">
            </td>
            <td>
                This page only
            </td>
        </tr>
        <tr>
            <td valign="middle">
                <input id="printTypeSubDocs" type="radio" name="printType">
            </td>
            <td>
                This page with all related subpages
            </td>
        </tr>
        <tr>
            <td valign="middle" colspan="2">
                <div id="divIncludeSources">
                    <input id="chkPrintSourceFragments" type="checkbox" name="chkPrintSourceFragments">Include Sources</input>
                </div>
            </td>
        </tr>
        <tr>
            <td align="right" colspan="2">
                <span style="cursor: pointer" onclick="createPDF(getPrintSubDocs(),getPrintSourceFragments());">
                    PDF
                    <img src="images/portal/pdficon_large.gif" alt="PDF" /></span>&nbsp;&nbsp;&nbsp;&nbsp;
                <span style="cursor: pointer" onclick="print(getPrintSubDocs(),getPrintSourceFragments());">
                    Print
                    <img src="images/portal/print.gif" alt="Print"/></span>&nbsp;&nbsp;&nbsp;&nbsp;
                    <span style="cursor: pointer"
                        onclick="printPromptClose();">Close
                        <img src="images/portal/close.gif"/></span>
            </td>
        </tr>
    </table>
</div>
<div id="docNavMsg">
    <table class="printText" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td align="center" colspan="2">
                <b><font color="darkred" size="+2">
                    <img src="images/portal/icon_tip_24.gif">&nbsp;Tip</font></b><br>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <hr>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <font size="+1">W</font>e would like to suggest hiding the <b>'Table of Contents'</b>
                to make your document navigation a faster experience.<br>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                You can do so by clicking the "hide table of contents" icon&nbsp;<img style="border-right: #cecece 1px solid;
                    border-top: #cecece 1px solid; border-left: #cecece 1px solid; border-bottom: #cecece 1px solid"
                    src="images/portal/hide_toc2.gif"><br>
                <br>
            </td>
        </tr>
        <tr>
            <td valign="bottom" nowrap align="left">
                <span style="cursor: hand" onclick="doNotAsk();">Do not suggest again</span>
            </td>
            <td align="right">
                <span style="cursor: hand" onclick="tocInteraction();resetCookies();">Hide TOC
                    <img src="images/portal/hide_toc2.gif"></span>&nbsp;&nbsp;&nbsp;&nbsp;<span style="cursor: hand"
                        onclick="aicpaMsgClose();">Close
                        <img src="images/portal/close.gif"></span>
            </td>
        </tr>
    </table>
</div>
<table id="DocTable" cellspacing="0" cellpadding="0" width="100%" border="0">
    <tr class="TocTop" valign="top">
        <td width="2%">
            &nbsp;<img src="images/portal/header_bullet.gif">
        </td>
        <!-- <td valign="middle" style="border-top:solid 1px #aeaeae;border-bottom:solid 1px #aeaeae;">
			<img src="images/portal/document.gif">
		</td> -->
        <td id="DocTab" valign="middle" align="left" runat="server">
            <!-- <span onClick="docInteraction('doc');" style="CURSOR:pointer">
				<img id="docToggler" src="images/portal/document_active.jpg"></span>
			<span onClick="docInteraction('link');" style="CURSOR:pointer">
				<img id="linkToggler" src="images/portal/links_selectable.jpg"></span>
			<span onClick="docInteraction('archive');" style="CURSOR:pointer">
				<img id="archiveToggler" src="images/portal/archive_selectable.jpg"></span> -->
            <asp:ImageButton ID="docToggler" runat="server" ImageUrl="../images/portal/document_active.jpg"
                AlternateText="View Document" ToolTip="View Document" Visible="True" OnClick="docToggler_Click">
            </asp:ImageButton><asp:ImageButton ID="gotoToggler" runat="server" ImageUrl="../images/portal/goto_selectable.jpg"
                AlternateText="GOTO" ToolTip="GOTO" Visible="False" OnClick="gotoToggler_Click">
            </asp:ImageButton><asp:ImageButton ID="linkToggler" runat="server" ImageUrl="../images/portal/links_selectable.jpg"
                AlternateText="View Reference Links" ToolTip="View Reference Links" Visible="False"
                OnClick="linkToggler_Click"></asp:ImageButton><asp:ImageButton ID="archiveToggler"
                    runat="server" ImageUrl="../images/portal/archive_selectable.jpg" AlternateText="View Archives"
                    ToolTip="View Archives" Visible="False" OnClick="archiveToggler_Click"></asp:ImageButton><asp:ImageButton
                        ID="xrefToggler" runat="server" ImageUrl="../images/portal/xref_selectable.jpg"
                        AlternateText="Cross Reference" ToolTip="Cross Reference" Visible="False" OnClick="xrefToggler_Click">
                    </asp:ImageButton><asp:ImageButton ID="jsectToggler" runat="server" ImageUrl="../images/portal/jsections_selectable.jpg"
                        AlternateText="Join Sections" ToolTip="Join Sections" Visible="False" OnClick="jsectToggler_Click">
                    </asp:ImageButton>
        </td>
        <td id="ContentTypesTd" valign="bottom" align="left" runat="server">
            <asp:PlaceHolder ID="DocumentFormatPlaceHolder" runat="server"></asp:PlaceHolder>
            &nbsp;
        </td>
        <td id="DocNavTd" valign="middle" align="right" runat="server">
            <img id="tocToggler" title="Show Table of Contents" style="cursor: pointer" onclick="tocInteraction();"
                src="images/portal/DOConly.gif">
            <asp:ImageButton ID="ClearSearchImageButton" runat="server" ImageUrl="../images/portal/clearSearch.gif"
                AlternateText="clear search" ToolTip="clear search" Visible="False" OnClick="ClearSearchImageButton_Click">
            </asp:ImageButton><asp:ImageButton ID="showSource" runat="server" ImageUrl="../images/portal/page_code.gif"
                AlternateText="show source content" ToolTip="show source content" Visible="False"
                OnClick="showSource_Click"></asp:ImageButton><asp:ImageButton ID="hideSource" runat="server"
                    ImageUrl="../images/portal/page_uncode.gif" AlternateText="hide source content"
                    ToolTip="hide source content" Visible="False" OnClick="hideSource_Click">
            </asp:ImageButton>&nbsp;<asp:ImageButton ID="PrintImageButton" runat="server" ImageUrl="../images/portal/print.gif"
                AlternateText="print" ToolTip="print" Visible="False" OnClick="printPrompt">
            </asp:ImageButton><asp:ImageButton ID="NonDocPrintImageButton" runat="server" ImageUrl="../images/portal/print.gif"
                AlternateText="print" ToolTip="print" Visible="False"></asp:ImageButton>&nbsp;&nbsp;
            <asp:ImageButton ID="PrevDocImageButton" runat="server" ImageUrl="../images/portal/doc_back.gif"
                AlternateText="previous document" ToolTip="previous document" Visible="False"
                OnClick="PrevDocImageButton_Click"></asp:ImageButton><asp:ImageButton ID="NextDocImageButton"
                    runat="server" ImageUrl="../images/portal/doc_next.gif" AlternateText="next document"
                    ToolTip="next document" Visible="False" OnClick="NextDocImageButton_Click">
            </asp:ImageButton>&nbsp;
            <asp:ImageButton ID="HelpImageButton" runat="server" ImageUrl="../images/portal/icon_help_16.gif"
                AlternateText="help" ToolTip="help" OnClick="HelpImageButton_Click"></asp:ImageButton>&nbsp;
        </td>
        <td align="right" width="2%">
            <img src="images/portal/header_bullet.gif">&nbsp;
        </td>
    </tr>
    <tr valign="top">
        <td rowspan="4">
            <img height="400" src="images/portal/border.gif">
        </td>
        <td id="DocCopyTd" align="left" colspan="3" runat="server">
            <b><font color="darkred">
                <asp:Label ID="DocCopyLabel" runat="server"></asp:Label></font></b>
        </td>
        <td align="right" rowspan="4">
            <img src="images/portal/border.gif">
        </td>
    </tr>
    <tr valign="top">
        <td id="DocRefTd" align="left" colspan="3" runat="server">
            &nbsp;&nbsp;<asp:PlaceHolder ID="DocRefPlaceHolder" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
    <tr style="background-position: 50% bottom; background-image: url(images/portal/bg4.gif);
        background-repeat: repeat-x" valign="top">
        <td colspan="3" height="12">
        </td>
    </tr>
    <tr valign="top">
        <td id="DocTd" colspan="3" runat="server">
            <asp:Literal ID="DocLiteral" runat="server" EnableViewState="False"></asp:Literal>
        </td>
    </tr>
</table>
<input id="Xvalue" type="hidden" name="Xvalue" runat="server">
<input id="Yvalue" type="hidden" name="Yvalue" runat="server">
<asp:Label ID="documentJSLabel" runat="server" Visible="False"></asp:Label>

<script>
    tocImageToggler();
    docNavStopTimer();
    //reScrollDocument();
</script>

