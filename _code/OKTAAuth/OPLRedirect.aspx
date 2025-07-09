<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OPLRedirect.aspx.cs" Inherits="OKTAAuth.OPLRedirect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/scripts/jquery-2.1.3.min.js" />
</head>
<body>
    <form id="form1" runat="server" >
        <asp:TextBox ID="hidUserId" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="hidReferringSite" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="hidDomain" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="hidSecurityToken" runat="server"></asp:TextBox>
        <asp:Button ID="btnLogin" runat="server" Text="Login"  PostBackUrl="http://localhost:55868/SeamlessLogin.aspx"/><%--PostBackUrl="http://odp.knowlysis.com/mainui/SeamlessLogin.aspx" />--%><%--PostBackUrl="<%=strAction%>" />--%>
    </form>

    <script type="text/javascript">
        $().ready(function () {
            $("#btnLogin").click();
            alert('clicked');
        });
        
    </script>
</body>
</html>
