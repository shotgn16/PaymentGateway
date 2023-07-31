using PaymentGateway.request.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.request
{
    internal class httpWrapper : IDisposable
    {
        internal httpClient _client;
        internal StringContent _content;
        internal httpWrapper() => _client = new httpClient();
        internal httpWrapper(HttpClientHandler _handler) => _client = new httpClient(_handler);

        internal async Task SetAuthorizationHeader(string header, string value)
        {
            if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(value))
                await _client.SetAuthorisationHeader(header, value);
        }

        internal async Task<(string, HttpStatusCode)> GetAsync(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);

            return (await response.Content.ReadAsStringAsync(), response.StatusCode);
        }

        internal async Task<(string, HttpStatusCode)> PostAsync(string url, HttpContent content)
        {
            HttpResponseMessage response = await _client.PostAsync(url, content);

            return (await response.Content.ReadAsStringAsync(), response.StatusCode);
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
