using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public abstract class AbstractState : AbstractLogicNode
	{
		protected virtual void End()
		{
			ChangeState(LogicNodeState.Ended);
		}
	}
}