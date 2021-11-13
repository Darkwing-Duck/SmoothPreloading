using System.Collections;
using System.Threading.Tasks;

namespace SmoothPreloading.Editor.Tests
{
	public abstract class AsyncTest
	{
		protected IEnumerator ExecuteAsync(Task task)
		{
			while (!task.IsCompleted) yield return null;
			if (task.IsFaulted) throw task.Exception;
		}
	}
}