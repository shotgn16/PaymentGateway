using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Gateway.Logger;
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
        public static async Task<IFlurlResponse> getToken(bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Generate Access Token)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(1);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(internalConfig.internalConfiguration.TokenAuth_Body, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.PostAsync(data);
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> getCertificate(bool whitelisted = false, IFlurlResponse response = null, string url = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get Certificate)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    url = await buildRequest.BuildURL(2);

                    MyLogger.GetInstance().Debug("Loading request content");

                    FlurlHttp.ConfigureClient(url, cli =>
                    cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

                    var data = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.GetAsync();
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());

            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> getUserInfo(string username = null, string code = null, bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get User Info)");

            try
            {
                internalConfig.internalConfiguration.reqUsername = username; internalConfig.internalConfiguration.reqCode = code;

                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(3);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(string.Empty, Encoding.UTF8, "appliation/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).GetAsync();
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());

            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> getRechargeProviders(bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get Recharge Providers)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(4);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).GetAsync();
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> createRechargeRequest(bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Create Recharge Request)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(5);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(internalConfig.internalConfiguration.RechargeRequest_Body, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).PostAsync(data);
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }

            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");

            return response;
        }

        public static async Task<IFlurlResponse> commitRechargeRequest(bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Commit Recharge Request)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(6);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).PostAsync(data);
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> getPayments(bool whitelisted = false, IFlurlResponse response = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Get User Payment Array)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    var url = await buildRequest.BuildURL(7);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).GetAsync();
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }

        public static async Task<IFlurlResponse> getUsersCredit(bool whitelisted = false, IFlurlResponse response = null, string url = null)
        {
            MyLogger.GetInstance().Debug("Initializing API call (Retrieve User Credit)");

            try
            {
                whitelisted = securityManagement.WhitelistAddress(applicationConfiguration.Credentials.ServerAddress).Result;

                MyLogger.GetInstance().Debug("Checking if address is whitelisted");
                if (whitelisted)
                {
                    MyLogger.GetInstance().Debug("Building address url");
                    url = await buildRequest.BuildURL(8);

                    MyLogger.GetInstance().Debug("Loading request content");
                    var data = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                    MyLogger.GetInstance().Debug("Sending API request...");
                    response = await url.WithOAuthBearerToken(internalConfig.internalConfiguration.Token).GetAsync();
                }

                else
                {
                    MyLogger.GetInstance().Error("Error 404: Address: [" + applicationConfiguration.Credentials.ServerAddress + "] not whitelisted!");
                }
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: ", await ex.GetResponseStringAsync());
            }

            MyLogger.GetInstance().Debug("Returning API response");
            return response;
        }
    }
}
 