using System.Text.Json.Serialization;
namespace Avigilon.Core.Models;

public class TimeLineResult
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("result")]
    public Timeline Result { get; set; }
}

public class Timeline
{
    [JsonPropertyName("timelines")]
    public List<CameraTimeline> Timelines { get; set; }
}

public class CameraTimeline
{
    [JsonPropertyName("cameraId")]
    public string CameraId { get; set; }
    [JsonPropertyName("record")]
    public Record[] Record { get; set; }
    [JsonPropertyName("motion")]
    public Record[] Motion { get; set; }

}

public class Record
{
    [JsonPropertyName("start")]
    public string Start { get; set; }
    [JsonPropertyName("end")]
    public string End { get; set; }
}