using UnityEngine;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public enum LogicGraphState
	{
		None,
		Stopped,
		Running,
		Paused,
		Complete
	}
	
	public abstract class AbstractLogicGraph
	{
		private IGraphStructure m_GraphStructure;
		public IGraphStructure structure => m_GraphStructure;

		private LogicFlowWrapper m_FlowWrapper;
		protected LogicFlowWrapper flowWrapper => m_FlowWrapper;

		private LogicGraphState m_State = LogicGraphState.None;
		public LogicGraphState state => m_State;
		
		#region Structure
		
		private void ConstructGraph()
		{
			m_FlowWrapper = new LogicFlowWrapper();
			if(m_GraphStructure == null)
				m_GraphStructure = GetStructureInstance();
			
			DoConstructGraph();
		}

		protected abstract IGraphStructure GetStructureInstance();
		protected abstract void DoConstructGraph();

		#endregion

		
		public void Initialize()
		{
			ConstructGraph();
			InitializeNodes();
			InitializeConnections();

			m_State = LogicGraphState.Stopped;
		}


		private void InitializeNodes()
		{
			var allNodes = m_GraphStructure.GetAllNodes();
			foreach (var node in allNodes)
				if(node is IInitializeNode initializeNode)
					initializeNode.Initialize();
		}
		
		private void InitializeConnections()
		{
			var allConnections = m_GraphStructure.GetAllConnections();
			foreach (var connection in allConnections)
				if(connection is IInitializeConnection initializeConnection)
					initializeConnection.Initialize();
		}


#region Flow

		
		public void Start()
		{
			if (m_State != LogicGraphState.Stopped)
				return;
			
			DoStart();

			m_State = LogicGraphState.Running;
		}

		protected abstract void DoStart();

		public void Update()
		{
			if (m_State != LogicGraphState.Running)
				return;
			
			m_FlowWrapper.Update(Time.deltaTime);
			DoUpdate();
		}

		protected abstract void DoUpdate();
		public void Stop()
		{
			if (m_State == LogicGraphState.Running || m_State == LogicGraphState.Paused)
			{
				DoStop();
				m_State = LogicGraphState.Stopped;
			}
		}

		protected abstract void DoStop();

		public void Resume()
		{
			if (m_State != LogicGraphState.Paused)
				return;
		}
		

#endregion

	}
}