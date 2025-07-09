<%@ Control CodeBehind="DesktopModuleTitle.ascx.cs" Language="c#" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.DesktopModuleTitle" %>

<%--

   The PortalModuleTitle User Control is responsible for displaying the title of each
   portal module within the portal -- as well as optionally the module's "Edit Page"
   (if such a page has been configured).

--%>

<asp:label id="ModuleTitle" cssclass="Head" EnableViewState="false" runat="server" />&nbsp;&nbsp;<asp:hyperlink id="EditButton" cssclass="CommandButton" EnableViewState="false" runat="server" />
