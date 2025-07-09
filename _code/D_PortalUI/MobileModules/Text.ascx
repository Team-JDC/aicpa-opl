<%@ Control Language="C#" Inherits="AICPA.Destroyer.UI.Portal.MobilePortalModuleControl" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Register TagPrefix="ASPNETPortal" Namespace="AICPA.Destroyer.UI.Portal.MobileControls" Assembly="ASPNETPortal" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Title" Src="~/MobileModuleTitle.ascx" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<%--

    The Text Mobile User Control renders text modules in the mobile portal. 

    The control consists of two pieces: a summary panel that is rendered when
    portal view shows a summarized view of all modules, and a multi-part panel 
    that renders the module details.

--%>

<script runat="server">

    String mobileSummary = "";
    String mobileDetails = "";
   
    //*********************************************************************
    //
    // Page_Load Event Handler
    //
    // The Page_Load event handler on this User Control is used to
    // load the contents of the text message from a file, and databind
    // the message to the module contents.
    //
    //*********************************************************************
    
    void Page_Load(Object sender, EventArgs e) {

        // Obtain the selected item from the HtmlText table
        AICPA.Destroyer.UI.Portal.HtmlTextDB text = new AICPA.Destroyer.UI.Portal.HtmlTextDB();
        SqlDataReader dr = text.GetHtmlText(ModuleId);
        
        if (dr.Read()) {

            // Dynamically add the file content into the page
            mobileSummary = Server.HtmlDecode((String) dr["MobileSummary"]);
            mobileDetails = Server.HtmlDecode((String) dr["MobileDetails"]);
        }
        
        DataBind();
       
        // Close the datareader
        dr.Close();       
    }
        
</script>

<mobile:Panel id="summary" runat="server">
    <DeviceSpecific>
        <Choice Filter="isJScript">
            <ContentTemplate>
                <ASPNETPortal:Title runat="server" />
                <font face="Verdana" size="-2">
                    <%# mobileSummary %>
                    <asp:LinkButton runat="server" Visible="<%# mobileDetails != String.Empty %>" Text="more" CommandName="Details" />
                </font>
                <br>
                <br>
            </ContentTemplate>
        </Choice>
    </DeviceSpecific>
</mobile:Panel>

<ASPNETPortal:Title runat="server" />
<mobile:TextView runat="server" Text="<%# mobileDetails %>" Font-Name="Verdana" Font-Size="Small" />
<ASPNETPortal:LinkCommand runat="server" Text="back" CommandName="summary" Font-Name="Verdana" Font-Size="Small" />
