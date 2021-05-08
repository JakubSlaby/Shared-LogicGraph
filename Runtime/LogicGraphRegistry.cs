using System;
using System.Collections.Generic;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public static class LogicGraphRegistry
	{
		private static List<WeakReference<AbstractLogicGraph>> m_LogicGraphWeakReferenceList = new List<WeakReference<AbstractLogicGraph>>();

		public static void RegisterGraph(AbstractLogicGraph graph)
		{
			m_LogicGraphWeakReferenceList.Add(new WeakReference<AbstractLogicGraph>(graph));
		}

		public static IReadOnlyList<WeakReference<AbstractLogicGraph>> GetLogicGraphs()
		{
			return m_LogicGraphWeakReferenceList;
		}
	}
}