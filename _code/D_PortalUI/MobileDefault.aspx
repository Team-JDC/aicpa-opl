<%@ Page Language="c#" Inherits="AICPA.Destroyer.UI.Portal.MobileDefault" CodeBehind="MobileDefault.aspx.cs" AutoEventWireup="True" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Register TagPrefix="ASPNETPortal" Namespace="AICPA.Destroyer.UI.Portal.MobileControls" Assembly="ASPNETPortal" %>
<%--

    The MobileDefault.aspx page is used to load and populate each Mobile Portal View.  It accomplishes
    this by reading the layout configuration of the portal from the Portal Configuration
    system. At the top level is a tab view, implemented using a TabbedPanel custom control. 
    Each portal view is inserted into this control, and portal modules (each implemented 
    as an ASP.NET user control) are instantiated and inserted into tabs.

--%>
<mobile:Form runat="server" Wrapping="NoWrap" Paginate="true" PagerStyle-Font-Name="Verdana" PagerStyle-ForeColor="#ffffff" PagerStyle-Font-Size="Small" id="Form1">
    <mobile:DeviceSpecific id="DeviceSpecific1" runat="server">
        <Choice BackColor="#000000" Filter="isJScript">
            <HeaderTemplate>
                <table cellSpacing="0" cellPadding="0" width="100%" border="0">
                    <tr>
                        <td>
                            <img height="45" src="data/mobilelogo.gif" width="180">
                        </td>
                    </tr>
                </table>
                <table height="270" cellSpacing="0" cellPadding="0" width="100%" bgColor="#ffffff" border="0">
                    <tbody>
                        <tr>
                            <td>
                                <img height="220" src="images/spacer.gif" width="2">
                            </td>
                            <td vAlign="top">
            </HeaderTemplate>
            <FooterTemplate>
                </td>
                <td>
                    <img height="220" src="images/spacer.gif" width="2">
                </td>
                </tr></tbody></table>
            </FooterTemplate>
        </Choice>
        <Choice>
            <HeaderTemplate>
                <mobile:Label id="Label1" runat="server" StyleReference="title">
                    Porter Starter Kit</mobile:Label>
            </HeaderTemplate>
        </Choice>
    </mobile:DeviceSpecific>
    <ASPNETPortal:TabbedPanel id="TabView" runat="server" ActiveTabTextColor="#ffffff" ActiveTabColor="#000000" TabTextColor="#000000" TabColor="#bbbb9a" ontabactivate="TabView_OnTabActivate">
    </ASPNETPortal:TabbedPanel>
</mobile:Form>
