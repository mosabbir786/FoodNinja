using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Custom
{
    public class TimeFormater
    {
        public static string GetRelativeTime(string dateTimeString)
        {
            const string format = "hh:mm tt dd-MM-yyyy";

            if(DateTime.TryParseExact(dateTimeString, format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                var span = DateTime.Now - dateTime;

                if (span.TotalDays >= 365)
                    return $"{(int)(span.TotalDays / 365)}y";
                if (span.TotalDays >= 30)
                    return $"{(int)(span.TotalDays / 30)}m";
                if (span.TotalDays >= 7)
                    return $"{(int)(span.TotalDays / 7)}w";
                if (span.TotalDays >= 1)
                    return $"{(int)span.TotalDays}d";
                if (span.TotalHours >= 1)
                    return $"{(int)span.TotalHours}hr";
                if (span.TotalMinutes >= 1)
                    return $"{(int)span.TotalMinutes}min";
                if (span.TotalSeconds >= 1)
                    return $"{(int)span.TotalSeconds}sec";

                return "just now";
            }
            return "Invalid date";
        }
    }
}
