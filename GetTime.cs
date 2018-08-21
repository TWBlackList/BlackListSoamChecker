using System;

namespace BlackListSoamChecker
{
    public class GetTime
    {
        public static long GetUnixTime()
        {
            return (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string DecodeUnixTime(long time, int offset)
        {
            DateTime dtime = new DateTime(1970, 1, 1).AddSeconds(time + offset * 3600);
            return dtime.ToString("YYYY-MM-DD hh:mm:ss");
        }

        public static string GetExpiresTime(long time)
        {
            if (time == 0) return "永久封鎖";
            DateTime dtime = new DateTime(1970, 1, 1).AddSeconds(time + 28800);
            return dtime.ToString("YYYY-MM-DD hh:mm:ss");
        }

        public static bool GetIsExpired(long time)
        {
            if (time == 0) return false;
            if (GetUnixTime() >= time - 30) return true;
            return false;
        }

        /*public static int GetTimeOffset()
        {
        *
        }*/
    }
}