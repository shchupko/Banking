
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;


namespace Banking.UnitTests.Mock
{
    //public class MockMailSender : Mock<IMailSender> 
    //{
    //    public MockMailSender(MockBehavior mockBehavior = MockBehavior.Strict)
    //        : base(mockBehavior)
    //    {
    //        this.Setup(p => p.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MailAddress>()))
    //            .Callback((string email, string subject, string body, MailAddress address) =>
    //            Console.WriteLine(String.Format("Send mock email to: {0}, subject {1}", email, subject)));
    //    }
    //}
}
