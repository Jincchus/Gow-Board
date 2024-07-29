using System;

namespace GowBoard.Utility
{
    public static class DateTimeUtility
    {
        private static readonly TimeZoneInfo KoreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

        public static DateTime ConvertToKoreanTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, KoreanTimeZone);
        }

        public static DateTime GetKoreanNow()
        {
            return ConvertToKoreanTime(DateTime.UtcNow);
        }
    }
}
