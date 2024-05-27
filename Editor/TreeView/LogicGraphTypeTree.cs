using UnityEditor.IMGUI.Controls;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphTypeTree : AbstractLogicGraphTreeView
	{
		
		public LogicGraphTypeTree(TreeViewState state) : base(state)
		{
		}


		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem(-1, -1);
			var types = LogicGraphEditorRegistry.GetAllGraphTypes();
			for(int i=0; i<types.Length; i++)
			{
				var item = new TreeViewItem(i, 0, types[i].Name);
				root.AddChild(item);
			}

			if (!root.hasChildren)
				root.AddChild(new TreeViewItem(0, 0, "No script graphs"));
			
			return root;
		}

		public override LogicGraphEditorWindowState.LogicGraphTreeView GraphType =>
			LogicGraphEditorWindowState.LogicGraphTreeView.Script;
	}
}