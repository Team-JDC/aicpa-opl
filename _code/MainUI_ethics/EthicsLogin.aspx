<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EthicsLogin.aspx.cs" Inherits="MainUI.EthicsLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Ethics Login</title>
    <link href="Styles/main.css" rel="stylesheet" />
    <link href="Styles/ethics.css" rel="stylesheet" />        
    <link rel="icon" type="image/png" href='images/icons/favicon.png'/>
    <link rel="shortcut icon" type="image/ico" href='images/icons/favicon.ico'/> 
    <link href='images/icons/icon-72.png' rel="apple-touch-icon-precomposed" />
</head>

<body id="application-body" class="cpe">
    <form id="form2" runat="server">
    <div class="header">
        <a href="#">
            <div class="logo floatl"></div>
       </a>
    </div>
    <div class="tool-container3">
        <h2 class="borderbot" style="padding-left:0;">Please enter your username and password.</h2><br />
        <table cellpadding="0" cellspacing="7">
            <asp:RadioButtonList ID="rblUsers" runat="server" />
            <asp:Label ID="LoginMessageLabel" runat="server"></asp:Label>
            <tr>
                <td><asp:Label ID="UserNameLabel" runat="server" Text="Email:"></asp:Label></td>
                <td><asp:TextBox ID="UserNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="PassWordLabel" runat="server" Text="Password:"></asp:Label></td>
                <td><asp:TextBox ID="PasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button id="btnLogin" runat="server" Text="Login" onclick="btnLogin_Click" /></td>                
            </tr>
            <tr>
                <td><a href="SendPassword.aspx">Forgotten Password?</a></td>
            </tr>
            <tr>
                <td><a href="EthicsRegister.aspx">Register if you don't have an account</a></td>
            </tr>
        </table>    
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
        <br />
    </div>
    </form>
</body>
</html>

