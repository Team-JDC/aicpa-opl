<%@ Register TagPrefix="ASPNETPortal" TagName="Banner" Src="DesktopPortalBanner.ascx" %>
<%@ Page language="c#" Codebehind="D_copyRight.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.D_Copyright" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AICPA ReSouce</title>
		<LINK href="ASPNETPortal.css" type="text/css" rel="stylesheet">
			<script src="Scripts/Destroyer.js" type="text/javascript"></script>
	</HEAD>
	<body leftMargin="0" topMargin="0" onload="init();">
		<form id="mainForm" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr class="HeadBg">
					<td colSpan="2"><ASPNETPORTAL:BANNER id="Banner" runat="server" SelectedTabIndex="0"></ASPNETPORTAL:BANNER></td>
				</tr>
				<tr>
					<td width="100%" height="100%">
						<h5 align="center">Copyright Notices</h5>
					</td>
				</tr>
				<tr id="TrAicpaCopyright" runat="server" visible="true">
					<td width="100%" height="100%">
						<h5 align="center"><U>American Institute of Certified Public Accountants (AICPA) 
								Literature</U></h5>
						<h5 align="center"><B>Copyright &copy; American Institute of Certified Public Accountants, 
								Inc. All Rights Reserved.</B></h5>
						<p><U>AICPA Professional Standards</U>, <U>PCAOB Standards and Related Rules</U>, <U>Technical 
								Practice Aids</U>, Audit and Accounting Guides, Audit Risk Alerts, <U>Accounting 
								Trends &amp; Techniques</U>, <U>Audit and Accounting Manual</U>,&nbsp;Checklists 
							and Illustrative Financial Statements, E-MAP, and ABMOnline. Copyright American 
							Institute of Certified Public Accountants, Inc. All Rights Reserved.<BR>
							<BR>
							You may not reproduce or transmit in any form or by any means, electronic or 
							mechanical, including photocopying, recording, and storage in an information 
							retrieval system, nor may you modify or create derivative works based on the 
							text of any file, or any part thereof, without the prior written permission of 
							the American Institute of Certified Public Accountants. Permission requests may 
							be addressed to Permissions Department, AICPA,&nbsp;220 Leigh Farm Road, 
							Durham, NC 27707. Or, e-mailed to <A href="mailto:copyright@aicpa.org">copyright@aicpa.org.</A></p>
							<br/>
					</td>
				</tr>
				<tr id="TrFasbCopyright" runat="server" visible="false">
					<td width="100%" height="100%">
						<h5 align="center"><U>Financial Accounting Standards Board (FASB) Literature</U></h5>
						<h5 align="center"><B>Copyright &copy; Financial Accounting Standards Board, Norwalk, 
								Connecticut. All Rights Reserved.</B></h5>
						<p><i>FASB Accounting Standards Codification&trade;</i>, <U>Original Pronouncements</U>,<U>Original 
								Pronouncements, as amended</U>,<U>Current Text</U>, <U>EITF Abstracts</U>, 
							Special Report Implementation Guides, <U>Accounting for Derivative Instruments and 
								Hedging Activities</U>, and Comprehensive Topical Index. Copyright 
							Financial Accounting Standards Board, 401 Merritt 7, Norwalk, Connecticut 
							06856. All Rights Reserved. Unless expressly granted in the AICPA License for 
							this product, permission to reproduce FASB copyrighted materials in any form 
							must be obtained from FASB.
							<BR>
							Inclusion of FASB, GASB, and AICPA pronouncements and literature in or as a 
							part of this product does not constitute an endorsement of AICPA or CPA2Biz 
							Inc., or of this and any other product by FASB, GASB, or the Financial 
							Accounting Foundation.</p>
						<h5 align="center"><U>FASB Codification&mdash;Notice to Users</U></h5>
						<P>The <i>FASB Accounting Standards Codification&trade;</i> is the single source of 
							nongovernmental authoritative U.S. GAAP for interim and annual periods ending 
							after September 15, 2009. At that same time, all previous level a-d U.S. GAAP 
							standards issued by a standard setter are superseded. This product visually 
							marks all previous level a-d U.S. GAAP in a manner that displays on both the 
							electronic screen and printed copies. Users of this product may not use, rely 
							on, or cite previous level a-d U.S. GAAP standards as authoritative U.S. GAAP 
							in connection with financial accounting issues.
						</P>
					</td>
				</tr>
				<tr id="TrGasbCopyright" runat="server" visible="false">
					<td width="100%" height="100%">
						<h5 align="center"><U>Governmental Accounting Standards Board (GASB) Literature</U></h5>
						<h5 align="center"><B>Copyright &copy; Governmental Accounting Standards Board, Norwalk, 
								Connecticut. All Rights Reserved.</B></h5>
						<p><U>GASB Codification of Governmental Accounting and Financial Reporting Standards</U>,
							<U>Original Pronouncements</U>, <U>Implementation Guides</U>, <U>Topical Index</U>, 
							and <U>Finding List of Original Pronouncements</U>. Copyright Governmental 
							Accounting Standards Board, 401 Merritt 7, Norwalk, Connecticut 06856. All 
							Rights Reserved. Unless expressly granted in the AICPA License for this 
							product, permission to reproduce GASB copyrighted materials in any form must be 
							obtained from GASB.</p>
						<p>Inclusion of FASB, GASB, and AICPA pronouncements and literature in or as a part 
							of this product does not constitute an endorsement of AICPA or CPA2Biz Inc., or 
							of this and any other product by FASB, GASB, or the Financial Accounting 
							Foundation.</p>
					</td>
				</tr>
				<tr>
					<td align="center" width="100%" height="100%"><br>
						<span style="CURSOR: pointer" onclick="javascript:history.go(-1)">
							<h5 align="center">Back to Home</h5>
						</span></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
