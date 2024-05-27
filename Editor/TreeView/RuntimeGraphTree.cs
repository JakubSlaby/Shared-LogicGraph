using UnityEditor.IMGUI.Controls;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class RuntimeGraphTree : AbstractLogicGraphTreeView
	{
		public RuntimeGraphTree(TreeViewState state) : base(state)
		{
			
		}
		

		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem(-1, -1);

			var allLogicGraphs = LogicGraphRuntimeRegistry.GetLogicGraphs();

			for (int i = 0; i < allLogicGraphs.Count; i++)
			{
				var logicGraphReference = allLogicGraphs[i];
				if (!logicGraphReference.TryGetTarget(out var logicGraph))
					continue;
				root.AddChild(new TreeViewItem(i,0, logicGraph.GetType().Name));
			}
			
			if(!root.hasChildren)
				root.AddChild(new TreeViewItem(0, 0, "No runtime graphs"));
			return root;
		}


		public override LogicGraphEditorWindowState.LogicGraphTreeView GraphType =>
			LogicGraphEditorWindowState.LogicGraphTreeView.Runtime;
	}
}