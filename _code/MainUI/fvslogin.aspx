<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fvslogin.aspx.cs" Inherits="MainUI.fvslogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="Styles/main.css" rel="stylesheet" />
</head>
<body id="application-body">
    <form id="form1" runat="server">
     <div class="main">
        <div class="header">
            <div class="logo"><img src="images/logo1.png" alt="AICPA Publications Home" border="0" /></div>
        </div>
    <div class="clear"></div>
    <div class="tool-container2">
       <p>
        <h4 class="big">Welcome to the Forensic and Valuation Services Library</h4>
        </p>
       
        <p>          
        <asp:Button id="btnLogin" runat="server" Text="Continue" 
            onclick="btnLogin_Click" /></p>
    </div>
</div>
    </form>
</body>
</html>
