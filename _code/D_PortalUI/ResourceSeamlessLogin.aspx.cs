using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using AICPA.Destroyer.User;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content.Site;
using ASPENCRYPTLib;
using AICPA.Destroyer.User.Event;


namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for ProflitSeamlessLogin.
	/// </summary>
	public partial class ProflitSeamlessLogin : DestroyerUi
	{
		const string KEY = "$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//for logging
			AICPA.Destroyer.User.Event.IEvent logEvent = null;

			try
			{
				//decrypt the guid
				ICryptoManager objCM = new CryptoManager();
				ICryptoContext objContext = objCM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 1, Missing.Value);
				ICryptoKey objKey = objContext.GenerateKeyFromPassword(KEY,  ASPENCRYPTLib.CryptoAlgorithms.calgSHA, ASPENCRYPTLib.CryptoAlgorithms.calgRC4, 128);

				//Decrypt the Guid
				string encryptedGuid = Request.Params["hidEncPersGUID"];			
				ICryptoBlob objBlob = objCM.CreateBlob();
				objBlob.Hex = encryptedGuid;
				string decryptedGuid = objKey.DecryptText(objBlob);
				decryptedGuid = decryptedGuid.ToLower();
				Guid userGuid = new Guid(decryptedGuid);

				//Get the Referring Site the URL and the Domain
				string referringSite = Request.Params["hidSourceSiteCode"];
				string Url =  Request.Params["hidURL"];
				string Domain = Request.Params["hidDomain"];

				//log the params
				string logMsg = string.Format("referringSite='{0}', Url='{1}', Domain='{2}', decryptedGuid='{3}'", referringSite, Url, Domain, decryptedGuid);
				logEvent = new AICPA.Destroyer.User.Event.Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx", "Page_Load", "Page Params", logMsg);
				logEvent.Save(false);

				//get the current user from the session
				IUser retUser = (IUser)Session[SESSPARAM_CURRENTUSER];
				if(retUser == null)
				{
					//Call the constructor based on the referringSite
					if (referringSite == "C2B")
					{
						retUser = new User.User(encryptedGuid, userGuid, ReferringSite.C2b);
						retUser.LogOn(Session.SessionID);
					}
					else if (referringSite == "CSC")
					{
						retUser = new User.User(userGuid, ReferringSite.Csc);
						retUser.LogOn(Session.SessionID,Domain);
					}
					else
					{
						retUser = new User.User(userGuid,ReferringSite.Exams);
						retUser.LogOn(Session.SessionID,Domain);
					}
					//add the user back to the session
					if(retUser.UserSecurity.Authenticated)
					{
						Session[SESSPARAM_CURRENTUSER] = retUser;
					}
					else
					{
						Session[SESSPARAM_CURRENTUSER] = retUser;
						Response.Redirect(PAGE_AUTHENTICATIONFAILED, false);
					}
				}	
		
				Response.Redirect(Url, false);
			}
			catch(Exception ex)
			{
				string err = ex.Message;

				//log error
				logEvent = new AICPA.Destroyer.User.Event.Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx", "Page_Load", "Error", err);
				logEvent.Save(false);

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
	}
}
