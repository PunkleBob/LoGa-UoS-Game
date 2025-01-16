using System;

namespace LoGaCulture.LUTE
{
    public enum TimeOfDay
    {
        Morning, // 6:00 AM – 12:00 PM(midday)
        Afternoon, // 12:00 PM – 6:00 PM
        Evening, // 6:00 PM – 9:00 PM
        Night // 9:00 PM – 6:00 AM
    }

    public static class TimeOfDayRanges
    {
        public static readonly TimeRange MorningRange = new TimeRange(TimeSpan.FromHours(6), TimeSpan.FromHours(12));
        public static readonly TimeRange AfternoonRange = new TimeRange(TimeSpan.FromHours(12), TimeSpan.FromHours(18));
        public static readonly TimeRange EveningRange = new TimeRange(TimeSpan.FromHours(18), TimeSpan.FromHours(21));
        public static readonly TimeRange NightRange = new TimeRange(TimeSpan.FromHours(21), TimeSpan.FromHours(6));  // Wraps over midnight

        // Helper method to get TimeOfDay based on the current time
        public static TimeOfDay GetTimeOfDay(DateTime currentTime)
        {
            TimeSpan currentTimeSpan = currentTime.TimeOfDay;

            if (MorningRange.IsInRange(currentTimeSpan)) return TimeOfDay.Morning;
            if (AfternoonRange.IsInRange(currentTimeSpan)) return TimeOfDay.Afternoon;
            if (EveningRange.IsInRange(currentTimeSpan)) return TimeOfDay.Evening;
            return TimeOfDay.Night;  // Nighttime, if none of the above match
        }
    }
}
