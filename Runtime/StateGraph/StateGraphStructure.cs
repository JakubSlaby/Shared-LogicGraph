﻿using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class StateGraphStructure : AbstractGraphStructure
	{
		private AbstractLogicNode m_StartNode; 
		public AbstractLogicNode StartNode
		{
			get
			{
				return m_StartNode != null ? m_StartNode : m_Nodes[0];
			}
			set
			{
				if(value != null)
					AddNode(value);
				m_StartNode = value;
			}
		}
		
		protected override AbstractLogicConnection CreateDefaultConnectionInstance()
		{
			return new LogicConnection();
		}

	}
}