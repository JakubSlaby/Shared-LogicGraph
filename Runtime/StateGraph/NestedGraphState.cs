using System;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public class NestedGraphState<T> : AbstractNestedGraphState
		where T : AbstractLogicGraph, new()
	{
		protected override AbstractLogicGraph CreateGraphInstance()
		{
			return new T();
		}

		public override Type NestedGraphType => typeof(T);
	}
}