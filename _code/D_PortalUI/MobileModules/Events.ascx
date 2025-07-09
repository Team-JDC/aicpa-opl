<%@ Control Language="C#" Inherits="AICPA.Destroyer.UI.Portal.MobilePortalModuleControl" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Title" Src="~/MobileModuleTitle.ascx" %>
<%@ Register TagPrefix="ASPNETPortal" Namespace="AICPA.Destroyer.UI.Portal.MobileControls" Assembly="ASPNETPortal" %>
<%@ Import Namespace="System.Data" %>

<%--

    The Events Mobile User Control renders event modules in the portal. 

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
    // obtain a DataSet of announcement information from the Events
    // table, and then databind the results to the module contents.
    //
    //*******************************************************

    void Page_Load(Object sender, EventArgs e) {

        // Obtain announcement information from Events table
        AICPA.Destroyer.UI.Portal.EventsDB ev = new AICPA.Destroyer.UI.Portal.EventsDB();
        ds = ev.GetEvents(ModuleId);

        // DataBind User Control
        DataBind();
    }
                  
                  
    //*********************************************************************
    //
    // SummaryView_OnItemCommand Event Handler
    //
    // The SummaryView_OnItemCommand event handler is called when the user
    // clicks on a "More" link in the summary view. It calls the 
    // ShowEventDetails utility method to show details of the event.
    //
    //*********************************************************************

    void SummaryView_OnItemCommand(Object sender, RepeaterCommandEventArgs e) {
        ShowEventDetails(e.Item.ItemIndex);
    }

    //*********************************************************************
    //
    // EventsList_OnItemCommand Event Handler
    //
    // The EventsList_OnItemCommand event handler is called when the user
    // clicks on an item in the list of events. It calls the
    // ShowEventDetails utility method to show details of the event.
    //
    //*********************************************************************

    void EventsList_OnItemCommand(Object sender, ListCommandEventArgs e) {
        ShowEventDetails(e.ListItem.Index);
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
    // ShowEventDetails Method
    //
    // The ShowEventDetails method sets the active pane of
    // the module to the details view, and shows the details of the
    // given item.
    //
    //*********************************************************************
    
    void ShowEventDetails(int itemIndex) {
    
        currentIndex = itemIndex;

        // Switch the visible pane of the multi-panel view to show
        // event details.
        MainView.ActivePane = EventDetails;

        // rebind the details panel
        EventDetails.DataBind();

        // Make the parent tab switch to details mode, showing this module.
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

<mobile:Panel runat="server" id="summary">
    <DeviceSpecific>
        <Choice Filter="isJScript">
        
            <ContentTemplate>
                <ASPNETPortal:Title runat="server" />
                <font face="Verdana" size="-2">
                <asp:Repeater runat="server" DataSource="<%# ds %>" OnItemCommand="SummaryView_OnItemCommand">
                    <ItemTemplate>
                        <b><%# DataBinder.Eval(Container.DataItem, "Title") %></b><br>
                        <i><%# DataBinder.Eval(Container.DataItem, "WhereWhen") %></i>&nbsp;
                        <asp:LinkButton runat="server" Text="more" /><br><br>
                    </ItemTemplate>
                </asp:Repeater>
                </font><br>
            </ContentTemplate>
                
        </Choice>
    </DeviceSpecific>
</mobile:Panel>

<ASPNETPortal:MultiPanel runat="server" id="MainView" Font-Name="Verdana" Font-Size="Small">

    <ASPNETPortal:ChildPanel id="EventsList" runat="server">
        <ASPNETPortal:Title runat="server" />
        <mobile:List runat="server" OnItemCommand="EventsList_OnItemCommand" DataTextField="Title" DataSource="<%# ds %>" />
    </ASPNETPortal:ChildPanel>

    <ASPNETPortal:ChildPanel id="EventDetails" runat="server">
        <ASPNETPortal:Title runat="server" Text='<%# FormatChildField("Title") %>' />
        <mobile:Label runat="server" Font-Italic="true" Text='<%# FormatChildField("WhereWhen") %>' />
        &nbsp;<br>
        <mobile:TextView runat="server" Text='<%# FormatChildField("Description") %>' />
        <ASPNETPortal:LinkCommand runat="server" OnClick="DetailsView_OnClick" Text="back" />
    </ASPNETPortal:ChildPanel>

</ASPNETPortal:MultiPanel>

