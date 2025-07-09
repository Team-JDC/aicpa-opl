<%@ Page language="c#" Codebehind="Register.aspx.cs" AutoEventWireup="True" Inherits="ExamCandidate.Register" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Register</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<link href="ASPNETPortal.css" type="text/css" rel="stylesheet">
			<script type="text/javascript" src="Scripts/Destroyer.js"></script>
			<script>
			function register ()
			{
				document.Register.submit();				
			}			
			</script>
	</HEAD>
	<BODY>
		<form id="Register" method="post" runat="server">
			<table width="%100">
				<tr>
					<td>
						
					 <h4>Welcome to AICPA Online Professional Library</h4>
                     <h5>As a candidate for the uniform CPA Exam, you have access to <i>AICPA Professional Standards</i>, <i>FASB Original Pronouncements</i>, and <i>FASB Accounting Standards Codification</i>. This content is aligned to the Authoratative Literature presented on the CPA Exam. The <i>FASB Accounting Standards Codification</i> will be included on the CPA Exam begining January 1, 2011.</h5>
						<p>Please create a username and password below and click <img src="images/btn-go.gif">
							Each time you return to the product, go to <a href="http://publication.aicpa.org/exams/home.aspx">
								http://publication.aicpa.org/exams.home.aspx</a> and enter this username and 
							password.
						</p>
						<p>You can begin your research by clicking on a title from the My Online 
							Publications section of the homepage or by performing a search from the search 
							dialog at the top of the screen.
						</p>
						<br>
					</td>
				</tr>
				<tr>
					<td>
						<table border="0" cellpadding="0" cellspacing="0">
							<tr>
								<td><IMG src="images/cornerLeft.gif"></td>
								<td style="BACKGROUND-POSITION: 50% top; BACKGROUND-IMAGE: url(images/topBg.gif); BACKGROUND-REPEAT: repeat-x"></td>
								<td><IMG src="images/cornerRight.gif"></td>
							</tr>
							<tr>
								<td style="BACKGROUND-POSITION: left 50%; BACKGROUND-IMAGE: url(images/bgLeft.gif); BACKGROUND-REPEAT: repeat-y" noWrap align=right width=20></td>
								<td>
									<table>
										<tr>
											<td align="right" valign="bottom"><span class="formtext">User Name:</span>
											</td>
											<td><INPUT type="text" name="UserName" ID="UserName" runat="server"></td>
											<td><span class="formtext">6-20 characters; no spaces, periods, or symbols</span>
											</td>
										</tr>
										<tr>
											<td align="right" valign="middle"><span class="formtext">Password:</span>
											</td>
											<td><INPUT type="password" name="Password" ID="Password" runat="server"></td>
											<td><span class="formtext">6-12 characters;no spaces, periods, or symbols</span>
											</td>
										</tr>
										<tr>
											<td align="right"><span class="formtext">Confirm Password:</span>
											</td>
											<td><INPUT type="password" name="Passwordcfrm" ID="Passwordcfrm" runat="server"></td>
											<td><INPUT type="hidden" name="Guid" ID="Hidden2" runat="server"></td>
										</tr>
										<tr>
											<td></td>
                                            <td align="right">
                                            	<INPUT type="checkbox" id="agree" name="agree" runat="server">
                                            </td>
                                            <td align="left">
                                            	<span class="formtext"><a href="Javascript:window.parent.changeContent(3);">Accept License Agreement</a></span>
											</td>
                                        </tr>
                                        <tr>
											<td></td>
                                            
                                            <td align="right">
												<!--<asp:ImageButton id="registerBtn" runat="server" ImageUrl="images/btn-go.gif"></asp:ImageButton> -->
												<input type="image" onClick="Javascript:register();" src="images/btn-go.gif">
											</td>
                                            <td></td>
										</tr>
									</table>
								</td>
								<td style="BACKGROUND-POSITION: right 50%; BACKGROUND-IMAGE: url(images/bgRight.gif); BACKGROUND-REPEAT: repeat-y" width=22></td>
							</tr>
							<tr>
								<td><IMG src="images/cornerBottomLeft.gif"></td>
								<td style="BACKGROUND-POSITION: 50% bottom; BACKGROUND-IMAGE: url(images/bottomBg.gif); BACKGROUND-REPEAT: repeat-x"></td>
								<td style="BACKGROUND-IMAGE: url(images/cornerBottomRight.gif)"></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<P></P>
						<br>
						<p>This site supports PC use of Microsoft Internet Explorer 5.5 or later and 
							Netscape 7.0 or later</p>
					</td>
				</tr>
			</table>
			<asp:label ID="jslabel" visible="false" Runat="server"></asp:label>
		</form>
	</BODY>
</HTML>
