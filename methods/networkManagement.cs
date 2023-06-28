using Gateway.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.methods
{
    internal class networkManagement
    {
        public static async Task<string> HostToIp(string address, string returnValue = "")
        {
            try
            {
                MyLogger.GetInstance().Debug("NetworkConversion verifing host type");

                if (address.Contains("-") || address.Contains(".com") || address.Contains(".co.uk"))
                {
                    MyLogger.GetInstance().Debug("NetworkConversion getting IP address");

                    IPHostEntry ip = await Dns.GetHostEntryAsync(address);
                    address = ip.AddressList[0].ToString();

                    MyLogger.GetInstance().Debug("NetworkConversion returning host type");

                    returnValue = Task.FromResult(address).Result;
                }

                else if (address.Contains("."))
                {
                    returnValue = Task.FromResult(address).Result;
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return returnValue;
        }

        public static void Dispose()
        {
            GC.Collect();
        }
    }
}
