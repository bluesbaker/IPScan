using IPScan.Supports;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class ParametersTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Parse_GetParams()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = start.Range(end);

            Assert.IsNotNull(range);
        } 

    }
}