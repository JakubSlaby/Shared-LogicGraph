 #if LOGIC_GRAPH_EDITOR && UNITY_EDITOR
using System;
using System.Collections.Generic;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial class AbstractLogicFlow : IGraphFlowData
	{
		IReadOnlyCollection<IGraphNodeData> IGraphFlowData.ActiveNodes => m_ActiveNodes;
		IReadOnlyCollection<IGraphEdgeData> IGraphFlowData.ActiveEdges => Array.Empty<IGraphEdgeData>();
	}
}
#endif