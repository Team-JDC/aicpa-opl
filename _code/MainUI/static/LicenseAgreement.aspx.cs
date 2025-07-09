using System;
using MainUI.Shared;

namespace MainUI
{
	public partial class LicenseAgreement : System.Web.UI.Page
	{
        public string NodeId { get; private set; }
        public string NodeType { get; private set; }

		protected void Page_Load(object sender, EventArgs e)
		{
            NodeId = Context.Request[ContextManager.REQPARAM_ID];
            NodeType = Context.Request[ContextManager.REQPARAM_TYPE];
		}
	}
}