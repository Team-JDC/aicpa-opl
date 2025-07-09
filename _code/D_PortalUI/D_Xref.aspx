<%@ Page language="c#" Codebehind="D_Xref.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.D_Xref" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Cross Reference</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" name="vs_targetSchema">
		<style type="text/css">
			H1 { FONT-WEIGHT: bold; FONT-SIZE: large; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: left }
			H2{ FONT-WEIGHT: bold; FONT-SIZE: 12px; COLOR: #000000; FONT-FAMILY: arial;TEXT-ALIGN: left}
			.tdh1 { FONT-WEIGHT: bold; BACKGROUND: #577ab4; COLOR: #ffffff; FONT-FAMILY: arial; TEXT-ALIGN: center }
			.tdh2 { FONT-WEIGHT: bold; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: center }
			.tdor { FONT-WEIGHT: bold; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: center }
			.tdd1 { VERTICAL-ALIGN: top; FONT-FAMILY: arial; TEXT-ALIGN: center }
			.tdt1 { VERTICAL-ALIGN: top }
	</style>
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<h1>Cross Reference</h1>
		<h2>Find the location of legacy content in the Codification</h2>
		<form id="SearchForm" method="post" runat="server">
			<table width="60" border="0">
				<tr>
					<td class="tdt1">
						<table width="24" border="0">
							<tr>
								<td class="tdh1" colSpan="2">By Standard</FONT></td>
							</tr>
							<tr>
								<td class="tdh2">Type</td>
								<td class="tdh2">Number</td>
							</tr>
							<tr>
								<td class="tdd1" width="10"><asp:dropdownlist id="StandardTypesDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="StandardTypesDropDown_SelectedIndexChanged"
										Width="8em"></asp:dropdownlist>&nbsp;-</td>
								<td class="tdd1" width="14"><asp:dropdownlist id="StandardNumbersDropDown" runat="server" Width="12em"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
					<td class="tdor" width="8">&nbsp;OR&nbsp;</td>
					<td class="tdt1">
						<table width="32" border="0">
							<TBODY>
								<tr>
									<td class="tdh1" colSpan="4">By Codification</td>
								</tr>
				</tr>
				<tr>
					<td class="tdh2">Topic</td>
					<td class="tdh2">Subtopic</td>
					<td class="tdh2">Section</td>
				</tr>
				<tr>
					<td class="tdd1" width="11"><asp:dropdownlist id="DropdownTopic" runat="server" AutoPostBack="True" Width="8em" onselectedindexchanged="DropdownTopic_SelectedIndexChanged"></asp:dropdownlist>&nbsp;-</td>
					<td class="tdd1" width="11"><asp:dropdownlist id="DropdownStopic" runat="server" AutoPostBack="True" Width="8em" onselectedindexchanged="DropdownStopic_SelectedIndexChanged"></asp:dropdownlist>&nbsp;-</td>
					<td class="tdd1" width="11"><asp:dropdownlist id="DropdownSect" runat="server" AutoPostBack="True" Width="8em"></asp:dropdownlist></td>
				</tr>
			</table>
			</TD></TR></TBODY></TABLE>
			<p><asp:button id="SubmitButton" onclick="SubmitButton_Click" runat="server" Text="Generate Report"></asp:button><asp:button id="Clear" onclick="Clear_Click" runat="server" Text="Clear"></asp:button></p>
			<hr>
			<asp:datagrid id="ResultsDataGrid" runat="server" HorizontalAlign="Left" BorderWidth="1px" BorderStyle="Solid"
				BorderColor="Gray" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2">
				<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				<HeaderStyle Font-Names="Arial" Font-Bold="True" HorizontalAlign="Center" ForeColor="White" VerticalAlign="Middle"
					BackColor="#577AB4"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="StandardType" HeaderText="StandardType"></asp:BoundColumn>
					<asp:BoundColumn DataField="StandardID" HeaderText="Standard Number"></asp:BoundColumn>
					<asp:BoundColumn DataField="Para_Label" HeaderText="Paragraph/Label"></asp:BoundColumn>
					<asp:BoundColumn DataField="Topic" HeaderText="Topic"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Subtopic">
						<ItemTemplate>
							<%#DataBinder.Eval(Container,"DataItem.Subtopic")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="Section" HeaderText="Section"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Paragraph">
						<ItemTemplate>
							<a href='<%#DataBinder.Eval(Container,"DataItem.CodLink")%>' target='_top'>
								<%#DataBinder.Eval(Container,"DataItem.CodPara")%>
								<%#DataBinder.Eval(Container,"DataItem.term")%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></form>
	</body>
</HTML>
