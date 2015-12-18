using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonProject.Model;

using Moq;

namespace LessonProject.UnitTest.Mock
{
    public partial class MockRepository
    {
        public List<Language> Languages { get; set; }


        public void GenerateLanguages()
        {
            Languages = new List<Language>();
            Languages.Add(new Language()
            {
                ID = 1,
                Code = "en",
                Name = "English"
            });
            Languages.Add(new Language()
            {
                ID = 2,
                Code = "ru",
                Name = "Русский"
            });
            this.Setup(p => p.Languages).Returns(Languages.AsQueryable());
        }
    }
}
