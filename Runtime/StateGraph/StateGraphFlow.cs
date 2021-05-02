using System.Collections.Generic;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class StateGraphFlow : AbstractLogicFlow
	{
		public StateGraphFlow(AbstractLogicNode node)
		{
			SetActiveNode(node);
		}

		private List<AbstractLogicPort> m_ActiveOutputPorts = new List<AbstractLogicPort>();
		private List<AbstractLogicConnection> m_ActiveLogicConnections = new List<AbstractLogicConnection>();

		protected override void OnActiveNodeSet(AbstractLogicNode node)
		{
			var outputPorts = node.GetOutputPorts();
			m_ActiveOutputPorts.AddRange(outputPorts);
			foreach (var port in outputPorts)
			{
				var connections = port.Connections;
				foreach (var connection in connections)
				{
					if (connection is IInvokedConnection invokedConnection)
						invokedConnection.onConnectionInvoked += OnConnectionInvoked;
					if(connection is IActivateConnection activateConnection)
						activateConnection.Activate();
				}
				m_ActiveLogicConnections.AddRange(connections);
			}
		}

		protected override void OnActiveNodeRemoved(AbstractLogicNode node)
		{
			foreach (var connection in m_ActiveLogicConnections)
			{
				if (connection is IInvokedConnection invokedConnection)
					invokedConnection.onConnectionInvoked -= OnConnectionInvoked;
				if(connection is IActivateConnection activateConnection)
					activateConnection.Deactivate();
			}
			m_ActiveLogicConnections.Clear();
		}

		private void OnConnectionInvoked(IInvokedConnection obj)
		{
		}

		protected override void EvaluateNode(AbstractLogicNode node, float deltaTime)
		{
			if(node.State == LogicNodeState.None)
			{
				if (node is IActivationNode activationNode)
					activationNode.Activate();

				// set this node to active
				node.ChangeState(LogicNodeState.Active);
				
				// execute the single call
				if (node.State == LogicNodeState.Active)
				{
					if(node is IExecuteNode executeNode)
						executeNode.Execute();
					if (node is IExecuteAsyncNode executeAsyncNode)
					{
						TrackAsyncNode(node, executeAsyncNode.Execute());
					}
				}
			}

			if (node.State == LogicNodeState.Active)
			{
				if(node is IUpdateNode updateNode)
					updateNode.Update(deltaTime);
			}

			if (node.State != LogicNodeState.Ended)
				return;

			if (IsTrackedAsynchronously(node))
				return;
			
			
		}
	}
}