using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace AICPA.Destroyer.User
{
    public class UserMail
    {
        /// <summary>
        /// Sends an e-mail message using the default SMTP mail server.
        /// </summary>
        /// <param name="subject">The message subject.</param>
        /// <param name="messageBody">The message body.</param>
        /// <param name="toAddress">The recipient's e-mail address.</param>
        /// <remarks>The default SMTP settings must be set in the web.config for this to work.
        ///   <system.net>
        ///    <mailSettings>
        ///      <smtp from="fromAddress@mailServer.com">
        ///        <network host="mailServer.com" port="25" userName="fromAddress" password="Password"/>
        ///      </smtp>
        ///    </mailSettings>
        /// </remarks>
        /// <example>
        /// <code>
        ///   // Send a quick e-mail message
        ///   SendEmail.SendMessage("This is a Test", 
        ///                         "This is a test message...",
        ///                         "noboday@nowhere.com",
        ///                         "somebody@somewhere.com", 
        ///                         "ccme@somewhere.com");
        /// </code>
        /// </example>
        public static void Send(string subject, string messageBody, string toAddress)
        {
            MailMessage message = new MailMessage();
            // Instantiate a new instance of SmtpClient
            SmtpClient client = new SmtpClient();

            // Set the recipient's address
            message.To.Add(new MailAddress(toAddress));

            // Set the subject and message body text
            message.Subject = subject;
            message.Body = messageBody;

            // Send the e-mail message
            client.Send(message);
        }
    }
}
