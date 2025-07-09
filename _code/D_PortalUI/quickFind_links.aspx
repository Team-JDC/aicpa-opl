<%@ Page language="c#" Codebehind="quickFind_links.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.quickFind_links" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>quickFind_links</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0">
		<link href="ASPNETPortal.css" type="text/css" rel="stylesheet">
		<script>
			function goTo(){
				window.parent.location.href = "DesktopDefault.aspx?tabindex=4&tabid=98&&h_tp=QuickFind";
				return;
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="FlowLayout" topmargin="0">
		<form id="Form1" method="post" runat="server">
			<table border="0" cellpadding="1" cellspacing="0" id="quickFindLinks" runat="server" width="100%">
			</table>
			<asp:label ID="resourceOnly" Visible=false Runat=server>
				<H2 style="padding-left:20px"><font color="darkred">Introduction</font></H2>

				<p><font color="black">AICPA <i>Professional Standards</i> and <i>Technical Practice Aids</i>
				bring together for continuing reference currently effective pronouncements
				issued by the AICPA.  These pronouncements are codified and arranged by topic.</p>

				<p><font color="black"><b>Quick Find</b> is designed to facilitate your navigation of AICPA reSOURCE by enabling you to
				easily find by <b><i>Document Type </i></b>or <b><i>Subject Matter</i></b> where the currently effective pronouncements are located.</p>

				<H2 style="padding-left:20px"><font color="darkred">How to Use</font></H2>

				<H3 style="padding-left:25px">By Document Type</H3>

				<p><font color="black">Do you know the number of a particular Statement on Auditing
				Standard, Statement of Position or other original pronouncement issued by the AICPA, and do not know where to find
				it within the reSOURCE Library?</p>

				<ul>
				<li style="margin: 0px 85px 0px 25px;">You
					can quickly navigate to that document by clicking on the applicable type
					of pronouncement (or other document) from the <b style='mso-bidi-font-weight:
					normal'>document type</b> list on the left side of the screen</li>
				<li style="margin: 0px 85px 0px 25px;">A
					second list will be returned of all currently effective documents in a
					given category</li>
				<li style="margin: 0px 85px 0px 25px;">Navigate
					to the original pronouncement you are looking to find, and click on the
					link</li>
				<li style="margin: 0px 85px 0px 25px;">If you
					currently subscribe to the title where that document is located, you will
					be taken to the appropriate section of the reSOURCE Library <i
					>(if you do not subscribe to the title where the document is located, you will
					receive an error message)</i></li>
				</ul>

				<H3 style="padding-left:25px">By Subject Matter</H3>

				<p><font color="black">Are you looking for an AICPA pronouncement in one of the
				broad categories of Accounting, Auditing &amp; Attestation, Compilation &amp;
				Review, Consulting, CPE, Peer Review, PFP, Quality Review, Tax, the AICPA
				Bylaws or the AICPA Code of Conduct?</p>
				<p><font color="black">Simply select the applicable category from the <b>subject matter</b> list on the left side of
				the screen and follow the steps listed above.</p>			
			</asp:label>
			<asp:Label ID="resourceAndEmap" Runat=server Visible=false>
				<H2 style="padding-left:20px"><font color="darkred">Introduction</font></H2>
				<P><font color="black">AICPA <I>Professional Standards</I> and <I>Technical Practice Aids</I> bring 
				together for continuing reference currently effective pronouncements issued by 
				the AICPA. These pronouncements are codified and arranged by topic.</font></P>
				<P><font color="black">E-MAP contains a vast collection of practice management tools and guidance, 
				including exhibits created in Word or Excel for easy downloading and use.</font></P>
				<P><font color="black"><B>Quick Find</B> is designed to facilitate the navigation of your 
				subscriptions by enabling you to easily find where the currently effective 
				pronouncements or practice management exhibits are located.</font></P>
				<H2 style="padding-left:20px"><font color="darkred">How to Use</font></H2>
				<H3 style="padding-left:25px">By Document Type</H3>
				<P><font color="black">Do you know the number of a particular Statement on Auditing Standard, 
				Statement of Position or other original pronouncement issued by the AICPA, and 
				do not know where to find it within the reSOURCE Library?</font></P>
				<UL>
				<LI style="margin: 0px 85px 0px 25px;">You can quickly 
				navigate to that document by clicking on the applicable type of pronouncement 
				(or other document) from the <B style="mso-bidi-font-weight: normal">document 
				type</B> list on the left side of the screen 
				<LI style="margin: 0px 85px 0px 25px;">A second list will be 
				returned of all currently effective documents in a given category 
				<LI style="margin: 0px 85px 0px 25px;">Navigate to the 
				original pronouncement you are looking to find, and click on the link 
				<LI style="margin: 0px 85px 0px 25px;">If you currently 
				subscribe to the title where that document is located, you will be taken to the 
				appropriate section of the reSOURCE Library <I>(if you do not subscribe to the 
				title where the document is located, you will receive an error message)</I> 
				</LI></UL>
				<H3 style="padding-left:25px">By Subject Matter</H3>
				<P><font color="black">Are you looking for a particular practice management document or an AICPA 
				pronouncement in one of the broad categories of Accounting, Auditing &amp; 
				Attestation, Compilation &amp; Review, Consulting, CPE, Peer Review, PFP, 
				Quality Review, Tax, the AICPA Bylaws or the AICPA Code of Conduct?</font></P>
				<P><font color="black">Simply select the applicable category from the <B>subject matter</B> list on 
				the left side of the screen and follow the links.</font></P>			
			</asp:Label>
			<asp:Label ID="emapOnly" Runat=server Visible=false>
				<H2 style="padding-left:20px"><font color="darkred">Introduction</font></H2>

				<p><font color="black">E-MAP contains a vast collection of practice management tools and guidance,
				including exhibits created in Word or Excel for easy downloading and use.</p>

				<p><font color="black"> <b>Quick Find</b> is designed to facilitate the navigation of your subscription by enabling you to
				easily find by practice management exhibits.</p>

				<p><font color="black">To use, simply click on the broad subject from
				the list on the left side of the screen and select the exhibit that you are looking
				for.</p>			
			</asp:Label>
			<asp:Label ID="jsLabel" Runat="server" Visible="False"></asp:Label>
		</form>
	</body>
</HTML>
