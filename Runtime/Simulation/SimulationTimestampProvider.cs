using System;

namespace SmoothPreloading.Simulation
{
	public class SimulationTimestampProvider : ISimulationTimeProvider
	{
		private static readonly DateTime ZeroTime = new DateTime(1970, 1, 1);
		public long time => (long)DateTime.UtcNow.Subtract(ZeroTime).TotalMilliseconds;
	}
}