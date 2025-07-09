<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_Search" CodeBehind="D_Search.ascx.cs" AutoEventWireup="True" enableViewState="False"%>
<style type="text/css">
    .style1
    {
        width: 22px;
    }
</style>
<table id="SearchTable" cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr vAlign="top">
		<td width="517" colspan="2">
			<img src="images/portal/searchTitle.gif">
		</td>
		<td id="SearchNavTd" align="right" runat="server">
			<asp:imagebutton id="HelpImageButton" runat="server" AlternateText="help" ToolTip="help" ImageUrl="../images/portal/icon_help_16.gif" onclick="HelpImageButton_Click"></asp:imagebutton>
		</td>
	</tr>
	<tr>
		<td colspan="3" align="left" valign="top">
			<table width="100%" border="0" cellspacing="0" cellpadding="0" height="3" background="images/portal/repeat_breadcrumb.jpg">
				<tr>
					<td height="3"></td>
				</tr>
			</table>
		</td>

	</tr>
	<tr vAlign="top">
		<td colspan="2" style="padding-left:20px;padding-top:10px;">
			<table border="0" cellpadding="0" cellspacing="0">
				<tr>
					<td style="background-image:url(images/portal/backgrounds/topBg.gif);background-position:top;background-repeat:repeat-x;"><img src='images/portal/backgrounds/cornerLeft.gif'></td>
					<td style="background-image:url(images/portal/backgrounds/topBg.gif);background-position:top;background-repeat:repeat-x;" colspan=3></td>
					<td style="background-image:url(images/portal/backgrounds/cornerRight.gif);" 
                        class="style1"></td>
				</tr>		
				<tr>
					<td style="background-image:url(images/portal/backgrounds/bgLeft.gif);background-position:left;background-repeat:repeat-y;" nowrap class="searchTxt" valign="middle" width="90" align="right">Search for:&nbsp;</td>
					<td id="SearchTd" runat="server" valign="middle" width="350">
						<asp:textbox id="SearchTextBox" runat="server" Width="368px"></asp:textbox>
					</td>
					<td valign="middle">
						<asp:ImageButton ImageUrl="~/images/portal/go-butt.jpg" Runat="server" id="searchSubmit" onclick="searchSubmit_Click"></asp:ImageButton>
					</td>
					<td class="bannerText" valign=middle>
						&nbsp;<b>
							<asp:LinkButton Runat="server" ID="cleanSearch" CssClass="searchTypeRadio" onclick="cleanSearch_Click">clear search</asp:LinkButton>

							</b></td>
					<td style="background-image:url(images/portal/backgrounds/bgRight.gif);background-position:right;background-repeat:repeat-y;" 
                        class="style1"></td>
				</tr>
				<tr>
					<td style="background-image:url(images/portal/backgrounds/bgLeft.gif);background-position:left;background-repeat:repeat-y;" height="20" nowrap class="searchTxt" valign="top" width="90"></td>
					<td height="20" width="372" valign="top">
						<asp:RadioButton id="AnyWordsRadioButton" runat="server" Text="Any Words" CssClass="searchTypeRadio"
							GroupName="searchType"></asp:RadioButton>
						<asp:RadioButton id="AllWordsRadioButton" runat="server" Text="All Words" Checked="True" CssClass="searchTypeRadio"
							GroupName="searchType"></asp:RadioButton>
						<asp:RadioButton id="PhraseRadioButton" runat="server" Text="Phrase" CssClass="searchTypeRadio" GroupName="searchType"></asp:RadioButton>
						<asp:RadioButton id="BooleanRadioButton" runat="server" Text="Boolean" CssClass="searchTypeRadio"
							GroupName="searchType"></asp:RadioButton>
					</td>
					<td colspan=3 height="20" style="background-image:url(images/portal/backgrounds/bgRight.gif);background-position:right;background-repeat:repeat-y;"></td>
				</tr>
				<tr>
					<td style="background-image:url(images/portal/backgrounds/bgLeft.gif);background-position:left;background-repeat:repeat-y;" class="searchTxt" valign="baseline" nowrap width="90" height="20"></td>
					<td width="372" height="20">
						<asp:CheckBox id="ShowExcerptsCheckBox" runat="server" Text="Show Excerpts" Checked="True" CssClass="searchOptionCheck"></asp:CheckBox>
					</td>
					<td height="20" colspan=3  style="background-image:url(images/portal/backgrounds/bgRight.gif);background-position:right;background-repeat:repeat-y;"></td>
				</tr>
				<tr vAlign="top">
					<td style="background-image:url(images/portal/backgrounds/bgLeft.gif);background-position:left;background-repeat:repeat-y;" class="searchTxt" valign="baseline" nowrap width="90" height="20"></td>
					<td width="372" height="20">
						<asp:CheckBox id="SearchUnsubscribedCheckBox" runat="server" Text="Search Unsubscribed Content"
							CssClass="searchOptionCheck"></asp:CheckBox>
					</td>
					<td height="20" colspan=3 style="background-image:url(images/portal/backgrounds/bgRight.gif);background-position:right;background-repeat:repeat-y;"></td>
				</tr>
				<tr>
					<td style="background-image:url(images/portal/backgrounds/bottomBg.gif);background-position:bottom;background-repeat:repeat-x;"><img src='images/portal/backgrounds/cornerBottomLeft.gif'></td>
					<td style="background-image:url(images/portal/backgrounds/bottomBg.gif);background-position:bottom;background-repeat:repeat-x;" colspan=3></td>
					<td style="background-image:url(images/portal/backgrounds/cornerBottomRight.gif);" 
                        class="style1"></td>
				</tr>	
			</table>
		</td>
	</tr>
	
</table>
<div id="hiddenArea">
	<br>
	<table id="SelectedSearchDimensionsTable" border="0" class="searchTable" cellSpacing="0"
		cellPadding="0" width="100%" runat="server">
		<tr>
			<td colspan="2" class="searchResultHeader">Selected Search Dimensions</td>
		</tr>
	</table>
	<table class="searchTable" id="RefinementSearchDimensionsTableHolder" runat="server" border="0"
		cellSpacing="0" cellPadding="0" width="100%">
		<tr>
			<td class="searchResultHeader"><asp:Linkbutton runat="server" id="showRefinementSearchDimensionsLink" onclick="showRefinementSearchDimensionsLink_Click">Refine Your Search</asp:Linkbutton></td>
			<td class="searchResultHeader" align="right" valign="top">
				<asp:ImageButton ImageUrl="~/images/portal/topicbar_down.gif" Runat="server" id="showRefinementSearchDimensions" onclick="expandeCollapseRefinement"></asp:ImageButton>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<table id="RefinementSearchDimensionsTable" class="searchTableResults" border="0" cellSpacing="0"
					cellPadding="0" width="100%" runat="server">
				</table>
			</td>
		</tr>
	</table>
	<br>
	<table id="SearchSummaryTable" border="0" cellSpacing="0" cellPadding="0" width="100%"
		runat="server" class="searchTable">
		<tr vAlign="top">
			<td vAlign="top" class="resultsNavHeader">
				<asp:Label id="PageFirstHitLabel" runat="server"></asp:Label>&nbsp;-
				<asp:Label id="PageLastHitLabel" runat="server"></asp:Label>&nbsp;of
				<asp:Label id="TotalHitsLabel" runat="server"></asp:Label></td>
			<td id="SearchResultsSummaryNextPrev" vAlign="top" align="right" runat="server" class="resultsNavHeader">
				<asp:LinkButton id="PrevHitsLinkButton" runat="server" onclick="PrevHitsLinkButton_Click">prev</asp:LinkButton>&nbsp;
				<asp:LinkButton id="NextHitsLinkButton" runat="server" onclick="NextLinkButton_Click">next</asp:LinkButton></td>
		</tr>
		<tr>
			<td height="10" align=right colspan=2>
				<asp:Label ID="unsubscribedLegend" Runat=server Visible=False></asp:Label>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<table id="SearchResultsTable" border="1" style="BORDER-BOTTOM: #cccccc 1px solid" cellSpacing="0"
					cellPadding="0" width="95%" runat="server">
				</table>
			</td>
		</tr>
		<tr>
			<td height="10"></td>
		</tr>		
		<tr>
			<td vAlign="top" class="resultsNavFooter">
				<asp:Label id="PageFirstHitLabelBtm" runat="server"></asp:Label>&nbsp;-
				<asp:Label id="PageLastHitLabelBtm" runat="server"></asp:Label>&nbsp;of
				<asp:Label id="TotalHitsLabelBtm" runat="server"></asp:Label>
			</td>		
			<td vAlign="top" align="right"  class="resultsNavFooter">
				<asp:LinkButton id="PrevHitsLinkButtomBtn" runat="server" onclick="PrevHitsLinkButton_Click">prev</asp:LinkButton>&nbsp;
				<asp:LinkButton id="NextHitsLinkButtomBtn" runat="server" onclick="NextLinkButton_Click">next</asp:LinkButton>			
			</td>
		</tr>
	</table>
	<asp:Label ID="searchNotFound" Runat="server" Visible=False></asp:Label>
	<asp:Label ID="whatToDo" Runat=server Visible=False>
	<p>Suggestions:</p>
	<ul>
		<li class="SearchSuggestions">Make sure all words are spelled correctly.</li>
		<li class="SearchSuggestions">Try different keywords.<BR></li>
		<li class="SearchSuggestions">Try more general keywords.</li>
	</ul>
	</asp:Label>
	<asp:Label id="jsLabel" runat="server"></asp:Label>
</div>
<div id="WaitingDiv" style="DISPLAY:none;PADDING-TOP:50px">
	<center>
		<img src="images/portal/loading.gif">&nbsp; searching...please wait...
	</center>
</div>
