namespace Avigilon.Core.Interfaces;

public interface ITokenProvider
{
    Task<string> GenerateSessionTokenAsync(string userNonce, string userKey, string userName, string userPassword, string clientName);
}
