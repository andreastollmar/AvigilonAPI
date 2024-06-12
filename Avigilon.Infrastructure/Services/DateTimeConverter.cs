using System.Globalization;

namespace Avigilon.Infrastructure.Services;

public class DateTimeConverter
{
    public string ConvertDateTimeToUtc(string date, string time)
    {
        // Combine date and time strings
        string dateTimeString = $"{date}T{time}";

        // Parse the combined string into a DateTime object
        DateTime localDateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

        // Define the local time zone (CEST)
        TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);

        return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}
