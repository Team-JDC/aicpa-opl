<%@ Page language="c#" Codebehind="ManageBooks.aspx.cs" AutoEventWireup="True" Inherits="Rainbow.Portals._Destroyer.ManageBooks" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Manage Books</title>
		<meta http-equiv="Page-Enter" content="blendTrans(Duration=0.3)">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="styles.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="scripts/common.js"></script>
		<script>
			var maxHeight = 350;
			var minHeight = 1;
			var globalRow;
			
			function init()
			{
				getSize();
				getDescArea('DataGridBooks');
			}
			function getXYvalues(){
				document.getElementById("xVal").value = event.clientX;
				document.getElementById("yVal").value = event.clientY;
				return;
			}			
		</script>
	</HEAD>
	<body onload="init();" MS_POSITIONING="GridLayout" topmargin=0 leftmargin=0>
		<form id="Form1" method="post" runat="server">
			<input id="Id" type="hidden"> <input id="EditWho" type="hidden">
			<input type="hidden" id="xVal" runat="server" name="xVal">
			<input type="hidden" id="yVal" runat="server" NAME="yVal">
			<div align="center"><asp:datagrid id="DataGridBooks" Width=100% runat="server" CssClass="grid_style" AllowSorting="True" AutoGenerateColumns="False">
					<SelectedItemStyle Wrap="True"></SelectedItemStyle>
					<EditItemStyle Wrap="True" BackColor="#FFFFCC"></EditItemStyle>
					<AlternatingItemStyle Wrap="True"></AlternatingItemStyle>
					<ItemStyle Wrap="True"></ItemStyle>
					<HeaderStyle Wrap="True"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn HeaderText="BookInstanceId">
							<ItemStyle Wrap="True" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" ID="BookInstanceDbID" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="2" HeaderText="Title" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'>
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="standard-text" ID="Title" MaxLength="256" Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'>
								</asp:TextBox>
								<asp:requiredfieldvalidator EnableClientScript="True" id="RequiredfieldvalidatorTitle" runat="server" ErrorMessage="Title is required."
									ControlToValidate="Title" Display="Dynamic"></asp:requiredfieldvalidator>
								<asp:Label runat="server" ID="BookInstanceId" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
								</asp:Label>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="1" HeaderText="Name" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="False" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>'>
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox Enabled='<%# (DataBinder.Eval(Container, "DataItem.IsEditable")) %>' CssClass="standard-text" ID="Name" MaxLength="128" Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>'>
								</asp:TextBox>
								<asp:requiredfieldvalidator EnableClientScript="True" id="RequiredfieldvalidatorName" runat="server" ErrorMessage="Name is required."
									ControlToValidate="Name" Display="Dynamic"></asp:requiredfieldvalidator>
							</EditItemTemplate>
						</asp:TemplateColumn>

						<asp:TemplateColumn SortExpression="4" HeaderText="Version" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Version") %>' ID="Label4">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Version") %>' ID="Version">
								</asp:Label>
							</EditItemTemplate>
						</asp:TemplateColumn>

						<asp:TemplateColumn HeaderText="Copyright" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Copyright") %>' ID="Label2">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="standard-text" ID="Copyright" MaxLength="256" Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Copyright") %>'>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>

						<asp:TemplateColumn HeaderText="Description" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="Label5">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="standard-text" ID="Description" MaxLength="256" Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
								</asp:TextBox>								
							</EditItemTemplate>
						</asp:TemplateColumn>

						<asp:TemplateColumn HeaderText="Location" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="False" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SourceUri") %>'>
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="standard-text" ID="Location" MaxLength="512" Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SourceUri") %>'>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="3" HeaderText="Publish Date" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PublishDate") %>'>
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PublishDate") %>' ID="Label3">
								</asp:Label>
							</EditItemTemplate>
						</asp:TemplateColumn>
						
						<asp:TemplateColumn HeaderText="Status" HeaderStyle-CssClass="grid-header">
							<ItemStyle Wrap="True" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.BookStatus") %>' ID="Status">
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						
						<asp:TemplateColumn>
							<ItemStyle Font-Size="X-Small" Wrap="False" HorizontalAlign="Left" CssClass="grid-item"></ItemStyle>
							<HeaderStyle CssClass="grid-header"></HeaderStyle>
							<HeaderTemplate>
								<asp:LinkButton Runat="server" CssClass="grid-header-links" Text="Add Book" CommandName='<%# COMMAND_ADDBOOK %>' CausesValidation="False" ID="AddBook">
								</asp:LinkButton>&nbsp;|&nbsp;
								<asp:LinkButton Runat="server" CssClass="grid-header-links" Text="<%# GetToggleArchiveText() %>" CommandName='<%# COMMAND_TOGGLEARCHIVED %>' CausesValidation="False" ID="ToggleArchive">
								</asp:LinkButton>
							</HeaderTemplate>
							<ItemTemplate>
								<img src="images/spacer.gif" width="3">
								<asp:ImageButton runat="server" ImageUrl="images/icon-pencil.gif" AlternateText="Edit" CommandName="Edit"
									CausesValidation="false" ID="editButton"></asp:ImageButton>

								<asp:ImageButton runat="server" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.Id") %>' ImageUrl="images/icon-clone.gif" AlternateText="Clone" CommandName="Clone"
									CausesValidation="false" ID="cloneButton"></asp:ImageButton>
								
								<asp:ImageButton runat="server" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.Id") %>' CommandName='ArchiveData' CausesValidation="false" ID="archiveButton">
								</asp:ImageButton>
								
								<asp:ImageButton Visible='<%# (DataBinder.Eval(Container, "DataItem.IsEditable")) %>' ImageUrl="images/build.gif" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.Id") %>' AlternateText='Build' CommandName='<%# COMMAND_BUILD %>' CausesValidation="false" ID="BuidBookLinkButton">
								</asp:ImageButton>
								<asp:imagebutton Runat="server" ImageUrl="images/action_check_out.gif" AlternateText="TOC" CommandName="DisplayToc"
									CausesValidation="False" ID="displayBookTOC" NAME="editTree"></asp:imagebutton>

								<asp:imagebutton Runat="server" ImageUrl="images/help.gif" AlternateText="Error Information" CommandName="ERROR_Display"
									CausesValidation="False" ID="imgHolder" NAME="imgHolder" Visible="False"></asp:imagebutton>									
							</ItemTemplate>
							<EditItemTemplate>
								<img src="images/spacer.gif" width="3">
								<asp:ImageButton runat="server" ImageUrl="images/icon-pencil-x.gif" AlternateText="Cancel" CommandName="Cancel"
									CausesValidation="false"></asp:ImageButton>
								<img src="images/spacer.gif" width="3">
								<asp:ImageButton CausesValidation="True" runat="server" ImageUrl="images/save.gif" AlternateText="Save" CommandName="Update"></asp:ImageButton>
							</EditItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:datagrid></div>
				<!-- Page links -->
				<table border="0" cellpadding="4" cellspacing="0" align="right">
					<tr>
						<td><asp:LinkButton CssClass="grid-paging" ID="prevPage" Text="Previous" CommandName="prevPage"
								Runat="server" Visible="False" onclick="DataGrid_prevPage"></asp:LinkButton></td>
						<td><asp:Label CssClass="grid-text" ID="stats" Runat="server" Visible="True"></asp:Label></td>
						<td><asp:LinkButton CssClass="grid-paging" ID="nextPage" Text="Next" CommandName="nextPage" Runat="server"
								Visible="False" onclick="DataGrid_nextPage"></asp:LinkButton></td>
					</tr>
					<tr>
						<td colspan=3 align=right nowrap valign=top>
							<asp:LinkButton CssClass="grid-paging" ID="showAll" Text="Show All Books" CommandName="showAll" Runat="server"	Visible="False" onclick="DataGrid_setShowAllFlag"></asp:LinkButton>
						</td>
					</tr>
				</table>
				<!-- End of page links -->				
			<div id="treeContainer" style="VISIBILITY: visible"><asp:placeholder id="treeHolder" runat="server"></asp:placeholder></div>
			<div id="addNewFolder">
				<table cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td>&nbsp;</td>
						<td noWrap>
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td class="tree_menu" noWrap>Error Description:
									</td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td noWrap>
										<table border=0 cellpadding=0 cellspacing=0 width="250px">
											<tr>
												<td class="treeView_button">
													<asp:Label ID="errorMsg" Runat="server" Visible=False></asp:Label>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td align="right"><input class="treeView_button" onclick="hidemenu();" type="button" value="Close"></td>
								</tr>
							</table>
						</td>
						<td>&nbsp;</td>
					</tr>
				</table>
			</div>
			<asp:label id="jsLabel" runat="server" Visible="False"></asp:label>
		</form>
	</body>
</HTML>
