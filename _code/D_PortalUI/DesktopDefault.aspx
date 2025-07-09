<%@ Page language="c#" CodeBehind="DesktopDefault.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.DesktopDefault" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="DesktopPortalBanner.ascx" %>
<%--

   The DesktopDefault.aspx page is used to load and populate each Portal View.  It accomplishes
   this by reading the layout configuration of the portal from the Portal Configuration
   system, and then using this information to dynamically instantiate portal modules
   (each implemented as an ASP.NET User Control), and then inject them into the page.

--%>
<html>
    <head>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <title>AICPA Online Publications</title>
        <link href="ASPNETPortal.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" src="Scripts/Destroyer.js"></script>
    </head>
    <body leftmargin="0" bottommargin="0" rightmargin="0" topmargin="0" marginheight="0" marginwidth="0" onload="init();" onkeypress="pressedKey();">
		<DIV id="prepage" class="waitMsgShow">
			<center><br><br><br><br>
			<TABLE width=40% cellpadding=2 cellspacing=0 border=0>
				<tr>
					<td valign=middle align=center rowspan=3><img src="images/portal/backgrounds/aicpa_wait.gif"></td>
					<td></td>
				</tr>
				<tr>
					<td id='animWait'><font size="8px" face="Verdana" color="darkred">Loading..... </font></td>
				</tr>
				<tr>
					<td valign=top>
						<script type="text/javascript">
							var bar1= createBar(270,15,'white',1,'darkred','darkblue',85,7,3,"");
						</script>
					</td>
				</tr>
			</TABLE>
			</center> 
		</DIV> 
        <form runat="server">
          <table width="100%" cellspacing="0" cellpadding="0" border="0" width=100%>
                <tr valign="top">
                    <td colspan="2">
                        <ASPNETPortal:Banner id="Banner" SelectedTabIndex="0" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>                        
                        <table width="100%" cellspacing="0" cellpadding="3" border="0">
                            <tr height="*" valign="top">
                                <!--<td width="2"></td>-->
                                <td id="LeftPane" Visible="false" Width="200" runat="server">
                                </td>
                                <td id="ContentPane" Visible="false" Width="*" runat="server">
                                </td>
                                <!--<td width="1"></td>-->
                                <td id="RightPane" Visible="false" Width="170" runat="server">
                                </td>
                                <!--<td width="2"></td>-->
                            </tr>
                        </table>
                    </td>
                </tr>
                <!--<tr>
					<td style="border-top: silver 1px solid;" align=right>
						<img src="images/portal/aicpa_title.gif">
					</td>
				</tr>-->
            </table>
        </form>     
		<script language="javascript">
			tocResize();
			documentResize();
		</script>
    </body>
</html>
