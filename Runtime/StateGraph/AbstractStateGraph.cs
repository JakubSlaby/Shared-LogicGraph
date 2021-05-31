﻿using UnityEngine;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public abstract class AbstractStateGraph : AbstractLogicGraph
	{
		private StateGraphStructure m_fsmStructure;
		protected AnyState AnyState => m_fsmStructure.AnyNode;
		
		protected override IGraphStructure GetStructureInstance()
		{
			return m_fsmStructure = new StateGraphStructure();
		}

		protected override void DoStart()
		{
			var flow = new StateGraphFlow(m_fsmStructure.StartNode);
			flow.onFlowComplete += OnFlowComplete;
			FlowWrapper.AddFlow(flow);

			if (m_fsmStructure.AnyNode != null && m_fsmStructure.AnyNode.OutputPort.HasConnections)
			{
				var anyFlow = new AnyStateFlow(m_fsmStructure.AnyNode);
				FlowWrapper.AddFlow(anyFlow);
			}
		}

		private void OnFlowComplete(AbstractLogicFlow flow)
		{
			flow.onFlowComplete -= OnFlowComplete;
			CompleteGraph();
		}

		protected override void DoUpdate()
		{
			FlowWrapper.Update(Time.deltaTime);
		}

		protected override void DoStop()
		{
			
		}

		protected override void DoComplete()
		{
			
		}
	}


}