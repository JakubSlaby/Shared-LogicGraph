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

	public delegate void LogicGraphCallback(AbstractLogicGraph graph);
	
	public abstract partial class AbstractLogicGraph
	{
		private IGraphStructure m_GraphStructure;
		public IGraphStructure structure => m_GraphStructure;

		private LogicFlowWrapper m_FlowWrapper;
		protected LogicFlowWrapper FlowWrapper => m_FlowWrapper;

		private LogicGraphState m_State = LogicGraphState.None;
		public LogicGraphState state => m_State;

		private LogicGraphCallback m_OnGraphStart;
		private LogicGraphCallback m_OnGraphComplete;
		public event LogicGraphCallback onGraphStart
		{
			add { m_OnGraphStart += value; }
			remove { m_OnGraphStart -= value; }
		}

		public event LogicGraphCallback onGraphComplete
		{
			add { m_OnGraphComplete += value; }
			remove { m_OnGraphComplete -= value; }
		}

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

		internal void Construct()
		{
			ConstructGraph();
		}
		
		public void Initialize()
		{
			#if UNITY_EDITOR
			LogicGraphRuntimeRegistry.RegisterGraph(this);
			#endif
			
			Construct();
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
			if (m_State != LogicGraphState.Stopped && m_State != LogicGraphState.Complete)
				return;
			
			DoStart();

			m_State = LogicGraphState.Running;
			
			m_OnGraphStart?.Invoke(this);
		}

		protected abstract void DoStart();

		public void Update()
		{
			this.Update(Time.deltaTime);
		}

		public void Update(float deltaTime)
		{
			if (m_State != LogicGraphState.Running)
				return;
			
			m_FlowWrapper.Update(deltaTime);
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

			m_State = LogicGraphState.Running;
		}

		protected void CompleteGraph()
		{
			m_State = LogicGraphState.Complete;
			DoComplete();
			m_OnGraphComplete?.Invoke(this);
		}

		protected abstract void DoComplete();

#endregion

	}
}