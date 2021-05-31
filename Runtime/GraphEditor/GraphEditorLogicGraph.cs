using System.Collections.Generic;
using Microsoft.Msagl.Core.Layout;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractLogicGraph : IGraphData, IAutoLayoutGraphData
	{
		IReadOnlyCollection<IGraphNodeData> IGraphData.Nodes => structure.GetAllNodes();
		IReadOnlyCollection<IGraphEdgeData> IGraphData.Edges => structure.GetAllConnections();
		public IReadOnlyCollection<IGraphFlowData> Flows => FlowWrapper.Flows;

		GeometryGraph IAutoLayoutGraphData.ToMSAL()
		{
			return structure.ToMSAL();
		}
	}
}