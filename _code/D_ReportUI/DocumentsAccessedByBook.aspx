<%@ Page Title="Documents Accessed by Book" Language="C#" AutoEventWireup="true" CodeBehind="DocumentsAccessedByBook.aspx.cs"
    Inherits="D_ReportUI.DocumentsAccessedByBook" MasterPageFile="~/ReportNav.Master" %>
    
<%@ Register tagPrefix="telerik" namespace="Telerik.Web.UI" assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentHolder" runat="server">
    <form id="theForm" runat="server" defaultbutton="GoButton">
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
    <h1 style="color: #597790; margin-top: 0px; padding-top: 0px;">Documents Accessed by Book</h1>
    <table>
        <tr>
            <td>
                <b><asp:Label ID="BeginDateLabel" runat="server" Text="Begin Date: " AssociatedControlID="BeginRadDatePicker"
                    style="color: #597790"></asp:Label></b>
                <telerik:RadDatePicker ID="BeginRadDatePicker" runat="server"></telerik:RadDatePicker>
                <b><asp:Label ID="EndDateLabel" runat="server" Text="End Date: " AssociatedControlID="EndRadDatePicker"
                    style="color: #597790"></asp:Label></b>
                <telerik:RadDatePicker ID="EndRadDatePicker" runat="server"></telerik:RadDatePicker>
                <asp:Button ID="GoButton" Text="Submit" runat="server" />
            </td>
            <td>
                <img id="loadAnimation" alt="Loading..." title="Loading..." src="img/loader.gif" />
            </td>
        </tr>
    </table>
    <telerik:RadGrid ID="ReportRadGrid" runat="server" GridLines="None" OnNeedDataSource="ReportRadGrid_NeedDataSource">
        <HeaderContextMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>
        <MasterTableView GroupLoadMode="client" ShowGroupFooter="true" 
            AutoGenerateColumns="False" GroupsDefaultExpanded="false" AllowFilteringByColumn="True" AllowSorting="true">
            <Columns>
                <telerik:GridBoundColumn DataField="Year" HeaderText="Year" 
                    DataType="System.Int32" ReadOnly="True" SortExpression="Year" AutoPostBackOnFilter="false"
                    CurrentFilterFunction="EqualTo" ShowFilterIcon="false" FilterDelay="200">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MonthStr" HeaderText="Month" 
                    ReadOnly="True" SortExpression="Month" UniqueName="MonthStr" AutoPostBackOnFilter="false"
                    CurrentFilterFunction="EqualTo" ShowFilterIcon="false" FilterDelay="200">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Day" DataType="System.Int32" 
                    HeaderText="Day" ReadOnly="True" SortExpression="Day" UniqueName="Day" AutoPostBackOnFilter="false"
                    CurrentFilterFunction="EqualTo" ShowFilterIcon="false" FilterDelay="200">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="BookName" HeaderText="Book Name" 
                    ReadOnly="True" SortExpression="BookName" UniqueName="BookName" Aggregate="CountDistinct"
                    FooterText="Number of unique books: " AutoPostBackOnFilter="false" 
                    CurrentFilterFunction="Contains" FilterDelay="200">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Total" DataType="System.Int32" 
                    HeaderText="Documents Accessed" ReadOnly="True" SortExpression="Total" UniqueName="Total"
                    Aggregate="Sum" FooterText="Total Documents Accessed: ">
                    <FilterTemplate></FilterTemplate>
                </telerik:GridBoundColumn>
            </Columns>
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField FieldName="Year" />
                        <telerik:GridGroupByField FieldName="MonthStr" HeaderText="Month" />
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="Year" />
                        <telerik:GridGroupByField FieldName="Month" />
                    </GroupByFields>
                </telerik:GridGroupByExpression>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField FieldName="Day" />
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="Day" />
                    </GroupByFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="false" AllowDragToGroup="false" AllowRowsDragDrop="false"
            Resizing-AllowColumnResize="false" Resizing-AllowRowResize="false">
        </ClientSettings>
        <GroupingSettings RetainGroupFootersVisibility="true" />
        <FilterMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>
    </form>
    
    <script type="text/javascript" language="javascript">
        (function($)
        {
            $(document).ready(function()
            {
                $('#loadAnimation').hide();

                $(<%= "'#" + GoButton.ClientID + "'" %>).click(function()
                {
                    $('#loadAnimation').show();
                    $('#theForm').submit();
                });
            });
        })($telerik.$);
    </script>
</asp:Content>
