using crypto;
using PaymentGateway.methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.request
{
    internal class customClient
    {
        public static HttpClientHandler _handler = new HttpClientHandler();
        public static HttpClient _client = new HttpClient(_handler)
        { Timeout = TimeSpan.FromSeconds(10) };

        public static async Task ConfigureHandler(string _endpoint)
        {
            if (_endpoint == "api/v3/certificates/root")
            {
                _handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }
            else
            {
                _handler.SslProtocols = securityManagement.tlsVersion();
            }
        }
    }
}
