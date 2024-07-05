using UnityEditor.IMGUI.Controls;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class RuntimeGraphTree : AbstractLogicGraphTreeView
	{
		
		private TreeViewItem m_Root;
		public override TreeViewItem RootItem => m_Root;
		public RuntimeGraphTree(TreeViewState state) : base(state)
		{
			
		}
		

		protected override TreeViewItem BuildRoot()
		{
			m_Root = new TreeViewItem(-1, -1);

			var allLogicGraphs = LogicGraphRuntimeRegistry.GetLogicGraphs();

			for (int i = 0; i < allLogicGraphs.Count; i++)
			{
				var logicGraphReference = allLogicGraphs[i];
				if (!logicGraphReference.TryGetTarget(out var logicGraph))
					continue;
				m_Root.AddChild(new TreeViewItem(i,0, logicGraph.GetType().Name));
			}
			
			if(!m_Root.hasChildren)
				m_Root.AddChild(new TreeViewItem(0, 0, "No runtime graphs"));
			return m_Root;
		}


		public override LogicGraphEditorWindowState.LogicGraphTreeView GraphType =>
			LogicGraphEditorWindowState.LogicGraphTreeView.Runtime;
	}
}