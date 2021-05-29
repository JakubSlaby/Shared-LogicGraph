using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract class AbstractForwardFlow : AbstractLogicFlow
	{
		private HashSet<AbstractLogicPort> m_InvokedPorts = new HashSet<AbstractLogicPort>();
		private HashSet<AbstractLogicConnection> m_InvokedConnections = new HashSet<AbstractLogicConnection>();

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
			var outputPorts = node.GetOutputPorts();
			foreach (var outputPort in outputPorts)
			{
				if (outputPort is IInvokedPort invokedPort)
					invokedPort.OnPortInvoked -= OnOutputPortInvoked;
				
				m_InvokedPorts.Remove(outputPort);
				
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
			if (m_InvokedConnections.Contains(connection) || m_InvokedPorts.Contains(connection.From))
				return;
			
			m_InvokedConnections.Add(connection);
			m_InvokedPorts.Add(connection.From);
		}

		private void OnOutputPortInvoked(AbstractLogicPort port)
		{
			if (m_InvokedPorts.Contains(port))
				return;
			
			m_InvokedPorts.Add(port);
		}

		private List<AbstractLogicNode> m_HelperEvaluatedTransitionCandidates = new List<AbstractLogicNode>();
		private List<AbstractLogicConnection> m_HelperEvaluatedConnections = new List<AbstractLogicConnection>();
		protected override void Evaluate(List<AbstractLogicNode> buffer, float deltaTime)
		{
			// evaluate nodes that are activated
			base.Evaluate(buffer, deltaTime);
			
			if(m_InvokedConnections.Count == 0 && m_InvokedPorts.Count == 0)
				return;
			
			// if we have queued invoked ports / connections, process those
			foreach (var connection in m_InvokedConnections)
			{
				if (m_HelperEvaluatedTransitionCandidates.Contains(connection.From.Node))
					continue;

				m_HelperEvaluatedTransitionCandidates.Add(connection.From.Node);
				m_HelperEvaluatedConnections.Add(connection);
				m_InvokedPorts.Remove(connection.From);
			}
			m_InvokedConnections.Clear();

			foreach (var port in m_InvokedPorts)
			{
				if(m_HelperEvaluatedTransitionCandidates.Contains(port.Node))
					continue;
				
				if(!port.HasDefaultConnections)
					continue;

				var connections = port.Connections;
				foreach (var connection in connections)
				{
					if (connection is IInvokedConnection)
						continue;

					m_HelperEvaluatedConnections.Add(connection);
					break;
				}
			}
			m_InvokedPorts.Clear();
			m_HelperEvaluatedTransitionCandidates.Clear();

			if (m_HelperEvaluatedConnections.Count > 0)
			{
				EvaluateConnections(m_HelperEvaluatedConnections);
				m_HelperEvaluatedConnections.Clear();
			}
			
		}

		
		protected virtual void EvaluateConnections(List<AbstractLogicConnection> buffer)
		{
			foreach (var connection in buffer)
			{
				TransitionNodes(connection);
			}
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

            if (node.State != LogicNodeState.Ended)
            	return;
			
			// TODO: When the node is evaluated, make sure we try to execute any pending transitions from THIS node at this time, not after all nodes are complete - this way we can nicely execute the next transitioned in node on the same frame, giving us an option to process multiple nodes in the flow instantly

			var outputPorts = node.GetOutputPorts();
			foreach (var outputPort in outputPorts)
			{
				if (outputPort.HasDefaultConnections)
				{
					m_InvokedPorts.Add(outputPort);
				}
			}
		}

	}
}