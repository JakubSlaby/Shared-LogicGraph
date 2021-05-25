using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public static class LogicGraphRuntimeRegistry
	{
#if UNITY_EDITOR
		private static List<WeakReference<AbstractLogicGraph>> m_LogicGraphWeakReferenceList = new List<WeakReference<AbstractLogicGraph>>();
#endif
		
		[Conditional("UNITY_EDITOR")]
		public static void RegisterGraph(AbstractLogicGraph graph)
		{
#if UNITY_EDITOR
			m_LogicGraphWeakReferenceList.Add(new WeakReference<AbstractLogicGraph>(graph));
#endif
		}

		public static IReadOnlyList<WeakReference<AbstractLogicGraph>> GetLogicGraphs()
		{
#if UNITY_EDITOR
			return m_LogicGraphWeakReferenceList;
#else
			return 	Array.Empty<WeakReference<AbstractLogicGraph>>();		
#endif
		}
	}
}