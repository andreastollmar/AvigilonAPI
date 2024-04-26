namespace AvigilonApi.Models
{
    public class MediaContract
    {
        public string Session { get; set; }
        public string CameraId { get; set; }
        public string? Format { get; set; } // Default fmp4, can get webm, mpd
        public string? Media { get; set; } // Default video, can get audio and meta
        public string? Range { get; set; }
        public string? Quality { get; set; }
        public bool Burnin { get; set; } = false; // Specifies wether to burn metadata information on video
        public string? Size { get; set; } // specify size of video output, size= 640,480,le
        public string? T { get; set; } // Start streaming from the specified date-time reference in ISO 8601 format. Specify Live for live video,
                                       // Example t = live or t = 2016-03-10T21:01:00.057Z,c Dunno what 057Z is, 'C' is for closest match
    }
}
