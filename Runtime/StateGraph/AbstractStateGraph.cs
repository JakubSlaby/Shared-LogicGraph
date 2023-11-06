using UnityEngine;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public abstract class AbstractStateGraph : AbstractLogicGraph
	{
		private StateGraphStructure m_fsmStructure;
		public new StateGraphStructure structure => m_fsmStructure;
		protected AnyState AnyState => m_fsmStructure.AnyNode;
		
		protected sealed override IGraphStructure GetStructureInstance()
		{
			return m_fsmStructure = CreateStateGraphStructure();
		}

		protected virtual StateGraphStructure CreateStateGraphStructure()
		{
			return new StateGraphStructure();
		}

		protected override void DoConstructGraph()
		{
		}

		protected override void DoStart()
		{
			var startNode = m_fsmStructure.StartNode;
			if (startNode == null)
			{
				Debug.LogError($"StateGraph {this.GetType().Name} has no start node.");
				CompleteGraph();
				return;
			}
			
			var flow = new StateGraphFlow(m_fsmStructure.StartNode);
			flow.onFlowComplete += OnFlowComplete;
			FlowWrapper.AddFlow(flow);

			if (m_fsmStructure.HasAnyStateFlow && m_fsmStructure.AnyNode.OutputPort.HasConnections)
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