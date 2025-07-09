namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
	using System;
	using System.IO;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using AICPA.Destroyer.Shared;

	/// <summary>
	///		Summary description for D_Help.
	/// </summary>
	public partial class D_Help : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{

		#region const
		protected const string SUBCRIPTIONS = "MySubscriptions";
		protected const string TOC = "Toc";
		protected const string DOCUMENT = "Document";
		protected const string QUICKFIND = "QuickFind";
		protected const string SEARCH = "Search";
		protected const string SEARCH_BOOLEAN = "Search/Boolean";
		protected const string SEARCH_CLEAN = "Search/Clean";
		protected const string DOCUMENT_PRINT = "documentPrint";
		protected const string DOCUMENT_PRINT_CUTOFF = "documentPrintCutoff";
		protected const string NO_SUB = "noSub";
		protected const string TOC_EXPAND = "tocExpand";
		#endregion const

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string helpTopic = Request[DestroyerUi.REQPARAM_HELPTOPIC];
			string helpDoc = string.Empty;

			switch(helpTopic)
			{
				case SUBCRIPTIONS:
						helpDoc = "Help/home.htm";
					break;
				case TOC:
						helpDoc = "Help/otherTips.htm#toc";
					break;
				case DOCUMENT:
						helpDoc = "Help/document.htm";
					break;
				case QUICKFIND:
						helpDoc = "Help/quickFind.htm";
					break;
				case SEARCH:
						helpDoc = "Help/search.htm";
					break;
				case SEARCH_BOOLEAN:
						helpDoc = "Help/search.htm#BooleanOperators";
					break;
				case SEARCH_CLEAN:
						helpDoc = "Help/search.htm#searchClean";
					break;
				case DOCUMENT_PRINT:
						helpDoc = "Help/document.htm#documentPrint";
					break;
				case DOCUMENT_PRINT_CUTOFF:
					helpDoc = "Help/document.htm#documentPrintCutoff";
					break;
				case NO_SUB:
						helpDoc = "Help/otherTips.htm#notSub";
					break;
				case TOC_EXPAND:
					helpDoc = "Help/otherTips.htm#tocExpand";
					break;
				case null:
					helpDoc = "Help/start.htm";
					break;
				default:
					helpDoc = "Help/otherTips.htm#"+helpTopic;
					break;
			}
			
			if(helpDoc != string.Empty && helpTopic != "")
			{
				helpJSLabel.Text = "<script>loadHelp('"+helpDoc+"');</script>";
				helpJSLabel.Visible = true;
			}

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
