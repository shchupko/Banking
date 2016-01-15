using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Banking.CommonTests
{
    public class MockHttpResponse : Mock<HttpResponseBase>
    {
        public MockHttpResponse(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {

        }
    }
}
