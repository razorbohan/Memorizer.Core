using System;

namespace Memorizer.Logic
{
    public static class Extensions
    {
        public static Postpone NextLevel(this Postpone currentLevel)
        {
            var postpones = (Postpone[])Enum.GetValues(typeof(Postpone));
            int index = Array.IndexOf(postpones, currentLevel) + 1;
            return (postpones.Length == index) ? currentLevel : postpones[index];
        }

        public static string Trimize(this string @string)
        {
            return @string.Replace("\n", "").Replace("\r", "").Trim();
        }

        public static DateTime GetTomorrow(int add = 1)
        {
            var tomorrow = DateTime.Now.AddDays(add);
            var tomorrowMidDateTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
            return tomorrowMidDateTime;
        }
    }
}
