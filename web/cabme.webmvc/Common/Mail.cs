using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;

namespace cabme.webmvc.Common
{
    public class Mail
    {
        public static void SendMail(string to, string from, string subject, string body)
        {
            Task t = new Task(() =>
            {
                try
                {
                    ///Smtp config
                    string smtp = ConfigurationManager.AppSettings["Smtp"];
                    string user = ConfigurationManager.AppSettings["EmailUser"];
                    string pwd = ConfigurationManager.AppSettings["EmailPassword"];
                    SmtpClient client = new SmtpClient(smtp);
                    client.Credentials = new NetworkCredential(user, pwd);
                    client.EnableSsl = false;

                    ///mail details
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress(from);
                    msg.To.Add(to);
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.Body = body;
                    msg.Priority = MailPriority.Normal;
                    client.Send(msg);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                }
            });
            t.Start();
        }
    }
}