using PaymentGateway.methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PaymentGateway.request
{
    public class soapv3 : IDisposable
    {
        public async Task<object> QueryParentPay(string operation, object rV = null)
        {
            using (var wrapper = new httpWrapper())
            {
                //Declaring a new content, including the request body, encoding and request content type...
                StringContent _content = new StringContent(await soap.ConstructSoapRequest(operation), Encoding.UTF8, "text/xml");

                //Adding headers to the request content
                _content.Headers.Add("Application", "MyQ X & ParentPay Payment Gateway, v1.0");
                _content.Headers.Add("Installation", applicationConfiguration.Credentials.InstallationName);
                _content.Headers.Add("SOAPAction", await soap.HandleRequestOperations(operation));

                var response = await wrapper.PostAsync("https://www.parentpay.com/P247WS/PubMethods.asmx", _content);

                rV = response.Item1;
            }

            return rV;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
