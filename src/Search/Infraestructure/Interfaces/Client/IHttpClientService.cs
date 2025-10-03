namespace Infraestructure.Interfaces.Client
{
    public interface IHttpClientService
    {
        public Task<T> Post<U, T>(string baseUrl, string endpoint, U request);
    }
}
