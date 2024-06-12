namespace Avigilon.Core.Models;

public class SuccessMsg
{
    public string SaveLocation { get; set; }
    public List<string> FileSaved { get; set; } = new List<string>();
}
