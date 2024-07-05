using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphWindow : EditorWindow
	{
		[MenuItem(itemName: "Tools/Logic Graph Window")]
		private static void MenuOption()
		{
			var window = EditorWindow.GetWindow<LogicGraphWindow>();
			window.Show();
		}

		private ToolbarMenu m_TreeViewSelector;

		[SerializeField]
		private LogicGraphEditorWindowState m_State;

		private LogicGraphEditorSidebar m_Sidebar;
		
		private void OnEnable()
		{
			Construct();

			m_Sidebar.OnSelectionChange -= OnSelectionChanged;
			m_Sidebar.OnSelectionChange += OnSelectionChanged;
			
		}

		private void OnDestroy()
		{
			if(m_Sidebar != null)
				m_Sidebar.OnSelectionChange -= OnSelectionChanged;
		}




		private void Construct()
		{
			if (m_State == null)
				m_State = ScriptableObject.CreateInstance<LogicGraphEditorWindowState>();
			
			var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(GraphEditorUtil.FindAssetPathToCallingScript("./Styling/GraphEditorStyleSheet.uss"));
			this.rootVisualElement.styleSheets.Add(uss);
			
			var splitView = new TwoPaneSplitView();
			var left = new VisualElement();
			var right = new VisualElement();
			
			splitView.Add(left);
			splitView.Add(right);
			
			this.rootVisualElement.Add(splitView);

			m_Sidebar = new LogicGraphEditorSidebar(m_State);
			left.Add(m_Sidebar);
		
			m_LogicGraphView = new LogicGraphView();
			m_LogicGraphView.style.flexGrow = new StyleFloat(1);
			right.Add(m_LogicGraphView);
		}


		private DropdownMenuAction.Status StatusGraphTree(DropdownMenuAction arg)
		{
			// if ((LogicGraphTreeView) arg.userData == m_SelectedTreeView)
			// 	return DropdownMenuAction.Status.Checked;
			
		
			
			return DropdownMenuAction.Status.Normal;
		}

		
		private void HandleTreeViewGUI()
		{
			
		}

		private LogicGraphView m_LogicGraphView;
		
		
		
		private void OnSelectionChanged(AbstractLogicGraphTreeView abstractLogicGraphTreeView)
		{
			
			
			if (abstractLogicGraphTreeView.GraphType == LogicGraphEditorWindowState.LogicGraphTreeView.Script)
			{
				var selection = abstractLogicGraphTreeView.GetSelection();
				if (selection.Count == 0)
					return;
				int selectedId = selection[0];

				var item = abstractLogicGraphTreeView.FindItem(selectedId);
				if (item is LogicGraphTreeViewItem logicGraphItem)
				{
					
					m_LogicGraphView.ShowGraph(logicGraphItem.GraphType);
					
				}
				
			}
			
			if (abstractLogicGraphTreeView.GraphType == LogicGraphEditorWindowState.LogicGraphTreeView.Runtime)
			{
				var selection = abstractLogicGraphTreeView.GetSelection();
				if (selection.Count == 0)
					return;
				int selectedId = selection[0];
				var allGraphs = LogicGraphRuntimeRegistry.GetLogicGraphs();

				if (selectedId >= allGraphs.Count || selectedId < 0)
					return;

				var weakReference = allGraphs[selectedId];
				if (!weakReference.TryGetTarget(out var graphInstance))
					return;
				
				m_LogicGraphView.ShowGraph(graphInstance);
			}
		}


		private void Update()
		{
			if(!Application.isPlaying)
				return;
			
			if(m_LogicGraphView != null)
				m_LogicGraphView.GraphView.Update();
		}
	}
}