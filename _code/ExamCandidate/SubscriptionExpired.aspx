<%@ Page language="c#" Codebehind="SubscriptionExpired.aspx.cs" AutoEventWireup="True" Inherits="ExamCandidate.SubscriptionExpired" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>SubscriptionExpired</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="ASPNETPortal.css" type=text/css rel=stylesheet >
		<script>
 		function redirect(Url)
 		{
 			window.parent.redirect(Url);
 			return;
 		}		
 	</script>
	</HEAD>
	<BODY>
		<form id="Register" method="post" runat="server">
			<table width="%100" align="center">
				<tr>
					<td>
						<table width="%100" align="center">
							<tr>
								<td>
                                    <h4>Welcome to AICPA Online Professional Library</h4>
                                  </td>
							</tr>
							<!-- <tr>
								<td>
									<table width="100%" border="0" cellspacing="0" cellpadding="0" height="3" style="BACKGROUND-IMAGE: url(images/repeat_breadcrumb.jpg);BACKGROUND-REPEAT: repeat-x">
										<tr>
											<td height="3"></td>
										</tr>
									</table>
								</td>
							</tr> -->
						</table>
					</td>
				</tr>
				<tr>
				</tr>
			</table>
			<table border="0" cellpadding="0" cellspacing="0" >
				<tr>
					<td style="BACKGROUND-POSITION:50% top; BACKGROUND-IMAGE:url(images/topBg.gif); BACKGROUND-REPEAT:repeat-x"><img src='images/cornerLeft.gif'></td>
					<td style="BACKGROUND-POSITION:50% top; BACKGROUND-IMAGE:url(images/topBg.gif); BACKGROUND-REPEAT:repeat-x"></td>
					<td><img src='images/cornerRight.gif'></td>
				</tr>
				<tr>
					<td style="BACKGROUND-POSITION:left 50%; BACKGROUND-IMAGE:url(images/bgLeft.gif); BACKGROUND-REPEAT:repeat-y"
						nowrap width="30" align="right"></td>
					<td>
						<table width="100%" align="center">
							<tr>
								<td>
									<p style='FONT-SIZE:14pt;FONT-FAMILY:Arial;'><b>User Account has Expired</b></p>
									<p style='FONT-SIZE:10pt;FONT-FAMILY:Arial;'>This user account has expired and is no longer valid</p>
									<p style='FONT-SIZE:10pt;FONT-FAMILY:Arial;'>Click <b><asp:hyperlink id="redirect" Runat="server">here</asp:hyperlink></b> to renew your 
										registration.</p>	
										
								</td>
							</tr>
						</table>
					</td>
					<td style="BACKGROUND-POSITION:right 50%; BACKGROUND-IMAGE:url(images/bgRight.gif); BACKGROUND-REPEAT:repeat-y"
						width="22"></td>
				</tr>
				<tr>
					<td style="BACKGROUND-POSITION:50% bottom; BACKGROUND-IMAGE:url(images/bottomBg.gif); BACKGROUND-REPEAT:repeat-x"><img src='images/cornerBottomLeft.gif'></td>
					<td style="BACKGROUND-POSITION:50% bottom; BACKGROUND-IMAGE:url(images/bottomBg.gif); BACKGROUND-REPEAT:repeat-x"></td>
					<td style="BACKGROUND-IMAGE:url(images/cornerBottomRight.gif)"></td>
				</tr>
			</table>
			</TR></TABLE>
		</form>
	</BODY>
</HTML>
