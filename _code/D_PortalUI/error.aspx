<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="DesktopPortalBanner.ascx" %>
<%@ Page language="c#" Codebehind="error.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.error" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AICPA OnLine Publications</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" name="vs_targetSchema">
		<LINK href="ASPNETPortal.css" type="text/css" rel="stylesheet">
		<script type="text/javascript" src="Scripts/Destroyer.js"></script>
		<script>
			function displayInfo(){

				if(document.getElementById("errors") == null){
					setTimeout("displayInfo()",5);
					return;
				}
				var oDiv = document.getElementById("errors").style;
				oDiv.visibility = "visible";
				return;
			}
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" marginwidth="0" marginheight="0"
		MS_POSITIONING="FlowLayout" onload="init();">
		<form id="Form1" method="post" runat="server">
			<input id="ErrorPage" type="hidden" value="true" name="ErrorPage">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr vAlign="top">
					<td colSpan="2">
						<ASPNETPORTAL:BANNER id="Banner" runat="server" SelectedTabIndex="0"></ASPNETPORTAL:BANNER>
					</td>
				</tr>
				<tr>
					<td>
						<h1>Our apologies...</h1>
					</td>
				<tr>
					<td><br>
						<asp:label id="errorMsgLb" runat="server" Visible="True"></asp:label></td>
				</tr>
				<tr>
					<td><br>
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td><IMG src="images/portal/icon_tip_24.gif"></td>
								<td>
									<ul class="subscription_listings">
										<li>
											Return to your <A href="javascript:history.back();">previous location</A>.
										</li>
									</ul>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<br><br>
			<asp:Label ID="ShowErrorInfo" Runat="server" Visible="False" CssClass="linkLike">Technical Information</asp:Label>
			<br><div id="errors">
				<table cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td>&nbsp;</td>
						<td noWrap>
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td class="linkLike" noWrap>Error Description:
									</td>
								</tr>
								<tr>
									<td class="linkLike" noWrap>
										<asp:Label ID="ErrorDescription" Runat="server" Visible=True CssClass=linkLike></asp:Label>
									</td>
								</tr>								
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td class="linkLike" noWrap>
										<asp:label id="errorStack" Runat="server" Visible="False" CssClass="linkLike"></asp:label>
									</td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
							</table>
						</td>
						<td>&nbsp;</td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</HTML>
