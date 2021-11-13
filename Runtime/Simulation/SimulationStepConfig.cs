namespace SmoothPreloading.Simulation
{
	public class SimulationStepConfig
	{
		public readonly float duration;
		public readonly bool fullSimulation;
		public readonly ISimulationTimeProvider timeProvider;

		public SimulationStepConfig(float duration, bool fullSimulation, ISimulationTimeProvider timeProvider)
		{
			this.duration = duration;
			this.fullSimulation = fullSimulation;
			this.timeProvider = timeProvider;
		}

		public SimulationStepConfig(float duration, bool fullSimulation = false)
			: this(duration, fullSimulation, new SimulationTimestampProvider()) { }
	}
}