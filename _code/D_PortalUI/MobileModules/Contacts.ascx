<%@ Control Language="C#" Inherits="AICPA.Destroyer.UI.Portal.MobilePortalModuleControl" Debug="true" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Register TagPrefix="ASPNETPortal" TagName="Title" Src="~/MobileModuleTitle.ascx" %>
<%@ Register TagPrefix="ASPNETPortal" Namespace="AICPA.Destroyer.UI.Portal.MobileControls" Assembly="ASPNETPortal" %>
<%@ Import Namespace="System.Data" %>

<%--

    The Contacts Mobile User Control renders Contacts modules in the
    mobile portal. 

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
    // obtain a DataSet of contact information from the Contacts
    // database, and then databind the results to the module contents.
    //
    //*********************************************************************
    
    void Page_Load(Object sender, EventArgs e) {
    
        // Obtain announcement information from Contacts table
        AICPA.Destroyer.UI.Portal.ContactsDB ct = new AICPA.Destroyer.UI.Portal.ContactsDB();
        ds = ct.GetContacts(ModuleId);

        // DataBind User Control
        DataBind();
    }
                  
    //*********************************************************************
    //
    // SummaryView_OnClick Event Handler
    //
    // The SummaryView_OnClick event handler is called when the user
    // clicks on the link in the summary view. It shows the list of
    // contacts.
    //
    //*********************************************************************

    void SummaryView_OnClick(Object sender, EventArgs e) {
    
        // Switch the visible pane of the multi-panel view to show
        // the list of contacts.
        MainView.ActivePane = ContactsList;

        // Make the parent tab switch to details mode, showing this module.
        Tab.ShowDetails(this);
    }

    //*********************************************************************
    //
    // ContactsList_OnItemCommand Event Handler
    //
    // The ContactsList_OnItemCommand event handler is called when the user
    // clicks on a contact in the contact list. It shows the details of the
    // contact.
    //
    //*********************************************************************

    void ContactsList_OnItemCommand(Object sender, ListCommandEventArgs e) {
    
        currentIndex = e.ListItem.Index;
        ContactDetails.DataBind();

        // Switch the visible pane of the multi-panel view to show
        // contact details.
        MainView.ActivePane = ContactDetails;
            
        // rebind the details panel
        ContactDetails.DataBind();
    }

    //*********************************************************************
    //
    // DetailsView_OnClick Event Handler
    //
    // The DetailsView_OnClick event handler is called when the user
    // clicks on a link in the contact details view to return to the
    // list of contacts.
    //
    //*********************************************************************

    void DetailsView_OnClick(Object sender, EventArgs e) {
    
        ContactsList.DataBind();

        // Switch the visible pane of the multi-panel view to show
        // the list of contacts.
        MainView.ActivePane = ContactsList;
    }


    //*********************************************************************
    //
    // GetPhoneNumber Method
    //
    // The GetPhoneNumber method extracts a phone number from a contact
    // entry, if possible, using a regular expression search.
    //
    //*********************************************************************

    private String GetPhoneNumber(String s) {
    
        if (s != String.Empty) {
        
            // Look for a phone number.
            Match phoneMatch = Regex.Match(s, "\\+?[\\d\\(\\)\\.-]+");
            s = phoneMatch.Success ? phoneMatch.ToString() : String.Empty;
        }

        return s; 
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
                <font face="Verdana" size="-2">Click <a runat="server" OnServerClick="SummaryView_OnClick">
                        here</a> to access the directory of contacts. </font>
                <br>
            </ContentTemplate>
        </Choice>
    </DeviceSpecific>
</mobile:Panel>

<ASPNETPortal:MultiPanel runat="server" id="MainView" Font-Name="Verdana" Font-Size="Small">

    <ASPNETPortal:ChildPanel id="ContactsList" runat="server">
        <ASPNETPortal:Title runat="server" />
        <mobile:List runat="server" OnItemCommand="ContactsList_OnItemCommand" DataTextField="Name" DataSource="<%# ds %>">
            <DeviceSpecific>
                <Choice Filter="isJScript">
                    <HeaderTemplate>
                        <table width="95%" border="0" cellspacing="0" cellpadding="0">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <font face="Verdana" size="-2"><a href='<%# "mailto:" + DataBinder.Eval(((MobileListItem)Container).DataItem, "Email") %>'>
                                        <%# DataBinder.Eval(((MobileListItem)Container).DataItem, "Name") %>
                                    </a></font>
                            </td>
                            <td align="right">
                                <font face="Verdana" size="-2">
                                    <asp:LinkButton runat="server" Text="more" />
                                </font>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </Choice>
            </DeviceSpecific>
        </mobile:List>
    </ASPNETPortal:ChildPanel>
    
    <ASPNETPortal:ChildPanel id="ContactDetails" runat="server">
        <ASPNETPortal:Title runat="server" Text='<%# FormatChildField("Name") %>' />
        <b>Role: </b>
        <mobile:Label runat="server" Text='<%# FormatChildField("Role") %>' />
        <b>Email: </b>
        <mobile:Link runat="server" NavigateUrl='<%# "mailto:" + FormatChildField("Email") %>' Text='<%# FormatChildField("Email") %>' />
        <b>Contacts: </b>
        <br>
        <mobile:PhoneCall runat="server" Visible='<%# FormatChildField("Contact1") != String.Empty %>' PhoneNumber='<%# GetPhoneNumber(FormatChildField("Contact1")) %>' Text='<%# FormatChildField("Contact1") %>' AlternateFormat="{0}" />
        <mobile:PhoneCall runat="server" Visible='<%# FormatChildField("Contact2") != String.Empty %>' PhoneNumber='<%# GetPhoneNumber(FormatChildField("Contact2")) %>' Text='<%# FormatChildField("Contact2") %>' AlternateFormat="{0}" />
        <ASPNETPortal:LinkCommand runat="server" OnClick="DetailsView_OnClick" Text="back to list" />
    </ASPNETPortal:ChildPanel>
    
</ASPNETPortal:MultiPanel>
