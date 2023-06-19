using System.Threading.Tasks;
using Gateway.Logger;
using Newtonsoft.Json;
using PaymentGateway.methods;
using PaymentGateway.request.response;
using System.Net.Http;
using System.Text;
using Flurl.Http;
using System.Linq;
using Azure.Identity;
using System.IdentityModel.Tokens;
using Azure.Core;
using System.IdentityModel;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Threading;

namespace PaymentGateway.request
{
    internal class microsoftGraph
    {
        private static ClientSecretCredential clientSecretCredential;
        private static TokenRequestContext tokenRequestContext;
        private static ValueTask<AccessToken> response;
        private static StringContent data;

        private static async Task<string> secretAuth()
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            clientSecretCredential = new ClientSecretCredential(
                applicationConfiguration.Credentials.AzureTenantID,
                applicationConfiguration.Credentials.AzureClientID,
                applicationConfiguration.Credentials.AzureClientSecret, options);

            tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            response = clientSecretCredential.GetTokenAsync(tokenRequestContext);

            return response.Result.Token;
        }

        public static async Task<string> getUserList(string consumerName, string response = null, object returnValue = null)
        {
            try
            {
                data = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var url = "https://graph.microsoft.com/v1.0/users?$filter=displayName eq '" + consumerName + "'&$select=displayName," + applicationConfiguration.Credentials.AzureCustomAttribute;
                response = await url.WithOAuthBearerToken(await secretAuth()).GetAsync().Result.ResponseMessage.Content.ReadAsStringAsync();

                var rData = JsonConvert.DeserializeObject<graphUtility.RootModel>(response);

                if (await ValidateUser(consumerName, rData.Value.FirstOrDefault().DisplayName))
                    response = (string)rData.Value.FirstOrDefault().AdditionalProperties.FirstOrDefault().Value;
            }

            catch (FlurlHttpException ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return response;
        }

        private static async Task<bool> ValidateUser(string consumerName, string displayName, bool returnValue = false)
        {
            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrWhiteSpace(consumerName))
            {
                if (consumerName == displayName)
                    returnValue = true;

                else if (consumerName != displayName)
                    returnValue = false;
            }

            else if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(consumerName)) 
                returnValue = false;

            return returnValue;
        }

        public static void Dispose()
        {
            clientSecretCredential = null;
            data.Dispose();

            GC.Collect();
        }
    }
}
