using System;
using System.Net;
using System.Net.Mail;

namespace TKC.Data
{
	public class Mailer
	{
		public bool SendEmail(string from, string to, string subject, string body)
		{
            try
            {
                // Set up the SMTP client
                using (var client = new SmtpClient("smtp.your-email-provider.com"))
                {
                    // Specify your SMTP credentials
                    client.Credentials = new NetworkCredential(from, "your-email-password");
                    client.EnableSsl = true;

                    // Construct the email message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from),
                        Subject = subject,
                        Body = body
                    };

                    // Set the recipient email address
                    mailMessage.To.Add(to);

                    // Send the email
                    client.Send(mailMessage);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
	}
}

