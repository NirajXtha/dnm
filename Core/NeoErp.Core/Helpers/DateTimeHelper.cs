using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime ConvertToLocalTime(DateTime utcTime, string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                return utcTime;
            }
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcTime, timeZoneId);
        }

        public static DateTime GetNepalStandardTime(DateTime utcTime)
        {
            return ConvertToLocalTime(utcTime, "Nepal Standard Time");
        }

    }
}