<%@ Page language="c#" Codebehind="license.aspx.cs" AutoEventWireup="True" Inherits="ExamCandidate.licenese" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>licenese</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script>
			function documentResize()
			{
				var maxHeight = screen.height + document.documentElement.scrollTop;
				var HeaderHeight = 400;
				var parentHeight = maxHeight - HeaderHeight;
				//var childHeight = parentHeight - HeaderHeight;
				//document.getElementById("containerIframe").style.height = parentHeight;
				document.getElementById("frameHolder").style.height = parentHeight;
				return;
			}	
		</script>
  </HEAD>
	<BODY onLoad="documentResize();">
		<form id="Register" method="post" runat="server">
			<table width="%100" align="center">
				<tr>
					<td>
						<table width="%100" align="center">
							<tr>
								<td>
                                    <h4 style="font:bold 18px Arial, Helvetica, sans-serif; color:#ff6700; padding:0 0 5px 0; margin:0 0 10px 0; border-bottom:1px dotted #ccc;">Welcome to AICPA Online Professional Library</h4>
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
					<table border="0" cellpadding="0" cellspacing="0" align="center">
						<tr>
							<td style="BACKGROUND-POSITION:50% top;BACKGROUND-IMAGE:url(images/topBg.gif);BACKGROUND-REPEAT:repeat-x"><img src='images/cornerLeft.gif'></td>
							<td style="BACKGROUND-POSITION:50% top;BACKGROUND-IMAGE:url(images/topBg.gif);BACKGROUND-REPEAT:repeat-x"></td>
							<td><img src='images/cornerRight.gif'></td>
						</tr>
						<tr>
							<td style="BACKGROUND-POSITION:left 50%;BACKGROUND-IMAGE:url(images/bgLeft.gif);BACKGROUND-REPEAT:repeat-y"
								nowrap width="30" align="right"></td>
							<td>
								<table width="100%" align="center">
									<tr>
										<td><iframe SRC="LicenseText.htm" scrolling="yes" id="frameHolder" width="700" marginwidth="10" marginheight="0"
												frameborder="0"></iframe>
										</td>
									</tr>
								</table>
							</td>
							<td style="BACKGROUND-POSITION:right 50%;BACKGROUND-IMAGE:url(images/bgRight.gif);BACKGROUND-REPEAT:repeat-y"
								width="22"></td>
						</tr>
						<tr>
							<td style="BACKGROUND-POSITION:50% bottom;BACKGROUND-IMAGE:url(images/bottomBg.gif);BACKGROUND-REPEAT:repeat-x"><img src='images/cornerBottomLeft.gif'></td>
							<td style="BACKGROUND-POSITION:50% bottom;BACKGROUND-IMAGE:url(images/bottomBg.gif);BACKGROUND-REPEAT:repeat-x"></td>
							<td style="BACKGROUND-IMAGE:url(images/cornerBottomRight.gif)"></td>
						</tr>
					</table></TR></TABLE>
		</form>
	</BODY>
</HTML>
