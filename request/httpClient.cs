using Gateway.Logger;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PaymentGateway.request.response
{
    internal class httpClient : IDisposable
    {
        internal readonly object _lock = new object();
        internal HttpClient _instance;
        public HttpClient Instance { get { return _instance; } }

        public httpClient()
        {
            _instance = new HttpClient();
            _instance.Timeout = TimeSpan.FromSeconds(10);
        }

        public httpClient(HttpClientHandler _handler)
        {
            _instance = new HttpClient(_handler);
            _instance.Timeout = TimeSpan.FromSeconds(10);
        }

        internal async Task<bool> SetAuthorisationHeader(string header, string value, bool v = false)
        {
            AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue(header, value);
            Instance.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            if (Instance.DefaultRequestHeaders.Authorization != null)
                v = true;

            return v;
        }

        internal async Task<HttpResponseMessage> GetAsync(string url)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = await Instance.GetAsync(url);
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return responseMessage;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = await Instance.PostAsync(url, content);
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: ", ex);
            }

            return responseMessage;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
