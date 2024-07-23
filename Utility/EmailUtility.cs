using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;

namespace GowBoard.Utility
{
    public static class EmailUtility
    {
        public static SmtpClient GetSmtpClient()
        {
            var smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
            var host = smtpSection.Network.Host;
            var port = smtpSection.Network.Port;
            var userName = smtpSection.Network.UserName;
            var password = smtpSection.Network.Password;

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };

            return client;
        }

        public static bool SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = GetSmtpClient(); 
            var fromEmail = ((SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp")).From;

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(toEmail);

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }catch(Exception ex)
            {
                return false;
            } 
            
        }
    }
}