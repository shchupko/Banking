using System;
using log4net;
using log4net.Config;

namespace Banking
{
    public class Logger
    {
        private static readonly ILog log = LogManager.GetLogger("UI");

        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}