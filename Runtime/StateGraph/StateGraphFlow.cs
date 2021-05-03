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
		private AbstractLogicConnection m_ActiveDefaultConnection = null;
		private List<AbstractLogicConnection> m_ActiveOutputConnections = new List<AbstractLogicConnection>();

		private AbstractLogicConnection m_InvokedConnection;

		protected override void OnActiveNodeSet(AbstractLogicNode node)
		{
			m_InvokedConnection = null;
			
			
			var outputPorts = node.GetOutputPorts();
			m_ActiveOutputPorts.AddRange(outputPorts);
			foreach (var port in outputPorts)
			{
				var connections = port.Connections;
				foreach (var connection in connections)
				{
					if (connection is IInvokedConnection invokedConnection)
						invokedConnection.onConnectionInvoked += OnOutputConnectionInvoked;
					else if (m_ActiveDefaultConnection == null)
						m_ActiveDefaultConnection = connection;
					
					if(connection is IActivateConnection activateConnection)
						activateConnection.Activate();
				}
				m_ActiveOutputConnections.AddRange(connections);
			}
		}

		protected override void OnActiveNodeRemoved(AbstractLogicNode node)
		{
			ActivePortsStopListening();
			ActiveConnectionsStopListening();
		}

		private void ActivePortsStopListening()
		{
			m_ActiveDefaultConnection = null;
			if (m_ActiveOutputPorts.Count == 0)
				return;

			foreach (var port in m_ActiveOutputPorts)
			{
				if (port is IInvokedPort invokedPort)
					invokedPort.onPortInvoked += OnOutputPortInvoked;
			}
			m_ActiveOutputPorts.Clear();
		}
		private void ActiveConnectionsStopListening()
		{
			if(m_ActiveOutputConnections.Count == 0)
				return;

			foreach (var connection in m_ActiveOutputConnections)
			{
				if (connection is IInvokedConnection invokedConnection)
					invokedConnection.onConnectionInvoked -= OnOutputConnectionInvoked;
				if(connection is IActivateConnection activateConnection)
					activateConnection.Deactivate();
			}
			m_ActiveOutputConnections.Clear();
		}

		private void OnOutputConnectionInvoked(AbstractLogicConnection connection)
		{
			ActivePortsStopListening();
			ActiveConnectionsStopListening();
			
			m_InvokedConnection = connection;
		}

		private void OnOutputPortInvoked(AbstractLogicPort port)
		{
			// When an invoked port doesn't have a default logic connection, we will wait for any of the invoked connections to trigger
			AbstractLogicConnection defaultConnection = null;
			foreach (var connection in port.Connections)
			{
				if (connection is IInvokedConnection)
					continue;
				
				defaultConnection = connection;
				break;
			}

			if (defaultConnection == null)
				return;
			
			m_InvokedConnection = defaultConnection;
			
			ActivePortsStopListening();
			ActiveConnectionsStopListening();
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


			AbstractLogicConnection connection = m_InvokedConnection;
			if (connection == null && m_ActiveDefaultConnection != null)
			{
				connection = m_ActiveDefaultConnection;
			}
			m_InvokedConnection = null;

			if (connection == null)
			{
				if (m_ActiveOutputConnections.Count > 0)
					CompleteFlow();
				return;
			}

			TransitionNodes(connection);
		}
		
		
	}
}