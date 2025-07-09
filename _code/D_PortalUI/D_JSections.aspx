<%@ Page language="c#" Codebehind="D_JSections.aspx.cs" AutoEventWireup="True" Inherits="AICPA.Destroyer.UI.Portal.JSections" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>JSections</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" name="vs_targetSchema">
		<style type="text/css">
H1 {
	FONT-WEIGHT: bold; FONT-SIZE: large; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: left
}
H2{ FONT-WEIGHT: bold; FONT-SIZE: 12px; COLOR: #000000; FONT-FAMILY: arial;TEXT-ALIGN: left}
.tdh1 {
	FONT-WEIGHT: bold; BACKGROUND: #577ab4; COLOR: #ffffff; FONT-FAMILY: arial; TEXT-ALIGN: center
}
.tdh2 {
	FONT-WEIGHT: bold; FONT-SIZE: 14px; COLOR: #000000; FONT-FAMILY: arial; TEXT-ALIGN: center
}
.rbit {
	FONT-SIZE: 14px; COLOR: #000000; FONT-FAMILY: arial; TEXT-ALIGN: center
}
.tdor {
	FONT-WEIGHT: bold; COLOR: #577ab4; FONT-FAMILY: arial; TEXT-ALIGN: center
}
.tdd1 {
	VERTICAL-ALIGN: top; FONT-FAMILY: arial; TEXT-ALIGN: center
}
.tdt1 {
	VERTICAL-ALIGN: top
}
</style>
<script language="JavaScript">
	function CheckAll(checkAllBox)
	{
		var frm = document.Form1;
		var ChkState = checkAllBox.checked;
		var box;
		
		for (i = 0; i < frm.length; i++)
		{
			box = frm.elements[i];
			
			if (box.type == 'checkbox' && box.name.indexOf('joinCheck') != -1)
				box.checked = ChkState;
		}
	}
	
	function CheckChanged()
	{
		var frm = document.Form1;
		var boolAllChecked = true;
		var box;
		
		for(i = 0; i < frm.length; i++)
		{
			box = frm.elements[i];
			
			if (box.type == 'checkbox' && box.name.indexOf('joinCheck') != -1 && box.checked == false)
			{
				boolAllChecked = false;
				break;
			}
		}
		
		for (i = 0; i < frm.length; i++)
		{
			box = frm.elements[i];
			
			if (box.type == 'checkbox' && box.name.indexOf('checkAll') != -1)
			{
				box.checked = boolAllChecked;	
				break;
			}
		}
	}
</script>
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<h1>Join Sections</h1>
		<h2>Join all of the selected sections under a given topic</h2>
		<form id="Form1" method="post" runat="server">
			<table width="48" border="0">
				<tr vAlign="middle">
					<td class="tdh1" colSpan="2">Required</td>
					<td class="tdh1" colSpan="1">Options</td>
				</tr>
				<tr vAlign="middle">
					<td class="tdh2">Topic</td>
					<td class="tdd1" width="34"><asp:dropdownlist id="jsTopic" runat="server" Width="32em" OnSelectedIndexChanged="jsTopic_SelectedIndexChanged"
							AutoPostBack="True" CssClass="rbit"></asp:dropdownlist></td>
					<td class="tdd1" width="18"><asp:checkbox id="Intersection" runat="server" Width="16em" AutoPostBack="True" CssClass="rbit"
							Text="Include Intersection Subtopics" oncheckedchanged="Intersection_CheckedChanged"></asp:checkbox></td>
				</tr>
				<tr vAlign="middle">
					<td class="tdh2">Section</td>
					<td class="tdd1" width="34"><asp:dropdownlist id="jsSection" runat="server" Width="32em" CssClass="rbit"></asp:dropdownlist></td>
					<td class="tdd1" width="18"><asp:radiobuttonlist id="rbSec" runat="server" Width="16em" AutoPostBack="True" CssClass="rbit" RepeatDirection="Horizontal"
							Height="24px" onselectedindexchanged="rbSec_SelectedIndexChanged">
							<asp:ListItem Value="FASB" Selected="True">FASB</asp:ListItem>
							<asp:ListItem Value="SEC">SEC</asp:ListItem>
							<asp:ListItem Value="ALL">ALL</asp:ListItem>
						</asp:radiobuttonlist></td>
				</tr>
			</table>
			<p><asp:button id="SubmitButton" OnClick="SubmitButton_Click" runat="server" Text="Get Sections" Enabled="False"></asp:button></p>
			<hr>
			<p style="float: left; padding-top: 15px;"><asp:datagrid id="Documents" HorizontalAlign="Left" Runat="server" ForeColor="Transparent" BorderColor="DarkGray"
				CellPadding="2">
				<ItemStyle Font-Names="Arial" HorizontalAlign="Center"></ItemStyle>
				<HeaderStyle Font-Names="Arial" Font-Bold="True" HorizontalAlign="Center" ForeColor="White" BackColor="#577AB4"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<HeaderTemplate>
							<input type="checkbox" id="checkAll" name="checkAll" runat="server" onclick="CheckAll(this);" />
						</HeaderTemplate>
						<ItemTemplate>
							<input type="checkbox" id="joinCheck" name="joinCheck" runat="server" onclick="CheckChanged();" />
							<asp:Label ID="lblBookTitle" Runat="server" Visible="False" Text='<%#DataBinder.Eval(Container, "DataItem.BookTitle")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Topic">
						<ItemTemplate>
							<asp:Label ID="lblTopicNum" Runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.TopicNum")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Subtopic">
						<ItemTemplate>
							<asp:Label ID="lblSubtopicNum" Runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.SubtopicNum")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Section">
						<ItemTemplate>
							<asp:Label ID="lblSectionNum" Runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.SectionNum")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Title">
						<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<%#DataBinder.Eval(Container, "DataItem.TopicNum")%>
							<%#DataBinder.Eval(Container, "DataItem.TopicTitle")%>
							&gt;
							<%#DataBinder.Eval(Container, "DataItem.SubtopicNum")%>
							<%#DataBinder.Eval(Container, "DataItem.SubtopicTitle")%>
							&gt;
							<%#DataBinder.Eval(Container, "DataItem.SectionNum")%>
							<%#DataBinder.Eval(Container, "DataItem.SectionTitle")%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></p>
			<p style="clear: left; padding-top: 5px;">
				<asp:Button ID="JoinSubmit" Runat="server" Text="Join Sections" Visible="False" OnClick="JoinSubmit_Click"></asp:Button>
				<asp:Button ID="JoinWithSourcesSubmit" Runat="server" Text="Join Sections with Sources" Visible="False" OnClick="JoinWithSourcesSubmit_Click"></asp:Button>
			</p>
		</form>
	</body>
</HTML>
