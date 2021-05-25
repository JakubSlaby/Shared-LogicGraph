using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphTypeTree : TreeView
	{
		public event Action OnSelectionChanged;
		
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
			
			return root;
		}

		protected override void SelectionChanged(IList<int> selectedIds)
		{
			base.SelectionChanged(selectedIds);
			OnSelectionChanged?.Invoke();
		}

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return false;
		}
	}
}