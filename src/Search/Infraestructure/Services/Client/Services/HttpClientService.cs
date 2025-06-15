using Infraestructure.Interfaces.Client;
using System.Net.Http.Json;

namespace Infraestructure.Services.Client.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> Post<U, T>(string baseUrl, string endpoint, U request)
        {
            string url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            var httpResponse = await _httpClient.PostAsJsonAsync(url, request);

            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadFromJsonAsync<T>();
        }
    }
}
