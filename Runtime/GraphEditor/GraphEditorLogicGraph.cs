using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Layout;
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
		GeometryGraph IAutoLayoutGraphData.ToMSAL()
		{
			return structure.ToMSAL();
		}
	}
}