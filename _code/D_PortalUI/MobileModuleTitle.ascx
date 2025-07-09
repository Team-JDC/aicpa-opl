<%@ Control %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<%@ Import Namespace="AICPA.Destroyer.UI.Portal" %>

<%--

    The MobileModuleTitle User Control is responsible for displaying the title of 
    each portal module within the mobile portal. It include device-specific
    templates for richer rendering of the title on Pocket PCs.

--%>

<script language="C#" runat="server">

    public String Text;
    
    //*********************************************************************
    //
    // Page_Load Event Handler
    //
    // The Page_Load event handler executes after the user control is loaded
    // and inserted into the control tree.
    //
    // The Page_Load event handler checks to see if 
    //
    //*********************************************************************


    void Page_Load(Object sender, EventArgs e) {
    
        if (Text == null) {
        
            // If the Text property has not been explicitly specified, 
            // walk the parent control chain to find a MobilePortalModuleControl,
            // and obtain the title from the corresponding module.
        
            MobilePortalModuleControl module = null;
            Control control = this;
            
            while (module == null && (control = control.Parent) != null) {
                module = control as MobilePortalModuleControl;
            }
                    
            Text = module.ModuleTitle;
        }
    
        // Databind the User control. 
        DataBind();
    }
    
</script>

<mobile:Panel runat="server">
    <DeviceSpecific>
        <Choice Filter="isJScript">
            <ContentTemplate>
                <font face="Verdana" size="-1" color="#666633"><b>
                        <%# Text %>
                    </b></font>
                <br>
                <hr noshade size="1pt" color="#666633">
            </ContentTemplate>
        </Choice>
    </DeviceSpecific>
    <mobile:Label runat="server" ForeColor="#666633" Font-Size="Large" Font-Bold="True">
        <%# Text %>
    </mobile:Label>
</mobile:Panel>
