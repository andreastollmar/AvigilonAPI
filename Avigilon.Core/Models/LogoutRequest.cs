using System.Text.Json.Serialization;

namespace Avigilon.Core.Models;

public class LogoutRequest
{
    [JsonPropertyName("session")]
    public string Session { get; set; }
}
