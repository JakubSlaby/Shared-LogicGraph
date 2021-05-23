using System;
using System.Collections.Generic;
using System.Dynamic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public enum LogicPortDirection
	{
		Input = 0,
		Output = 1
	}

	public enum LogicPortType
	{
		Single = 0,
		Multiple = 1
	}

	

	internal class DefaultLogicPort : AbstractLogicPort
	{
		public DefaultLogicPort(string id, string label, LogicPortDirection direction, LogicPortType type) : base(id, label, direction, type)
		{
		}
	}
	
	public abstract partial class AbstractLogicPort : ILogicPort
	{
		public string Id { get; private set; }
		public string Label { get; private set; }
		public LogicPortDirection Direction { get; private set; }
		public LogicPortType Type { get; private set; }
		
		public AbstractLogicNode Node { get; internal set; }
		internal IGraphStructure Structure => Node?.Structure ?? null;
		
		public AbstractLogicPort(string id, string label, LogicPortDirection direction, LogicPortType type)
		{
			this.Id = id;
			this.Label = label;
			this.Direction = direction;
			this.Type = type;
		}

		private List<AbstractLogicConnection> m_Connections;
		private AbstractLogicConnection[] m_ConnectionsCache;

		public bool HasConnections => m_Connections?.Count > 0;
		public AbstractLogicConnection[] Connections
		{
			get
			{
				if (m_ConnectionsCache == null)
					m_ConnectionsCache = m_Connections?.ToArray() ?? Array.Empty<AbstractLogicConnection>();
				return m_ConnectionsCache;
			}
		}

		internal void AddConnection(AbstractLogicConnection connection)
		{
			if (connection.From != this && connection.To != this)
				return;
			
			if(m_Connections == null)
				m_Connections = new List<AbstractLogicConnection>();
			
			if(!m_Connections.Contains(connection))
				m_Connections.Add(connection);

			m_ConnectionsCache = null;
		}

		internal void RemoveConnection(AbstractLogicConnection connection)
		{
			m_Connections.Remove(connection);
			m_ConnectionsCache = null;
		}
	}

	public interface ILogicPort
	{
		string Id { get; }
		string Label { get; }
		LogicPortDirection Direction { get;  }
		LogicPortType Type { get; }
		
		AbstractLogicNode Node { get; }
		
		bool HasConnections { get; }
		AbstractLogicConnection[] Connections { get; }
	}

	public interface IInvokedPort : ILogicPort
	{
		void Invoke();
		event Action<AbstractLogicPort> OnPortInvoked;
	}

}