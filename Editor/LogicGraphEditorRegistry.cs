using System;
using System.Collections.Generic;
using UnityEditor;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public static class LogicGraphEditorRegistry
	{
		private static Type[] s_AllApplicableTypes;
		
		internal static void RefreshTypeCache()
		{
			var logicGraphs = TypeCache.GetTypesDerivedFrom<AbstractLogicGraph>();

			List<Type> output = new List<Type>();
			foreach (var type in logicGraphs)
			{
				if(type.IsAbstract)
					continue;
				
				output.Add(type);
			}

			s_AllApplicableTypes = output.ToArray();
		}
		
		internal static Type[] GetAllGraphTypes()
		{
			if(s_AllApplicableTypes == null)
				RefreshTypeCache();
			return s_AllApplicableTypes;
		}
	}
}