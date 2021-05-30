using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract class AbstractForwardFlow : AbstractLogicFlow
	{
		// Nodes that were invoked for transition
		private HashSet<AbstractLogicNode> m_InvokedNodes = new HashSet<AbstractLogicNode>();
		private Dictionary<AbstractLogicNode, AbstractLogicPort> m_InvokedPorts = new Dictionary<AbstractLogicNode, AbstractLogicPort>();
		private Dictionary<AbstractLogicNode, AbstractLogicConnection> m_InvokedConnections = new Dictionary<AbstractLogicNode, AbstractLogicConnection>();
		
		protected override void OnActiveNodeSet(AbstractLogicNode node)
		{
			var outputPorts = node.GetOutputPorts();
			foreach (var outputPort in outputPorts)
			{
				if (outputPort is IInvokedPort invokedPort)
					invokedPort.OnPortInvoked += OnOutputPortInvoked;

				var connections = outputPort.Connections;
				foreach (var connection in connections)
				{
					if (connection is IInvokedConnection invokedConnection)
						invokedConnection.onConnectionInvoked += OnOutputConnectionInvoked;
					
					// cache default connection?
					
					if(connection is IActivateConnection activateConnection)
						activateConnection.Activate();
				}
			}
		}

		protected override void OnActiveNodeRemoved(AbstractLogicNode node)
		{
			m_InvokedNodes.Remove(node);
			m_InvokedPorts.Remove(node);
			m_InvokedConnections.Remove(node);
			
			var outputPorts = node.GetOutputPorts();
			foreach (var outputPort in outputPorts)
			{
				if (outputPort is IInvokedPort invokedPort)
					invokedPort.OnPortInvoked -= OnOutputPortInvoked;
				
				
				var connections = outputPort.Connections;
				foreach (var connection in connections)
				{
					if (connection is IInvokedConnection invokedConnection)
						invokedConnection.onConnectionInvoked -= OnOutputConnectionInvoked;
				}
			}
		}
		private void OnOutputConnectionInvoked(AbstractLogicConnection connection)
		{
			if (m_InvokedNodes.Contains(connection.From.Node))
				return;
			
			m_InvokedConnections.Add(connection.From.Node, connection);
			m_InvokedNodes.Add(connection.From.Node);
		}

		private void OnOutputPortInvoked(AbstractLogicPort port)
		{
			if (m_InvokedNodes.Contains(port.Node))
				return;
			
			m_InvokedPorts.Add(port.Node, port);
			m_InvokedNodes.Add(port.Node);
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
            	}
            }

            if (node.State == LogicNodeState.Active)
            {
            	if(node is IUpdateNode updateNode)
            		updateNode.Update(deltaTime);
            }

			// perform transition if the current node exit was invoked or was ended
			if (node.State == LogicNodeState.Ended || m_InvokedNodes.Contains(node))
			{
				EvaluateNodeTransition(node);
			}
		}

		private void EvaluateNodeTransition(AbstractLogicNode node)
		{
			// node outputs were not invoked and it is not ended
			if (!m_InvokedNodes.Contains(node) && node.State != LogicNodeState.Ended)
				return;

			if (m_InvokedNodes.Remove(node))
			{
				if (m_InvokedConnections.TryGetValue(node, out var connection))
				{
					m_InvokedConnections.Remove(node);
					EvaluateTransition(connection);
					return;
				}
				
				if(m_InvokedPorts.TryGetValue(node, out var port))
				{
					m_InvokedPorts.Remove(node);
					TransitionOut(port);
					return;
				}
			}

			if (node.State == LogicNodeState.Ended)
			{
				TransitionOut(node);
			}
		}

		protected virtual bool TransitionOut(AbstractLogicPort port)
		{
			if (!port.HasDefaultConnections)
				return false;

			var connections = port.Connections;
			foreach (var connection in connections)
			{
				if (connection is IInvokedConnection)
					continue;
				
				EvaluateTransition(connection);
				return true;
			}
			
			return false;
		}

		protected virtual bool TransitionOut(AbstractLogicNode node)
		{
			if (!node.HasPorts(LogicPortDirection.Output))
				return false;
			var outputPorts = node.GetOutputPorts();
			foreach (var outputPort in outputPorts)
			{
				if (!outputPort.HasDefaultConnections)
					continue;

				return TransitionOut(outputPort);
			}

			return false;
		}

		protected virtual void EvaluateTransition(AbstractLogicConnection connection)
		{
			TransitionNodes(connection);
		}

	}
}