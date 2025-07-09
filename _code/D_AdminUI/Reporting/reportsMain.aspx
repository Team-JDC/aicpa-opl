<%@ Page language="c#" Codebehind="reportsMain.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI.Reporting.reportsMain" %>
<%@ Register TagPrefix="radts" Namespace="Telerik.WebControls" Assembly="RadTabStrip" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>reportsMain</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../styles.css" type="text/css" rel="stylesheet">
		<script>
		function showSubMenu(id){
			var obj = eval("document.getElementById('"+id+"')");
			var imgId = "img_"+id;
			var imgObj = eval("document.getElementById('"+imgId+"')");
			var imgSrc = imgObj.src;
			
			imgObj.src = imgSrc.indexOf("plusik.gif") > -1 ? "../images/minus_L.gif" : "../images/plusik.gif";
			obj.style.visibility = obj.style.visibility == "hidden" ? "visible" : "hidden";
			obj.style.position = obj.style.position == "absolute" ? "relative" : "absolute";
			return;
		}
		
		function buildReport(tab){
			//var reportId = tab.ID;
			var reportId = tab;
			//reportId = reportId.replace("tab","");
			var objFrame = document.getElementById("reportFrame");
			var location = "reports.aspx?reportType="+reportId;
			objFrame.src = location;
			return;
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" topmargin="0" leftmargin="0">
		<form id="Form1" method="post" runat="server">
			<table border="0" cellpadding="0" cellspacing="0" width="100%" height="100%">
				<!--<tr>
					<td style="BORDER-BOTTOM:#333333 1px solid">&nbsp;</td>
					<td width="1%" height="1%" align="right">
						<radTS:RadTabStrip id="errorMenu" Runat="server" SelectedIndex="0" Theme="../RadControls/Tabstrip/ClassicBlue"
							AfterClientTabClick="buildReport">
							<TabCollection>
								<radts:Tab Text="Top 25 Visited Books" ID="tab0" Width="100"></radts:Tab>
								<radts:Tab Text="Top 25 Unique Users (Access)" ID="tab1" Width="100"></radts:Tab>
								<radts:Tab Text="Searched with results" ID="tab2" Width="100"></radts:Tab>
								<radts:Tab Text="Searched without results" ID="tab3" Width="100"></radts:Tab>
								<radts:Tab Text="Errors Summary" ID="tab4" Width="100"></radts:Tab>
								<radts:Tab Text="System Errors" ID="tab5" Width="100"></radts:Tab>
								<radts:Tab Text="Navagation Errors" ID="tab6" Width="100"></radts:Tab>
							</TabCollection>
						</radTS:RadTabStrip>
					</td>
				</tr>-->
				<tr>
					<td width="18%" valign="top" align="left" style="BORDER-RIGHT:gray 1px solid; BORDER-TOP:gray 1px solid">
						<table border="0" cellpadding="0" cellspacing="0" width="100%">
							<tr>
								<td align="center" style="BACKGROUND-IMAGE:url(../images/headerBg.gif);BACKGROUND-REPEAT:repeat-x;HEIGHT:20px">
									<b><font face="Arial" color="#5e5e5f">REPORTS</font></b>
								</td>
							</tr>
							<tr>
								<td nowrap><br>
									<span style="CURSOR:hand" onclick="buildReport(0)">
										&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-1">Visited Books</font>
									 </span>
								</td>
							</tr>
							
							<tr>
								<td nowrap>
									<span style="CURSOR:hand" onclick="showSubMenu('usersTable')">
										&nbsp; <img src='../images/plusik.gif' id="img_usersTable">
										<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-1">Top 25 Users </font>
									</span>
								</td>
							</tr>
							
							<tr>
								<td nowrap>
									<table border="0" cellpadding="0" cellspacing="0" id="usersTable" style="VISIBILITY:hidden;POSITION:absolute">
										<tr>
											<td nowrap rowspan="2" width="22"></td>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(1)">C2B</span>
												</font>
											</td>
										</tr>
										<tr>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(7)">Exam Candidates</span>
												</font>
											</td>
										</tr>
									</table>
								</td>
							</tr>
														
							<tr>
								<td nowrap>
									<span style="CURSOR:hand" onclick="showSubMenu('searchTable')">
										&nbsp; <img src='../images/plusik.gif' id="img_searchTable">
										<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-1">Search History </font>
									</span>
								</td>
							</tr>
							<tr>
								<td nowrap>
									<table border="0" cellpadding="0" cellspacing="0" id="searchTable" style="VISIBILITY:hidden;POSITION:absolute">
										<tr>
											<td nowrap rowspan="2" width="22"></td>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(2)">Searched with returns</span>
												</font>
											</td>
										</tr>
										<tr>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(3)">Searched without returns</span>
												</font>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td nowrap>
									<span style="CURSOR:hand" onclick="showSubMenu('errorTable');">&nbsp; <img src='../images/plusik.gif' id="img_errorTable">
										<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-1">Errors</font>
									</span>
								</td>
							</tr>
							<tr>
								<td nowrap>
									<table border="0" cellpadding="0" cellspacing="0" id="errorTable" style="VISIBILITY:hidden;POSITION:absolute">
										<tr>
											<td nowrap rowspan="3" width="22"></td>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(4);">Errors Summary</span>
												</font>
											</td>
										</tr>
										<tr>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(5);">System Errors</span>
												</font>
											</td>
										</tr>
										<tr>
											<td nowrap><img src='../images/hr.gif'>&nbsp;
												<font face="Verdana, Arial, Helvetica, sans-serif" color="#5e5e5f" size="-2">
													<span style="CURSOR:hand" onclick="buildReport(6);">Navagation Errors</span>
												</font>
											</td>
										</tr>																		
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td colspan="2" style="BORDER-TOP:gray 1px solid"><iframe id='reportFrame' width="100%" height="100%" src="reports.aspx?reportType=0" frameborder="no"></iframe>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
