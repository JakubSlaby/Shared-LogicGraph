﻿using System;
using System.Collections.Generic;

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

	public abstract class AbstractLogicPort<T> : AbstractLogicPort
		where T : struct
	{
		public T Result { get; private set; }
		
		protected AbstractLogicPort(string id, string label, LogicPortDirection direction, LogicPortType type) : base(id, label, direction, type)
		{
		}

		public void Invoke(T result)
		{
			Result = result;
		}
	}
	
	public abstract class AbstractLogicPort : ILogicPort
	{
		public string id { get; private set; }
		public string label { get; private set; }
		public LogicPortDirection direction { get; private set; }
		public LogicPortType type { get; private set; }
		
		public AbstractLogicNode Node { get; internal set; }
		internal IGraphStructure Structure => Node?.Structure ?? null;
		
		public AbstractLogicPort(string id, string label, LogicPortDirection direction, LogicPortType type)
		{
			this.id = id;
			this.label = label;
			this.direction = direction;
			this.type = type;
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
			
			if(m_Connections.Contains(connection))
				m_Connections.Add(connection);
		}

		internal void RemoveConnection(AbstractLogicConnection connection)
		{
			m_Connections.Remove(connection);
		}
	}

	public interface ILogicPort
	{
		string id { get; }
		string label { get; }
		LogicPortDirection direction { get;  }
		LogicPortType type { get; }
		
		AbstractLogicNode Node { get; }
		
		bool HasConnections { get; }
		AbstractLogicConnection[] Connections { get; }
		
	}

}