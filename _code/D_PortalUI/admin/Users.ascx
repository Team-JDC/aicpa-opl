<%@ Control Inherits="AICPA.Destroyer.UI.Portal.Users" CodeBehind="Users.ascx.cs" Language="c#" AutoEventWireup="True" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>

<ASPNETPortal:title runat="server" id="Title1" />
<table cellpadding="2" cellspacing="0" border="0">
    <tr valign="top">
        <td width="100">
            &nbsp;
        </td>
        <td class="Normal">
            <asp:Literal id="Message" runat="server" />
            <br><br>
        </td>
    </tr>
    <tr valign="top">
        <td>
            &nbsp;
        </td>
        <td class="Normal">
            Registered Users:&nbsp;
            <asp:DropDownList id="allUsers" DataTextField="Email" DataValueField="UserID" runat="server" />
            &nbsp;
            <asp:ImageButton ImageUrl="~/images/edit.gif" CommandName="edit" AlternateText="Edit this user" runat="server" ID="EditBtn" />
            <asp:ImageButton ImageUrl="~/images/delete.gif" AlternateText="Delete this user" runat="server" ID="DeleteBtn" onclick="DeleteUser_Click" />
            &nbsp;
            <asp:LinkButton id="addNew" cssclass="CommandButton" CommandName="Add" Text="Add New User" runat="server" />
        </td>
    </tr>
</table>
