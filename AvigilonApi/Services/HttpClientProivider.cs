using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace AvigilonApi.Services
{
    public class HttpClientProivider
    {

        public HttpClient GetHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage _, X509Certificate2 _, X509Chain _, SslPolicyErrors _) => true;

            var httpClient = new HttpClient(httpClientHandler);

            return httpClient;
        }
    }
}
