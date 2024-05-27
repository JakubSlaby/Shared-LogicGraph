using System;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public abstract partial class AbstractNestedGraphState : AbstractState, IUpdateNode, IActivationNode
	{
		private AbstractLogicGraph m_NestedGraph;
		
		public virtual void Activate()
		{
			m_NestedGraph = CreateGraphInstance();
			m_NestedGraph.Initialize();
			m_NestedGraph.onGraphComplete += OnCompletedGraph;
			m_NestedGraph.Start();
		}

		protected abstract AbstractLogicGraph CreateGraphInstance();

		public virtual Type NestedGraphType => m_NestedGraph?.GetType() ?? null;

		public virtual void Deactivate()
		{
			m_NestedGraph.onGraphComplete -= OnCompletedGraph;
			m_NestedGraph = null;
		}
		
		public override void Execute()
		{
		}

		public virtual void Update(float deltaTime)
		{
			m_NestedGraph.Update(deltaTime);
		}
		
		protected virtual void OnCompletedGraph(AbstractLogicGraph graph)
		{
			m_NestedGraph.onGraphComplete -= OnCompletedGraph;
			End();
		}
	
	}
}