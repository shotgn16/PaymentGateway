using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Flurl.Http;
using Flurl.Http.Configuration;
using Gateway.Logger;
using NLog.LayoutRenderers.Wrappers;
using PaymentGateway.methods;

namespace PaymentGateway.request
{
    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }
    }

    public class flurlRest
    {
        public static async Task<string> getToken(bool whitelisted = false, string JResponse = null)
        {

            MyLogger.GetInstance().Debug("Initializing API call (Generate Access Token)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.PostAsync(await buildRequest.BuildURL(1), new StringContent(internalConfig.internalConfiguration.TokenAuth_Body, Encoding.UTF8, "application/json"));
                    
                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }

        public static async Task<string> getCertificate(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get Certificate)");

            using (HttpClientHandler _handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                using (var wrapper = new httpWrapper(_handler))
                {
                    whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                    MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                    if (whitelisted)
                    {
                        MyLogger.GetInstance().Debug("Sending API request...");
                        var response = await wrapper.GetAsync(await buildRequest.BuildURL(2));

                        JResponse = response.Item1;
                    }

                    else
                    {
                        MyLogger.GetInstance().Error("Error 404: Address [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                    }
                }

                MyLogger.GetInstance().Debug("Returning API response");
                return JResponse;
            }
        }

        public static async Task<string> getUserInfo(string username = null, string code = null, bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get User Info)");

            internalConfig.internalConfiguration.reqUsername = username; internalConfig.internalConfiguration.reqCode = code;

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper._client.SetAuthorisationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.GetAsync(await buildRequest.BuildURL(3));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }

        public static async Task<string> getRechargeProviders(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get Recharge Providers)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper.SetAuthorizationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.GetAsync(await buildRequest.BuildURL(4));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }

        public static async Task<string> createRechargeRequest(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Create Recharge Request)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper.SetAuthorizationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.PostAsync(
                        await buildRequest.BuildURL(5), 
                        new StringContent(internalConfig.internalConfiguration.RechargeRequest_Body, 
                        Encoding.UTF8, "application/json"));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");

            return JResponse;
        }

        public static async Task<string> commitRechargeRequest(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Commit Recharge Request)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper.SetAuthorizationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.PostAsync(await buildRequest.BuildURL(6), new StringContent(string.Empty, Encoding.UTF8, "application/json"));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }

        public static async Task<string> getPayments(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get User Payment Array)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper.SetAuthorizationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.GetAsync(await buildRequest.BuildURL(7));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }

        public static async Task<string> getUsersCredit(bool whitelisted = false, string JResponse = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Retrieve User Credit)");

            using (var wrapper = new httpWrapper())
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    await wrapper.SetAuthorizationHeader("Bearer", internalConfig.internalConfiguration.Token);

                    MyLogger.GetInstance().Debug("Sending API request...");
                    var response = await wrapper.GetAsync(await buildRequest.BuildURL(8));

                    JResponse = response.Item1;
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return JResponse;
        }
    }
}
 