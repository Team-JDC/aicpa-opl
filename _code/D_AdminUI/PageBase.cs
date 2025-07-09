using System;
using System.Web;
using System.Web.UI;
using System.Net.Mail;
using System.Text;

namespace D_AdminUI
{
	/// <summary>
	/// Summary description for PageBase.
	/// </summary>
	public class PageBase : System.Web.UI.Page
	{

		protected override void OnError(EventArgs e)
		{
			HttpContext ctx = HttpContext.Current;
			StringBuilder errorDisplay = new StringBuilder("");
			string eStk = string.Empty;

			Exception exception = ctx.Server.GetLastError ();

			eStk = exception.StackTrace;
			eStk = eStk.Replace("\n","<br>");
			
			//loading the string builder
			errorDisplay.Append("<html>");
			errorDisplay.Append("<head>");
			errorDisplay.Append("<LINK href='styles.css' type='text/css' rel='stylesheet'>");
			errorDisplay.Append("<script src='scripts/common.js'></script>");
			errorDisplay.Append("<script>");
			errorDisplay.Append("	function getXYvalues(){");
			errorDisplay.Append("		pageLoadedFlag = true;");//alert(event.clientX);");
			errorDisplay.Append("		showmenu(5,event.clientY+20);");
			errorDisplay.Append("		return;");
			errorDisplay.Append("	}");
			errorDisplay.Append("</script>");
			errorDisplay.Append("</head>");
			errorDisplay.Append("<body>");

			errorDisplay.Append("<h1>Our apologies...</h1>");
			errorDisplay.Append("<br><p>The item you requested does not exist on this server or cannot be served at this moment. The error has been logged.</p>");
			errorDisplay.Append("<br><p style='cursor:hand' onClick='getXYvalues();'>Click here for technical details</p>");

			errorDisplay.Append("<div id='addNewFolder'>");
			errorDisplay.Append("<table cellSpacing='0' cellPadding='0' width='100%' border='0'>");
			errorDisplay.Append("<tr><td class='tree_menu' noWrap>Error Message:<br>");
			errorDisplay.Append(exception.Message);
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td>");
			errorDisplay.Append("<hr color='#cfe6f2' SIZE='1'>");
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td class='tree_menu' noWrap>Error Source:<br>");
			errorDisplay.Append(exception.Source);
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td>");
			errorDisplay.Append("<hr color='#cfe6f2' SIZE='1'>");
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td class='tree_menu' noWrap>Error Stack Trace:<br>");
			errorDisplay.Append(eStk);
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td>");
			errorDisplay.Append("<hr color='#cfe6f2' SIZE='1'>");
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td class='tree_menu' noWrap>Error URL:<br>");
			errorDisplay.Append(ctx.Request.Url.ToString());
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td>");
			errorDisplay.Append("<hr color='#cfe6f2' SIZE='1'>");
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("<tr><td align='right'>");
			errorDisplay.Append("<input class='treeView_button' onclick='hidemenu();' type='button' value='Close'>");
			errorDisplay.Append("</td></tr>");
			errorDisplay.Append("</table>");
			errorDisplay.Append("</div>");
			errorDisplay.Append("</html>");
			//------------------------------

			ctx.Response.Write(errorDisplay.ToString());
			ctx.Server.ClearError ();
			//this.sendEmail();
			base.OnError (e);

		}

		private void sendEmail()
		{
            MailMessage msg = new MailMessage();
            MailAddress from = new MailAddress("wdalton@knowlysis.com");
            MailAddress to = new MailAddress("wdalton@knowlysis.com");
			msg.Subject = "Error Msg";
			msg.Body = "There was an error";

			try 
			{
                SmtpClient client = new SmtpClient("mail.knowlysis.com");
                client.Send(msg);
			} 
			catch (HttpException ex) 
			{
				Response.Write("HTTP Error: " + ex.ToString());
			} 
			catch (Exception ex) 
			{
				Response.Write("Error: " + ex.ToString());
			} 			
		}
	}
}
