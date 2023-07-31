using PaymentGateway.methods;
using PaymentGateway.request.response;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.request
{
    public class soapv3 : IDisposable
    {
        public async Task<object> QueryParentPay(string operation, object returnValue = null)
        {
            xmlUtility.HandleSimplePaymentReportResult handleSimplePaymentReportResult;
            xmlUtility.HandleMessageUpdateRequestResult handleMessageUpdateRequestResult;

            using (var wrapper = new httpWrapper())
            {
                //Declaring a new content, including the request body, encoding and request content type...
                StringContent _content = new StringContent(await soap.ConstructSoapRequest(operation), Encoding.UTF8, "text/xml");

                //Adding headers to the request content
                _content.Headers.Add("Application", "MyQ X & ParentPay Payment Gateway, v1.0");
                _content.Headers.Add("Installation", applicationConfiguration.Credentials.InstallationName);
                _content.Headers.Add("SOAPAction", await soap.HandleRequestOperations(operation));

                var response = await wrapper.PostAsync("https://www.parentpay.com/P247WS/PubMethods.asmx", _content);

                using (var Soap = new soap())
                    returnValue = Soap.handleResponse(response.Item1, operation).Result;
            }

            return returnValue;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
