using System.Threading.Tasks;

namespace SmoothPreloading
{
	public enum PreloadingTaskStatus
	{
		Idle, Running, Success, Error
	}

	public interface ISmoothPreloadingStep
	{
		PreloadingTaskStatus status { get; }
		float progress { get; }
		float weight { get; }
		string errorReason { get; }

		Task Run();
	}
}