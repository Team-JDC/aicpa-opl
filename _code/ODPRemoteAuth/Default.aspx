<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ODPRemoteAuth.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" >
    <div>
    <table>
        <tr>
            <td class="style1">
                <asp:Label ID="lblUserID" runat="server" Text="User ID" AssociatedControlID="hidUserId"></asp:Label>            
            </td>        
            <td class="style2">
                <!-- ID can be anything as long as it unique to the Referring site listed below -->
                <asp:TextBox ID="hidUserId" runat="server" Text="dwatsonceb@knowlysis.com"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="lblReferringSite" runat="server" Text="Referring Site" AssociatedControlID="hidReferringSite"></asp:Label>            
            </td>        
            <td class="style2">
                <!-- Reffering Sites be must configured in OPL Before they are valid, Not any value will work, these will be assigned to the organization -->
                <asp:DropDownList ID="hidReferringSite" runat="server" 
                    OnSelectedIndexChanged="hidReferringSite_Change" AutoPostBack="True">
                    <asp:ListItem Text="CEB" Value="Ceb"></asp:ListItem>
                    <asp:ListItem Text="McGladrey" Value="Mcgdy"></asp:ListItem>
                    <asp:ListItem Text="McGladrey Asc" Value="Mcgdyasc"></asp:ListItem>
                </asp:DropDownList>
            </td>           
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="lblDomain" runat="server" Text="Domain" AssociatedControlID="hidDomain"></asp:Label>            
            </td>        
            <td class="style2">
                <!-- Domains must be configured in OPL Before they are valid, Not any value will work, these will be assigned to the organization -->
                <asp:DropDownList ID="hidDomain" runat="server" 
                    OnSelectedIndexChanged="hidDomain_Change" AutoPostBack="True" >
                    <asp:ListItem Text="Ceb" Value="Ceb"></asp:ListItem>
                    <asp:ListItem Text="McGladrey" Value="mcgladrey"></asp:ListItem>
                    <asp:ListItem Text="McGladrey Asc" Value="mcgladreyasc"></asp:ListItem>
                </asp:DropDownList>
            </td>           
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label1" runat="server" Text="Requested Security Token" AssociatedControlID="hidSecurityToken"></asp:Label>
            </td>
            <td class="style2">
                <asp:TextBox ID="hidSecurityToken" runat="server" ReadOnly></asp:TextBox>
            </td>
        </tr>    
        <tr>
            <td>
                <asp:Button ID="btnRequestToken" runat="server"
                    Text="Request Token" onclick="btnRequestToken_Click" />
            </td> <%--PostBackUrl="~/SeamlessLogin.aspx"--%>
            <td>
                <asp:Button ID="btnLogin" runat="server" Text="Login"  PostBackUrl="https://library.aicpa.org/mainui/SeamlessLogin.aspx"/><%--PostBackUrl="http://odp.knowlysis.com/mainui/SeamlessLogin.aspx" />--%><%--PostBackUrl="<%=strAction%>" />--%>
            </td>
        </tr>
    </table>    
    </div>
    </form>

    

</body>
</html>
