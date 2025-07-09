<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SAMLSeamless.aspx.cs" Inherits="WebsiteDemo.SAMLSeamless" %>

<%@ Import Namespace="dk.nita.saml20.identity" %>
<%@ Import Namespace="dk.nita.saml20.config" %>
<%@ Import Namespace="dk.nita.saml20.Schema.Core" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body onload="document.forms[0].submit();">
   <% if (Saml20Identity.IsInitialized()) { %>
    <form id="frmSeamless" action="<%=strAction%>" method="post">
    <div>
        <input type="hidden" name="hidSourceSiteCode" value="<%=hidSourceSiteCode%>" />
        <input type="hidden" name="hidDomain" value="<%=hidDomain%>" />
        <input type="hidden" name="hidEncPersGUID" value="<%=hidEncPersGUID%>" />
        <input type="hidden" name="hidEmail" value="<%=hidEmail%>" />
        <input type="hidden" name="hidURL" value="<%=hidURL%>" />
    </div>
    </form>
    
    <% } %>
</body>
</html>
