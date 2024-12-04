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

                if (span.Days > 1)
                    return $"{span.Days} d";
                if (span.Days == 1)
                    return "1d";
                if (span.Hours > 1)
                    return $"{span.Hours} hr";
                if (span.Hours == 1)
                    return "1hr";
                if (span.Minutes > 1)
                    return $"{span.Minutes} min";
                if (span.Minutes == 1)
                    return "1min";
                return "now";
            }
            return "Invalid date";
        }
    }
}
