using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial class AbstractLogicConnection : IGraphEdgeData

	{
		IGraphPortData IGraphEdgeData.input => To;
		IGraphPortData IGraphEdgeData.output => From;
	}
}