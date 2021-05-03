using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract class AbstractLogicFlow
	{
		private List<AbstractLogicNode> m_ActiveNodes;
		private Dictionary<AbstractLogicNode, Task> m_ActiveAsyncTasks;

		public event Action<AbstractLogicFlow> onFlowComplete;
		
		public AbstractLogicFlow()
		{
			m_ActiveNodes = new List<AbstractLogicNode>();
			m_ActiveAsyncTasks = new Dictionary<AbstractLogicNode, Task>();
		}

		protected void SetActiveNode(AbstractLogicNode node)
		{
			if (m_ActiveNodes.Contains(node))
			{
				Debug.LogError($"Node {node} is already active");
				return;
			}
			
			m_ActiveNodes.Add(node);
			m_EvaluationBuffer.Add(node);
			
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

			m_ActiveNodes.Remove(node);
			
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
			var toPort = connection.To;
			var toNode = toPort.Node;

			if (m_ActiveNodes.Contains(toNode))
			{
				Debug.LogError($"Trying to transition To an active node {toNode}, from node {fromNode}, connection {connection}. Stopping.");
				return;
			}
			
			RemoveActiveNode(fromNode);
			SetActiveNode(toNode);
		}

		protected bool IsTrackedAsynchronously(AbstractLogicNode node)
		{
			return m_ActiveAsyncTasks.ContainsKey(node);
		}
		protected async void TrackAsyncNode(AbstractLogicNode node, Task task)
		{
			m_ActiveAsyncTasks.Add(node, task);
			await task;
			m_ActiveAsyncTasks.Remove(node);
		}

		private List<AbstractLogicNode> m_EvaluationBuffer = new List<AbstractLogicNode>();
		public virtual void Update(float deltaTime)
		{
			m_EvaluationBuffer.Clear();
			m_EvaluationBuffer.AddRange(m_ActiveNodes);

			while (m_EvaluationBuffer.Count > 0)
			{
				var node = m_EvaluationBuffer[0];
				m_EvaluationBuffer.RemoveAt(0);
				EvaluateNode(node, deltaTime);
			}

			if (m_ActiveNodes.Count == 0)
			{
				CompleteFlow();
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


	}
}