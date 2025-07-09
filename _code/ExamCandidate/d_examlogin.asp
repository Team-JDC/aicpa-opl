<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Developer Studio">
<META HTTP-EQUIV="Content-Type" content="text/html; charset=iso-8859-1">
<TITLE>Exams</TITLE>
<!--METADATA TYPE="TypeLib" UUID="{B72DF063-28A4-11D3-BF19-009027438003}"-->

<%

	'Create the crypto objects
	Set CM = Server.CreateObject("Persits.CryptoManager")
	Set Context = CM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0","", True)
	Set key = Context.GenerateKeyFromPassword("$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk",calgSHA, calgRC4,128)

	' Encrypt text, place encrypted data into a blob
	Guid = "{" + Request.QueryString("Guid")+ "}"
	'Response.Write(Guid)
	Set Blob = key.EncryptText(Guid)

	'Set the server variables
	encryptedGuid = Blob.Hex


	Url = "default.aspx"

	SiteCode = "Exam"


%>


        <link href="ASPNETPortal.css" type="text/css" rel="stylesheet">
		<script>

			function init(){

			    document.forms[0].submit();
				return;
			}
		</script>
    </head>
    <body leftmargin="0" bottommargin="0" rightmargin="0" topmargin="0" marginheight="0" marginwidth="0" onLoad="init();">

<!-- old aicpa server:
		<form action="http://64.154.62.230/D_PortalUI/ResourceSeamlessLogin.aspx" method=post id="Form1" name="frmProflitSeamlessLogin">
-->
<!-- new aicpa server: -->
<!-- TODO: 2010-08-10 sburton: this needs to be updated to use the new external IP address of our site when that is configured -->
		<form action="https://publication.cpa2biz.com/MainUI/ResourceSeamlessLogin.aspx" method=post id="Form1" name="frmProflitSeamlessLogin">
<!-- knowlysis server:-->
        <!--<form action="http://odp.knowlysis.com/MainUI/ResourceSeamlessLogin.aspx" method=post id="Form1" name="frmProflitSeamlessLogin">-->
<!-- -->
		<input id="hidEncPersGUID" type="hidden" name="hidEncPersGUID" value=<%=encryptedGuid%>>
		<input id="hidSourceSiteCode" type="hidden" value="<%=SiteCode%>" name="hidSourceSiteCode">
		<input id="hidURL" type="hidden" value="<%=Url%>" name="hidURL">
		<input id="hidDomain" type="hidden" value="exam" name="hidDomain">
          <table width="100%" cellspacing="0" cellpadding="0" border="0" width=100% ID="Table1">
                  <tr>
                    <td align=center valign=middle><br><br>

                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
