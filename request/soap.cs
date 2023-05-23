using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Gateway.Logger;
using PaymentGateway.methods;
using PaymentGateway.methods.timers;
using PaymentGateway.request.response;
using PaymentGateway.exceptions;

namespace PaymentGateway.request
{
    public class soap
    {
        internal static async Task<object> httpSoap(string op, object returnValue = null)
        {
            try
            {
                MyLogger.GetInstance().Debug("Preparing new request, {0}", op);
                var soapString = await ConstructSoapRequest(op);

                MyLogger.GetInstance().Debug("Assigning security protocols and request headers...");

                var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
                content.Headers.Add("Application", "MyQ X & ParentPay Payment Gateway, v1.0");
                content.Headers.Add("Installation", applicationConfiguration.Credentials.InstallationName);

                MyLogger.GetInstance().Debug("Setting SOAPAction...");

                if (op == "handleSimplePaymentReport")
                {
                    content.Headers.Add("SOAPAction", "http://www.pay24-7.com/P247WS/PubMethods/handleSimplePaymentReport");

                    bool whitelistedHost = await securityManagement.WhitelistAddress(parentpayConfiguration.ParentPay.handleSimplePaymentReport);
                    if (whitelistedHost == false)
                    {
                        var address = parentpayConfiguration.ParentPay.handleSimplePaymentReport;
                        if (address.Contains(".com")) { address = await securityManagement.GetDomain(address); } address = networkManagement.HostToIp(address).Result;

                        MyLogger.GetInstance().Error("Error 404: Address: [" + address + "] not whitelisted!");
                    }
                }

                else if (op == "handleMessageUpdateRequest")
                {
                    content.Headers.Add("SOAPAction",
                        "http://www.pay24-7.com/P247WS/PubMethods/handleMessageUpdateRequest");

                    MyLogger.GetInstance().Debug("Verifying address whitelisted");

                    bool whitelistedHost = await securityManagement.WhitelistAddress(parentpayConfiguration.ParentPay.handleMessageUpdateRequest);
                    if (whitelistedHost == false)
                    {
                        var address = parentpayConfiguration.ParentPay.handleMessageUpdateRequest;
                        if (address.Contains(".com")) { address = await securityManagement.GetDomain(address); } address = networkManagement.HostToIp(address).Result;

                        MyLogger.GetInstance().Error("Error 404: Address: [" + address + "] not whitelisted!");
                    }
                }

                MyLogger.GetInstance().Debug("Sending request...");

                using (var response = await customClient._client.PostAsync("https://www.parentpay.com/P247WS/PubMethods.asmx", content))
                {
                    MyLogger.GetInstance().Debug("Handling API response");
                    returnValue = await handleResponse(response, op);
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Errror: " + ex.ToString());
            }

            return returnValue;
        }


        private static async Task<object> handleResponse(HttpResponseMessage response, string operation, object returnVariable = null)
        {
            try
            {
                var r = await parseResponse(response.Content.ReadAsStringAsync().Result, operation);

                if (operation == "handleSimplePaymentReport")
                {
                    returnVariable = DeserializePaymentReport(r);
                }

                else if (operation == "handleMessageUpdateRequest")
                {
                    returnVariable = DeserializeMessageUpdate(r);
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return Task.FromResult(returnVariable).Result;
        }

        public static async Task<string> tillBalanceString(xmlUtility.HandleSimplePaymentReportResult paymentReport, string returnValue = null)
        {
            try
            {
                foreach (xmlUtility.PaymentVO User in paymentReport.PaymentArray)
                {
                    await restUtility.rootParse(await flurlRest.getUserInfo(User.ConsumerName, User.Identifier.ToString()).Result.ResponseMessage.Content.ReadAsStringAsync());

                    myqConfiguration.MyQ.UserNewBalance = await data.db.getLatestTillBalance(await data.DatabaseHash.dbEncrypt(User.Identifier.ToString(), 1), await data.DatabaseHash.dbEncrypt(myqConfiguration.MyQ.UserID, 0));

                    returnValue = returnValue + "<string>" + "Print Credits: £" + myqConfiguration.MyQ.UserNewBalance + "</string>";
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return returnValue;
        }

        private static async Task<string> IdentifierString(string returnValue = null)
        {
            try
            {
                foreach (xmlUtility.PaymentVO User in updateParentPay.xml.PaymentArray)
                {
                    returnValue = returnValue + "<string>" + User.Identifier + "</string>";
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return returnValue;
        }

        private static async Task<string> ConstructSoapRequest(string op)
        {
            MyLogger.GetInstance().Debug("Creating request body");

            string returnValue = "";

            switch (op)
            {
                case "handleSimplePaymentReport":
                    returnValue = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"">
                    <soap:Body>
                    <handleSimplePaymentReport xmlns=""http://www.pay24-7.com/P247WS/PubMethods"">
                    <orgIdentifier>" + applicationConfiguration.Credentials.OrgIdentifier + @"</orgIdentifier>
                    <username>" + applicationConfiguration.Credentials.Username + @"</username>
                    <password>" + applicationConfiguration.Credentials.Password + @"</password>
                    <service>" + applicationConfiguration.Credentials.ServiceID + @"</service>
                    <minPaymentId>" + 0 + @"</minPaymentId>
                    <maxPaymentId>" + -1 + @"</maxPaymentId>
                    </handleSimplePaymentReport>
                    </soap:Body>
                    </soap:Envelope>");
                    break;

                case "handleMessageUpdateRequest":
                    returnValue = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>
                    <handleMessageUpdateRequest xmlns=""http://www.pay24-7.com/P247WS/PubMethods"">
                    <req>
                    <OrgIdentifier>" + applicationConfiguration.Credentials.OrgIdentifier + @"</OrgIdentifier>
                    <Username>" + applicationConfiguration.Credentials.Username + @"</Username>
                    <Password>" + applicationConfiguration.Credentials.Password + @"</Password>
                    <Messages>" + await tillBalanceString(updateParentPay.xml) + @"</Messages>
                    <UserIdentifiers>" + await IdentifierString() + @"</UserIdentifiers>
                    </req>
                    </handleMessageUpdateRequest>
                    </soap:Body>
                    </soap:Envelope>");

                    break;
            }

            return Task.FromResult(returnValue).Result;
        }

        public static Task<string> parseResponse(string response, string op, string returnValue = "")
        {
            MyLogger.GetInstance().Debug("Trimming response data...");

            try
            {
                if (op == "handleSimplePaymentReport")
                {
                    var myXDocument = XDocument.Load(new StringReader(response));
                    var xns = XNamespace.Get("http://www.pay24-7.com/P247WS/PubMethods");
                    var soapBody = myXDocument.Descendants(xns + "handleSimplePaymentReportResult").First().ToString();

                    returnValue = soapBody;
                }

                else if (op == "handleMessageUpdateRequest")
                {
                    var myXDocument = XDocument.Load(new StringReader(response));
                    var xns = XNamespace.Get("http://www.pay24-7.com/P247WS/PubMethods");
                    var soapBody = myXDocument.Descendants(xns + "handleMessageUpdateRequestResult").First().ToString();

                    returnValue = soapBody;
                }
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", ex);
            }
            return Task.FromResult(returnValue);
        }

        public static xmlUtility.HandleSimplePaymentReportResult DeserializePaymentReport(string response, xmlUtility.HandleSimplePaymentReportResult result = null)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating serializer");

                XmlSerializer Serializer = new XmlSerializer(typeof(xmlUtility.HandleSimplePaymentReportResult));

                MyLogger.GetInstance().Debug("Creating string reader...");

                using (StringReader r = new StringReader(response))
                {
                    MyLogger.GetInstance().Debug("Deserializing...");

                    result = (xmlUtility.HandleSimplePaymentReportResult)Serializer.Deserialize(r);

                    MyLogger.GetInstance().Debug("Checking status...");

                    if (result.SuccessState == 0)
                    {
                        MyLogger.GetInstance().Info("Status Ok, NumRecords {0}", result.NumRecords);
                    }

                    else if (result.SuccessState == 1 || result.SuccessState == 2)
                    {
                        var exception = new RequestException("Failed to download payment report. Will try again soon...");
                        MyLogger.GetInstance().Warning("Warning: " + exception.Message, exception);
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return result;
        }

        public static xmlUtility.HandleMessageUpdateRequestResult DeserializeMessageUpdate(string response, xmlUtility.HandleMessageUpdateRequestResult result = null)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating serializer");

                XmlSerializer Serializer = new XmlSerializer(typeof(xmlUtility.HandleMessageUpdateRequestResult));

                MyLogger.GetInstance().Debug("Creating string reader...");

                using (StringReader r = new StringReader(response))
                {
                    MyLogger.GetInstance().Debug("Deserializing...");

                    result = (xmlUtility.HandleMessageUpdateRequestResult)Serializer.Deserialize(r);

                    MyLogger.GetInstance().Debug("Checking status...");

                    if (result.SuccessState == 0)
                    {
                        MyLogger.GetInstance().Info("Status Ok, UsersModified {0}", result.UsersModified);
                    }

                    else if (result.SuccessState == 1 || result.SuccessState == 2)
                    {
                        var exception = new RequestException("failed to update till balance. Will try again soon...");
                        MyLogger.GetInstance().Error("Error: ", exception);
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return result;
        }
    }
}