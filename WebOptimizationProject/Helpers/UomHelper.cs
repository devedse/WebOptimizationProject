using System;

namespace WebOptimizationProject.Helpers
{
    public static class UomHelper
    {
        public static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            var theNumber = Math.Sign(byteCount) * num;
            return $"{theNumber}{suf[place]}";
        }

        public static String SecondsToString(long seconds)
        {
            string[] suf = { "Second", "Minute", "Hour" };
            if (seconds == 0)
                return "0 " + suf[0];
            long bytes = Math.Abs(seconds);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 60)));
            double num = Math.Round(bytes / Math.Pow(60, place), 1);
            var theNumber = Math.Sign(seconds) * num;
            return $"{theNumber} {suf[place]}{(num == 1 ? "" : "s")}";
        }
    }
}
