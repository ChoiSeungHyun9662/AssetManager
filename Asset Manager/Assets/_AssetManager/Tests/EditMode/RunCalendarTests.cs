using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class RunCalendarTests
    {
        [Test]
        public void V3CalendarContainsTenPlayableQuartersAndEightyEightBusinessDays()
        {
            var calendar = RunCalendar.CreateMvpCalendar();

            Assert.That(calendar.PlayableQuarters, Has.Count.EqualTo(10));
            Assert.That(calendar.TotalPlayableBusinessDays, Is.EqualTo(88));
        }

        [Test]
        public void V3CalendarUsesEightBusinessDaysBeforeFinalFiscalYearAndTenInFinalFiscalYear()
        {
            var calendar = RunCalendar.CreateMvpCalendar();

            Assert.That(calendar.IsPlayableQuarter(1, 1), Is.True);
            Assert.That(calendar.GetPlayableBusinessDays(1, 1), Is.EqualTo(8));
            Assert.That(calendar.IsVacationQuarter(1, 4), Is.True);
            Assert.That(calendar.IsPlayableQuarter(1, 4), Is.False);

            Assert.That(calendar.IsPlayableQuarter(2, 3), Is.True);
            Assert.That(calendar.GetPlayableBusinessDays(2, 3), Is.EqualTo(8));
            Assert.That(calendar.IsVacationQuarter(2, 4), Is.True);
            Assert.That(calendar.IsPlayableQuarter(2, 4), Is.False);

            Assert.That(calendar.IsPlayableQuarter(3, 4), Is.True);
            Assert.That(calendar.IsVacationQuarter(3, 4), Is.False);
            Assert.That(calendar.GetPlayableBusinessDays(3, 4), Is.EqualTo(10));
        }
    }
}
