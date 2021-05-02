using System;
using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public interface IGraphStructure
	{
		void AddNode(AbstractLogicNode node);
		void RemoveNode(AbstractLogicNode node);
		
		AbstractLogicConnection Connect(AbstractLogicNode from, AbstractLogicNode to, AbstractLogicConnection connection);
		AbstractLogicConnection Connect(AbstractLogicPort from, AbstractLogicPort to, AbstractLogicConnection connection);
		
		
		AbstractLogicNode[] GetAllNodes();
		AbstractLogicConnection[] GetAllConnections();
	}

	
	public abstract class AbstractGraphStructure : IGraphStructure
	{
		protected List<AbstractLogicNode> m_Nodes = new List<AbstractLogicNode>();
		protected AbstractLogicNode[] m_NodesCache;
		protected List<AbstractLogicConnection> m_Connections = new List<AbstractLogicConnection>();
		protected AbstractLogicConnection[] m_ConnectionsCache;
		
		public void AddNode(AbstractLogicNode node)
		{
			if (m_Nodes.Contains(node))
				return;

			if (!node.HasPorts(LogicPortDirection.Input))
			{
				node.AddPort(new InputPort());
			}

			if (!node.HasPorts(LogicPortDirection.Output))
			{
				node.AddPort(new OutputPort());
			}
			
			node.Structure = this;
			m_Nodes.Add(node);
			m_NodesCache = null;
		}

		public void RemoveNode(AbstractLogicNode node)
		{
			if (m_Nodes.Remove(node))
			{
				node.Structure = null;
				m_NodesCache = null;
			}
		}

		protected virtual AbstractLogicConnection CreateDefaultConnectionInstance()
		{
			return new LogicConnection();
		}

		public AbstractLogicConnection Connect(AbstractLogicNode from, AbstractLogicNode to)
		{
			return Connect(from, to, null);
		}
		
		public AbstractLogicConnection Connect(AbstractLogicNode from, AbstractLogicNode to, AbstractLogicConnection connection)
		{
			if(from == to)
				throw new Exception("Cannot Connect nodes. Same node.");

			var fromOutputPorts = from.GetOutputPorts();
			var toInputPorts = to.GetInputPorts();
			
			if(fromOutputPorts.Length == 0) throw new Exception("Cannot Connect nodes. No output ports.");
			if(toInputPorts.Length == 0) throw new Exception("Cannot Connect nodes. No input ports.");
			if(fromOutputPorts.Length > 1) throw new Exception("Cannot Connect nodes. More than 1 output port - cannot determine default.");
			if(toInputPorts.Length > 1) throw new Exception("Cannot Connect nodes. More than 1 input port - cannot determine default.");

			return Connect(fromOutputPorts[0], toInputPorts[0], connection);
		}

		public AbstractLogicConnection Connect(AbstractLogicPort from, AbstractLogicPort to, AbstractLogicConnection connection)
		{
			if(from == to) throw new Exception("Cannot Connect Ports. Same port.");
			if(from.direction == to.direction) throw new Exception("Cannot Connect Ports. Same direction.");
			if(from.Node == to.Node) throw new Exception("Cannot Connect Ports. Same parent node.");
			// we provided the ports the other way around
			if (from.direction == LogicPortDirection.Input && to.direction == LogicPortDirection.Output)
			{
				var t = from;
				from = to;
				to = t;
			}

			if (connection == null)
			{
				connection = CreateDefaultConnectionInstance();
			}

			connection.From = from;
			connection.To = to;
			
			from.AddConnection(connection);
			to.AddConnection(connection);
			AddConnection(connection);

			return connection;
		}


		internal void AddConnection(AbstractLogicConnection connection)
		{
			if (m_Connections.Contains(connection))
				return;
			
			m_Connections.Add(connection);
			m_ConnectionsCache = null;
		}

		internal void RemoveConnection(AbstractLogicConnection connection)
		{
			if (m_Connections.Remove(connection))
			{
				connection.From?.RemoveConnection(connection);
				connection.To?.RemoveConnection(connection);
				m_ConnectionsCache = null;
			}
		}
		
		AbstractLogicNode[] IGraphStructure.GetAllNodes()
		{
			if (m_NodesCache == null)
				m_NodesCache = m_Nodes.ToArray();
			return m_NodesCache;
		}

		AbstractLogicConnection[] IGraphStructure.GetAllConnections()
		{
			if (m_ConnectionsCache == null)
				m_ConnectionsCache = m_Connections.ToArray();
			return m_ConnectionsCache;
		}
	}
}