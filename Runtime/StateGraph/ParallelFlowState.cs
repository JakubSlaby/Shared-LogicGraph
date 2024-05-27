using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class ParallelFlowState : AbstractState
	{
		public readonly ParallelOutputPort OutputPort;

		public ParallelFlowState()
		{
			AddPort(OutputPort = new ParallelOutputPort("out", "Parallel Tracks"));
		}
		public override void Execute()
		{
			End();
		}
	}
}