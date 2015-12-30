using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Banking.Domain.Mail
{
    public class NotifyMail : INotifyMail
    {
        public IQueryable<MailTemplate> GetMailTemplates()
        {
            MailTemplateConfigSection configInfo = (MailTemplateConfigSection)ConfigurationManager.GetSection("mailTemplateConfig");
            return configInfo.MailTemplates.OfType<MailTemplate>().AsQueryable<MailTemplate>();
        }

        public void SendNotify(string templateName, string email,
            Func<string, string> subject,
            Func<string, string> body)
        {
            var template = GetMailTemplates().FirstOrDefault(p => string.Compare(p.Name, templateName, true) == 0);
            if (template == null)
            {
                Logger.Log.ErrorFormat("Can't find template (" + templateName + ")");
            }
            else
            {
                MailSender.SendMail(email,
                    subject.Invoke(template.Subject),
                    body.Invoke(template.Template));
            }
        }
    }
}
