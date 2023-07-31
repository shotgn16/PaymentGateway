using Gateway.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.methods
{
    internal class securityManagement
    {
        internal static SslProtocols tlsVersion()
        {
            MyLogger.GetInstance().Debug("SecurityProtocolAssignment creating hander");

            var returnValue = SslProtocols.Default;

            MyLogger.GetInstance().Debug("SecurityProtocolAssignment reading protocol configurations");

            switch (applicationConfiguration.Credentials.TLSVersion) {
                case 1.0M: returnValue = SslProtocols.Tls; internalConfig.internalConfiguration.TLS = "TLS 1.0"; break;
                case 1.1M: returnValue = SslProtocols.Tls11; internalConfig.internalConfiguration.TLS = "TLS 1.1"; break;
                case 1.2M: returnValue = SslProtocols.Tls12; internalConfig.internalConfiguration.TLS = "TLS 1.2"; break;
                case 1.3M: returnValue = SslProtocols.Tls13; internalConfig.internalConfiguration.TLS = "TLS 1.3"; break;
                default: returnValue = SslProtocols.Tls12; internalConfig.internalConfiguration.TLS = "TLS 1.2"; break;
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
                if (applicationConfiguration.Credentials.whitelistedAddresses.Contains(address))
                    trustedAddress = true;

                else if (address.Contains(".com") || address.Contains(".co.uk")) {
                    address = await GetDomain(address);
                }

                else if (address.Contains(".local")) {
                    address = networkManagement.HostToIp(address).Result;

                    if (applicationConfiguration.Credentials.whitelistedAddresses.Contains(address))
                        trustedAddress = true;
                }
            }

            catch (Exception ex){
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                //Disposing
                networkManagement.Dispose();
            }

            //Disposing
            networkManagement.Dispose();

            return Task.FromResult(trustedAddress).Result;
        }
    }
}
