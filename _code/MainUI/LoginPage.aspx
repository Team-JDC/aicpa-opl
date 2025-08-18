<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="MainUI.LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!DOCTYPE html>
<html class="no-js yes-js">
    <head>
        <meta charset="utf-8"/>
        <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1"/>
        <title>AICPA | OPL</title>
        <meta name="description" content=""/>
        <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"/>

        <!--[if lt IE 9 ]> <script>document.documentElement.className+=' lt-ie9';</script> <![endif]-->
        <!-- Place favicon.ico and apple-touch-icon.png in the root directory -->
        <link rel="shortcut icon" href="/favicon.ico"/>
        <link rel="stylesheet" href="/elements/css/normalize.css"/>
        <link rel="stylesheet" href="/elements/css/bootstrap.min.css"/>
        <link rel="stylesheet" href="/elements/css/main.css"/>
        <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css"/>
        <!--[if lt IE 9 ]>
        <link rel="stylesheet" href="elements/css/ie.css">
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
        <![endif]-->
        <script type='text/javascript' src='elements/js/modernizr.custom.19057.js'></script>
        
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
                            <a class="logo" href="/Default"><img src="elements/img/logo-aicpa-main.png" alt="AICPA | OPL" /></a>
                        </div>
                    </div>
                </div>
                <div id="primary_nav_outer" class="row primary_nav_outer">
                </div>
                <div id="main" class="clearfix">
                    <div id="leftcol" class="col-sm-12">
                        <div class="leftcol_inner">
                            <div class="leftcol_header">
                                <h1>Secure Login</h1>
                            </div>
                            <div class="leftcol_content clearfix">
                                <div class="col-sm-6 pad20">
                                    <label>Username</label>
                                    <br>
                                    <input type="text">
                                    <br>
                                    <label>Password</label>
                                    <br>
                                    <input type="password">
                                    <br>
                                    <button class="btn long">Login</button>
                                    <input type="checkbox" class="left10"> Stay logged in
                                    <br>
                                    <br>
                                    <br>
                                    <a href="#">Forgot Password?</a> &nbsp;|&nbsp; <a href="#">Create an Account</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div><!-- #page_inner -->
        </div><!-- #page -->
		<!--<script type="text/javascript" src="elements/js/jquery-1.9.1.min.js"></script>-->
		<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
		<script type='text/javascript' src='/elements/js/jquery.easing.min.js'></script>
		<script type="text/javascript" src="/elements/js/bootstrap.min.js"></script>
		<script type="text/javascript" src="/elements/js/main.js"></script>
    </body>
</html>
