<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="RemoteAuth.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

<style type="text/css">
/* copied from opl */
body#application-body {
	background:#0b1f33 url(images/main-bg.gif) repeat-x top left;
	margin:0 34px;
}

h1 {
color:#ff6700;
font:18px Arial, Helvetica, sans-serif;
margin:0;
padding:20px 0 0;
}

.borderbot {border-bottom:1px solid #ccc; }
.tool-container3 {width:80%; margin:0 auto; margin-top:30px; padding:25px; background:#f3f6f8 url(images/accordian-bg2.gif) repeat-x bottom left; border:1px solid #999; font:12px Arial; }
.tool-container3 label {float:none; }


</style>
</head>
<body id="application-body">    

    <form id="form1" runat="server">
    <div class="tool-container3">
    <h1 class="borderbot">Login Page</h1><br>
    <table>
        <tr>
            <td class="style1">
                <asp:Label ID="lblUserID" runat="server" Text="Email Address" AssociatedControlID="hidUserId"></asp:Label>            
            </td>        
            <td class="style2">
                <!-- ID can be anything as long as it unique to the Referring site listed below -->
                <asp:TextBox ID="hidUserId" runat="server" Text=""></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;    </td>
            <td>
                <asp:Button ID="btnLogin" runat="server" Text="Login" 
                    onclick="btnLogin_Click1"/>
            </td>
        </tr>
    </table>    
    </div>
    </form>   

</body>
</html>
