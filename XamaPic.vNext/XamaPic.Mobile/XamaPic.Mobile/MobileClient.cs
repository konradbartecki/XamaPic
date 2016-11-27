using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XamaPic.Mobile
{
    public class MobileClient
    {
        //private const string BaseAddress = "http://localhost:5010/";
        private const string BaseAddress = "http://xamapicvnext.azurewebsites.net/";
        private static HttpClient _client;
        private HttpClient Client => _client ?? (_client = CreateClient());

        private HttpClient CreateClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri(BaseAddress)
            };
        }

        public void Reset()
        {
            Client?.Dispose();
        }

        public async Task<Stream> GetFilteredImage(Stream image)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/photos/filtered");
            request.Content = new StreamContent(image);
            var response = await Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStreamAsync();
            throw new Exception($"{nameof(GetFilteredImage)} failed - {response.ReasonPhrase}");
        }

    }
}
