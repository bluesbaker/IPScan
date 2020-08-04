using IPScan.SUP;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;

namespace Tests
{
    public class IPAddressExtensionTests
    {
        [Test]
        public void Range_TwoIPAddresses_RangeRequired()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = start.Range(end);

            CollectionAssert.AllItemsAreInstancesOfType(range, typeof(IPAddress));
        }

        [Test]
        public void Range_RangeResult_UniqueItemsRequired()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = start.Range(end);

            CollectionAssert.AllItemsAreUnique(range);
        }

        [Test]
        public void Range_StartLessEnd_ExceptionRequired()
        {
            var start = IPAddress.Parse("0.0.0.1");
            var end = IPAddress.Parse("0.0.0.0");

            var exc = Assert.Throws<ArgumentException>(delegate
            {
                var range = start.Range(end);
            });

            Assert.AreEqual(exc.Message, "Beginning of the range must not be less than the end");
        }

        [Test]
        public void Range_ResultEquelMock_IsTrue()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = start.Range(end);

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