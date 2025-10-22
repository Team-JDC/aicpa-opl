using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AICPA.Destroyer.User;

namespace MainUI
{
    public partial class SendPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendPassword_Click(object sender, EventArgs e)
        {
            IUser user = new User();
            
            string returnMessage = user.SendUserPassword(UserNameTextBox.Text);

            if (returnMessage != "")
            {
                ErrorLabel.Text = returnMessage;
            }
            else
            {
                ErrorLabel.Text = "Your current password has been sent to the email address. Return to <a href='EthicsLogin.aspx'>Login</a>.";
            }
        }
    }
}