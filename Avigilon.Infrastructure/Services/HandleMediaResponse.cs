﻿namespace Avigilon.Infrastructure.Services;

public class HandleMediaResponse : IHandleMediaResponse
{
    public async Task<bool> SaveMediaResponse(string camera, byte[] mediaData, bool isImg, string date, string time)
    {
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var downloadsFolderPath = Path.Combine(userProfilePath, "Downloads");
        if (isImg)
        {
            if (!Directory.Exists(downloadsFolderPath))
            {
                Directory.CreateDirectory(downloadsFolderPath);
            }
            var fileName = $"{camera}_" + $"{date}:{time}" + ".jpeg";
            fileName = fileName.Replace(":", "_");
            string filePath = Path.Combine(downloadsFolderPath, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(mediaData);
            }
        }
        else
        {
            if (!Directory.Exists(downloadsFolderPath))
            {
                Directory.CreateDirectory(downloadsFolderPath);
            }
            var fileName = $"{camera}_" + $"{date}:{time}" + ".mp4";
            fileName = fileName.Replace(":", "_");
            string filePath = Path.Combine(downloadsFolderPath, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(mediaData, 0, mediaData.Length);
            }
        }

        return true;
    }

}
