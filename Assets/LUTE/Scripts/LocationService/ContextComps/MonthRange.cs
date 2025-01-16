namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Class representing a month range with a start and end month.
    /// Mostly used in the context of month-based conditions.
    /// </summary>
    public class MonthRange
    {
        public int Start { get; }
        public int End { get; }

        public MonthRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        // Check if the given month is within this range
        public bool IsInRange(int month)
        {
            if (End >= Start)  // Normal range
            {
                return month >= Start && month <= End;
            }
            else  // Wraps around (e.g., Winter: December to February)
            {
                return month >= Start || month <= End;
            }
        }
    }
}
