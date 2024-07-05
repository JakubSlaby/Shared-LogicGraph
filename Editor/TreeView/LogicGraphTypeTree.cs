using System;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Repositories.LogicGraph.Runtime.GraphEditor;
using UnityEditor.IMGUI.Controls;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphTypeTree : AbstractLogicGraphTreeView
	{
		private TreeViewItem m_Root;
		public override TreeViewItem RootItem => m_Root;
		
		public LogicGraphTypeTree(TreeViewState state) : base(state)
		{
		}


		protected override TreeViewItem BuildRoot()
		{
			m_Root = new TreeViewItem(-1, -1);
			var types = LogicGraphEditorRegistry.GetAllGraphTypes();

			List<Type> primaryTypes = new List<Type>();
			List<Type> remainingTypes = new List<Type>();

			foreach (var type in types)
			{
				var primaryGraph = type.GetCustomAttribute<PrimaryLogicGraphAttribute>();
				if(primaryGraph != null)
					primaryTypes.Add(type);
				else
					remainingTypes.Add(type);
			}

			int id = 0;
			for(int i=0; i<primaryTypes.Count; i++)
			{
				var item = new LogicGraphTreeViewItem(++id, 0, primaryTypes[i]);
				m_Root.AddChild(item);
			}

			TreeViewItem otherGraphsParent = m_Root;
			if (primaryTypes.Count > 0)
			{
				var separator = new TreeViewItem(++id, otherGraphsParent.depth+1, "----");
				m_Root.AddChild(separator);
				
				// otherGraphsParent = new TreeViewItem(++id, otherGraphsParent.depth+1, "Nested and Other Graphs");
				// m_Root.AddChild(otherGraphsParent);
			}
			
			for(int i=0; i<remainingTypes.Count; i++)
			{
				var item = new LogicGraphTreeViewItem(++id, otherGraphsParent.depth+1, remainingTypes[i]);
				otherGraphsParent.AddChild(item);
			}

			if (!m_Root.hasChildren)
				m_Root.AddChild(new TreeViewItem(0, 0, "No script graphs"));
			
			return m_Root;
		}

	

		public override LogicGraphEditorWindowState.LogicGraphTreeView GraphType =>
			LogicGraphEditorWindowState.LogicGraphTreeView.Script;

	}
}