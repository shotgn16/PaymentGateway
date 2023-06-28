using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiTest;
using Flurl;
using Gateway.Logger;
using NLog;
using PaymentGateway.methods;

namespace PaymentGateway.request
{
    public class buildRequest
    {
        public static async Task<string> BuildURL(int _endpointID, string returnValue = null)
        {
            MyLogger.GetInstance().Debug("Building url");

            string _endpoint = EndpointBuilder(_endpointID).Result;
            var url = await urlBuilder(_endpointID);

            if (_endpointID >= 10)
              returnValue = url;

            else 
              returnValue = url + _endpoint;

            MyLogger.GetInstance().Debug("Returning built url...");

            return returnValue;
        }

        internal static async Task<string> urlBuilder(int _endpointID, string returnValue = null)
        {
            if (_endpointID >= 10)
                returnValue = "http://www.pay24-7.com/P247WS/PubMethods/";

            else if (_endpointID <= 9)
                returnValue = await restUrl(_endpointID);

            return returnValue;
        }

        internal static async Task<string> restUrl(int _endpointID)
        {
            if (_endpointID == 2)
             return $"http://{applicationConfiguration.Credentials.ServerAddress}:8090/";
            
            else
             return $"https://{applicationConfiguration.Credentials.ServerAddress}:8090/";
        }

        internal static async Task<string> EndpointBuilder(int _endpointID)
        {
            var returnValue = "";

            switch (_endpointID)
            {
                case 1: returnValue = "api/auth/token"; break;
                case 2: returnValue = "api/v3/certificates/ca"; break;
                case 3: returnValue = $"api/v3/users/find?{await activeDirectory.UserInfoType(internalConfig.internalConfiguration.reqUsername, internalConfig.internalConfiguration.reqCode)}"; break;
                case 4: returnValue = "api/v3/rechargeProviders"; break;
                case 5: returnValue = "api/v3/rechargeProviders/external/payments"; break;
                case 6: returnValue = await endpointBuilder("api/v3/rechargeProviders/external/payments/{paymentID}/commit"); break;
                case 7: returnValue = await endpointBuilder("api/v3/rechargeProviders/external/payments?userId={userID}"); break;
                case 8: returnValue = await endpointBuilder("api/v3/users/{userID}/accounts"); break;
                case 10: returnValue = "handleSimplePaymentReport"; break;
                case 11: returnValue = "handleSimpleMessageUpdateRequest"; break;
            }

            return returnValue;
        }

        internal static async Task<string> endpointBuilder(string _endpoint)
        {
            if (_endpoint.Contains("{paymentID}"))
                _endpoint = _endpoint.Replace("{paymentID}", internalConfig.internalConfiguration.PaymentID);

            if (_endpoint.Contains("{userID}"))
                _endpoint = _endpoint.Replace("{userID}", internalConfig.internalConfiguration.UserID);

            return _endpoint;
        }
    }
}
