namespace Avigilon.Infrastructure.Services;
public class TokenProvider(IHttpClientProvider httpClientProvider) : ITokenProvider
{
    private readonly IHttpClientProvider _httpClientProvider = httpClientProvider;

    public async Task<string> GenerateSessionTokenAsync(string userNonce, string userKey, string userName, string userPassword, string clientName)
    {
        var httpClient = _httpClientProvider.GetRegularClient();

        var generator = new AuthTokenGenerator(userNonce, userKey);
        var authToken = generator.GenerateToken();
        Uri requestUri = new Uri("https://srv03367:8443/mt/api/rest/v1/login");

        var serializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        string bodyContent = JsonSerializer.Serialize(new LoginRequestContract
        {
            AuthorizationToken = authToken,
            Username = userName,
            Password = userPassword,
            ClientName = clientName
        }, serializerSettings);

        StringContent content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, content);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to login: response status code was {httpResponseMessage.StatusCode}");
        }

        LoginResponseContract loginResponseContract = JsonSerializer.Deserialize<LoginResponseContract>(await httpResponseMessage.Content.ReadAsStringAsync(), serializerSettings);
        if (loginResponseContract?.Status != "success")
        {
            throw new Exception("Failed to login: " + loginResponseContract?.Status);
        }

        var session = loginResponseContract.Result.Session;
        return session;
    }

}
