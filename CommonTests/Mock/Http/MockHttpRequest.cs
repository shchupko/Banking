﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Banking.CommonTests
{
    public class MockHttpRequest : Mock<HttpRequestBase>
    {
        public MockHttpRequest(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior)
        {
            this.Setup(r => r.HttpMethod).Returns("GET");
        }
    }
}
