using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class NestedGraphState<T> : AbstractState, IUpdateNode, IActivationNode
		where T : AbstractLogicGraph, new()
	{
		private T m_NestedGraph;
	
		public void Activate()
		{
			m_NestedGraph = new T();
			m_NestedGraph.Initialize();
			m_NestedGraph.onGraphComplete += OnCompletedGraph;
			m_NestedGraph.Start();
		}

		public void Deactivate()
		{
			m_NestedGraph.onGraphComplete -= OnCompletedGraph;
			m_NestedGraph = null;
		}

		
		public override void Execute()
		{
		}

		public void Update(float deltaTime)
		{
			m_NestedGraph.Update(deltaTime);
		}
		
		private void OnCompletedGraph(AbstractLogicGraph graph)
		{
			m_NestedGraph.onGraphComplete -= OnCompletedGraph;
			End();
		}

	}
}