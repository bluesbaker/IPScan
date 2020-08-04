using IPScan.BLL;
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
            _scanner = new Scanner(new ScannerParameters());
        }

        [Test]
        public void GetPingReplyAsync_GoogleDNSAddress_SuccessResponse()
        {
            _scanner.Parameters.Address = IPAddress.Parse("8.8.8.8");

            var task = _scanner.GetPingReplyAsync();           
            task.Wait();

            Assert.That(task.Result.Status, Is.EqualTo(IPStatus.Success));
        }

        [Test]
        public void GetPingReplyAsync_BadAddress_TimedOutResponse()
        {
            _scanner.Parameters.Address = IPAddress.Parse("44.44.44.44");

            var task = _scanner.GetPingReplyAsync();
            task.Wait();

            Assert.That(task.Result.Status, Is.EqualTo(IPStatus.TimedOut));
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
