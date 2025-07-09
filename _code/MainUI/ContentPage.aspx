<%@ Page Title="" Language="C#" MasterPageFile="~/ODPMaster.Master" AutoEventWireup="true" CodeBehind="ContentPage.aspx.cs" Inherits="MainUI.ContentPage" %>

 <%----%>
<asp:Content ID="cRight" ContentPlaceHolderID="cphLeft" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            setLoading(true);

            var tDoc = '<asp:Literal ID="lTargetDoc" runat="server" Text="<%$RouteValue:targetdoc%>" />';
            var tPtr = '<asp:Literal ID="lTargetPtr" runat="server" Text="<%$RouteValue:targetptr%>" />';
            var nType = '<asp:Literal ID="lNodeType" runat="server" Text="<%$RouteValue:nodetype%>" />';
            var search = '<%=Request.QueryString["search"]%>';
            if (search == "true") {
                setShowHighlights(true);
            } else {
                setShowHighlights(false);
            }

            if (nType.toLowerCase() == 'site') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'book') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'document') {
                doTocLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'format') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'bookfolder') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'documentanchor') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'namedanchor') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'sitefolder') {
                doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'join') {
                doJoinInternal(tDoc, tPtr);
            } else {
                doLinkRoute(tDoc, tPtr);
            }
        });
    

    </script>

    <div id="leftcol" class="col-sm-9"  style="min-height:55vh">
       <%-- <div class="leftcol_inner">
        </div>--%>
    </div>
</asp:Content>

<asp:Content ID="cLeft" ContentPlaceHolderID="cphRight" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var tDoc = '<asp:Literal ID="rTargetDoc" runat="server" Text="<%$RouteValue:targetdoc%>" />';
            var tPtr = '<asp:Literal ID="rTargetPtr" runat="server" Text="<%$RouteValue:targetptr%>" />';
            var nType = '<asp:Literal ID="rNodeType" runat="server" Text="<%$RouteValue:nodetype%>" />';
            
            if (nType.toLowerCase() == 'site') {
                loadDocumentWidgetByIdType(tDoc, nType);
            } else if (nType.toLowerCase() == 'book') {
                loadDocumentWidgetByIdType(tDoc, nType);
            } else if (nType.toLowerCase() == 'document') {
                loadDocumentWidgetByIdType(tDoc, nType);
            } else if (nType.toLowerCase() == 'format') {
                //doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'bookfolder') {
                //doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'documentanchor') {
                loadDocumentWidgetByIdType(tDoc, nType);
            } else if (nType.toLowerCase() == 'namedanchor') {
                loadDocumentWidgetByIdType(tDoc, nType);
                //doBreadcrumbLink(tDoc, nType);
            } else if (nType.toLowerCase() == 'sitefolder') {
                //
                loadDocumentWidgetByIdType(tDoc, nType);
                //doBreadcrumbLink(tDoc, nType);
            } else {
                loadDocumentWidget(tDoc, tPtr);
            }

        });
    </script>
<div id="rightcol" class="col-sm-3">
</div>
</asp:Content>