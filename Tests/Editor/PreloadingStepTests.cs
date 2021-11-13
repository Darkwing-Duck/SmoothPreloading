using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using SmoothPreloading.Simulation;

namespace SmoothPreloading.Editor.Tests
{
	[TestFixture]
	public class PreloadingStepTests : AsyncTest
	{
		[Test]
		public void CreateStep_StatusShouldEqualIdle()
		{
			var step = new EmptySuccessPreloadingStep();
			Assert.AreEqual(PreloadingTaskStatus.Idle, step.status);
			Assert.AreEqual(0f, step.progress);
		}

		[Test(ExpectedResult = null)]
		public IEnumerator RunAndCompleteStep_StatusShouldBeSuccess()
		{
			var step = new EmptySuccessPreloadingStep();
			yield return ExecuteAsync(step.Run());
			Assert.AreEqual(PreloadingTaskStatus.Success, step.status);
			Assert.IsNull(step.errorReason);
			Assert.AreEqual(1f, step.progress);
		}

		[Test(ExpectedResult = null)]
		public IEnumerator RunAndCompleteStep_StatusShouldBeError()
		{
			var step = new EmptyErrorPreloadingStep();
			yield return ExecuteAsync(step.Run());
			Assert.AreEqual(PreloadingTaskStatus.Error, step.status);
			Assert.AreEqual("Test Error", step.errorReason);
			Assert.AreEqual(0.5f, step.progress);
		}

		[Test(ExpectedResult = null)]
		public IEnumerator RunAndCompleteSimulationStep_NotFullSimulation()
		{
			var step = new TestSimulationPreloadingStep(false);
			yield return ExecuteAsync(step.Run());
			Assert.AreEqual(PreloadingTaskStatus.Success, step.status);
			Assert.IsNull(step.errorReason);
			Assert.AreEqual(0.3f, step.progress);
		}

		[Test(ExpectedResult = null)]
		public IEnumerator RunAndCompleteSimulationStep_FullSimulation()
		{
			var step = new TestSimulationPreloadingStep(true);
			yield return ExecuteAsync(step.Run());
			Assert.AreEqual(PreloadingTaskStatus.Success, step.status);
			Assert.IsNull(step.errorReason);
			Assert.AreEqual(1f, step.progress);
		}
	}

	public class EmptySuccessPreloadingStep : SmoothPreloadingStep
	{ }

	public class EmptyErrorPreloadingStep : SmoothPreloadingStep
	{
		protected override Task Processing()
		{
			SetProgress(0.5f);
			SetError("Test Error");
			return Task.CompletedTask;
		}
	}

	public class TestSimulationPreloadingStep : SimulationPreloadingStep
	{
		private static readonly TestSimulationTimeProvider TimeProvider = new TestSimulationTimeProvider();
		public TestSimulationPreloadingStep(bool fullSimulation)
			: base(new SimulationStepConfig(1f, fullSimulation, TimeProvider), 100) { }

		protected override Task Processing()
		{
			TimeProvider.IncrementTime(config.fullSimulation ? 1000 : 300);
			return Task.CompletedTask;
		}
	}

	public class TestSimulationTimeProvider : ISimulationTimeProvider
	{
		private long _time;
		public void IncrementTime(int value) => _time += value;
		public long time => _time;
	}
}