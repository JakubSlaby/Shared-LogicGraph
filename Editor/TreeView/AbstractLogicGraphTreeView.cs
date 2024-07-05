using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public abstract class AbstractLogicGraphTreeView : TreeView
	{
		public event Action<AbstractLogicGraphTreeView> OnSelectionChanged;
		public abstract LogicGraphEditorWindowState.LogicGraphTreeView GraphType { get; }
		public abstract TreeViewItem RootItem { get; }

		
		protected AbstractLogicGraphTreeView(TreeViewState state) : base(state)
		{
		}
		
		protected override void SelectionChanged(IList<int> selectedIds)
		{
			base.SelectionChanged(selectedIds);
			OnSelectionChanged?.Invoke(this);
		}

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return false;
		}

		public TreeViewItem FindItem(int id)
		{
			return this.FindItem(id, RootItem);
		}
		
		
	}
}