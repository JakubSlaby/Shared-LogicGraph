using System;
using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial interface IGraphStructure
	{
		AbstractLogicNode AddNode(AbstractLogicNode node);
		void RemoveNode(AbstractLogicNode node);
		
		T AddNode<T>(T node) where T : AbstractLogicNode;
		
		AbstractLogicConnection Connect(AbstractLogicNode from, AbstractLogicNode to, AbstractLogicConnection connection);
		AbstractLogicConnection Connect(AbstractLogicPort from, AbstractLogicPort to, AbstractLogicConnection connection);
		
		AbstractLogicNode[] GetAllNodes();
		AbstractTriggerNode[] GetAllTriggerNodes();
		AbstractLogicConnection[] GetAllConnections();
	}

	[Serializable]
	public abstract partial class AbstractGraphStructure : IGraphStructure
	{
		private List<AbstractLogicNode> m_Nodes = new List<AbstractLogicNode>();
		private AbstractLogicNode[] m_NodesCache;

		private List<AbstractTriggerNode> m_TriggerNodes = new List<AbstractTriggerNode>();
		private AbstractTriggerNode[] m_TriggerNodesCache;

		private List<AbstractLogicConnection> m_Connections = new List<AbstractLogicConnection>();
		private AbstractLogicConnection[] m_ConnectionsCache;


		AbstractLogicNode IGraphStructure.AddNode(AbstractLogicNode node)
		{
			return AddNode<AbstractLogicNode>(node);
		}

		public void RemoveNode(AbstractLogicNode node)
		{
			if (m_Nodes.Remove(node))
			{
				if (node is AbstractTriggerNode triggerNode && m_TriggerNodes.Remove(triggerNode))
					m_TriggerNodesCache = null;
				
				node.Structure = null;
				m_NodesCache = null;
			}
		}

		public T AddNode<T>(T node) where T : AbstractLogicNode
		{
			if (m_Nodes.Contains(node))
				return null;

			if (!node.HasPorts(LogicPortDirection.Input))
			{
				node.AddPort(new DefaultLogicPort("input", "Input", LogicPortDirection.Input, LogicPortType.Multiple));
			}

			if (!node.HasPorts(LogicPortDirection.Output))
			{
				node.AddPort(new DefaultLogicPort("output", "Output", LogicPortDirection.Output, LogicPortType.Multiple));
			}
			
			node.Structure = this;
			m_Nodes.Add(node);
			m_NodesCache = null;

			if (node is AbstractTriggerNode triggerNode)
			{
				m_TriggerNodes.Add(triggerNode);
				m_TriggerNodesCache = null;
			}
			
			return node;
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
			if(from.Direction == to.Direction) throw new Exception("Cannot Connect Ports. Same direction.");
			if(from.Node == to.Node) throw new Exception("Cannot Connect Ports. Same parent node.");
			// we provided the ports the other way around
			if (from.Direction == LogicPortDirection.Input && to.Direction == LogicPortDirection.Output)
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

		AbstractLogicNode[] IGraphStructure.GetAllNodes() => AllNodes;

		internal AbstractLogicNode[] AllNodes
		{
			get
			{
				if (m_NodesCache == null)
				{
					if (m_Nodes.Count == 0)
						m_NodesCache = new AbstractLogicNode[] { new EmptyGraphNode() };
					else
						m_NodesCache = m_Nodes.ToArray();
				}
				return m_NodesCache;
			}
		}

		AbstractTriggerNode[] IGraphStructure.GetAllTriggerNodes()
		{
			if (m_TriggerNodesCache == null)
				m_TriggerNodesCache = m_TriggerNodes.ToArray();
			return m_TriggerNodesCache;
		}

		AbstractLogicConnection[] IGraphStructure.GetAllConnections()
		{
			if (m_ConnectionsCache == null)
				m_ConnectionsCache = m_Connections.ToArray();
			return m_ConnectionsCache;
		}
	}
}