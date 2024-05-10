namespace Avigilon.Infrastructure.Services;

public class HandleMediaResponse
{
    public async Task<bool> SaveMediaResponse(string camera, byte[] mediaData, bool isImg)
    {
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var downloadsFolderPath = Path.Combine(userProfilePath, "Downloads"); // Get the downloadsfolder
        if (isImg)
        {
            var directoryPath = "../AvigilonApi/Images/"; // Change to downloads or another specific folder
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var fileName = $"{camera}_" + DateTime.Now.ToShortTimeString() + ".jpeg";
            fileName = fileName.Replace(":", "_");
            string filePath = Path.Combine(directoryPath, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(mediaData);
            }
        }
        else
        {
            var directoryPath = "../AvigilonApi/Videos/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var fileName = $"{camera}_" + DateTime.Now.ToShortTimeString() + ".mp4";
            fileName = fileName.Replace(":", "_");
            string filePath = Path.Combine(directoryPath, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(mediaData, 0, mediaData.Length);
            }
        }

        return true;
    }

}
