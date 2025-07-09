<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_MySubscriptions" CodeBehind="D_MySubscriptions.ascx.cs" AutoEventWireup="True" enableViewState="True"%>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<!--<table border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td><img src="images/portal/aicpa_top2.gif"></td>
	</tr>
	<tr>
		<td align="center">-->
			<table cellSpacing="0" cellPadding="0" width="100%" border="0" ><!--style="BACKGROUND-IMAGE:url(images/portal/aicpa_bg.gif);BACKGROUND-REPEAT:repeat-y"-->
				<tr vAlign="top">
					<td width="10"></td>
					<td class="publicationHeader">
						<!--<img src="images/portal/reSource2.gif"> -->
						<img src="images/portal/publicationsHeader.gif">
					</td>
					<td id="MySubscriptionsNavTd" align="right" runat="server">
						<asp:imagebutton id="HelpImageButton" runat="server" AlternateText="help" ToolTip="help" ImageUrl="../images/portal/icon_help_16.gif" onclick="HelpImageButton_Click"></asp:imagebutton>
					</td>
					<td width="10"></td>
				</tr>
				<tr>
					<td></td>
					<td colspan="2">
						<table width="100%" border="0" cellspacing="0" cellpadding="0" height="3" background="images/portal/repeat_breadcrumb.jpg">
							<tr>
								<td height="3"></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr vAlign="top">
					<td width="3"></td>
					<td colspan="4">
						<table id="MySubscriptionsTable" border="0" runat="server" width="100%">
						</table>
					</td>
					<td width="10"></td>
				</tr>
				<tr>
					<td colspan=6 align=center>
						<font face='verdana' color='darkred' size=-1><b>
							<asp:label ID="copyright" Runat="server">
								<span style='cursor:hand;cursor:pointer;' onClick='copyRightGoTo();'>Copyrights and other Notices</span>
							</asp:label>
						</b></font>
					</td>
				</tr>
			</table>
		<!--</td>
	</tr>
	<tr>
		<td><img src="images/portal/aicpa_bottom2.gif"></td>
	</tr>
</table>-->
<input id="BookId" type="hidden" NAME="BookId" runat="server">
<input id="Type" type="hidden" NAME="Type" runat="server">
