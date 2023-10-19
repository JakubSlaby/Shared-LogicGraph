#if LOGIC_GRAPH_EDITOR
using System.Collections.Generic;
using Plugins.Repositories.GraphEditor.Runtime.Utils;
using UnityEditor;
using UnityEngine;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractLogicNode : IGraphNodeData, IGraphDataSource
	{
		// IGraphNodeData
		
		private Rect m_Position;

		Rect IGraphNodeData.position
		{
			get => m_Position;
			set => m_Position = value;
		}

		IReadOnlyCollection<IGraphPortData> IGraphNodeData.InputPorts => GetInputPorts();

		IReadOnlyCollection<IGraphPortData> IGraphNodeData.OutputPorts => GetOutputPorts();
		
#if UNITY_EDITOR
		// IGraphDataSource
		string IGraphDataSource.GetScriptPath()
		{
			return GraphDataSource.GetSourcePath();
		}
#endif
	}
}
#endif