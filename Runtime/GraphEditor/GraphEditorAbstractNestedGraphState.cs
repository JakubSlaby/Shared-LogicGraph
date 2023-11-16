#if LOGIC_GRAPH_EDITOR && UNITY_EDITOR
using System;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	
	public abstract partial class AbstractNestedGraphState : INestedGraphNodeData
	{
		Type INestedGraphNodeData.NestedGraphType => this.NestedGraphType;

		IGraphData INestedGraphNodeData.NestedGraph => this.m_NestedGraph;
		public string NestedGraphDisplayName => NestedGraphType?.Name ?? "Unknown";

		public string NodeDisplayName => $"Nested Graph";
	}
}
#endif