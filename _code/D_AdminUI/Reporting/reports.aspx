<%@ Page language="c#" Codebehind="reports.aspx.cs" AutoEventWireup="True" Inherits="D_AdminUI.Reporting.reports" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>reports</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../styles.css" type="text/css" rel="stylesheet">
	<script>
		function showDiv(id){
			var oDiv = eval("document.getElementById('"+id+"')");
			oDiv.style.display = oDiv.style.display == "none" ? "block" : "none";
			return;
		}
	</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td><h2><font color="darkblue"><asp:label id="titleLabel" Runat="server" Visible="False"></asp:label></font></h2></td>
					<td align="right" valign=top>
						<table cellSpacing="0" cellPadding="0" border="0">
							<TR>
								<td onclick="javascript:window.history.back();" style="CURSOR: hand">
									<asp:Image id="goBkBtn" runat="server" ImageUrl="../images/prevDoc.gif" Visible=False AlternateText='previous report'></asp:Image>
								</td>
								<td>&nbsp;&nbsp;</td>
								<td style="CURSOR: hand" onclick="javascript:window.print();">
									<IMG src="../images/print.gif" title='print report'>
								</td>
							</TR>
						</table>
					</td>
				</tr>
			</table>
			
			
			<asp:datagrid id="user_access" runat="server" Width="100%" CssClass="grid_style" AutoGenerateColumns="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:ButtonColumn DataTextField="userId" HeaderText="User Id" CommandName="getUserBooks" HeaderStyle-CssClass="grid-header"
						ItemStyle-CssClass="grid-item"></asp:ButtonColumn>
					<asp:BoundColumn DataField="userType" HeaderText="User Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="accessCount" HeaderText="User Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>					
					<asp:BoundColumn DataField="userId" Visible="False"></asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			
			<asp:datagrid id="exam_access" runat="server" Width="100%" CssClass="grid_style" AutoGenerateColumns="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:ButtonColumn DataTextField="userId" HeaderText="User Id" CommandName="getUserBooks" HeaderStyle-CssClass="grid-header"
						ItemStyle-CssClass="grid-item"></asp:ButtonColumn>
					<asp:BoundColumn DataField="userName" HeaderText="User Name">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>						
					<asp:BoundColumn DataField="userType" HeaderText="User Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="accessCount" HeaderText="User Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="userId" Visible="False"></asp:BoundColumn>
				</Columns>
			</asp:datagrid>			
			
			<asp:datagrid id="dd_userBooks" runat="server" AutoGenerateColumns="False" CssClass="grid_style" Width="100%" Visible="False">
				<AlternatingItemStyle BackColor="#fafafa"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="bookName" HeaderText="Book Name">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="89%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="hitCount" HeaderText="Times Visited">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="loggedInfo" Visible="False"></asp:BoundColumn>
				</Columns>
			</asp:datagrid>

			<asp:datagrid id="dd_userHist" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="50%" Visible="False">
				<AlternatingItemStyle BackColor="#fafafa"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="usertype" HeaderText="User Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="accessedDate" HeaderText="Visit Date">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:datagrid id="top_documents" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="100%" Visible="False">
				<AlternatingItemStyle BackColor="#fafafa"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>

					<asp:ButtonColumn DataTextField="bookTitle" HeaderText="Book" CommandName="getBookDocuments"
						HeaderStyle-CssClass="grid-header" ItemStyle-CssClass="grid-item" HeaderStyle-Width="79%">
					</asp:ButtonColumn>
						
					<asp:BoundColumn DataField="totalHits" HeaderText="Times Visited">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:ButtonColumn DataTextField="uniqueUsers" HeaderText="Unique Users" CommandName="getBookUsers"
						HeaderStyle-CssClass="grid-header" ItemStyle-CssClass="grid-item" HeaderStyle-Width="10%">
					</asp:ButtonColumn>
					
					<asp:BoundColumn DataField="bookName" Visible="False">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					
					<asp:BoundColumn DataField="bookTitle" Visible="False">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:datagrid id="dd_bookUsers" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="60%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="userId" HeaderText="User Id">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="79%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="totalHits" HeaderText="Times Visited">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:datagrid id="searchFound" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="60%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="keyWord" HeaderText="User Id">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="79%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="searchedCount" HeaderText="Times Searched">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:datagrid id="searchNotFound" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="60%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="keyWord" HeaderText="User Id">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="79%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="searchedCount" HeaderText="Times Searched">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="20%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>

			<asp:datagrid id="errorSummary" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="100%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="errorTypeDescription" HeaderText="Error Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="70%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="totalHits" HeaderText="Error Occurence">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="40%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="errorType" Visible=False>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:datagrid id="errorDetail" runat="server" AutoGenerateColumns="false" CssClass="grid_style"
				Width="100%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="errorTypeDescription" HeaderText="Error Type">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="15%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="description" HeaderText="Error Description">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="70%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="totalHits" HeaderText="Error Occurence">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="15%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			
			<asp:DataGrid ID="bookDocument" Runat="server" AutoGenerateColumns="False" CssClass="grid_style"
			width="100%" Visible="False">
				<AlternatingItemStyle BackColor="#FAFAFA"></AlternatingItemStyle>
				<Columns>
					<asp:BoundColumn DataField="rowId">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="1%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>				
					<asp:BoundColumn DataField="DocumentTitle" HeaderText="Document Title">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="69%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="DocumentHits" HeaderText="Total Hits">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="15%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="UniqueUsers" HeaderText="Unique Users">
						<HeaderStyle CssClass="grid-header"></HeaderStyle>
						<ItemStyle Width="15%" CssClass="grid-item"></ItemStyle>
					</asp:BoundColumn>
				</Columns>			
			</asp:DataGrid>
		</form>
	</body>
</HTML>
