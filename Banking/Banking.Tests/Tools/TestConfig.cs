using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.UnitTests.Tools
{
    class TestConfig 
    {
        private Configuration configuration;

        public TestConfig(string configPath)
        {
            var configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configPath;
            configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }


        public string ConnectionStrings(string connectionString)
        {
            return configuration.ConnectionStrings.ConnectionStrings[connectionString].ConnectionString;
        }

        public string Lang
        {
            get { return configuration.AppSettings.Settings["Lang"].Value; }
        }

        public bool EnableMail
        {
            get { return bool.Parse(configuration.AppSettings.Settings["EnableMail"].Value); }
        }

        //public IQueryable<IconSize> IconSizes
        //{
        //    get
        //    {
        //        IconSizesConfigSection configInfo = (IconSizesConfigSection) configuration.GetSection("iconConfig");
        //        if (configInfo != null)
        //        {
        //            return configInfo.IconSizes.OfType<IconSize>().AsQueryable<IconSize>();
        //        }
        //        return null;
        //    }
        //}

        //public IQueryable<MimeType> MimeTypes
        //{
        //    get
        //    {
        //        MimeTypesConfigSection configInfo = (MimeTypesConfigSection) configuration.GetSection("mimeConfig");
        //        return configInfo.MimeTypes.OfType<MimeType>().AsQueryable<MimeType>();
        //    }
        //}

        //public IQueryable<MailTemplate> MailTemplates
        //{
        //    get
        //    {
        //        MailTemplateConfigSection configInfo =
        //            (MailTemplateConfigSection) configuration.GetSection("mailTemplatesConfig");
        //        return configInfo.MailTemplates.OfType<MailTemplate>().AsQueryable<MailTemplate>();
        //    }
        //}

        //public MailSetting MailSetting
        //{
        //    get { return (MailSetting) configuration.GetSection("mailConfig"); }
        //}
    }
}