namespace Avigilon.Infrastructure.Services;
public class HttpClientProvider : IHttpClientProvider
{
    private static readonly HttpClient _httpClient;

    static HttpClientProvider()
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (HttpRequestMessage _, X509Certificate2 _, X509Chain _, SslPolicyErrors _) => true
        };
        _httpClient = new HttpClient(httpClientHandler);
    }

    public HttpClient GetHttpClient(string session)
    {
        if (session != null)
        {
            string authHeaderValue = $"x-avg-session {session}";
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization") ||
                _httpClient.DefaultRequestHeaders.Authorization.ToString() != authHeaderValue)
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", authHeaderValue);
            }
        }
        return _httpClient;
    }

    public HttpClient GetRegularClient()
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (HttpRequestMessage _, X509Certificate2 _, X509Chain _, SslPolicyErrors _) => true
        };
        var httpClient = new HttpClient(httpClientHandler);

        return httpClient;
    }
}
