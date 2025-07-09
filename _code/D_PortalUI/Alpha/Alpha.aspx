<%@ Page CodeBehind="Alpha.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.Alpha.Alpha" %>
<%@ Register TagPrefix="radP" Namespace="Telerik.WebControls" Assembly="RadPanelbar" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<HTML>
	<HEAD>
		<title>AICPA Online Publications</title>
		<link href="../ASPNETPortal.css" type="text/css" rel="stylesheet">
			<script type="text/javascript" src="../Scripts/Destroyer.js"></script>
			<script language="javascript">
	
				function onAfterClick(obj)
				{
				document.forms[0].Guid.value = obj.ID;
				document.forms[0].hidSourceSiteCode.value = obj.Value;
				if (obj.Targer != "_self")
				{
						document.forms[0].hidDomain.value = obj.Target;
				}
				else 
				{
	   					document.forms[0].hidDomain.value = "empty"
				}		
				  
				}
				
				function openWindow(url){
					window.open(url, 'AICPA_reSource');
					return;
				}
				
			</script>
	</HEAD>
	<body topmargin="0" leftmargin="0" onload="init();">
		<form id="mainForm" method="post" runat="server">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr class="HeadBg">
					<td colspan="2">
						<table border="0" cellpadding="0" cellspacing="0" width="100%">
							<tr>
								<td height="72">
									<img src="../images/portal/mainLogo.gif" style="HEIGHT:72px">
								</td>
								<td valign="top" align="right" class="bannerText"><span id="todaysDate"></span></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td width="20%">
						<div class="container">
							<div class=""><radp:radpanelbar id="RadPanelbar1" Width="189px" ExpandEffectSettings="duration=0.4" ExpandEffect="Fade"
									Theme="WinXPPanel" ContentFile="~/Alpha/Panelbar.xml" Runat="server"></radp:radpanelbar></div>
						</div>
					</td>
					<td valign="top">
						<img src="../images/portal/resource_logo_pms286.gif">
					</td>
				</tr>
			</table>
			<input id="Guid" type="hidden" name="Guid" runat="server">
			<input id="hidEncPersGUID" type="hidden" name="hidEncPersGUID" runat="server">
			<input id="hidSourceSiteCode" type="hidden" name="" runat="server">
			<input id="hidURL" type="hidden" value="default.aspx" name="" runat="server">
			<INPUT id="hidDomain" type="hidden" name="hidDomain" runat="server">
			<INPUT id="reSourceURL" type="hidden" name="reSourceURL">
			<br>
			<INPUT id="Submit1" type="submit" value="Submit" name="Submit1" runat="server" onserverclick="Submit1_ServerClick">
		</form>
	</body>
</HTML>
