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
				return m_StartNode ?? (AllNodes.Length > 0 ? AllNodes[0] : null);
			}
			set
			{
				if(value != null)
					AddNode(value);
				m_StartNode = value;
			}
		}

		private AnyState m_AnyState;
		public AnyState AnyNode
		{
			get
			{
				if (m_AnyState == null)
					AddNode(m_AnyState = new AnyState());

				return m_AnyState;
			}
		}

		internal bool HasAnyStateFlow => m_AnyState != null;
		
		protected override AbstractLogicConnection CreateDefaultConnectionInstance()
		{
			return new LogicConnection();
		}

	}
}