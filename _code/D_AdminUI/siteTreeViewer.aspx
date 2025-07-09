<%@ Register TagPrefix="radt" Namespace="Telerik.WebControls" Assembly="RadTreeView" %>
<%@ Page language="c#" Codebehind="siteTreeViewer.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI.siteTreeViewer" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>siteTreeViewer</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="styles.css" type="text/css" rel="stylesheet">
		<script src="scripts/common.js"></script>
		<script>
			var xPos,yPos;
			
			function getPath(node)
			{
				var s = node.Text;
				var parent = node.Parent;
				while (parent != null)
				{
					s = parent.Text + " > " + s;
					parent = parent.Parent;
				}
				return;
			}
			
			function setPos()
			{
				document.getElementById("xPos").value = event.clientX;
				document.getElementById("yPos").value = event.clientY;
				return;
			}
			
		</script>
	</HEAD>
	<body bgColor="#fdfdfd" leftMargin="0" topMargin="0" onload="setGlobals();" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<input id="xPos" type="hidden" runat="server">
			<input id="yPos" type="hidden" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td class="tree_menu" style="WIDTH: 656px">&nbsp;
								</td>
								<TD noWrap align="right" width="50%">
									<table cellSpacing="0" cellPadding="0" border="0">
										<tr>
											<td><asp:label id="NewNodeId" runat="server" Visible="False">0</asp:label><asp:label id="IdHolder" runat="server" Visible="False">Label</asp:label><asp:label id="SiteIdLabel" runat="server"></asp:label></td>
											<td class="tree_menu" align="center"><asp:checkbox id="newSiteVersion" runat="server" Text="New Version"></asp:checkbox>&nbsp;&nbsp;&nbsp;</td>
											<td vAlign="bottom" align="center"><asp:imagebutton id="SaveBtn" runat="server" AlternateText="Save Site" ImageUrl="images/save.gif" onclick="TreeView_Save"></asp:imagebutton>&nbsp;</td>
											<td vAlign="top" align="center"></td>
										</tr>
									</table>
								</TD>
							</tr>
						</table>
					</td>
				<tr>
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td class="tree_menu" align="left"><asp:label id="siteStatusLb" runat="server"></asp:label></td>
								<td class="tree_menu" align="right"><asp:label id="siteIndexLb" runat="server"></asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
				<TR>
					<TD><radt:radtreeview id="TreeView" runat="server"></radt:radtreeview></TD>
				</TR>
			</table>
			<asp:label id="jsLabel" runat="server" Visible="False"></asp:label>
			<div id="addNewFolder">
				<table cellSpacing="0" cellPadding="0" border="0">
					<tr>
						<td>&nbsp;</td>
						<td noWrap>
							<table cellSpacing="0" cellPadding="0" border="0">
								<tr>
									<td noWrap><asp:label id="InputText" runat="server" Visible="False" CssClass="tree_menu">Label</asp:label></td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td>
										<asp:textbox id="NameBox" runat="server" Visible="False" CssClass="treeView_button"></asp:textbox>
										<asp:dropdownlist id="BookDB" runat="server" Visible="False" CssClass="treeView_button"></asp:dropdownlist>
									</td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr id="folderURIRowTitle" runat="server">
									<td class="tree_menu">
										Folder URI
									</td>
								</tr>
								<tr id="folderURIRowInput" runat="server">
									<td>
										<asp:textbox id="folderURI" runat="server" Visible="False" CssClass="treeView_button"></asp:textbox>
									</td>
								</tr>
								<tr id="folderURIDivision" runat="server">
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td noWrap align="right">
										<asp:button id="addButton" runat="server" Visible="False" Text="Add Folder" CssClass="treeView_button" onclick="TreeView_AddSiteFolder"></asp:button>
										<asp:button id="replaceButton" runat="server" Visible="False" Text="Replace Book" CssClass="treeView_button" onclick="TreeView_ReplaceBook"></asp:button>
										<asp:button id="reNameNodeBtn" runat="server" Visible="False" Text="Edit Folder" CssClass="treeView_button" onclick="TreeView_editFolder"></asp:button>
										<asp:button id="bookAddBtn" runat="server" Visible="False" Text="Add Book" CssClass="treeView_button" onclick="TreeView_addBook"></asp:button>
										<asp:button id="cancelBtn" runat="server" Visible="False" Text="Cancel" CssClass="treeView_button" onclick="TreeView_cancel"></asp:button>
									</td>
								</tr>
								<tr>
									<td noWrap></td>
								</tr>
							</table>
						</td>
						<td>&nbsp;</td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</HTML>
