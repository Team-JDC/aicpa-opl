using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AICPA.Destroyer.User;
using MainUI.Shared;

namespace MainUI
{
    public partial class EthicsAddUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click1(object sender, EventArgs e)
        {
           //IUserDalc userDalc = new UserDalc();
            string errorMessage = string.Empty;
            User user = new User();
            Guid userGuid = Guid.NewGuid();
            try
            {
                user.CreateUser(userGuid, ReferringSite.EthicsUser.ToString(), UserNameTextBox.Text, PasswordTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text);
                user = new User(userGuid, ReferringSite.EthicsUser);
                user.LogOn(Session.SessionID, "et-cod");
                Session["UserId"] = user.UserId.ToString();
                Session[ContextManager.SESSION_USER] = user;                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (string.IsNullOrEmpty(errorMessage))
                Response.Redirect("~/Ethics.aspx", false);
            else ErrorLabel.Text = errorMessage;
        }
    }
}