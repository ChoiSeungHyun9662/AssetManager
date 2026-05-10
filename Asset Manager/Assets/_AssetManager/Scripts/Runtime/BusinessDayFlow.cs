using System;

namespace AssetManager
{
    public static class BusinessDayFlow
    {
        public static RunSessionState AdvanceToNextBusinessDay(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.Calendar.RemainingBusinessDays <= 0)
            {
                return run;
            }

            var remainingBusinessDays = run.Calendar.RemainingBusinessDays - 1;
            var nextPhase = remainingBusinessDays == 0
                ? BusinessDayPhase.QuarterSettlement
                : BusinessDayPhase.AwaitingAction;

            return new RunSessionState(
                run.State,
                run.StaticData,
                new RunCalendarState(
                    run.Calendar.FiscalYear,
                    run.Calendar.Quarter,
                    remainingBusinessDays),
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(nextPhase, MarketAreaState.Market),
                run.RedemptionPressure);
        }

        public static RunSessionState ContinueAfterQuarterSettlement(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var calendar = RunCalendar.CreateMvpCalendar();

            if (run.Calendar.FiscalYear == 3 && run.Calendar.Quarter == 4)
            {
                return new RunSessionState(
                    RunState.Completed,
                    run.StaticData,
                    new RunCalendarState(run.Calendar.FiscalYear, run.Calendar.Quarter, 0),
                    run.Resources,
                    run.Performance,
                    run.AssetCards,
                    run.MarketTape,
                    run.Reservation,
                    run.OwnedAssets,
                    new BusinessDayState(BusinessDayPhase.FinalSettlement, MarketAreaState.Market),
                    run.RedemptionPressure);
            }

            var nextQuarter = run.Calendar.Quarter + 1;
            var businessDays = calendar.GetPlayableBusinessDays(run.Calendar.FiscalYear, nextQuarter);
            var nextPhase = calendar.IsVacationQuarter(run.Calendar.FiscalYear, nextQuarter)
                ? BusinessDayPhase.Vacation
                : BusinessDayPhase.AwaitingAction;

            var nextRun = new RunSessionState(
                run.State,
                run.StaticData,
                new RunCalendarState(run.Calendar.FiscalYear, nextQuarter, businessDays),
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(nextPhase, MarketAreaState.Market),
                run.RedemptionPressure);

            return nextPhase == BusinessDayPhase.AwaitingAction
                ? MarketTape.Advance(nextRun)
                : nextRun;
        }

        public static RunSessionState ContinueAfterVacation(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var calendar = RunCalendar.CreateMvpCalendar();
            var nextFiscalYear = run.Calendar.FiscalYear + 1;
            var businessDays = calendar.GetPlayableBusinessDays(nextFiscalYear, 1);

            var nextRun = new RunSessionState(
                run.State,
                run.StaticData,
                new RunCalendarState(nextFiscalYear, 1, businessDays),
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(BusinessDayPhase.AwaitingAction, MarketAreaState.Market),
                run.RedemptionPressure);

            return MarketTape.Refresh(nextRun);
        }
    }
}
