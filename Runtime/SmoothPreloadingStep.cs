using System.Threading.Tasks;

namespace SmoothPreloading
{
	public abstract class SmoothPreloadingStep : ISmoothPreloadingStep
	{
		public delegate float EaseFunction(float time);
		private static readonly EaseFunction LinearEaseFunction = time => time;

		public PreloadingTaskStatus status { get; protected set; }
		public string errorReason { get; private set; }
		public float weight { get; }
		public virtual float progress => _progress;
		protected float _progress;
		private EaseFunction _easeFunction = LinearEaseFunction;

		public SmoothPreloadingStep()
		{
			status = PreloadingTaskStatus.Idle;
			weight = 1;
		}

		public SmoothPreloadingStep(float weight)
		{
			this.weight = weight;
		}

		public virtual async Task Run()
		{
			status = PreloadingTaskStatus.Running;
			await Processing();
			if (status != PreloadingTaskStatus.Error) {
				status = PreloadingTaskStatus.Success;
			}
		}

		public void SetEase(EaseFunction ease) =>
			_easeFunction = ease ?? _easeFunction;

		protected void SetProgress(float value) =>
			_progress = _easeFunction(value);

		protected void SetError(string reason)
		{
			status = PreloadingTaskStatus.Error;
			errorReason = reason;
		}

		protected virtual Task Processing()
		{
			_progress = 1;
			return Task.CompletedTask;
		}
	}
}