using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for ExtTableRow.
	/// </summary>
	public class ExtTableRow:HtmlTableRow
	{
		public ExtTableRow()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public override string UniqueID
		{ 
			get
			{
				return this.ID; 
			}
		}
		
		public override string ClientID
		{ 
			get
			{
				return this.ID;
			}
		}
	}
}
