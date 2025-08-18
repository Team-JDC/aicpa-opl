<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="MainUI.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html class="no-js yes-js">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title>Error</title>
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    <!--[if lt IE 9 ]> <script>document.documentElement.className+=' lt-ie9';</script> <![endif]-->
    <!-- Place favicon.ico and apple-touch-icon.png in the root directory -->
    <link rel="shortcut icon" href="/favicon.ico" />
    <link rel="stylesheet" href="/elements/css/normalize.css" />
    <link rel="stylesheet" href="/elements/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/elements/css/main.css" />
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css" />
    <!--[if lt IE 9 ]>
        <link rel="stylesheet" href="elements/css/ie.css">
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
        <![endif]-->
    <script type='text/javascript' src='elements/js/modernizr.custom.19057.js'></script>

    <script type="text/javascript" language="javascript">
        function removeUnloadIfPresent() {
            try {
                removeUnloadEvent();
            }
            catch (ex) {
                // do nothing
            }
        }
    </script>
</head>
<body class="prelogin">
    <!--[if lt IE 7]>
            <p class="chromeframe">You are using an <strong>outdated</strong> browser. Please <a href="http://browsehappy.com/">upgrade your browser</a> or <a href="http://www.google.com/chromeframe/?redirect=true">activate Google Chrome Frame</a> to improve your experience.</p>
        <![endif]-->
    <div id="page">
        <div id="page_inner" class="container">
            <div id="header_container">
                <div id="header_outer" class="row">
                    <div class="gradient"></div>
                    <div class="header_inner">
                        <a class="logo" href="/default">
                            <img src="elements/img/logo-aicpa-main.png" alt="AICPA | OPL" /></a>
                    </div>
                </div>
            </div>
            <div id="primary_nav_outer" class="row">
            </div>
            <div id="main" class="clearfix">
                <div id="leftcol" class="col-sm-12">
                    <div class="leftcol_inner">
                        <!--<div class="leftcol_header">
                                <h1>Our apologies... </h1>                                
                            </div>-->
                        <div class="leftcol_content clearfix">
                            <div class="col-sm-12 pad20">
                                <p>
                                    <h4>Our apologies...</h4>
                                    <br />
                                    <div id="ErrorDisplay">
                                        <b><asp:Label ID="lblError" runat="server" ></asp:Label></b>
                                    </div>
                                    <!--
                                        <b>
                                            Error: 
                                        </b>
                                        
                                    <br />
                                    <b>Please contact customer support.<br />
                                        
                                        
                                    <% if (!hideReturnHomeLink) %>
                                    <% { %>
                                        Or return to the <a href="/Default" onclick="removeUnloadIfPresent();">homepage</a>.
                                    <% } %>
                                    <% if (showLoginLink) %>
                                       <% { %>
                                        <br />Or try <a href="/login">logging in again</a>.
                                        <% } %>
                                    </b>
                                        -->
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- #page_inner -->
    </div>
    <!-- #page -->
    <!--<script type="text/javascript" src="elements/js/jquery-1.9.1.min.js"></script>-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script type='text/javascript' src='/elements/js/jquery.easing.min.js'></script>
    <script type="text/javascript" src="/elements/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="/elements/js/main.js"></script>
</body>
</html>
