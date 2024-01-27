using System.Text.RegularExpressions;

namespace restaurant_api.Utils;
    public class ValidadeHourFormat
    {
      public bool IsValid(string hourAndMinutesString)
    {
        bool isValidOpenHour = Regex.IsMatch(hourAndMinutesString, "^[0-9]{2}:[0-9]{2}$");
        if (!isValidOpenHour) return false;

        var HourAndMinutes = hourAndMinutesString.Split(':');
        var hour = int.Parse(HourAndMinutes[0]);
        var minutes = int.Parse(HourAndMinutes[1]);
        bool isValidHourFormat = hour <= 24 && minutes < 60;
        if (!isValidHourFormat) return false;
        return true;
    }
    }
