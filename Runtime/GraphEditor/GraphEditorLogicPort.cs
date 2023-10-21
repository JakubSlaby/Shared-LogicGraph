#if LOGIC_GRAPH_EDITOR
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public static class LogicPortDirectionExtensions
	{
		public static GraphPortDirection ToGraphDirection(this LogicPortDirection direction)
		{
			return direction == LogicPortDirection.Input ? GraphPortDirection.Input : GraphPortDirection.Output;
		}
	}
	
	public abstract partial class AbstractLogicPort : IGraphPortData
	{
		IGraphNodeData IGraphPortData.Node => Node;

		GraphPortDirection IGraphPortData.Direction => Direction.ToGraphDirection();
	}
}
#endif