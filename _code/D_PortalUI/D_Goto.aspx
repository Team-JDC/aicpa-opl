<%@ Page language="c#" Codebehind="D_Goto.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.D_Goto" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>D_Goto</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0">
		<style type="text/css">
H1 { FONT-WEIGHT: bold; FONT-SIZE: large; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: left }
H2{ FONT-WEIGHT: bold; FONT-SIZE: 12px; COLOR: #000000; FONT-FAMILY: arial;TEXT-ALIGN: left}
.tdh1 { FONT-WEIGHT: bold; BACKGROUND: #577ab4; COLOR: #ffffff; FONT-FAMILY: arial; TEXT-ALIGN: center }
.tdh2 { FONT-WEIGHT: bold; FONT-SIZE: 14px; COLOR: #000000; FONT-FAMILY: arial; TEXT-ALIGN: center }
.rbit { FONT-SIZE: 14px; COLOR: #000000; FONT-FAMILY: arial; TEXT-ALIGN: center }
.tdor { FONT-WEIGHT: bold; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: center }
.tdd1 { VERTICAL-ALIGN: top; FONT-FAMILY: arial; TEXT-ALIGN: center }
.tdt1 { VERTICAL-ALIGN: top }
</style>
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<h1>Go To</h1>
		
		<h2>Navigate directly to topics, subtopics, and sections</h2>
		
		<form id="Form1" method="post" runat="server">
			<table width="48" border="0" cellpadding="3">
				<tr vAlign="middle">
					<td class="tdh2">Topic</td>
					<td class="tdd1" width="34"><asp:dropdownlist id="gtTopic" runat="server" Width="32em" OnSelectedIndexChanged="gtTopic_SelectedIndexChanged"
							AutoPostBack="True" CssClass="rbit"></asp:dropdownlist></td>
				</tr>
				<tr vAlign="middle">
					<td class="tdh2">Subtopic</td>
					<td class="tdd1" width="34"><asp:dropdownlist id="gtSubtopic" runat="server" Width="32em" OnSelectedIndexChanged="gtSubtopic_SelectedIndexChanged"
							AutoPostBack="True" CssClass="rbit"></asp:dropdownlist></td>
				</tr>
				<tr vAlign="middle">
					<td class="tdh2">Section</td>
					<td class="tdd1" width="34"><asp:dropdownlist id="gtSection" runat="server" Width="32em" OnSelectedIndexChanged="gtSection_SelectedIndexChanged"
							AutoPostBack="True" CssClass="rbit"></asp:dropdownlist></td>
				</tr>
			</table>
			<p><asp:button id="SubmitButton" OnClick="SubmitButton_Click" runat="server" Text="Go To" Enabled="False"></asp:button></p>
		</form>
		</TABLE>
	</body>
</HTML>
