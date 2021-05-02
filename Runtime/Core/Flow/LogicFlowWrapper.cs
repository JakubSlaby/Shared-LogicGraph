using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class LogicFlowWrapper
	{
		private List<AbstractLogicFlow> m_Flows;

		public LogicFlowWrapper()
		{
			m_Flows = new List<AbstractLogicFlow>();
		}

		public void AddFlow(AbstractLogicFlow flow)
		{
			if (m_Flows.Contains(flow))
				return;
			
			m_Flows.Add(flow);
		}

		public void RemoveFlow(AbstractLogicFlow flow)
		{
			m_Flows.Remove(flow);
		}

		public void Update(float deltaTime)
		{
			foreach (var flow in m_Flows)
			{
				flow.Update(deltaTime);
			}
		}
	}
}