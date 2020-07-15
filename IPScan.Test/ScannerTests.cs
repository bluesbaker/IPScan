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
    public class ScannerTests
    {
        private Scanner _scanner;

        [SetUp]
        public void SetUp()
        {
            var scannerParameters = new ScannerParameters()
            {
                Address = IPAddress.Parse("8.8.8.8")
            };
            _scanner = new Scanner(scannerParameters);
        }

        [Test]
        public void GetPingReplyAsync_GoogleAddress_SuccessResponse()
        {
            var task = _scanner.GetPingReplyAsync();
            
            task.Wait();
            Assert.That(task.Result.Status, Is.EqualTo(IPStatus.Success));
        }

        [Test]
        public void GetPingReplyAsync_AddressIsNull_ExceptionRequired()
        {
            _scanner.Parameters.Address = null;
            var exc = Assert.Throws<System.AggregateException>(delegate
            {
                Task<PingReply> reply = _scanner.GetPingReplyAsync();
                reply.Wait();
            });
            
            Assert.That(exc.InnerException, Is.TypeOf<ScannerException>());
        }
    }
}
