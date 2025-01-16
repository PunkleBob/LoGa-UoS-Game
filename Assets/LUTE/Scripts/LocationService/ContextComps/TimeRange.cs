using System;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Class representing a time range with a start and end time.
    /// Mostly used in the context of time-based conditions.
    /// </summary>
    public class TimeRange
    {
        public TimeSpan Start { get; }
        public TimeSpan End { get; }

        public TimeRange(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        // Check if the given time is within this time range
        public bool IsInRange(TimeSpan time)
        {
            if (End > Start)  // Normal range
            {
                return time >= Start && time < End;
            }
            else  // Wraps around midnight (e.g., Night range)
            {
                return time >= Start || time < End;
            }
        }
    }
}
