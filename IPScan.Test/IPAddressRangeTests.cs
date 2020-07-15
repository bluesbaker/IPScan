using IPScan.Supports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class IPAddressRangeTests
    {
        [Test]
        public void Get_TwoIPAddresses_RangeRequired()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = IPAddressRange.Get(start, end);

            CollectionAssert.AllItemsAreInstancesOfType(range, typeof(IPAddress));
        }

        [Test]
        public void Get_TwoIPStrings_RangeRequired()
        {
            var start = "0.0.0.0";
            var end = "0.0.0.5";

            var range = IPAddressRange.Get(start, end);

            CollectionAssert.AllItemsAreInstancesOfType(range, typeof(IPAddress));
        }

        [Test]
        public void Get_BadIPString_ExceptionRequired()
        {
            var start = "0.0.0.x";
            var end = "0.0.0.5";

            var exc = Assert.Throws<FormatException>(() => IPAddressRange.Get(start, end));

            Assert.IsNotEmpty(exc.Message);
        }

        [Test]
        public void Get_RangeResult_UniqueItemsRequired()
        {
            var start = "0.0.0.0";
            var end = "0.0.0.5";

            var range = IPAddressRange.Get(start, end);

            CollectionAssert.AllItemsAreUnique(range);
        }

        [Test]
        public void Get_StartLessEnd_ExceptionRequired()
        {
            var start = IPAddress.Parse("0.0.0.1");
            var end = IPAddress.Parse("0.0.0.0");

            var exc = Assert.Throws<ArgumentException>(delegate
            {
                var range = IPAddressRange.Get(start, end);
            });

            Assert.AreEqual(exc.Message, "Beginning of the range must not be less than the end");
        }

        [Test]
        public void Get_ResultEquelMock_IsTrue()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = IPAddressRange.Get(start, end);

            var mockRange = new List<IPAddress>
            {
                IPAddress.Parse("0.0.0.0"),
                IPAddress.Parse("0.0.0.1"),
                IPAddress.Parse("0.0.0.2"),
                IPAddress.Parse("0.0.0.3"),
                IPAddress.Parse("0.0.0.4"),
                IPAddress.Parse("0.0.0.5")
            };

            CollectionAssert.AreEqual(range, mockRange);
        }
        
    }
}