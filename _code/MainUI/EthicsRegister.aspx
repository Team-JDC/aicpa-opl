<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EthicsRegister.aspx.cs" Inherits="MainUI.EthicsAddUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add User</title>
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
        <h2 class="borderbot" style="padding-left:0;">Please enter user information below.</h2><br />
        <table cellpadding="0" cellspacing="7">
            <asp:RadioButtonList ID="rblUsers" runat="server" />
            <asp:Label ID="LoginMessageLabel" runat="server"></asp:Label>
            <tr>
                <td><asp:Label ID="UserNameLabel" runat="server" Text="Email address (Username):"></asp:Label></td>
                <td><asp:TextBox ID="UserNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label></td>
                <td><asp:TextBox ID="FirstNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label></td>
                <td><asp:TextBox ID="LastNameTextBox" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label></td>
                <td><asp:TextBox ID="PasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm password:"></asp:Label></td>
                <td><asp:TextBox ID="ConfirmPasswordTextBox" runat="server" Width="200px" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:ValidationSummary id="ValSummary" HeaderText="The following errors were found:" ShowSummary="True" DisplayMode="List" Runat="server"/>
					
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button id="btnRegister" runat="server" Text="Register" 
                        CausesValidation="true" onclick="btnRegister_Click1"/>
                </td>
            </tr>
        </table>    
        
        <asp:RegularExpressionValidator ID="valEmailAddress" ControlToValidate="UserNameTextBox"	ValidationExpression=".*@.*\..*" ErrorMessage="Email address is invalid." Display="None" EnableClientScript="true" Runat="server"/>
        <asp:RequiredFieldValidator id="valUserNameRequired" ControlToValidate="UserNameTextBox" ErrorMessage="UserName is a required field." EnableClientScript="true" Display="None" Runat="server"/>
        <asp:RequiredFieldValidator id="valFirstNameRequired" ControlToValidate="FirstNameTextBox" ErrorMessage="First Name is a required field." EnableClientScript="true" Display="None" Runat="server"/>
        <asp:RequiredFieldValidator id="valLastNameRequired" ControlToValidate="LastNameTextBox" ErrorMessage="Last Name is a required field." EnableClientScript="true" Display="None" Runat="server"/>
		<asp:RequiredFieldValidator id="valPasswordRequired" ControlToValidate="PasswordTextBox" ErrorMessage="Password is a required field." EnableClientScript="true" Display="None" Runat="server"/>
		<asp:RequiredFieldValidator id="valConfirmPasswordRequired" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="Password confirmation is a required field."  EnableClientScript="true" Display="None" Runat="server"/>
        <asp:CompareValidator id="valComparePassword" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="Password fields must match." ControlToCompare="PasswordTextBox" Display="None" EnableClientScript="true" Runat="server"/>
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
        <br />
    </div>
    </form>
</body>
</html>
