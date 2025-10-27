<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LMS.aspx.cs" Inherits="MainUI.LmsLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/main.css" rel="stylesheet" />
<script type="text/javascript">
    function getQueryVariable(variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] == variable) {
                return pair[1];
            }
        }
    }
    function setform() {
        lmsform.targetDoc.value = getQueryVariable("tDoc");
        lmsform.targetPtr.value = getQueryVariable("tPtr");
        lmsform.Context.value = getQueryVariable("context");

        lmsform.submit();
    }
</script>

</head>
<body id="application-body" onload="setform();">
    <form id="lmsform" runat="server" action="/LmsDoc.aspx" method="post">
        <input name="targetDoc" type="hidden" />
        <input name="targetPtr" type="hidden"/>
        <input name="Context" type="hidden"/>
    </form>
</body>

</html>
