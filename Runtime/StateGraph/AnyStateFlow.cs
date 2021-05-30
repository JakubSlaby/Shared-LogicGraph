using System.Collections.Generic;
using System.Linq;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class AnyStateFlow : AbstractForwardFlow
	{
		public AnyStateFlow(AnyState anyState)
		{
			SetActiveNode(anyState);
		}

		protected override void EvaluateTransition(AbstractLogicConnection connection)
		{
			var stateGraphFlow = FlowWrapper.Flows.FirstOrDefault(flow => flow is StateGraphFlow);
			if (stateGraphFlow != null)
			{
				FlowWrapper.RemoveFlow(stateGraphFlow);
			}

			var flow = new StateGraphFlow(connection.To.Node);
			FlowWrapper.AddFlow(flow);
		}
	}
}