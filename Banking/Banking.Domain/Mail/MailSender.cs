using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace Banking.Domain.Mail
{
    public class MailSender
    {
        public static void SendMail(string email, string subject, string body, MailAddress mailAddress = null)
        {

            try
            {
                bool enableMail = bool.Parse(ConfigurationManager.AppSettings["EnableMail"]);
                if (enableMail)
                {
                    MailSetting mailSetting = (MailSetting)ConfigurationManager.GetSection("mailConfig");

                    if (mailAddress == null)
                    {
                        mailAddress = new MailAddress(mailSetting.SmtpReply, mailSetting.SmtpUser);
                    }
                    MailMessage message = new MailMessage(
                        mailAddress,
                        new MailAddress(email))
                    {
                        Subject = subject,
                        BodyEncoding = Encoding.UTF8,
                        Body = body,
                        IsBodyHtml = true,
                        SubjectEncoding = Encoding.UTF8
                    };
                    SmtpClient client = new SmtpClient
                    {
                        Host = mailSetting.SmtpServer,
                        Port = mailSetting.SmtpPort,
                        UseDefaultCredentials = false,
                        EnableSsl = mailSetting.EnableSsl,
                        Credentials =
                            new NetworkCredential(mailSetting.SmtpUserName,
                                mailSetting.SmtpPassword),
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    client.Send(message);
                }
                
                {
                    Logger.Log.DebugFormat("Email : {0} {1} \t Subject: {2} {3} Body: {4}", email, Environment.NewLine, subject,
                        Environment.NewLine, body);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("Mail send exception", ex.Message);
            }
        }
    }
}