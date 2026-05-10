using System.Collections.Generic;

namespace AssetManager
{
    public sealed class RunCalendarQuarter
    {
        public RunCalendarQuarter(int fiscalYear, int quarter, int businessDays)
        {
            FiscalYear = fiscalYear;
            Quarter = quarter;
            BusinessDays = businessDays;
        }

        public int FiscalYear { get; }
        public int Quarter { get; }
        public int BusinessDays { get; }
    }

    public sealed class RunCalendarDefinition
    {
        public RunCalendarDefinition(IEnumerable<RunCalendarQuarter> playableQuarters)
        {
            var quarters = new List<RunCalendarQuarter>(playableQuarters);
            PlayableQuarters = quarters.AsReadOnly();

            var totalBusinessDays = 0;
            foreach (var quarter in quarters)
            {
                totalBusinessDays += quarter.BusinessDays;
            }

            TotalPlayableBusinessDays = totalBusinessDays;
        }

        public IReadOnlyList<RunCalendarQuarter> PlayableQuarters { get; }
        public int TotalPlayableBusinessDays { get; }

        public bool IsPlayableQuarter(int fiscalYear, int quarter)
        {
            foreach (var playableQuarter in PlayableQuarters)
            {
                if (playableQuarter.FiscalYear == fiscalYear && playableQuarter.Quarter == quarter)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsVacationQuarter(int fiscalYear, int quarter)
        {
            return (fiscalYear == 1 || fiscalYear == 2) && quarter == 4;
        }

        public int GetPlayableBusinessDays(int fiscalYear, int quarter)
        {
            foreach (var playableQuarter in PlayableQuarters)
            {
                if (playableQuarter.FiscalYear == fiscalYear && playableQuarter.Quarter == quarter)
                {
                    return playableQuarter.BusinessDays;
                }
            }

            return 0;
        }
    }

    public static class RunCalendar
    {
        public static RunCalendarDefinition CreateMvpCalendar()
        {
            return new RunCalendarDefinition(new[]
            {
                new RunCalendarQuarter(1, 1, 4),
                new RunCalendarQuarter(1, 2, 4),
                new RunCalendarQuarter(1, 3, 4),
                new RunCalendarQuarter(2, 1, 4),
                new RunCalendarQuarter(2, 2, 4),
                new RunCalendarQuarter(2, 3, 4),
                new RunCalendarQuarter(3, 1, 5),
                new RunCalendarQuarter(3, 2, 5),
                new RunCalendarQuarter(3, 3, 5),
                new RunCalendarQuarter(3, 4, 5)
            });
        }
    }
}
