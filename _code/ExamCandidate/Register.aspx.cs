using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ExamCandidate
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public partial class Register : common
	{


	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			
			// Put user code to initialize the page here
			if (Page.IsPostBack)
			{
				//The user wants to register
				// see if there is a GUID on the URL
				string examGuid  = Request.Params["id"];
				Guid userGuid;
				bool registerUser = true;
				string errorMsg = string.Empty;
				//Validate the information.
				Regex Pattern = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
				string sUserName = UserName.Value;
                //make sure that the username uses valid characters
				Match un = Pattern.Match(sUserName);

				if (un.Value == string.Empty)  
				{
					if((sUserName.Length < 6) || (sUserName.Length > 20))
					{
						UserName.Value = "";
						errorMsg = "The User Name should be 6 - 20 characters, Please enter another User Name";
						registerUser = false;
					}
					else
					{
						string sPassword = Password.Value;
						Match pwd = Pattern.Match(sPassword);
						if (pwd.Value == string.Empty)
						{
							if((sPassword.Length < 6) || (sPassword.Length > 12))
							{
								Password.Value = "";
								errorMsg = "The Password should be 6 - 12 characters, Please enter another Password";
								registerUser = false;
							}
							else 
							{
								if (Password.Value != Passwordcfrm.Value)
								{ 
									errorMsg = "The two passwords that were entered did not match, Please try again";
									Password.Value = "";
									Passwordcfrm.Value = "";
									registerUser = false;
								}
								else
								{
									if (!agree.Checked)
									{
										errorMsg = "You must accept the License Agreement before continuing";
										registerUser = false;
									}
								}

							}

						}
						else 
						{
							Password.Value = "";
							errorMsg = "The password is Not valid, Please try again";
							registerUser = false;
						}
					}
				}
				else 
				{
					UserName.Value = "";
					errorMsg = "The password is Not valid, Please try again";
					registerUser = false;
				}
								
				
				
				if (registerUser)
				{
					if ((examGuid != null) && (examGuid.Length > 1))
					{
						userGuid = new Guid(examGuid);
					}
					else 
					{
						HttpCookie c_Guid = Request.Cookies.Get("Guid");
						examGuid = c_Guid.Value;
						userGuid = new Guid(examGuid);
					}
				

					//Get current status
					bool inserted = this.registerUser(userGuid,UserName.Value,Password.Value);

					if (inserted)
					{
						jslabel.Text = "<script>window.parent.changeContent(8);</script>";
						jslabel.Visible = true;
					}
				}
				else
				{
					string alertError = "<script language='javascript'>alert('" + errorMsg +" ');</script>";
				
					Page.RegisterStartupScript("ErrorAlert", alertError);
				}
				
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
			this.registerBtn.Click += new System.Web.UI.ImageClickEventHandler(this.registerBtn_Click);

		}
		#endregion

		protected void registerBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Guid guid = new Guid("8D603F4B-4C7C-4B84-840A-D2AFFF3C0575");
			string userName = "damorales";
			string password = "david";
			//string examGuid  = Request.Params["id"];

			bool inserted = this.registerUser(guid,userName,password);

			string password2 = this.getPassword(guid);
			int status = this.getCurrentStatus(guid);
			int status2 = this.getCurrentStatus(userName,password);
		}
	}
}
