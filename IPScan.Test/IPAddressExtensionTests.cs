using IPScan.Supports;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class IPAddressExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Range_GetList_IsNotNull()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var range = start.Range(end);

            Assert.IsNotNull(range);
        }

        [Test]
        public void Range_GetList_UniqueItemsRequired()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.1.0");

            var range = start.Range(end);

            CollectionAssert.AllItemsAreUnique(range);
        }

        [Test]
        public void Range_AreEqualMock_True()
        {
            var start = IPAddress.Parse("0.0.0.0");
            var end = IPAddress.Parse("0.0.0.5");

            var rangeResult = start.Range(end);
            var requiredResult = new List<IPAddress>
            {
                IPAddress.Parse("0.0.0.0"),
                IPAddress.Parse("0.0.0.1"),
                IPAddress.Parse("0.0.0.2"),
                IPAddress.Parse("0.0.0.3"),
                IPAddress.Parse("0.0.0.4"),
                IPAddress.Parse("0.0.0.5")
            };

            CollectionAssert.AreEqual(rangeResult, requiredResult);
        }
        
    }
}