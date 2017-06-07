using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.Mail
{
    public interface INotifyMail
    {
        void SendNotify(string templateName, string email,
            Func<string, string> subject,
            Func<string, string> body);
    }
}
