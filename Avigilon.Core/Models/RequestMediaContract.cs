using System.Text.Json.Serialization;

public class RequestMediaContract
{
    [JsonPropertyName("camera")]
    public string Camera { get; set; }
    [JsonPropertyName("isImg")]
    public bool IsImg { get; set; }
    [JsonPropertyName("requestBody")]
    public List<RequestMediaBodyContract> RequestMediaBodyContracts { get; set; }
}

public class RequestMediaBodyContract
{
    [JsonPropertyName("date")]
    public string Date { get; set; }
    //public string EndDate { get; set; }
    [JsonPropertyName("time")]
    public string Time { get; set; }
}
