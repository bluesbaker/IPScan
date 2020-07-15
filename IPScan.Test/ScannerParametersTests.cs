using IPScan;
using IPScan.Supports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class ScannerParametersTests
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

            ScannerParameters scannerParameters = ScannerParameters.Parse(dictionary);
            Assert.That(scannerParameters.Address, Is.EqualTo(IPAddress.Parse("8.8.8.8")));
            Assert.That(scannerParameters.Timeout, Is.EqualTo(3000));
            Assert.That(scannerParameters.Port, Is.EqualTo(27015));
        }

        [Test]
        public void Parse_OnlyAddressKey_ParametersRequired()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["-ip"] = "8.8.8.8"
            };

            ScannerParameters scannerParameters = ScannerParameters.Parse(dictionary);
            Assert.That(scannerParameters.Address, Is.EqualTo(IPAddress.Parse("8.8.8.8")));
            Assert.That(scannerParameters.Timeout, Is.EqualTo(1000));
            Assert.That(scannerParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_EmptyKeyDictionary_DefaultPropertiesRequired()
        {
            var dictionary = new Dictionary<string, string>();

            ScannerParameters scannerParameters = ScannerParameters.Parse(dictionary);
            Assert.That(scannerParameters.Address, Is.Null);
            Assert.That(scannerParameters.Timeout, Is.EqualTo(1000));
            Assert.That(scannerParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_BadKey_DefaultPropertiesRequired()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["ip"] = "8.8.8.8"
            };

            ScannerParameters scannerParameters = ScannerParameters.Parse(dictionary);
            Assert.That(scannerParameters.Address, Is.Null);
            Assert.That(scannerParameters.Timeout, Is.EqualTo(1000));
            Assert.That(scannerParameters.Port, Is.EqualTo(0));
        }

        [Test]
        public void Parse_BadKeyValue_ExceptionRequired()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["-ip"] = "8.8.8.x"
            };

            var exc = Assert.Throws<ScannerException>(() => ScannerParameters.Parse(dictionary));
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

            var isCheck = ScannerParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.True);
        }

        [Test]
        public void CheckingRequiredKeys_WithoutAddressKey_IsFalse()
        {
            var dictionary = new Dictionary<string, string>()
            {
                ["-t"] = "3000"
            };

            var isCheck = ScannerParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.False);
        }

        [Test]
        public void CheckingRequiredKeys_EmptyDictionary_IsFalse()
        {
            var dictionary = new Dictionary<string, string>();

            var isCheck = ScannerParameters.CheckingRequiredKeys(dictionary);

            Assert.That(isCheck, Is.False);
        }


        // GetKeySetters method
        [Test]
        public void GetKeySetters_CollectionRequired()
        {
            var keySetters = ScannerParameters.GetKeySetters();

            CollectionAssert.AllItemsAreInstancesOfType(keySetters, typeof(KeySetterAttribute));
        }

        [Test]
        public void GetKeySetters_UniqueItemsRequired()
        {
            var keySetters = ScannerParameters.GetKeySetters();

            CollectionAssert.AllItemsAreUnique(keySetters);
        }
    }
}