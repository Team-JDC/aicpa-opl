using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AICPA.Destroyer.Content.Site;

namespace D_AdminUI
{
	/// <summary>
	/// Summary description for IndexStatusMonitor.
	/// </summary>
	public partial class IndexStatusMonitor : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			string sites = Request.QueryString["sites"];
			displaySites.Text = sites;

			if(sites.Length > 0)
			{
				this.getSiteInfo(sites);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		private void getSiteInfo(string siteData)
		{
			string[] siteIndx = siteData.Split(new char[1]{','});
			string sitesStr = string.Empty;

			foreach(string site in siteIndx)
			{
				string[] siteId = site.Split(new char[1]{'|'});
				sitesStr += this.getSiteIndexStatus(siteId[0]) + "|" + siteId[0] + "|" + siteId[1] + ",";
			}

			sitesStr = sitesStr.Substring(0,sitesStr.Length - 1);
			displaySites.Text += "|" + sitesStr;
			jsLabel.Text = "<script>replaceValues('"+sitesStr+"')</script>";
			jsLabel.Visible = true;
		}

		private string getSiteIndexStatus(string siteId)
		{
			Site site = new Site(Convert.ToInt16(siteId));
			return site.IndexBuildStatus.ToString();
		}
	}
}
