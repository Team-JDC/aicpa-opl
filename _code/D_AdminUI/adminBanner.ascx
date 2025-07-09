<%@ Register TagPrefix="radts" Namespace="Telerik.WebControls" Assembly="RadTabStrip" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="adminBanner.ascx.cs" Inherits="D_AdminUI.adminBanner" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table border="0" cellpadding="0" cellspacing="0" width="100%" bgcolor="#ffffff">
	<tr>
		<td colspan="2">
			<table border=0 cellpadding=0 cellspacing=0 width=100% style="background-image:url(images/apiBg.gif);background-repeat:repeat-x;">
				<tr>
					<td width="10%" valign="middle" align="center">
						<img src="images/adminLogo.gif">
					</td>
					<td valign=top align="right">
						<table border=0 cellpadding=0 cellspacing=0 width=100%>
							<tr>
								<td valign=top align="right">
									<a href="#" onClick="changeContent(1);" class="TopLinks">Site Manager</a> | 
									<a href="#" onClick="changeContent(2);" class="TopLinks">Book Manager</a> | 
									<a href="#" onClick="changeContent(3);" class="TopLinks">Subscription Manager</a> | 
									<a href="#" onClick="changeContent(4);" class="TopLinks">Reports</a>&nbsp;
								</td>
							</tr>
							<tr>
								<td height="30px">&nbsp;</td>
							</tr>
							<tr>
								<td class="bannerText" align="right">
									<span id="todaysDate"></span>
								</td>
							</tr>
						</table>
					
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td width="99%"  valign=top align=center>
			<table border=0 cellpadding=0 cellspacing=0 width=100%>
				<tr>
					<td class="iFrameClass">
						<iframe id='containerIframe' src="ManageSite.aspx" scrolling=auto frameborder=0 width=100% height="480px"></iframe>
					</td>
				</tr>
			</table>
		</td>
		<!--<td nowrap valign=top>
			<table border=0 cellpadding=0 cellspacing=0 width=100%>
				<tr>
					<td>
						<radTS:RadTabStrip width="100%" AfterClientTabClick="changeContent" id="tabMenu" Runat="server" Theme="../RadControls/TabStrip/app_themes/VerticalRight" SelectedIndex="0"></radTS:RadTabStrip>
					</td>
				</tr>
				<tr>
					<td height="170px" id="tableSpacer" style="BORDER-LEFT: silver 1px solid;">&nbsp;
					</td>
				</tr>
			</table>
		</td>-->
	</tr>
</table>
<asp:Label id="jsLabel" runat="server" Visible="False"></asp:Label>
