<%@ Page Title="FAF License Agreement" Language="C#" AutoEventWireup="true" CodeBehind="FAFLicenseAgreement.aspx.cs"
    Inherits="D_ReportUI.FAFLicenseAgreement" MasterPageFile="~/ReportNav.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentHolder" runat="server">
    <form id="theForm" runat="server">
    <%--Script references below are only needed for jQuery intellisense; you must comment out before running though!--%>
    <%--<asp:ScriptManager runat="server" ID="ScriptManager1">
       <Scripts>
           <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
           <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
           <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
       </Scripts>
    </asp:ScriptManager>--%>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ReportRadGrid">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ReportRadGrid" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <h1 style="color: #597790; margin-top: 0px; padding-top: 0px;">FAF License Agreement</h1>
    <telerik:RadGrid ID="ReportRadGrid" runat="server" GridLines="None" OnNeedDataSource="ReportRadGrid_NeedDataSource">
        <MasterTableView GroupLoadMode="client" ShowGroupFooter="false" 
            AutoGenerateColumns="false" GroupsDefaultExpanded="false" AllowFilteringByColumn="false" AllowSorting="false">
            <Columns>
                <telerik:GridBoundColumn DataField="LicenseAgreement" DataType="System.String" 
                    HeaderText="FAF License Agreement Status" ReadOnly="True" UniqueName="LicenseAgreement">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Total" DataType="System.Int32" 
                    HeaderText="Total C2B Users" ReadOnly="True" UniqueName="Total">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="false" AllowDragToGroup="false" AllowRowsDragDrop="false"
            Resizing-AllowColumnResize="false" Resizing-AllowRowResize="false">
        </ClientSettings>
    </telerik:RadGrid>
    </form>
</asp:Content>