using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.WebControls;
using ASPENCRYPTLib;

	/// <summary>
	/// Summary description for DefaultCS.
	/// </summary>
namespace AICPA.Destroyer.UI.Portal.Alpha
{ 
	public partial class Alpha : System.Web.UI.Page
	{
		const string KEY = "$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk";

		protected System.Web.UI.HtmlControls.HtmlInputHidden reSourceURL;
		protected System.Web.UI.HtmlControls.HtmlInputButton Button1;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//The event hook for the panelitem
            RadPanelbar1.AfterClientPanelItemClicked = "onAfterClick";
			//See If I can call a client side script
		

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



		protected void Submit1_ServerClick(object sender, System.EventArgs e)
		{
			try
			{
				string selectedGuid = Guid.Value.ToString();
				
				ICryptoManager objCM = new CryptoManager();
				ICryptoContext objContext = objCM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 1, Missing.Value);
				ICryptoKey objKey = objContext.GenerateKeyFromPassword(KEY,  ASPENCRYPTLib.CryptoAlgorithms.calgSHA, ASPENCRYPTLib.CryptoAlgorithms.calgRC4, 128);

				// Encrypt text, place encrypted data into a blob	
				ICryptoBlob objBlob = objKey.EncryptText(selectedGuid);
				// Display encrypted value in Hex and Base64 formats
				hidEncPersGUID.Value = objBlob.Hex;

				string encryptedUrl = Server.UrlEncode(hidURL.Value);
				//string Url = "~/../ResourceSeamlessLogin.aspx?hidEncPersGUID=" + hidEncPersGUID.Value + "&hidSourceSiteCode=" + hidSourceSiteCode.Value + "&hidURL=" + hidURL.Value + "&hidDomain=" + hidDomain.Value ;
//				string virtDir = Request.CurrentExecutionFilePath;
//				virtDir = virtDir.Substring(0, virtDir.IndexOf("/", 1));
//				string Url ="http://" +  Request.Url.Host + "/" + virtDir + "/ResourceSeamlessLogin.aspx?hidEncPersGUID=" + hidEncPersGUID.Value + "&hidSourceSiteCode=" + hidSourceSiteCode.Value + "&hidURL=" + encryptedUrl + "&hidDomain=" + hidDomain.Value ;
				string Url ="../ResourceSeamlessLogin.aspx?hidEncPersGUID=" + hidEncPersGUID.Value + "&hidSourceSiteCode=" + hidSourceSiteCode.Value + "&hidURL=" + encryptedUrl + "&hidDomain=" + hidDomain.Value ;
				//string Url ="http://destroyer.aicpa.org/D_PortalUI/ResourceSeamlessLogin.aspx?hidEncPersGUID=" + hidEncPersGUID.Value + "&hidSourceSiteCode=" + hidSourceSiteCode.Value + "&hidURL=" + encryptedUrl + "&hidDomain=" + hidDomain.Value ;

				//reSourceURL.Value = Url;
				string popupScript = "<script language='javascript'>openWindow('"+Url+"');//window.open('" + Url + "', 'reSource',width=400, height=200, scrollbars='yes', resizable='yes');</script>";
				
                //Obsolete function djf 1/19/10
				//Page.RegisterStartupScript("PopupScript", popupScript);
                //http://msdn.microsoft.com/en-us/library/aa479390.aspx
                ClientScript.RegisterStartupScript(this.GetType(),"PopupScript", popupScript);
				
				//Response.Redirect(Url,true);
				//Response.

				
			}
		
			catch(Exception ex)
			{
				Guid.Value = ex.Message;
			}

		  
		}

		

		
		

		        		
	}
}
