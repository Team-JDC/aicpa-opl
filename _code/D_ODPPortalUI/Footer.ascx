<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="D_ODPPortalUI.Footer" %>
<%@ Register TagPrefix="uc" TagName="toolbarHome" Src="~/Toolbars/ToolbarHome.ascx"  %>
<%@ Register TagPrefix="uc" TagName="toolbarDocuments" Src="~/Toolbars/ToolbarDocuments.ascx"  %>
<%@ Register TagPrefix="uc" TagName="toolbarTools" Src="~/Toolbars/ToolbarTools.ascx"  %>
<%@ Register TagPrefix="uc" TagName="toolbarPreferences" Src="~/Toolbars/ToolbarPreferences.ascx"  %>
   <div id="footer">
        <div class="prefooter">
            <ul class="tabs">
                <li><asp:LinkButton ID="lbtTabHome" runat="server" OnClick="lbtTabHome_Click" Text="Home" /></li>
                <li><asp:LinkButton ID="lbtTabDocuments" runat="server" OnClick="lbtTabDocuments_Click" Text="Documents" /></li>
                <li><asp:LinkButton ID="lbtTabTools" runat="server" OnClick="lbtTabTools_Click" Text="Tools" /></li>
                <li><asp:LinkButton ID="lbtTabPreferences" runat="server" OnClick="lbtTabPreferences_Click" Text="Preferences" /></li>
            </ul>
                <span class="right hide top5" style="right: 40px;">
                    <asp:ImageButton ID="ibtShowToolBar" runat="server" OnClick="ibtShowToolBar_Click" ImageUrl="~/images/show.gif" />
                    <asp:ImageButton ID="ibtHideToolBar" runat="server" OnClick="ibtHideToolBar_Click" ImageUrl="~/images/hide.gif" Visible="false" OnClientClick="$('#panel').slideToggle('slow');" />
                </span>
            
        </div>
        <div class="clear">
        </div>
        <div class="">
            <asp:Panel ID="pnlToolbar" Visible="true" runat="server">
            
                <div id="panel" class="postfooter">
                <asp:MultiView ID="mvwFooter" runat="server">
                    <asp:View ID="vwHome" runat="server">
                    <div id="divTabHome" class="tab_content">
                        <uc:toolbarHome ID="ucToolbarHome" runat="server" />
                    </div>
                    </asp:View>
                    <asp:View ID="vwDocuments" runat="server">
                    <div id="divTabDocuments" class="tab_content">
                        <uc:toolbarDocuments ID="ucToolbarDocuments" runat="server" />
                    </div>
                    </asp:View>
                    <asp:View ID="vwTools" runat="server">
                        <div id="divTabTools" class="tab_content">
                            <uc:toolbarTools ID="ucToolbarTools" runat="server" />
                        </div>
                    </asp:View>
                    <asp:View ID="vwPreferences" runat="server">
                        <div id="divTabPreferences" class="tab_content">
                            <uc:toolbarPreferences ID="ucToolbarPreferences" runat="server" />
                        </div>
                    </asp:View>
                </asp:MultiView>
                
                </div>
            </asp:Panel>
        </div>
    </div>