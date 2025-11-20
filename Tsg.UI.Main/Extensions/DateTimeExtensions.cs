using System;

namespace Tsg.UI.Main.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetStartDayDate(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 1);
        }

        public static DateTime GetEndDayDate(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }

        public static DateTime GetDateLast24Hour(this DateTime dt)
        {
            return dt.AddHours(-24);
        }

        public static DateTime GetDateLast7Days(this DateTime dt)
        {
            return dt.AddDays(-7);
        }
        public static DateTime GetDateLast30Days(this DateTime dt)
        {
            return dt.AddDays(-30);
        }
        public static DateTime GetDateLast365Days(this DateTime dt)
        {
            return dt.AddDays(-365);
        }

    }
}