using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class StateGraphFlow : AbstractForwardFlow
	{
		public StateGraphFlow(AbstractLogicNode node)
		{
			SetActiveNode(node);
		}
	}
}