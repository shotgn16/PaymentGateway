using Gateway.Logger;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using PaymentGateway.exceptions;

namespace PaymentGateway.methods
{
    internal class securityManagement
    {
        public static string address;

        internal static SslProtocols tlsVersion()
        {
            MyLogger.GetInstance().Debug("SecurityProtocolAssignment creating hander");

            var returnValue = SslProtocols.Default;

            MyLogger.GetInstance().Debug("SecurityProtocolAssignment reading protocol configurations");

            switch (applicationConfiguration.Credentials.TLSVersion)
            {
                case 1.0M: returnValue = SslProtocols.Tls; myqConfiguration.MyQ.TLS = "TLS 1.0"; break;
                case 1.1M: returnValue = SslProtocols.Tls11; myqConfiguration.MyQ.TLS = "TLS 1.1"; break;
                case 1.2M: returnValue = SslProtocols.Tls12; myqConfiguration.MyQ.TLS = "TLS 1.2"; break;
                case 1.3M: returnValue = SslProtocols.Tls13; myqConfiguration.MyQ.TLS = "TLS 1.3"; break;
                default: returnValue = SslProtocols.Tls12; myqConfiguration.MyQ.TLS = "TLS 1.2"; break;
            }

            MyLogger.GetInstance().Debug("SecurityProtocolAssignment assigning security protocols");

            return returnValue;
        }

        internal static async Task<string> GetDomain(string Input)
        {
            string[] hostParts = new System.Uri(Input).Host.Split('.');
            string domain = string.Join(".", hostParts.Skip(Math.Max(0, hostParts.Length - 2)).Take(2));

            return Task.FromResult(domain).Result;
        }

        public static async Task<bool> WhitelistAddress(string address, bool trustedAddress = false)
        {
            try
            {
                if (address.Contains(".com"))
                    address = await GetDomain(address);
                

                address = networkManagement.HostToIp(address).Result;

                if (applicationConfiguration.Credentials.whitelistedAddresses.Contains(address))
                    trustedAddress = true;
            }

            catch (Exception ex){
                var exception = new DnsException(ex.Message);
                MyLogger.GetInstance().Error("Error: " + exception, ex);
            }

            return Task.FromResult(trustedAddress).Result;
        }
    }
}
