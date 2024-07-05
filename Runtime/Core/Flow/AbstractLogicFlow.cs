using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractLogicFlow
	{
		private LogicFlowWrapper m_FlowWrapper;
		public LogicFlowWrapper FlowWrapper => m_FlowWrapper;
		
		private List<AbstractLogicNode> m_ActiveNodes;
		private AbstractLogicNode[] m_ActiveNodesCache;
		
		public event Action<AbstractLogicFlow> onFlowComplete;
		
		public AbstractLogicFlow()
		{
			m_ActiveNodes = new List<AbstractLogicNode>();
		}

		internal void Activate(LogicFlowWrapper flowWrapper)
		{
			m_FlowWrapper = flowWrapper;
			OnActivate();
		}
		
		protected virtual void OnActivate()
		{}
		

		internal void Deactivate()
		{
			OnDeactivate();

			while (m_ActiveNodes.Count > 0)
			{
				RemoveActiveNode(m_ActiveNodes[0]);
			}
			
			m_FlowWrapper = null;
		}

		protected virtual void OnDeactivate()
		{}

		protected void SetActiveNode(AbstractLogicNode node)
		{
			if (node == null)
				return;
			
			if (m_ActiveNodes.Contains(node))
			{
				Debug.LogError($"Node {node} is already active");
				return;
			}

			if (node.State == LogicNodeState.Active)
			{
				Debug.LogError($"Node {node.GetType().Name} is being set as Active but it's state is already Active. Internal issue, forcing state reset.");
				node.ChangeState(LogicNodeState.Ended);
			}
			
			if(node.State == LogicNodeState.Ended)
				node.ChangeState(LogicNodeState.None);
			
			m_ActiveNodes.Add(node);
			m_EvaluationBuffer.Add(node);
			m_ActiveNodesCache = null;

			OnActiveNodeSet(node);
		}

		protected virtual void OnActiveNodeSet(AbstractLogicNode node)
		{
			
		}
		

		protected void RemoveActiveNode(AbstractLogicNode node)
		{
			if (!m_ActiveNodes.Contains(node))
			{
				Debug.LogError($"Node {node} is not active, can't exit");
				return;
			}

			if(node.State != LogicNodeState.Ended)
				node.ChangeState(LogicNodeState.Ended);
			
			m_ActiveNodes.Remove(node);
			m_ActiveNodesCache = null;
			
			OnActiveNodeRemoved(node);
		}
		
		protected virtual void OnActiveNodeRemoved(AbstractLogicNode node)
		{
			
		}

		protected void TransitionNodes(AbstractLogicConnection connection)
		{
			var fromPort = connection.From;
			var fromNode = fromPort.Node;

			if (!m_ActiveNodes.Contains(fromNode))
			{
				Debug.LogError($"Trying to transition from a not active node {fromNode}, connection {connection}. Stopping");
				return;
			}
			
			if(fromNode.State != LogicNodeState.Ended)
				fromNode.ChangeState(LogicNodeState.Ended);
			
			RemoveActiveNode(fromNode);

			if (fromPort is ParallelOutputPort)
			{
				foreach (var parallelConnection in fromPort.Connections)
				{
					var parallelToNode = parallelConnection.To?.Node;
					if(parallelToNode == null)
						continue;

					if (m_ActiveNodes.Contains(parallelToNode))
						continue;
					
					SetActiveNode(parallelToNode);
				}

				return;
			}
			
			var toPort = connection.To;
			var toNode = toPort.Node;

			if (m_ActiveNodes.Contains(toNode))
				return;
			
			SetActiveNode(toNode);
		}


		private List<AbstractLogicNode> m_EvaluationBuffer = new List<AbstractLogicNode>();
		public virtual void Update(float deltaTime)
		{
			m_EvaluationBuffer.Clear();
			m_EvaluationBuffer.AddRange(m_ActiveNodes);

			Evaluate(m_EvaluationBuffer, deltaTime);

			if (m_ActiveNodes.Count == 0)
			{
				CompleteFlow();
			}
		}

		protected virtual void Evaluate(List<AbstractLogicNode> buffer, float deltaTime)
		{
			while (buffer.Count > 0)
			{
				var node = buffer[0];
				buffer.RemoveAt(0);
				EvaluateNode(node, deltaTime);
			}
		}
		protected abstract void EvaluateNode(AbstractLogicNode node, float deltaTime);

		protected void CompleteFlow()
		{
			while (m_ActiveNodes.Count > 0)
			{
				RemoveActiveNode(m_ActiveNodes[0]);
			}
			
			onFlowComplete?.Invoke(this);
		}

		protected bool IsNodeActive(AbstractLogicNode node)
		{
			return m_ActiveNodes.Contains(node);
		}

		public AbstractLogicNode[] ActiveNodes
		{
			get
			{
				if (m_ActiveNodesCache == null)
					m_ActiveNodesCache = m_ActiveNodes.ToArray();
				return m_ActiveNodesCache;
			}
		}

	}
}