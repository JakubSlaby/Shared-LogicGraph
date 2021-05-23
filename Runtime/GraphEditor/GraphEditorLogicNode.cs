using System.Collections.Generic;
using UnityEngine;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractLogicNode : IGraphNodeData
	{
		private Rect m_Position;

		Rect IGraphNodeData.position
		{
			get => m_Position;
			set => m_Position = value;
		}

		IReadOnlyCollection<IGraphPortData> IGraphNodeData.InputPorts => GetInputPorts();

		IReadOnlyCollection<IGraphPortData> IGraphNodeData.OutputPorts => GetOutputPorts();
	}
}