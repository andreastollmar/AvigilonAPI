using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AvigilonApi.Services
{
    public class TokenProvider(HttpClientProivider clientProivider, IConfiguration configuration)
    {
        private readonly HttpClientProivider _clientProivider = clientProivider;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> GenerateSessionTokenAsync()
        {
            var httpClient = _clientProivider.GetHttpClient();

            var generator = new AuthTokenGenerator(_configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Userkey"));
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
                Username = _configuration.GetValue("Avigilon:Login:Username", "Username"),
                Password = _configuration.GetValue("Avigilon:Login:Password", "Password"),
                ClientName = _configuration.GetValue("Avigilon:Secretkeys:Clientname", "Clientname")
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
}
