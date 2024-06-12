using Avigilon.Core.Models;

namespace Avigilon.Core.Interfaces;
public interface IAvigilonApiCalls
{
    Task<bool> SaveMediafile(string session, string time, string date, string camera, bool isImg);
    Task<List<CameraTimeline>> GetListOfTimelines(string session, RequestBodyContract bodyContract);
    Task<bool> Logout(string session);
}
