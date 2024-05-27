using System;
using System.Collections.Generic;
using WhiteSparrow.Shared.GraphEditor.Data;
using WhiteSparrow.Shared.GraphEditor.View;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphView : AbstractGraphViewContainer<CustomGraphView>
	{

		private HashSet<AbstractLogicGraph> m_TemporaryInstances = new HashSet<AbstractLogicGraph>();
		

		public void ShowGraph(Type graphType)
		{
			Cleanup();

			var instance = CreateInstance(graphType);
			base.ShowNestedGraph(instance);
		}
		
		
		public override void ShowNestedGraph(INestedGraphNodeData nestedGraphNode)
		{
			if (nestedGraphNode.NestedGraph == null)
			{
				var instance = CreateInstance(nestedGraphNode.NestedGraphType);
				base.ShowNestedGraph(instance);
				return;
			}
			
			base.ShowNestedGraph(nestedGraphNode);
		}

		private IGraphData CreateInstance(Type type)
		{
			var instance = (AbstractLogicGraph)Activator.CreateInstance(type);
			instance.Construct();

			m_TemporaryInstances.Add(instance);
			return instance;
		}

		protected override void OnGraphRemoved(IGraphData graph)
		{
			base.OnGraphRemoved(graph);

			if (graph is not AbstractLogicGraph alg)
				return;
			
			m_TemporaryInstances.Remove(alg);
		}
	}
}