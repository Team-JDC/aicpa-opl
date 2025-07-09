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

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Search;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_Search.
	/// </summary>
	public partial class D_Search : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get the search results from the session
			ISearchResults currentSearchResults = (ISearchResults)Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];

			//get the search criteria from the search results
			ISearchCriteria currentSearchCriteria = currentSearchResults.SearchCriteria;

			//get the dimension values passed on the request; if they are present, add them to the search criteria
			string dimensionValueIds = Request.Params[DestroyerUi.REQPARAM_SEARCHDIMENSIONVALUEIDS];
			if(dimensionValueIds != null && dimensionValueIds != DestroyerUi.EMPTY_STRING)
			{
				//start a new array
				ArrayList dimensionIds = new ArrayList();

				//split apart the value ids from the request and put them in the arraylist
				string[] dimValues = dimensionValueIds.Split(DestroyerBpc.ENDECA_DIMENSIONIDSEPCHAR);
				foreach(string dimValue in dimValues)
				{
					dimensionIds.Add(dimValue);
				}

				//put the dimension id array into the search criteria (replacing all the dimension value ids that were previously there)
				string[] newDimensionIds = (string[])dimensionIds.ToArray(typeof(string));
				currentSearchCriteria.DimensionIds = newDimensionIds;
			}

			//reset the page offset to zero (no offset, start at results 1-n)
			currentSearchCriteria.PageOffset = 0;

			//perform a search
			currentSearchResults = site.SiteIndex.Search(currentSearchCriteria);

			//store the search results back to the session
			Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = currentSearchResults;
            
			//redirect to the doc/toc page
			DestroyerUi.ShowTab(this.Page, DestroyerUi.PortalTab.Search, null);
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
	}
}
