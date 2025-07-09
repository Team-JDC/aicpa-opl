<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_Help" CodeBehind="D_Help.ascx.cs" AutoEventWireup="True" enableViewState="True"%>
<%@ Register TagPrefix="radm" Namespace="Telerik.WebControls" Assembly="RadMenu" %>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx"%>
<table id="HelpTable" border="0" cellSpacing="0" cellPadding="0" width="100%" height="100%"  style="border-left:solid 1px #C5C5C5;border-right:solid 1px #C5C5C5;">
	<tr vAlign="top" style="width:90%;height:32px;vertical-align:top;background-image:url(RadControls/Menu/Images/pageBg.gif);background-repeat:repeat-x;">
		<td align=left valign=middle style="padding-left:20px;">
			<!--<portal:title id="Title1" EditText="Edit" EditUrl="~/DesktopModules/D_HelpEdit.aspx" runat="server"></portal:title>-->
			<b><font face=" Arial, Verdana;" color="darkred"><span style='cursor:hand;cursor:pointer' onclick="loadHelp('help/start.htm');">HELP</span></font></b>
		</td>
		<td align=right style="padding-right:120px;">
			<radM:RadMenu 
				id="helpMenu" 
				runat="server" 
				AccessibilityEnabled="True" 
				contentfile="~/xmlFiles/HelpMenu/helpMenu.xml" 
				cssfile="~/RadControls/Menu/menu.css" 
				imagesbasedir="~/RadControls/Menu/Images/"
				onclientclick="helpLoader">
			</radM:RadMenu>
		</td>
	</tr>
	<!--<tr vAlign="top">
		<td id="HelpTd" runat="server" style="padding:50px 50px 50px 50px;"></td>
	</tr>-->
	<tr>
		<td height="100%" colspan="2">
			<iframe id="helpFrame" width="100%" src="Help/start.htm" height="800" frameborder=0></iframe>
		</td>
	</tr>
</table>
<script>helpFrameResize();</script>
<asp:Label ID="helpJSLabel" Visible="False" Runat="server"></asp:Label>