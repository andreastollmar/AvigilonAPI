namespace Avigilon.Core.Interfaces;
public interface IHandleMediaResponse
{
    Task<bool> SaveMediaResponse(string camera, byte[] mediaData, bool isImg, string date, string time);
}
