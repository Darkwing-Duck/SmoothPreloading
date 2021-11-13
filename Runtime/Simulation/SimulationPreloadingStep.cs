using System.Threading.Tasks;
using UnityEngine;

namespace SmoothPreloading.Simulation
{
	public abstract class SimulationPreloadingStep : SmoothPreloadingStep
	{
		private const int SimulationLoopInterval = 10;
		private long _startTime;

		public readonly SimulationStepConfig config;

		public SimulationPreloadingStep(SimulationStepConfig config, float weight) : base(weight)
		{
			this.config = config;
		}

		public override async Task Run()
		{
			_startTime = config.timeProvider.time;

			status = PreloadingTaskStatus.Running;
			var processingTask = Processing();
			var simulationTask = ProgressSimulation();
			await processingTask;

			if (config.fullSimulation) {
				await simulationTask;
			}

			CalculateProgress();

			if (status != PreloadingTaskStatus.Error) {
				status = PreloadingTaskStatus.Success;
			}
		}

		private async Task ProgressSimulation()
		{
			var isSimulate = true;
			while (isSimulate) {
				await Task.Delay(SimulationLoopInterval);
				CalculateProgress();
				isSimulate = config.fullSimulation
					? progress < 1
					: status == PreloadingTaskStatus.Running;
			}
		}

		private void CalculateProgress()
		{
			var elapsedTime = (config.timeProvider.time - _startTime) / 1000f;
			var currentProgress = Mathf.Clamp01(elapsedTime / config.duration);
			SetProgress(currentProgress);
		}
	}
}