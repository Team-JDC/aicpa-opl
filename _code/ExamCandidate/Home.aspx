<%@ Page language="c#" Codebehind="Home.aspx.cs" AutoEventWireup="True" Inherits="ExamCandidate.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Exam Candidate</title>
		<link href="ASPNETPortal.css" type="text/css" rel="stylesheet">
			<script type="text/javascript" src="Scripts/Destroyer.js"></script>
			<script>
			function changeContent(tab)
			{
				var oDoc = document.getElementById("containerIframe");

				switch(tab){
					case 1:
							oDoc.src = "Login.aspx";
						break;
					case 2:
							oDoc.src = "Register.aspx";
						break;
					case 3:
							oDoc.src = "License.aspx";
						break;
					case 4:
							oDoc.src = "PasswordReminder.aspx";
						break;
					case 5:
							oDoc.src = "SubscriptionExpired.aspx";
						break;		
					case 6:
							oDoc.src = "InvalidGuid.aspx";
						break;
					case 7:
								oDoc.src = "UserDenied.aspx";
						break;
					case 8:
								oDoc.src = "Success.aspx";
						break;	
				}
				//forcerReload(oDoc.src);
				return(false)
			}
			function transfer(Guid)
			{
				var Url = "ExamCandidate.aspx?Guid=" + Guid;
				//alert(Url);
				document.location = Url;
			}
			function redirect(Url)
 			{
 				document.location = Url;
 				return;
 			}				
			</script>
	</HEAD>
	<body style="background:#0b1f33; margin:0;" onload="init();">
		<DIV id="prepage" class="waitMsgShow">
			<center><br>
				<br>
				<br>
				<br>
				<TABLE width=40% cellpadding=2 cellspacing=0 border=0>
				<tr>
					<td valign=middle align=center rowspan=3><img src="images/portal/backgrounds/aicpa_wait.gif"></td>
					<td></td>
				</tr>
				<tr>
					<td id='animWait'><font size="8px" face="Verdana" color="darkred">Loading..... </font></td>
				</tr>
				<tr>
					<td valign=top>
						<script type="text/javascript">
							var bar1= createBar(270,15,'white',1,'darkred','darkblue',85,7,3,"");
						</script>
					</td>
				</tr>
			</TABLE>
			</center>
		</DIV>
		<form id="exam">
			<div class="exam-wrapper">	
            <div class="centered">
            	<img src="images/logo4.png" border="0" alt="AICPA Online Publications Library" style="margin-top:10px;">
                <div class="right">
                    <a href="#" onClick="changeContent(1);" class="TopLinks">Login</a> &nbsp;|&nbsp;
                    <a href="#" onClick="changeContent(2);" class="TopLinks">Register</a> &nbsp;|&nbsp;
                    <a href="#" onClick="changeContent(3);" class="TopLinks">View License</a>
                    <br />
                    <div class="date">
                    	<span id="todaysDate"></span>
                    </div>
				</div>
                <div class="clear" style="clear:both;"></div>
				<div class="iFrameClass">
					<iframe id='containerIframe' src="Login.aspx" scrolling="auto" frameborder="0" width="100%" onload="documentResize();"></iframe>
				</div>
            </div>    
			</div>				
			<asp:label ID="jslabel" visible="false" Runat="server"></asp:label>
		</form>
	</body>
</HTML>
