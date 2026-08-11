<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficeFix.aspx.cs" Inherits="MainUI.OfficeFix" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="Styles/main.css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
        var tDoc = '<%= HttpUtility.JavaScriptStringEncode(Request.QueryString["tdoc"]) %>';
        var tPtr = '<%= HttpUtility.JavaScriptStringEncode(Request.QueryString["tptr"]) %>';
        var tFldr = '<%= HttpUtility.JavaScriptStringEncode(Request.QueryString["tfldr"]) %>';
        var prod = '<%= HttpUtility.JavaScriptStringEncode(Request.QueryString["prod"]) %>';
        var vct = '<%= HttpUtility.JavaScriptStringEncode(Request.QueryString["vct"]) %>';

        function directDocLoad() {
            var Url = 'resourceseamlesslogin.aspx?prod=' + prod + '&tdoc=' + tDoc + '&tptr=' + tPtr;

            if (tFldr) {
                Url = Url + '&tfldr=' + encodeURIComponent(tFldr);
            }

            if (vct == '1') {
                Url = Url + "&vct=1";
                vct = "~" + vct + "~";

            }

            window.location = Url;
        }
             
        
    </script>


</head>

<body onload="directDocLoad();">
</body>
</html>

