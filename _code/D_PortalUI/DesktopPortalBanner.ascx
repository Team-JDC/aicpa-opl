<%@ Import Namespace="AICPA.Destroyer.UI.Portal" %>
<%@ Control CodeBehind="DesktopPortalBanner.ascx.cs" Language="c#" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.DesktopPortalBanner" %>
<%--

   The DesktopPortalBanner User Control is responsible for displaying the standard Portal
   banner at the top of each .aspx page.

   The DesktopPortalBanner uses the Portal Configuration System to obtain a list of the
   portal's sitename and tab settings. It then render's this content into the page.

--%>
<table width="100%" cellspacing="0" cellpadding="0" class="HeadBg" border="0">
	<tr>
		<td rowspan="3" width="10%" valign="middle" align="center">
			<img src="images/portal/mainLogo.gif">
		</td>
		<td valign="top" align="right">
			<asp:datalist id="tabs" cssclass="OtherTabsBg" repeatdirection="horizontal" ItemStyle-BorderWidth="0"
				EnableViewState="false" runat="server">
				<ItemTemplate>
					&nbsp;<a <%# getOnClickEvent(((TabStripDetails) Container.DataItem).TabId) %> href='<%= Global.GetApplicationPath(Request) %>/DesktopDefault.aspx?tabindex=<%# Container.ItemIndex %>&tabid=<%# ((TabStripDetails) Container.DataItem).TabId %>' class="OtherTabs" title="<%# ((TabStripDetails) Container.DataItem).ToolTip %>"><%# ((TabStripDetails) Container.DataItem).TabName %></a>&nbsp;|
				</ItemTemplate>
				<SelectedItemTemplate>
					&nbsp;<span class="SelectedTab"><%# ((TabStripDetails) Container.DataItem).TabName %></span>&nbsp;|
				</SelectedItemTemplate>
			</asp:datalist>
		</td>
	</tr>
	<tr>
		<td class="bannerText" align="right" height="25">
			<!--<asp:label id="siteName" CssClass="SiteTitle" EnableViewState="false" runat="server" />-->
			 
			<b>Search</b>&nbsp;
			<asp:textbox id="topSearchTxt" runat="server" CssClass="bannerText" Width="120px"></asp:textbox>
			<asp:ImageButton id="topSearchBtn" runat="server" ImageUrl="images/portal/arrow-sm.gif" onclick="topSearchSubmit"></asp:ImageButton>&nbsp;
		</td>
	</tr>
	<tr>
		<td height="25" valign="top" align="right" class="bannerText">
			<span id="todaysDate"></span><!-- &nbsp;&nbsp;&nbsp;<span onclick="sendToTutorial();" style="cursor:pointer" title='click here to go to the AICPA Tutorial'><img src="images/tutorialIcon2.gif" border=0>&nbsp;<b>Tutorial</b>&nbsp;</span> -->
		</td>
	</tr>
</table>
