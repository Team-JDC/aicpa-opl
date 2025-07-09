<%@ Page language="c#" Codebehind="D_PrintCopyRight.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.D_PrintCopyRight" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>D_PrintCopyRight</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0">
  </head>
  <body MS_POSITIONING="FlowLayout" topmargin=0 leftmargin=0 >
	
    <form id="Form1" method="post" runat="server">
		<table border=0 cellpadding=0 cellspacing=0 width="100%">
			<tr>
				<td align=right width="2%">
					<img src="images/portal/copyright.gif" >&nbsp;&nbsp;&nbsp;
				</td>
				<td align=left valign=bottom width="95%">
					<h4>
						<font color="darkred" face='Verdana'>
							<asp:Label ID="docCopyRight" Visible=True Runat=server></asp:Label>
						</font>
						<br>
						<font color="#666666" face=verdana size=-2>
							All content on this site is protected by United States and International Copyright law
						</font>
					</h4>
				</td>
			</tr>
		</table>
     </form>
	
  </body>
</html>
