<%@ Page Title="" Language="C#" MasterPageFile="~/ODPMaster.Master" AutoEventWireup="true" CodeBehind="Tools.aspx.cs" Inherits="MainUI.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphLeft" runat="server">
<script type="text/javascript">
    $().ready(function () {
        /* route:
        <base>/tools/<toolType>/<nType>/<tDoc>/<tPtr>

        Note: nType, tDoc, and tPtr may be used as something different. I had to give them names so we could access
        them.

        */
        var toolType = '<asp:Literal ID="lToolType" runat="server" Text="<%$RouteValue:tooltype%>" />';
        var tDoc = '<asp:Literal ID="lTargetDoc" runat="server" Text="<%$RouteValue:targetdoc%>" />';
        var tPtr = '<asp:Literal ID="lTargetPtr" runat="server" Text="<%$RouteValue:targetptr%>" />';
        var nType = '<asp:Literal ID="lNodeType" runat="server" Text="<%$RouteValue:nodetype%>" />';
        var g_NodeTypeArray = ['guidance', 'policy', 'standard'];

        //debugger;
        showDocumentToolbar(false);
        if (toolType.toLowerCase() == 'notes') {
            loadNotes();
        } else if (toolType.toLowerCase() == "bookmarks") {
            loadBookmarks();

            //        } else if (toolType.toLowerCase() == 'book') {
            //            doBreadcrumbLink(tDoc, nType);
            //        } else if (nType.toLowerCase() == 'document') {
            //            //
            //            doBreadcrumbLink(tDoc, nType);
            //        } else if (nType.toLowerCase() == 'format') {
            //            doBreadcrumbLink(tDoc, nType);
            //        } else if (nType.toLowerCase() == 'bookfolder') {
            //            doBreadcrumbLink(tDoc, nType);
            //        } else if (nType.toLowerCase() == 'namedanchor') {
            //            //
            //            doBreadcrumbLink(tDoc, nType);
            //        } else if (nType.toLowerCase() == 'sitefolder') {
            //            //
            //            doBreadcrumbLink(tDoc, nType);
        } else if (toolType.toLowerCase() == "savedsearches") {
            loadSavedSearches();
        }
        //else if (toolType.toLowerCase() == 'preferences') {
        //    loadPreferences();
        //}
        else if (toolType.toLowerCase() == 'goto') {
            loadGoTo();
        } else if (toolType.toLowerCase() == 'join') {
            loadJoin();
        } else if (toolType.toLowerCase() == 'xref') {
            loadXRef();
        } else if (toolType.toLowerCase() == 'howtoguide') {
            loadGuide();
        } else if (toolType.toLowerCase() == 'archive') {
            if ($.inArray(nType.toLowerCase(), g_NodeTypeArray) != -1) {
                loadArchive(tDoc);
            } else {
                loadArchiveByDocAndPtr(tDoc, tPtr);
            }
            // i chose to base64 decode/encode the string so we don't have to deal with filnames
        } else if (toolType.toLowerCase() == 'loadarchive') {
            var fn = Base64.decode(tDoc);
            var arr = fn.split('|');
            $("#aFafDocument").attr("href", "/content/link/" + arr[0] + "/" + arr[1]);
            showDocumentToolbar(true);
            doLoadArchiveContent(arr[2]);
        } else if (toolType.toLowerCase() == 'wlh') {
            doFafWhatLinksHereLink(tDoc, tPtr);
        } else {
            //doLinkRoute(tDoc, tPtr);
        }

    });
    

</script>

                 
<div id="leftcol" class="col-sm-12"  style="min-height:55vh">
    <%-- <div class="leftcol_inner">
    </div>--%>
</div>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphRight" runat="server">
</asp:Content>

