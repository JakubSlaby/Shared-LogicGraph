using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class LogicFlowWrapper
	{
		private List<AbstractLogicFlow> m_Flows;
		public IReadOnlyList<AbstractLogicFlow> Flows => m_Flows;
		
		
		public LogicFlowWrapper()
		{
			m_Flows = new List<AbstractLogicFlow>();
		}

		public void AddFlow(AbstractLogicFlow flow)
		{
			if (m_Flows.Contains(flow))
				return;
			
			flow.Activate(this);
			m_Flows.Add(flow);
			
		}

		public void RemoveFlow(AbstractLogicFlow flow)
		{
			m_Flows.Remove(flow);
			
			flow.Deactivate();
		}

		private List<AbstractLogicFlow> m_UpdateBuffer = new List<AbstractLogicFlow>();
		public void Update(float deltaTime)
		{
			m_UpdateBuffer.AddRange(m_Flows);
			try
			{
				foreach (var flow in m_UpdateBuffer)
				{
					flow.Update(deltaTime);
				}
			}
			finally
			{
				m_UpdateBuffer.Clear();
			}
		}
	}
}