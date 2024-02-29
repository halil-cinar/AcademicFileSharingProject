using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.ExtensionsMethods
{
    public class MailSender
    {
       

        private readonly string smtpServer;
        private readonly int    smtpPort;
        private readonly string smtpUsername;
        private readonly string smtpPassword;
        private readonly string smtpDisplayName;
        private readonly bool   smtpEnableSsl;
        private readonly string smtpDisplayAddress;   

        

        public MailSender(SmtpValues values)
        {
            this.smtpServer = values.SmtpServer;
            this.smtpPort = values.SmtpPort;
            this.smtpUsername = values.SmtpUsername;
            this.smtpPassword = values.SmtpPassword;
            this.smtpDisplayName=values.SmtpDisplayName;
            this.smtpEnableSsl =values.SmtpEnableSsl;
            this.smtpDisplayAddress = values.SmtpDisplayAddress;
        }


        public bool SendEmail( string subject, string body, bool isHtml = false, params string[] to)
        {
            try
            {

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@halilcinar.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml,
                };
                foreach (var item in to)
                {
                    mailMessage.To.Add(item);
                }
                

                SmtpClient smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpDisplayName, smtpPassword),
                    EnableSsl = smtpEnableSsl,
                };


                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("E-posta gönderirken bir hata oluştu: " + ex.Message);
                return false;
            }
        }
    }
}
