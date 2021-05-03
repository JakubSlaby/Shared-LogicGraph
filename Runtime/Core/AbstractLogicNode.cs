﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public enum LogicNodeState
	{
		None,
		Active,
		Ended
	}
	
	public abstract class AbstractLogicNode
	{
		private List<AbstractLogicPort> m_InputPorts;
		private AbstractLogicPort[] m_InputPortsCache;
		private List<AbstractLogicPort> m_OutputPorts;
		private AbstractLogicPort[] m_OutputPortsCache;

		private LogicNodeState m_State = LogicNodeState.None;
		public LogicNodeState State => m_State;
		
		internal IGraphStructure Structure { get; set; }

		internal void ChangeState(LogicNodeState targetState)
		{
			m_State = targetState;
		}

#region Connection Ports
		
		protected internal void AddPort(AbstractLogicPort port)
		{
			if (CheckPortDuplicate(port))
			{
				Debug.LogError($"Port with that Id={port.id} has already been assigned in node type={this.GetType().Name}");
				return;
			}

			port.Node = this;
			
			if (port.direction == LogicPortDirection.Input)
			{
				if(m_InputPorts == null)
					m_InputPorts = new List<AbstractLogicPort>();
				m_InputPorts.Add(port);
				m_InputPortsCache = null;
			}
			else if (port.direction == LogicPortDirection.Output)
			{
				if(m_OutputPorts == null)
					m_OutputPorts = new List<AbstractLogicPort>();
				m_OutputPorts.Add(port);
				m_OutputPortsCache = null;
			}
		}

		protected internal void RemovePort(AbstractLogicPort port)
		{
			if (port.Node != this)
				return;

			port.Node = null;
			if (m_InputPorts != null && m_InputPorts.Contains(port))
				m_InputPorts.Remove(port);
			if (m_OutputPorts != null && m_OutputPorts.Contains(port))
				m_OutputPorts.Remove(port);
		}

		private bool CheckPortDuplicate(AbstractLogicPort port)
		{
			var list = port.direction == LogicPortDirection.Input ? m_InputPorts : m_OutputPorts;
			if (list == null)
				return false;
			
			foreach (var existingPort in list)
			{
				if (existingPort.id == port.id)
					return true;
			}

			return false;
		}

		public AbstractLogicPort[] GetPorts(LogicPortDirection direction)
		{
			switch (direction)
			{
				case LogicPortDirection.Input:
					return GetInputPorts();
				case LogicPortDirection.Output:
					return GetOutputPorts();
			}
		
			return Array.Empty<AbstractLogicPort>();
		}

		public bool HasPorts(LogicPortDirection direction)
		{
			switch (direction)
			{
				case LogicPortDirection.Input:
					return m_InputPorts?.Count > 0;
				case LogicPortDirection.Output:
					return m_OutputPorts?.Count > 0;
				default:
					return false;
			}
		}
		
		public AbstractLogicPort[] GetInputPorts()
		{
			if (m_InputPortsCache == null)
				m_InputPortsCache = m_InputPorts?.ToArray() ?? Array.Empty<AbstractLogicPort>();
			return m_InputPortsCache;
		}
		public AbstractLogicPort[] GetOutputPorts()
		{
			if (m_OutputPortsCache == null)
				m_OutputPortsCache = m_OutputPorts?.ToArray() ?? Array.Empty<AbstractLogicPort>();
			return m_OutputPortsCache;
		}

		#endregion
	}

	public interface IInitializeNode
	{
		void Initialize();
	}
	
	public interface IExecuteNode
	{
		void Execute();
	}

	public interface IActivationNode
	{
		void Activate();
		void Deactivate();
	}

	public interface IUpdateNode
	{
		void Update(float deltaTime);
	}

}