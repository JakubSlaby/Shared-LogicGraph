using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class AnyState : AbstractState
	{
		public readonly OutputPort OutputPort;

		public AnyState()
		{
			AddPort(OutputPort = new OutputPort("Out", null, LogicPortType.Multiple));
		}
		
		public override void Execute()
		{
			
		}
	}
}