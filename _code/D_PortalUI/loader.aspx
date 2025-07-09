<%@ Page language="c#" Codebehind="loader.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.loader" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="DesktopPortalBanner.ascx" %>
<%--

   The DesktopDefault.aspx page is used to load and populate each Portal View.  It accomplishes
   this by reading the layout configuration of the portal from the Portal Configuration
   system, and then using this information to dynamically instantiate portal modules
   (each implemented as an ASP.NET User Control), and then inject them into the page.

--%>
<html>
    <head>
        <title>AICPA reSource</title>
        <link href="ASPNETPortal.css" type="text/css" rel="stylesheet"/>
		<script type="text/javascript" src="Scripts/Destroyer.js"></script>        
		<script type="text/javascript">
			function localInit(){
				window.location = "DesktopDefault.aspx";
				return;
			}
		</script>        
    </head>
    <body leftmargin="0" bottommargin="0" rightmargin="0" topmargin="0" marginheight="0" marginwidth="0" onLoad="localInit();">

        <form runat="server" ID="Form1">
          <table width="100%" cellspacing="0" cellpadding="0" border="0" width=100%>
                <tr valign="top">
                    <td colspan="2">
                        <ASPNETPortal:Banner id="Banner" SelectedTabIndex="0" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="middle"><br/><br/>
						<table width="40%" cellpadding=2 cellspacing=0 border=0>
							<tr>
								<td valign="middle" align="center" rowspan=3><img src="images/portal/backgrounds/aicpa_wait.gif"></td>
								<td></td>
							</tr>
							<tr>
								<td id='animWait'><font size="8px" face="Verdana" color="darkred">Loading..... </font></td>
							</tr>
							<tr>
								<td valign="top">
									<script type="text/javascript">
										var bar1= createBar(270,15,'white',1,'darkred','darkblue',85,7,3,"");
									</script>
								</td>
							</tr>
						</table>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
