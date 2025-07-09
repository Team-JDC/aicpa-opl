<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="True" Inherits="ExamCandidate.Login" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>Login</title>
<meta content=False name=vs_snapToGrid><LINK href="ASPNETPortal.css" type=text/css rel=stylesheet >
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<script type="text/javascript" src="Scripts/Destroyer.js"></script>
<script>
	function password()
	{
		top.window.location = "http://www.nasba.org/web/NCFA.nsf/pass";
	}

</script>
  </HEAD>
<body MS_POSITIONING="GridLayout">
<form id=Login method=post runat="server">
<table width=%100>
   <tr>
    <td>
      
      <h4>Welcome to AICPA Online Professional Library</h4>
      <h5>As a candidate for the Uniform CPA Examination, you have access to the AICPA Professional Standards, FASB Original Pronouncements, and FASB Accounting Standards Codification. This content is aligned to the Authoritative Literature presented on the Uniform CPA Examination.
        <br /><br />On January 1, 2011 the FASB Accounting Standards Codification will replace the FASB Original Pronouncements as the FASB standard referenced in the Financial Accounting and Reporting (FAR) section of the Uniform CPA Examination. Please be sure to familiarize yourself with the standard that corresponds with the date you plan to take the FAR section of the examination.
        <ul><li>CPA Candidates taking the FAR section of the CPA Examination before January 1, 2011 should reference the Pre-Codification FASB Literature found in the Archive Library.</li>
        <li>CPA Candidates taking the FAR section of the CPA Examination after December 31, 2010 should reference the FASB Accounting Standards Codification.</li></ul>
      </h5>
      <p>Please enter your username and password below and 
      click <IMG src="images/btn-go.gif" > to enter the product. </p>
      <p>To begin your research, select a title from the homepage, navigate from the breadcrumb, or perform a search. </p><br>
      <p></p></td></tr>
  <tr>
    <td style="PADDING-LEFT: 35px">
      <table cellSpacing=0 cellPadding=0 border=0>
        <tr>
          <td
          style="BACKGROUND-POSITION: 50% top; BACKGROUND-IMAGE: url(images/topBg.gif); BACKGROUND-REPEAT: repeat-x" 
          ><IMG src="images/cornerLeft.gif" ></td>
          <td 
          style="BACKGROUND-POSITION: 50% top; BACKGROUND-IMAGE: url(images/topBg.gif); BACKGROUND-REPEAT: repeat-x" 
          ></td>
          <td><IMG src="images/cornerRight.gif" ></td></tr>
        <tr>
          <td 
          style="BACKGROUND-POSITION: left 50%; BACKGROUND-IMAGE: url(images/bgLeft.gif); BACKGROUND-REPEAT: repeat-y" 
          noWrap align=right width=20></td>
          <td>
            <table>
              <tr>
                <td vAlign=bottom align=right>
                  <span class="formtext">User Name:</span></td>
                <td><INPUT id=UserName type=text 
                  name=UserName runat="server"></td></tr>
              <tr>
                <td vAlign=middle align=right>
                  <span class="formtext">Password:</span></td>
                <td><INPUT id=Password type=password 
                  name=Password runat="server"></td></tr>
              <tr>
                <td><INPUT id=Guid type=hidden name=Guid 
                  runat="server"></td>
                <td align=right>
					<asp:ImageButton ImageUrl="~/images/btn-go.gif" Runat="server" id="loginSubmit" onclick="loginSubmit_Click"></asp:ImageButton>
                </td>
              </tr>
            </table>
           </td>
          <td 
          style="BACKGROUND-POSITION: right 50%; BACKGROUND-IMAGE: url(images/bgRight.gif); BACKGROUND-REPEAT: repeat-y" 
          width=22></td></tr>
        <tr>
          <td 
          style="BACKGROUND-POSITION: 50% bottom; BACKGROUND-IMAGE: url(images/bottomBg.gif); BACKGROUND-REPEAT: repeat-x" 
          ><IMG src="images/cornerBottomLeft.gif" ></td>
          <td 
          style="BACKGROUND-POSITION: 50% bottom; BACKGROUND-IMAGE: url(images/bottomBg.gif); BACKGROUND-REPEAT: repeat-x" 
          ></td>
          <td style="BACKGROUND-IMAGE: url(images/cornerBottomRight.gif)">
          </td></tr></table></td></tr>
		<tr>
		<td>
      <P></P><br>
      <p>Forgot your <a href="Javascript:password();">Password?</a></p><br>
      <p>This site supports PC use of Microsoft Internet Explorer 7.0 or later and Firefox 3.0 or later</p></td></tr></table>
		<asp:label ID="jslabel" visible="false" Runat="server"></asp:label>
      </form> 
	</body>
</HTML>
