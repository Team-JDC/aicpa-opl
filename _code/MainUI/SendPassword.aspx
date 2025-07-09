<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendPassword.aspx.cs" Inherits="MainUI.SendPassword" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Password Reset</title>
    <link href="Styles/main.css" rel="stylesheet" />
    <link href="Styles/ethics.css" rel="stylesheet" />        
    <link rel="icon" type="image/png" href='images/icons/favicon.png'/>
    <link rel="shortcut icon" type="image/ico" href='images/icons/favicon.ico'/> 
    <link href='images/icons/icon-72.png' rel="apple-touch-icon-precomposed" />

</head>
<body id="application-body">
    <form id="resetpassword" runat="server">
    <div class="header">    	   
           <a href="#">
            <img src="images/logo.png" alt="Home" border="0" />
           </a>
       </div>
    </div>
    <div class="tool-container3">
        <h2 class="borderbot" style="padding-left:0;">Please enter your username. This will email a new password to the user.</h2><br />
        <table cellpadding="0" cellspacing="7">
            <asp:Label ID="LoginMessageLabel" runat="server"></asp:Label>
            <tr>
                <td><asp:Label ID="UserNameLabel" runat="server" Text="Username:"></asp:Label></td>
                <td><asp:TextBox ID="UserNameTextBox" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Button id="btnSendPassword" runat="server" Text="Send Password" onclick="btnSendPassword_Click" /></td>
            </tr>
        </table>    
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
        <br />
    </div>
    </form>
</body>
</html>
