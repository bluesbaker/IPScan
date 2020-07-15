using IPScan;
using IPScan.Supports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class IPScanParametersTests
    {
        // Parse static method
        [Test]
        public void Parse_KeyDictionary_ParametersRequired()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["-ip"] = "8.8.8.8",
                ["-t"] = "3000",
                ["-p"] = "27015"
            };

            IPScanParameters ipScanParameters = IPScanParameters.Parse(dictionary);
            Assert.That(ipScanParameters.Address, Is.EqualTo(IPAddress.Parse("8.8.8.8")));
            Assert.That(ipScanParameters.Timeout, Is.EqualTo(3000));
            Assert.That(ipScanParameters.Port, Is.EqualTo(27015));
        }

        [Test]
        public void Parse_OnlyAddressKey_ParametersRequired()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["-ip"] = "8.8.8.8"
            };

            IPScanParameters ipScanParameters = IPScanParameters.Parse(dictionary);
            Assert.That(ipScanParameters.Address, Is.EqualTo(IPAddress.Parse("8.8.8.8")));
            Assert.That(ipScanParameters.Timeout, Is.EqualTo(1000));
            Assert.That(ipScanParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_EmptyKeyDictionary_DefaultPropertiesRequired()
        {
            var dictionary = new Dictionary<string, string>();

            IPScanParameters ipScanParameters = IPScanParameters.Parse(dictionary);
            Assert.That(ipScanParameters.Address, Is.Null);
            Assert.That(ipScanParameters.Timeout, Is.EqualTo(1000));
            Assert.That(ipScanParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_BadKey_DefaultPropertiesRequired()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["ip"] = "8.8.8.8"
            };

            IPScanParameters ipScanParameters = IPScanParameters.Parse(dictionary);
            Assert.That(ipScanParameters.Address, Is.Null);
            Assert.That(ipScanParameters.Timeout, Is.EqualTo(1000));
            Assert.That(ipScanParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_BadKeyValue_ExceptionRequired()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["-ip"] = "8.8.8.x"
            };

            var exc = Assert.Throws<IPScanException>(() => IPScanParameters.Parse(dictionary));
            Assert.That(exc, Is.Not.Null);
        }


        // CheckingRequiredKeys static method
        [Test]
        public void CheckingRequiredKeys_WithAddressKey_IsTrue()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["-ip"] = "8.8.8.8"
            };

            var isCheck = IPScanParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.True);
        }

        [Test]
        public void CheckingRequiredKeys_WithoutAddressKey_IsFalse()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["-t"] = "3000"
            };

            var isCheck = IPScanParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.False);
        }

        [Test]
        public void CheckingRequiredKeys_EmptyDictionary_IsFalse()
        {
            var dictionary = new Dictionary<string, string>();

            var isCheck = IPScanParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.False);
        }


        // GetKeySetters method
        [Test]
        public void GetKeySetters_CollectionRequired()
        {
            var keySetters = IPScanParameters.GetKeySetters();

            CollectionAssert.AllItemsAreInstancesOfType(keySetters, typeof(KeySetterAttribute));
        }

        [Test]
        public void GetKeySetters_UniqueItemsRequired()
        {
            var keySetters = IPScanParameters.GetKeySetters();

            CollectionAssert.AllItemsAreUnique(keySetters);
        }
    }
}