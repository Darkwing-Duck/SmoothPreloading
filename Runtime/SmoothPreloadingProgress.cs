using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SmoothPreloading
{
	public class SmoothPreloadingProgress : SmoothPreloadingStep
	{
		private List<ISmoothPreloadingStep> _queue = new List<ISmoothPreloadingStep>();
		private int _currentIndex = -1;
		private float _unrealizedWeight;
		private float _totalWeight;

		private ISmoothPreloadingStep currentStep =>
			_queue.Count > 0 ? _queue[_currentIndex] : null;

		public override float progress => _progress + GetActualStepProgress();

		public SmoothPreloadingProgress AddStep(ISmoothPreloadingStep step)
		{
			if (status == PreloadingTaskStatus.Running) {
				throw new Exception("You can't add new preloading tasks while running");
			}

			_queue.Add(step);
			return this;
		}

		protected override async Task Processing()
		{
			if (_queue.Count <= 0) {
				SetProgress(1);
				return;
			}

			if (!RunNextStep()) return;

			var isRunning = true;

			_totalWeight = GetTotalWeight();

			while (isRunning) {
				await Task.Delay(10);
				isRunning = !CheckForCompletion();
			}
		}

		private bool CheckForCompletion()
		{
			var isCompleted = true;
			switch (currentStep.status) {
				case PreloadingTaskStatus.Success: {
					var actualTaskProgress = GetActualStepProgress();
					var stepTargetProgress = (currentStep.weight + _unrealizedWeight) / _totalWeight;
					SetProgress(_progress + actualTaskProgress);
					_unrealizedWeight = 0;
					if (actualTaskProgress < stepTargetProgress) {
						_unrealizedWeight = (stepTargetProgress - actualTaskProgress) * _totalWeight;
					}

					isCompleted = !RunNextStep();
					break;
				}
				case PreloadingTaskStatus.Running: {
					Debug.Log($"[{this.GetType().Name}] - {progress.ToString("0.##")}");
					isCompleted = false;
					break;
				}
				case PreloadingTaskStatus.Error: {
					SetError(currentStep.errorReason);
					isCompleted = true;
					break;
				}
			}

			return isCompleted;
		}

		private float GetTotalWeight() =>
			_queue.Sum(i => i.weight);

		private float GetActualStepProgress()
		{
			if (status != PreloadingTaskStatus.Running) return 0;
			return (currentStep.weight + _unrealizedWeight) / _totalWeight * currentStep.progress;
		}

		private bool RunNextStep()
		{
			if (_currentIndex + 1 >= _queue.Count) return false;
			_currentIndex++;
			currentStep.Run();
			return true;
		}
	}
}