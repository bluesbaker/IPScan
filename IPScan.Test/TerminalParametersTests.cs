using IPScan;
using IPScan.Supports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Tests
{
    public class TerminalParametersTests
    {
        // Parse
        [Test]
        public void Parse_ArgsArray_ParametersRequired()
        {
            var arguments = new[] { "-ip", "8.8.8.8", "-t", "1000", "-p", "27015" };

            var parameters = TerminalParameters.Parse(arguments);

            Assert.That(parameters["-ip"], Is.EqualTo("8.8.8.8"));
            Assert.That(parameters["-t"], Is.EqualTo("1000"));
            Assert.That(parameters["-p"], Is.EqualTo("27015"));
        }

        [Test]
        public void Parse_WithoutFirstKey_BadParametersRequired()
        {
            var arguments = new[] { "8.8.8.8", "-t", "1000", "-p", "27015" };

            var parameters = TerminalParameters.Parse(arguments);

            Assert.That(parameters.ContainsKey("ip"), Is.False);
            Assert.That(parameters["-t"], Is.EqualTo("1000"));
            Assert.That(parameters["-p"], Is.EqualTo("27015"));
        }

        [Test]
        public void Parse_WithoutFirstKeyAndWithDefaultKey_ParametersRequired()
        {
            var arguments = new[] { "8.8.8.8", "-t", "1000", "-p", "27015" };

            var parameters = TerminalParameters.Parse(arguments, "-ip");

            Assert.That(parameters["-ip"], Is.EqualTo("8.8.8.8"));
            Assert.That(parameters["-t"], Is.EqualTo("1000"));
            Assert.That(parameters["-p"], Is.EqualTo("27015"));
        }

        [Test]
        public void Parse_WithDefaultValue_ParametersRequired()
        {
            var arguments = new[] { "-ip", "8.8.8.8", "-t", "-p" };

            var parameters = TerminalParameters.Parse(arguments, defaultValue: "github");

            Assert.That(parameters["-ip"], Is.EqualTo("8.8.8.8"));
            Assert.That(parameters["-t"], Is.EqualTo("github"));
            Assert.That(parameters["-p"], Is.EqualTo("github"));
        }


        // Copy
        [Test]
        public void Clone_ParametersRequired()
        {
            var arguments = new[] { "-ip", "8.8.8.8", "-t", "1000", "-p", "27015" };

            var parameters = TerminalParameters.Parse(arguments);
            var copy = parameters.Copy();

            Assert.That(copy["-ip"], Is.EqualTo("8.8.8.8"));
            Assert.That(copy["-t"], Is.EqualTo("1000"));
            Assert.That(copy["-p"], Is.EqualTo("27015"));
        }

        [Test]
        public void Copy_AddressInjection_ParametersWithNewAddressRequired()
        {
            var arguments = new[] { "-ip", "8.8.8.8", "-t", "1000", "-p", "27015" };
            var addressInjection = new Dictionary<string, string>() { ["-ip"] = "7.7.7.7" };

            var parameters = TerminalParameters.Parse(arguments);
            var copy = parameters.Copy(addressInjection);

            Assert.That(copy["-ip"], Is.EqualTo("7.7.7.7"));
            Assert.That(copy["-t"], Is.EqualTo("1000"));
            Assert.That(copy["-p"], Is.EqualTo("27015"));
        }
    }
}
