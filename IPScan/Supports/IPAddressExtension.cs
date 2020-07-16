﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Supports
{
    public static class IPAddressExtension
    {
        public static IEnumerable<IPAddress> Range(this IPAddress startAddress, IPAddress endAddress)
        {
            var result = new Collection<IPAddress>();

            // convert IPAdresses to bytes
            var fromAsBytes = startAddress.GetAddressBytes();
            var toAsBytes = endAddress.GetAddressBytes();

            // reverse the from-to bytes 
            // for a correct calculate of range
            Array.Reverse(fromAsBytes);
            Array.Reverse(toAsBytes);

            // convert bytes to int
            var fromAsInt = BitConverter.ToInt32(fromAsBytes, 0);
            var toAsInt = BitConverter.ToInt32(toAsBytes, 0);

            if (fromAsInt > toAsInt)
            {
                throw new ArgumentException($"Beginning of the range must not be less than the end");
            }

            // calculate the range
            for (int ipAsInt = fromAsInt; ipAsInt <= toAsInt; ipAsInt++)
            {
                var ipAsBytes = BitConverter.GetBytes(ipAsInt);
                // more reverse... more!
                Array.Reverse(ipAsBytes);
                var ip = new IPAddress(ipAsBytes);
                result.Add(ip);
            }

            return result;
        }

        public static IEnumerable<IPAddress> Range(this IPAddress startAddress, string endAddress)
        {
            var end = IPAddress.Parse(endAddress);
            return Range(startAddress, end);
        }
    }
}