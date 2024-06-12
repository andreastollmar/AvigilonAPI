namespace Avigilon.Core.Interfaces;
public interface IHttpClientProvider
{
    HttpClient GetHttpClient(string session);
    HttpClient GetRegularClient();
}
