using System;

namespace LoGaCulture.LUTE
{
    public enum Season
    {
        Spring, // March, April, May
        Summer, // June, July, August
        Autumn, // September, October, November
        Winter // December, January, February
    }

    public static class SeasonRanges
    {
        public static readonly MonthRange SpringRange = new MonthRange(3, 5);  // March to May
        public static readonly MonthRange SummerRange = new MonthRange(6, 8);  // June to August
        public static readonly MonthRange AutumnRange = new MonthRange(9, 11); // September to November
        public static readonly MonthRange WinterRange = new MonthRange(12, 2); // December to February (wraps around)

        // Helper method to get the current season based on the month
        public static Season GetSeason(DateTime currentDate)
        {
            int month = currentDate.Month;

            if (SpringRange.IsInRange(month)) return Season.Spring;
            if (SummerRange.IsInRange(month)) return Season.Summer;
            if (AutumnRange.IsInRange(month)) return Season.Autumn;
            return Season.Winter;  // Winter if none of the above match
        }
    }
}