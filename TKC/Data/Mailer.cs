using System;
using System.Net;
using System.Net.Mail;

namespace TKC.Data
{
	public class Mailer
	{
		public bool SendEmail(string to, string subject, string body, string from, string password, string host, int port)
		{
            try
            {
                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(from, password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from),
                        Subject = subject,
                        Body = body
                    };

                    mailMessage.To.Add(to);

                    client.Send(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
	}
}

