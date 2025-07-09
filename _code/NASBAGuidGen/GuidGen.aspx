<%@ Page language="c#" Codebehind="GuidGen.aspx.cs" AutoEventWireup="True" Inherits="NASBAGUIDGEN.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>1000 GUIDs</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body bgColor="silver" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:Button id="Button1" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 312px" runat="server"
				Width="208px" Text="Generate/Download Guids" onclick="Button1_Click"></asp:Button>
			<div id="linkhider" runat="server">
				<asp:Label id="Label2" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 64px" runat="server"
					Width="336px" Font-Bold="True">GUIDs Last Generated:</asp:Label>
				<asp:Label id="Label3" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 96px" runat="server"
					Width="328px" Height="88px">Please note that all GUIDs generated are inserted into the AICPA Exam Candidate database as well as the downloadable text file.  Inappropriate use of this tool will bloat the database. Please use accordingly! If you have any questions please contact Lois Wolfteich at lwolfteich@aicpa.org. </asp:Label>
				<asp:Label id="Label1" runat="server" Width="328px" BorderColor="Black" BackColor="Silver"
					ForeColor="Black" Font-Bold="True" Font-Italic="True" Height="32px" Font-Size="Larger">Exam Canidates GUID Generation Tool</asp:Label>
				<asp:DropDownList id="DropDownList1" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 256px"
					runat="server" Width="208px">
					<asp:ListItem Value="1000">1000</asp:ListItem>
					<asp:ListItem Value="2000" Selected="True">2000</asp:ListItem>
					<asp:ListItem Value="3000">3000</asp:ListItem>
				</asp:DropDownList>
				<asp:Label id="Label4" style="Z-INDEX: 105; LEFT: 24px; POSITION: absolute; TOP: 240px" runat="server"
					Font-Bold="True" Width="192px"># of GUIDs</asp:Label></div>
		</form>
	</body>
</HTML>
