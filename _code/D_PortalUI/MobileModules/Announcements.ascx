<%@ Control language="c#" Inherits="AICPA.Destroyer.UI.Portal.MobilePortalModuleControl" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Title" Src="~/MobileModuleTitle.ascx" %>
<%@ Register TagPrefix="ASPNETPortal" Namespace="AICPA.Destroyer.UI.Portal.MobileControls" Assembly="ASPNETPortal" %>
<%@ Import Namespace="System.Data" %>
<%--

    The Announcements Mobile User Control renders announcement modules in the
    portal for mobile devices. 

    The control consists of two pieces: a summary panel that is rendered when
    portal view shows a summarized view of all modules, and a multi-part panel 
    that renders the module details.

--%>
<script runat="server">

    DataSet ds = null;
    int currentIndex = 0;

    //*********************************************************************
    //
    // Page_Load Event Handler
    //
    // The Page_Load event handler on this User Control is used to
    // obtain a DataSet of announcement information from the Announcements
    // table, and then databind the results to the module contents.  It uses 
    // the AICPA.Destroyer.UI.Portal.AnnouncementsDB() data component 
    // to encapsulate all data functionality.
    //
    //*******************************************************

    void Page_Load(Object sender, EventArgs e) {

        // Obtain announcement information from Announcements table
        AICPA.Destroyer.UI.Portal.AnnouncementsDB announcements = new AICPA.Destroyer.UI.Portal.AnnouncementsDB();
        ds = announcements.GetAnnouncements(ModuleId);

        // DataBind the User Control
        DataBind();
    }
                  
    //*********************************************************************
    //
    // SummaryView_OnItemCommand Event Handler
    //
    // The SummaryView_OnItemCommand event handler is called when the user
    // clicks on a "More" link in the summary view. It calls the
    // ShowAnnouncementDetails utility method to show details of the
    // announcement.
    //
    //*********************************************************************

    void SummaryView_OnItemCommand(Object sender, RepeaterCommandEventArgs e) {
        ShowAnnouncementDetails(e.Item.ItemIndex);        
    }

    //*********************************************************************
    //
    // AnnouncementsList_OnItemCommand Event Handler
    //
    // The AnnouncementsList_OnItemCommand event handler is called when the user
    // clicks on an item in the list of announcements. It calls the
    // ShowAnnouncementDetails utility method to show details of the
    // announcement.
    //
    //*********************************************************************

    void AnnouncementsList_OnItemCommand(Object sender, ListCommandEventArgs e) {
        ShowAnnouncementDetails(e.ListItem.Index);        
    }

    //*********************************************************************
    //
    // DetailsView_OnClick Event Handler
    //
    // The DetailsView_OnClick event handler is called when the user 
    // clicks in the details view to return to the summary view.
    //
    //*********************************************************************

    void DetailsView_OnClick(Object sender, EventArgs e) {
    
        // Make the parent tab show module summaries again.
        Tab.SummaryView = true;
    }

    //*********************************************************************
    //
    // ShowAnnouncementDetails Method
    //
    // The ShowAnnouncementDetails method sets the active pane of
    // the module to the details view, and shows the details of the
    // given item.
    //
    //*********************************************************************

    void ShowAnnouncementDetails(int itemIndex) {
    
        currentIndex = itemIndex;

        // Switch the visible pane of the multi-panel view to show
        // announcement details
        MainView.ActivePane = AnnouncementDetails;

        // Rebind the details panel
        AnnouncementDetails.DataBind(); 
        
        // Make the parent tab switch to details mode, showing this module
        Tab.ShowDetails(this);
    }
    
    //*********************************************************************
    //
    // FormatChildField Method
    //
    // The FormatChildField method returns the selected field as a string,
    // if the row is not empty.  If empty, it returns String.Empty.
    //
    //*********************************************************************

    string FormatChildField (string fieldName) {
    
        if (ds.Tables[0].Rows.Count > 0) 
            return ds.Tables[0].Rows[currentIndex][fieldName].ToString();
        else
            return String.Empty;
    }            

</script>
<mobile:Panel id="summary" runat="server">
    <DeviceSpecific>
        <Choice Filter="isJScript">
            <ContentTemplate>
                <ASPNETPortal:Title runat="server" />
                <font face="Verdana" size="-2">
                    <asp:Repeater id="announcementList" DataSource="<%# ds %>" OnItemCommand="SummaryView_OnItemCommand" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton runat="server">
                                <%# DataBinder.Eval(Container.DataItem, "Title") %>
                            </asp:LinkButton>
                            <br>
                        </ItemTemplate>
                    </asp:Repeater>
                </font>
                <br>
            </ContentTemplate>
        </Choice>
    </DeviceSpecific>
</mobile:Panel>
<ASPNETPortal:MultiPanel id="MainView" Font-Name="Verdana" Font-Size="Small" runat="server">
    <ASPNETPortal:ChildPanel id="AnnouncementsList" runat="server">
        <ASPNETPortal:Title runat="server" />
        <mobile:List runat="server" DataTextField="Title" DataSource="<%# ds %>" OnItemCommand="AnnouncementsList_OnItemCommand" />
    </ASPNETPortal:ChildPanel>
    <ASPNETPortal:ChildPanel id="AnnouncementDetails" runat="server">
        <ASPNETPortal:Title runat="server" Text='<%# FormatChildField("Title") %>' />
        <mobile:TextView runat="server" Text='<%# FormatChildField("Description") %>' />
        <mobile:Link runat="server" NavigateUrl='<%# FormatChildField("MobileMoreLink") %>' Visible='<%# FormatChildField("MobileMoreLink") != String.Empty %>' Text="read more" />
        <ASPNETPortal:LinkCommand runat="server" OnClick="DetailsView_OnClick" Text="back" />
    </ASPNETPortal:ChildPanel>
</ASPNETPortal:MultiPanel>
