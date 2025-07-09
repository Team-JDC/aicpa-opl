<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<%@ Register TagPrefix="radt" Namespace="Telerik.WebControls" Assembly="RadTreeView" %>
<%@ Register TagPrefix="radp" Namespace="Telerik.WebControls" Assembly="RadPanelbar" %>
<%@ Page CodeBehind="D_QuickFind.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_QuickFind" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<HTML>
	<HEAD>
	</HEAD>
	<body>
		<form runat="server" id="mainForm" method="post" style="WIDTH:100%">
			<table id="QuickFindTable" cellSpacing="0" cellPadding="0" width="100%">
				<tr vAlign="top">
					<td height="25"><portal:title id="Title1" EditText="Edit" EditUrl="~/DesktopModules/D_QuickFindEdit.aspx" runat="server"></portal:title>
					</td>
					<td id="QuickFindNavTd" align="right" runat="server" height="25">
						<asp:imagebutton id="HelpImageButton" runat="server" ImageUrl="../images/help.gif" ToolTip="help"
							AlternateText="help"></asp:imagebutton>
					</td>
				</tr>
				<tr vAlign="top">
					<td id="QuickFindTd" colspan="2" runat="server">
						<table width="100%">
							<tr>
								<td width="400">
									<div class="container">
										<radP:RadPanelbar
										ID="RadPanelbar1" 
										Runat="server" 
										ContentFile="~/xmlFiles/Panelbar.xml" 
										Theme="WinXPPanel"
										ExpandEffect="Fade" 
										ExpandEffectSettings="duration=0.4" 
										Width="250px"></radP:RadPanelbar>
									</div>
								</td>
								<td runat="server" id="content">
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>		
