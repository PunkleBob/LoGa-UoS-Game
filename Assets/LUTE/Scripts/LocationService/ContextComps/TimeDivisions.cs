namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Simple class to hold the division of time into different categories.
    /// Mostly used in the case of time variables and UDateTime implementations.
    /// </summary>
    /// 

    public abstract class TimeDivisions
    {
        public UDateTime uDateTime;

        public Season season;

        public TimeOfDay timeOfDay;

        public DaylightCycle daylightCycle;
    }
}