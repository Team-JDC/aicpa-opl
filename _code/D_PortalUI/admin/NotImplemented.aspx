<%@ Page CodeBehind="NotImplemented.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.NotImplemented" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="~/DesktopPortalBanner.ascx" %>
<%@ OutputCache Duration="600" VaryByParam="title" %>
<%@ Import Namespace="AICPA.Destroyer.UI.Portal" %>

<%--

   This page is the target for the fictious links in the sample data.

--%>

<HTML>
  <HEAD>
        <title>ASP.NET Portal Starter Kit: Content Not Implemented</title>
        <link rel="stylesheet" href='<%= Global.GetApplicationPath(Request) + "/ASPNETPortal.css" %>' type="text/css">
  </HEAD>
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
                            <br>
                            <table width="500" border="0">
                                <tr>
                                    <td class="Normal">
                                        <br>
                                        <br>
                                        <br>
                                        <br>
                                        <span id="title" class="Head" runat="server">Linked Content Not Provided</span>
                                        <br>
                                        <br>
                                        <hr noshade size="1">
                                        <br>
                                        The link you clicked was provided as a part of the sample data for the <b>Portal Starter Kit</b>. The content for this link is not provided as part of the sample 
                                        application.
                                        <br>
                                        <br>
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
</HTML>
