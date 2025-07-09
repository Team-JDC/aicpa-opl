<%@ Control Language="c#" Inherits="AICPA.Destroyer.UI.Portal.DesktopModules.D_Toc"
    CodeBehind="D_Toc.ascx.cs" AutoEventWireup="True" EnableViewState="True" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="Portal" TagName="Title" Src="~/DesktopModuleTitle.ascx" %>

<table cellspacing="0" cellpadding="0" width="200px" border="0">
    <tr>
        <td valign="bottom" align="right" width="2%">
            <img src="images/portal/titleBarLeft.gif"/>
        </td>
        <td align="left" style="background: url(images/portal/titleBarBg.gif); background-repeat: repeat-x;">
            <img src="images/portal/tocTitle.gif"/>
            <font face="Verdana" size="2" color="#ffffff"><b>Toc</b></font>
        </td>
        <td align="right" style="background: url(images/portal/titleBarBg.gif); background-repeat: repeat-x;">
            <span onclick="showToc();" style="cursor: hand; cursor: pointer" id="showTocTxt">
                <img src="images/portal/tocExpand.gif" title="expand/contract the table of contents pane"
                    id='tocImage'/>
            </span>
            <asp:ImageButton ID="SyncTocImageButton" runat="server" ImageUrl="../images/portal/reload.gif"
                ToolTip="synchronize table of contents with  document" AlternateText="synchronize table of contents with  document"
                OnClick="SyncTocImageButton_Click"></asp:ImageButton>
            <asp:ImageButton ID="HelpImageButton" runat="server" ToolTip="help" ImageUrl="../images/portal/icon_help_16.gif"
                AlternateText="help" OnClick="HelpImageButton_Click"></asp:ImageButton>
        </td>
        <td valign="top" align="left" width="2%">
            <img src="images/portal/titleBarRight.gif" alt="" />
        </td>
    </tr>
    <tr>
        <%--		<td colspan="4" align=left >
			<div id="tocArea" class="tocCollapse">
				<radt:radtreeview id="t" runat="server" ImagesBaseDir="~/images/Square/3DClassic" Skin="images/Square/3DClassic" CausesValidation="False"
					AutoPostBack="True" enableViewState="True" SingleExpandPath="True" RetainScrollPosition="True" AfterClientToggle="AfterToggleHandler"
					NodeCssClass="tn" NodeCssClassDisable="tn_d" NodeCssClassEdit="tn_e" NodeCssClassOver="tn_o" NodeCssClassSelect="tn_s"></radt:radtreeview>

			</div>
		</td>--%>
        <td colspan="4" align="left">
            <div id="tocArea" class="tocCollapse">
                <asp:Panel ID="Panel1" runat="server">
                    <telerik:RadTreeView ID="tocControl" runat="server" CausesValidation="false" EnableViewState="true"
                        SingleExpandPath="true" LoadingStatusPosition="BelowNodeText" AutoPostBack="True"
                        RetainScrollPosition="True" AfterClientToggle="AfterToggleHandler" EnableEmbeddedSkins="false"
                        NodeCssClass="tn" NodeCssClassDisable="tn_d" NodeCssClassEdit="tn_e" NodeCssClassOver="tn_o"
                        NodeCssClassSelect="tn_s" Skin="Outlook">
                    </telerik:RadTreeView>
                </asp:Panel>
            </div>
        </td>
    </tr>
</table>
<input type="hidden" id="tocExpanded" value="" runat="server" name="tocExpanded" />
<asp:Label ID="jsLabel_scroll" runat="server" Visible="False"></asp:Label>

<script type="text/jscript">    reloadedToc();</script>

