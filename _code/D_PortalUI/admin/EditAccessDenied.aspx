<%@ Page CodeBehind="EditAccessDenied.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.EditAccessDenied" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="~/DesktopPortalBanner.ascx" %>
<%@ OutputCache Duration="36000" VaryByParam="none" %>
<%@ Import Namespace="AICPA.Destroyer.UI.Portal" %>

<html>
  <head>
        <title>ASP.NET Starter Portal Kit</title>
        <link rel="stylesheet" href='<%= Global.GetApplicationPath(Request) + "/ASPNETPortal.css" %>' type="text/css" >
  </head>
    <body leftmargin="0" bottommargin="0" rightmargin="0" topmargin="0" marginheight="0" marginwidth="0">
        <form runat="server">
            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                <tr valign="top">
                    <td colspan="2">
                        <ASPNETPortal:Banner runat="server" id="Banner1" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <center>
                            <br/>
                            <table width="500" border="0">
                                <tr>
                                    <td class="Normal">
                                        <br/>
                                        <br/>
                                        <br/>
                                        <br/>
                                        <span class="Head">Edit Access Denied</span>
                                        <br/>
                                        <br/>
                                        <hr "noshade" size="1"/>
                                        <br/>
                                        Either you are not currently logged in, or you do not have access to modify the 
                                        current portal module content. Please contact the portal administrator to 
                                        obtain edit access for this module.
                                        <br/>
                                        <br/>
                                        <a href="<%=Global.GetApplicationPath(Request)%>/DesktopDefault.aspx">Return to Portal Home</a>
                                    </td>
                                </tr>
                            </table>
                        </center>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
