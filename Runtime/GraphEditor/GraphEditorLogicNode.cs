#if LOGIC_GRAPH_EDITOR
using System;
using System.Collections.Generic;
using Plugins.Repositories.GraphEditor.Runtime.Utils;
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
		
		// IGraphDataSource
		string IGraphDataSource.GetScriptPath()
		{
			return GraphDataSource.GetSourcePath();
		}
		
		
		float IGraphNodeData.TimeActivated => m_TimeActivated;
		float IGraphNodeData.TimeEnded => m_TimeEnded;
	}

}
#endif