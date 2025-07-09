namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Configuration;

	using AICPA.Destroyer.Shared;
	using AICPA.Destroyer.Content;
	using AICPA.Destroyer.Content.Site;
	using AICPA.Destroyer.Content.Book;
	using AICPA.Destroyer.Content.Document;

	/// <summary>
	///		Summary description for D_QuickFind.
	/// </summary>
	public class D_QuickFind : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell QuickFindNavTd;
		protected System.Web.UI.HtmlControls.HtmlTableCell QuickFindTd;
		protected Telerik.WebControls.RadPanelbar RadPanelbar1;
		protected System.Web.UI.HtmlControls.HtmlTableCell content;
		protected System.Web.UI.WebControls.ImageButton HelpImageButton;

		private void Page_Load(object sender, System.EventArgs e)
		{
			/*if (Application[AICPA.Destroyer.Shared.DestroyerUi.APPPARAM_QUICKFIND_DT_XML] == null)
			{
				// Create the Toc XML for the Document Type Tree Control
				XmlDocument DocType = new XmlDocument();

				//get the location of the Taxonomy XML
				string xmlPath = System.Configuration.ConfigurationSettings.AppSettings["TaxonomyXmlLocation"];
				
				//Load the XSL that will transform the xml
				DocType.Load(xmlPath);
				XslTransform treeTransform = new XslTransform();
				treeTransform.Load("QuickFind.xsl");
				
				//XmlReader myTocXml = treeTransform.Transform(DocType, null);
			}	*/
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
			this.HelpImageButton.Click += new System.Web.UI.ImageClickEventHandler(this.HelpImageButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void HelpImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			DestroyerUi.ShowHelp(this.Page, DestroyerUi.HelpTopic.QuickFind);
		}

		
	}
}
