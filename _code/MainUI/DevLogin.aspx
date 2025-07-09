<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DevLogin.aspx.cs" Inherits="MainUI.DevLogin" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title>UAT Login</title>
     <link rel="shortcut icon" href="/favicon.ico"/>
     <link rel="stylesheet" href="/elements/css/normalize.css"/>
     <link rel="stylesheet" href="/elements/css/bootstrap.min.css"/>
     <link rel="stylesheet" href="/elements/css/main.css"/>
     <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css"/>
     <link rel="stylesheet" href="/resources/jquery.treeview.css" />
</head>
<body class="prelogin">



    <form id="form1" runat="server">
    <div id="header_container">
        <div id="header_outer" class="row">
            <div class="gradient"></div>
            <div class="header_inner">
                <a class="logo" href="/default"><img src="/elements/img/logo-aicpa-main.png" alt="AICPA | OPL"></a>
            </div>
        </div>
    
    
    <div id="primary_nav_outer" class="row">
                </div>
                <div id="main" class="clearfix">
                    <div id="leftcol" class="col-sm-12">
                        <div class="leftcol_inner">
                            <div class="leftcol_header">
                                <h1>UAT Login</h1>
                            </div>
                            <div class="leftcol_content clearfix">
                                <div class="col-sm-6 pad20">
                                    <div class="leftcol_content">
										<asp:DropDownList ID="ddlUser" runat="server" />
										<br />
										<asp:Button id="btnLogin" runat="server" Text="Login" onclick="btnLogin_Click" class="btn" />  
									</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
    
    <div id="footer_container">
        <div class="footer_inner row clearfix">
            <div class="col-sm-6">
                <p class="copyright">Copyright © 2014 Association of International Certified Professional Accountants.</p>
            </div>
            <div class="col-sm-6">
                <ul class="footer_nav">
                    <li><a href="mailto:OPL@aicpa.org">Contact</a></li>
                    <li><a href="http://www.aicpa.org/PrivacyandTerms/Pages/cpyright.aspx">Privacy &amp; Terms</a></li>
                    <!--<li><a href="">Jobs</a></li>-->
                    <li class="last"><a href="/tools/howtoguide">Help</a></li>
                </ul>
                <ul class="footer_social">
                    <li><a class="rss" href="" target="_blank" title="RSS"></a></li>
                    <li><a class="facebook" href="" target="_blank" title="AICPA on Facebook"></a></li>
                    <li><a class="linkedin" href="" target="_blank" title="AICPA on LinkedIn"></a></li>
                    <li><a class="twitter" href="" target="_blank" title="AICPA on Twitter"></a></li>
                    <li><a class="blogger" href="" target="_blank" title="AICPA on ???"></a></li>
                </ul>
            </div>
        </div>
    </div>	
    </form>
</body>
</html>
