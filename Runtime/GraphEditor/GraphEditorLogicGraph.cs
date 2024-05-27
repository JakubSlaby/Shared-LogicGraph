#if LOGIC_GRAPH_EDITOR && UNITY_EDITOR
using System.Collections.Generic;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractLogicGraph : IGraphData, IFlowGraphData, IAutoLayoutGraphData
	{
		// IGraphData
		IReadOnlyCollection<IGraphNodeData> IGraphData.Nodes => structure.GetAllNodes();
		IReadOnlyCollection<IGraphEdgeData> IGraphData.Edges => structure.GetAllConnections();
		
		// IFlowGraphData
		public IReadOnlyCollection<IGraphFlowData> Flows => FlowWrapper.Flows;

		// IAutoLayoutGraphData
		#if UNITY_EDITOR
		Microsoft.Msagl.Core.Layout.GeometryGraph IAutoLayoutGraphData.ToMSAL()
		{
			return structure.ToMSAL();
		}
		#endif
	}
}
#endif