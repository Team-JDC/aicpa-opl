<%@ Register TagPrefix="DestroyerHeader" TagName="Banner" Src="adminBanner.ascx" %>
<%@ Register TagPrefix="radt" Namespace="Telerik.WebControls" Assembly="RadTreeView" %>
<%@ Page language="c#" Codebehind="ManageSite.aspx.cs" AutoEventWireup="True" Inherits="Rainbow.Portals._Destroyer.ManageSite" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Manage Sites</title>
		<meta http-equiv="Page-Enter" content="blendTrans(Duration=0.3)">
		<LINK href="styles.css" type="text/css" rel="stylesheet">
			<LINK href="images/TreeIcons/Styles/Classic/obout_treeview2.css" rel="stylesheet">
				<script language="javascript" src="scripts/common.js"></script>
				<script>
			var maxHeight = 350;
			var minHeight = 1;
			var globalRow;
			
			function reloadPage(){
				window.location.href = 'ManageSite.aspx';
				return;
			}
			function cannotArchive(){
				if(!waitForWidthChangeFlag){
					setTimeout("cannotArchive()",5);
					return;
				}
				alert('The Site cannot be Archived.  Change the Site Status to \'UNASSIGNED\' first.');			
				return;
			}
			
			function editArchive(){
				var editFlag = confirm("The site will be UNARCHIVED for edit.\n Continue unarchiving?");
				if(!editFlag){
					window.location.reload();
				}
				return false;
			}
			
			function refresh(){
				window.location.href = window.location.href;
				return;
			}
			
			function getTransferIndex(id){
				var oSelect = eval("document.getElementById('"+id+"')");
				var oTransfChecker = document.getElementById("indexTransfer");
				if(oSelect.value == 2){
					var transferIndx = confirm("Would you like to transfer the Site Index to Production?");
					oTransfChecker.checked = transferIndx;
				}else{
					oTransfChecker.checked = false;
				}
				return;
			}
			
			function setMonitor(id)
			{
				var oMonitoring = window.parent.document.getElementById("indexStatusMonitor");
				var temp = oMonitoring.value;
				
				oMonitoring.value = temp.length > 0 ? temp + "," + id : id;
				return;
			}
			
			
			function init()
			{
				getSize();
				getDescArea('SiteGrid');
				monitoringValues();
				return;
			}
			
			function monitoringValues(goOn)
			{
				monitorStr = window.parent.document.getElementById("indexStatusMonitor").value;
				gettingStatus = window.parent.document.getElementById("gettingStatusFlag").value;
				
				if(monitorStr.length < 3 || gettingStatus == "true"){
					return;
				}
				
				if(goOn != "true")
				{
					setTimeout("monitoringValues('true')",2000);
					return;
				}
				
				//alert('after time out');
								
				//alert("info: "+window.parent.document.getElementById("indexStatusMonitor").value);
				monitorStr = window.parent.document.getElementById("indexStatusMonitor").value;
				
				var element = monitorStr.split(",");
				var siteInfo,position;
				var myTable = document.getElementById("SiteGrid");
				var msg = "<img src='images/refresh.gif'>&nbsp;&nbsp;Reloading...";
				//alert(myTable);
					
				for(a=0;a<element.length;a++)
				{
					siteInfo = element[a].split("|");
					position = parseInt(siteInfo[1]) + 1;
					if(myTable.rows[position].cells.length < 5){
						position = position + 1;
					}
					//alert("old: "+myTable.rows[position].cells[5].innerText);
					myTable.rows[position].cells[5].innerHTML = msg;//"Refreshing...";
				}				
				
				if(monitorStr.length > 0){
					//alert('has values');
					window.parent.document.getElementById("gettingStatusFlag").value = "true";
					var oFrame = document.getElementById("monitorHolder");
					oFrame.src = "IndexStatusMonitor.aspx?sites="+monitorStr;					
				}
				return;			
			}
			
/*			function showErrorMsg(){
				pageLoadedFlag = true;
				showmenu(event.clientX,event.clientY);
				return false;
			}*/
			
			function getXYvalues(){
				document.getElementById("xVal").value = event.clientX;
				document.getElementById("yVal").value = event.clientY;
				return;
			}
				</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" onload="init();" MS_POSITIONING="GridLayout">
						<form id="siteForm" method="post" runat="server">
										<input id="xVal" type="hidden" name="xVal" runat="server"></TD>
										<input id="yVal" type="hidden" name="yVal" runat="server"></TD>
										<center><asp:datagrid id="SiteGrid" runat="server" AutoGenerateColumns="False"
												CssClass="grid_style" AllowSorting="True" Width="100%">
												<SelectedItemStyle Wrap="False"></SelectedItemStyle>
												<EditItemStyle Wrap="False" BackColor="#FFFFCC" Height="25"></EditItemStyle>
												<AlternatingItemStyle Wrap="False"></AlternatingItemStyle>
												<ItemStyle Wrap="False"></ItemStyle>
												<HeaderStyle Wrap="False"></HeaderStyle>
												<Columns>
													<asp:TemplateColumn HeaderText="SiteId">
														<ItemStyle Wrap="False"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" ID="siteDbId" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
															</asp:Label>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>' ID="Textbox1">
															</asp:TextBox>
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn SortExpression="2" HeaderText="Title">
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Wrap="False" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>' ID="Label1">
															</asp:Label>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:TextBox CssClass="standard-text" runat="server" ID="EntryTitle" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'>
															</asp:TextBox>
															<asp:requiredfieldvalidator id="RequiredfieldvalidatorTitle" runat="server" ErrorMessage="Title is required."
																ControlToValidate="EntryTitle" Display="Dynamic"></asp:requiredfieldvalidator>
															<asp:Label runat="server" ID="SiteId" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
															</asp:Label>
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn SortExpression="1" HeaderText="Name">
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Wrap="False" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>' ID="Label2">
															</asp:Label>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:TextBox CssClass="standard-text" runat="server" ID="EntryName" Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>'>
															</asp:TextBox>
															<asp:requiredfieldvalidator id="RequiredfieldvalidatorName" runat="server" ErrorMessage="Name is required."
																ControlToValidate="EntryName" Display="Dynamic"></asp:requiredfieldvalidator>
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn SortExpression="3" HeaderText="Version">
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Wrap="False" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Version") %>' ID="Label3">
															</asp:Label>
														</ItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Description">
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Wrap="False" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" ID="DescriptionArea" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
															</asp:Label>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:TextBox CssClass="standard-text" runat="server" ID="EntryDescription" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
															</asp:TextBox>
														</EditItemTemplate>
													</asp:TemplateColumn>

													<asp:TemplateColumn HeaderText="Site Status">
														<HeaderStyle CssClass="grid-header" Wrap="False"></HeaderStyle>
														<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" ID="SiteStatus" Text='<%# DataBinder.Eval(Container, "DataItem.SiteStatus")%>'>
															</asp:Label>
														</ItemTemplate>
													</asp:TemplateColumn>

													<asp:TemplateColumn SortExpression="4" HeaderText="Stage Status">
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Wrap="False" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" ID="StatusLabel" Text='<%# DataBinder.Eval(Container, "DataItem.StageStatus") %>'>
															</asp:Label>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:DropDownList CssClass="standard-text" runat="server" id="StatusDDL" Visible="True">
																<asp:ListItem Value="0">Unassigned</asp:ListItem>
																<asp:ListItem Value="1">Staging</asp:ListItem>
															</asp:DropDownList>
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Index Status">
														<HeaderStyle CssClass="grid-header" Wrap="False"></HeaderStyle>
														<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="grid-item"></ItemStyle>
														<ItemTemplate>
															<asp:Label runat="server" ID="SiteIndexStatus" Text='<%# DataBinder.Eval(Container, "DataItem.IndexStatus")%>'>
															</asp:Label>
														</ItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn>
														<HeaderStyle CssClass="grid-header"></HeaderStyle>
														<ItemStyle Font-Size="X-Small" Wrap="False" HorizontalAlign="Center" Width="138px" CssClass="grid-item"></ItemStyle>
														<HeaderTemplate>
															&nbsp;<asp:LinkButton id="AddLink" runat="server" CommandName='addSite' CssClass="grid-header-links">Add Site</asp:LinkButton>&nbsp;|&nbsp;
															<asp:LinkButton id="ArchiveLk" text="<%# GetToggleArchiveText() %>" runat="server" CommandName='showArchive' CssClass="grid-header-links"></asp:LinkButton>&nbsp;|&nbsp;
															<img src="images\reload.gif" title='Reload Page' style='cursor:hand;cursor:pointer' onClick='reloadPage();'>
															
														</HeaderTemplate>
														<ItemTemplate>
															<asp:imagebutton Runat="server" ImageUrl="images/help.gif" AlternateText="Error Information" CommandName="ERROR_Display"
																CausesValidation="False" ID="imgHolder" NAME="imgHolder" Visible="False"></asp:imagebutton>
															<asp:imagebutton runat="server" ImageUrl="images/icon-pencil.gif" AlternateText="Edit" CommandName="Edit"
																CausesValidation="false" ID="IconModify" NAME="IconModify"></asp:imagebutton>
															<asp:imagebutton Runat="server" ImageUrl="images/action_project_archive.gif" AlternateText="Archive" CommandName="Archive"
																CausesValidation="False" ID="IconDelete" NAME="IconDelete"></asp:imagebutton>
															<asp:imagebutton Runat="server" ImageUrl="images/indexBuild.gif" Visible="True" AlternateText="Build Site Index"
																CommandName="ReBuildIndex" CausesValidation="true" ID="reBuildIndex" NAME="reRuildIndex"></asp:imagebutton>
															<asp:imagebutton Runat="server" ImageUrl="images/toc_expand.gif" AlternateText="TOC" CommandName="TOC_Display"
																CausesValidation="False" ID="editTreeBtnDN" NAME="editTree"></asp:imagebutton>
														</ItemTemplate>
														<EditItemTemplate>
															<asp:imagebutton runat="server" ImageUrl="images/icon-pencil-x.gif" AlternateText="Cancel" CommandName="Cancel"
																CausesValidation="False" ID="UndoBtn" NAME="UndoBtn"></asp:imagebutton>
															<img src="images/spacer.gif" width="3">
															<asp:imagebutton runat="server" ImageUrl="images/save.gif" AlternateText="Save" CommandName="Update"
																CausesValidation="False" ID="SaveBtn" NAME="SaveBtn"></asp:imagebutton>
															

														</EditItemTemplate>
													</asp:TemplateColumn>
												</Columns>
											</asp:datagrid></center>
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
												<td colspan=3 align=right nowrap=true valign=top>
													<asp:LinkButton CssClass="grid-paging" ID="showAll" Text="Show All Sites" CommandName="showAll" Runat="server"	Visible="False" onclick="DataGrid_setShowAllFlag"></asp:LinkButton>
												</td>
											</tr>
										</table>
										<!-- End of page links -->
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
																<td class="treeView_button" noWrap>
																	<asp:datagrid id="errorGrid" AutoGenerateColumns="True" Runat="server" Visible="False"></asp:datagrid>
																	<asp:label id="errorLabel"  Runat="server" Visible="False"></asp:label>
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
								<input id="Id" type="hidden"></TD>
								<input id="EditWho" type="hidden"></TD>
								<asp:Label ID="jsLabel" Runat="server"></asp:Label></TD>
								<div style="visibility:hidden"><iframe id="monitorHolder" src="" height="20"></iframe></div>
								<!-- Page links -->
								<div style="visibility:hidden"><asp:checkbox id="indexTransfer" runat="server"></asp:checkbox></div>
							</form>
	</body>
</HTML>
