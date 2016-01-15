using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Banking.CommonTests
{
    public class MockHttpCachePolicy : Mock<HttpCachePolicyBase>
    {
        public MockHttpCachePolicy(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior)
        {
            
        }
    }
}
