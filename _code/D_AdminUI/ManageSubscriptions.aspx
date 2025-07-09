<%@ Page language="c#" Codebehind="ManageSubscriptions.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI.ManageSubscriptions" %>
<%@ Register TagPrefix="radt" Namespace="Telerik.WebControls" Assembly="RadTreeView" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Manage Subscriptions</title>
		<meta http-equiv="Page-Enter" content="blendTrans(Duration=0.3)">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="styles.css" type="text/css" rel="stylesheet">
		<script src="scripts/common.js"></script>
		<script>
			function setPos()
			{
				document.getElementById("xPos").value = event.clientX;
				document.getElementById("yPos").value = event.clientY;
				return;
			}
		</script>
	</HEAD>
	<body onload="setGlobals();" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<input type="hidden" id="xPos" runat="server" NAME="xPos"> <input type="hidden" id="yPos" runat="server" NAME="yPos">
			<radt:radtreeview id="SubscriptionTree" runat="server"></radt:radtreeview>
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
									<td><asp:label id="labelSubscriptionCode" runat="server" CssClass="standard-text">Code</asp:label><br>
										<asp:textbox id="SubscriptionCode" runat="server" CssClass="treeView_button" Enabled="False"
											MaxLength="32"></asp:textbox></td>
								</tr>
								<tr>
									<td><asp:label id="labelSubscriptionTitle" runat="server" CssClass="standard-text">Title</asp:label><br>
										<asp:textbox id="SubscriptionTitle" runat="server" Visible="False" CssClass="treeView_button"
											MaxLength="128"></asp:textbox>
										<asp:label id="labelNoBooksToAdd" runat="server" CssClass="standard-text">There are not any books left to add.</asp:label>
										<asp:dropdownlist id="BookDB" runat="server" Visible="False" CssClass="treeView_button"></asp:dropdownlist></td>
								</tr>
								<tr>
									<td><asp:label id="labelSubscriptionDescription" runat="server" CssClass="standard-text">Description</asp:label><br>
										<asp:textbox id="SubscriptionDescription" runat="server" Visible="False" CssClass="treeView_button"
											MaxLength="256"></asp:textbox></td>
								</tr>
								<tr>
									<td>
										<hr color="#cfe6f2" SIZE="1">
									</td>
								</tr>
								<tr>
									<td noWrap align="right"><asp:button id="saveButton" runat="server" Visible="False" CssClass="treeView_button" Text="Save" onclick="saveButton_Click"></asp:button><asp:button id="addBookButton" runat="server" Visible="False" CssClass="treeView_button" Text="Add Book" onclick="addBookButton_Click"></asp:button><asp:button id="cancelButton" runat="server" Visible="False" CssClass="treeView_button" Text="Cancel" onclick="cancelButton_Click"></asp:button></td>
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
