﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Banking.CommonTests
{
    public class MockHttpBrowserCapabilities : Mock<HttpBrowserCapabilitiesBase>
    {
        public MockHttpBrowserCapabilities(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior)
        {
            
        }
    }
}
