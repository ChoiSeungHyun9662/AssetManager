using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class RunStatusFormatterTests
    {
        [Test]
        public void FormatShowsRentArrearsInsteadOfRedemptionPressure()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            var status = RunStatusFormatter.Format(run);

            Assert.That(status, Does.Contain("월세 밀림 0/10"));
            Assert.That(status, Does.Not.Contain("환매 압력"));
        }
    }
}
