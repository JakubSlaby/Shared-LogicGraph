using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class LogicGraphWindow : EditorWindow
	{
		public enum LogicGraphTreeView
		{
			Script = 0,
			Runtime = 1
		}
		
		[MenuItem(itemName: "Tools/Logic Graph Window")]
		private static void MenuOption()
		{
			var window = EditorWindow.GetWindow<LogicGraphWindow>();
			window.Show();
		}

		private ToolbarMenu m_TreeViewSelector;
		private IMGUIContainer m_TreeViewContainer;

		[SerializeField]
		private LogicGraphTreeView m_SelectedTreeView = LogicGraphTreeView.Script;
		
		[SerializeField]
		private TreeViewState m_ScriptGraphTreeState = new TreeViewState();
		private LogicGraphTypeTree m_ScriptGraphTree;

		[SerializeField]
		private TreeViewState m_RuntimeGraphTreeState = new TreeViewState();
		private RuntimeGraphTree m_RuntimeGraphTree;
		
		private AbstractLogicGraph m_TypePreviewInstance;
		
		private void OnEnable()
		{
			Construct();

			m_ScriptGraphTree.OnSelectionChanged -= OnSelectionChanged;
			m_ScriptGraphTree.OnSelectionChanged += OnSelectionChanged;

			m_RuntimeGraphTree.OnSelectionChanged -= OnSelectionChanged;
			m_RuntimeGraphTree.OnSelectionChanged += OnSelectionChanged;
		}

		private void OnDisable()
		{
			m_ScriptGraphTree.OnSelectionChanged -= OnSelectionChanged;
			m_RuntimeGraphTree.OnSelectionChanged -= OnSelectionChanged;
		}


		private void Construct()
		{
			VisualTreeAsset template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/Repositories/LogicGraph/Editor/Styling/LogicGraphWindow.uxml");
			VisualElement ui = template.CloneTree();
			ui.style.flexGrow = new StyleFloat(1);
			this.rootVisualElement.Add(ui);

			if (m_ScriptGraphTreeState == null)
			{
				m_ScriptGraphTreeState = new TreeViewState();
			}

			if (m_ScriptGraphTree == null)
			{
				m_ScriptGraphTree = new LogicGraphTypeTree(m_ScriptGraphTreeState);
				m_ScriptGraphTree.Reload();
			}

			if (m_RuntimeGraphTree == null)
			{
				m_RuntimeGraphTree = new RuntimeGraphTree(m_RuntimeGraphTreeState);
				m_RuntimeGraphTree.Reload();
			}

			m_TreeViewSelector = ui.Q<ToolbarMenu>("tree-view-selector");
			m_TreeViewSelector.text = m_SelectedTreeView.ToString();
			m_TreeViewSelector.menu.AppendAction("Graph Scripts", SelectGraphTree, StatusGraphTree, LogicGraphTreeView.Script);
			m_TreeViewSelector.menu.AppendAction("Runtime Graphs", SelectGraphTree, StatusGraphTree, LogicGraphTreeView.Runtime);

			m_TreeViewContainer = ui.Q<IMGUIContainer>("TypeListContainer");
			m_TreeViewContainer.onGUIHandler += HandleTreeViewGUI;

			m_LogicGraphView = new LogicGraphView();
			m_LogicGraphView.style.flexGrow = new StyleFloat(1);
			var logicGraphContainer = ui.Q<VisualElement>("right-column");
			logicGraphContainer.Add(m_LogicGraphView);
		}

		private void SelectGraphTree(DropdownMenuAction arg)
		{
			m_SelectedTreeView = (LogicGraphTreeView) arg.userData;
			m_TreeViewSelector.text = m_SelectedTreeView.ToString();
			
			switch (m_SelectedTreeView)
			{
				case LogicGraphTreeView.Script:
					m_ScriptGraphTree.Reload();
					break;
				case LogicGraphTreeView.Runtime:
					m_RuntimeGraphTree.Reload();
					break;
			}
		}
		
		private DropdownMenuAction.Status StatusGraphTree(DropdownMenuAction arg)
		{
			if ((LogicGraphTreeView) arg.userData == m_SelectedTreeView)
				return DropdownMenuAction.Status.Checked;
			
		
			
			return DropdownMenuAction.Status.Normal;
		}

		
		private void HandleTreeViewGUI()
		{
			Rect imguiRect = new Rect(0, 0, m_TreeViewContainer.layout.width, m_TreeViewContainer.layout.height);
			switch (m_SelectedTreeView)
			{
				case LogicGraphTreeView.Script:
					m_ScriptGraphTree.OnGUI(imguiRect);
					break;
				case LogicGraphTreeView.Runtime:
					m_RuntimeGraphTree.OnGUI(imguiRect);
					break;
			}
		}

		private LogicGraphView m_LogicGraphView;
		
		
		private void ShowSpecificGraph(AbstractLogicGraph target)
		{
			m_LogicGraphView.SetGraph(target);
		}
		
		
		private void OnSelectionChanged(AbstractLogicGraphTreeView abstractLogicGraphTreeView)
		{
			if (abstractLogicGraphTreeView == m_ScriptGraphTree)
			{
				var selection = m_ScriptGraphTree.GetSelection();
				if (selection.Count == 0)
					return;
				int selectedId = selection[0];
				var allGraphTypes = LogicGraphEditorRegistry.GetAllGraphTypes();

				if (selectedId >= allGraphTypes.Length || selectedId < 0)
					return;

				Type type = allGraphTypes[selectedId];
				ShowGraphByType(type);
			}
			else
			{
				m_TypePreviewInstance = null;
				m_ScriptGraphTree.SetSelection(Array.Empty<int>(), TreeViewSelectionOptions.None);
			}
			
			if (abstractLogicGraphTreeView == m_RuntimeGraphTree)
			{
				var selection = m_RuntimeGraphTree.GetSelection();
				if (selection.Count == 0)
					return;
				int selectedId = selection[0];
				var allGraphs = LogicGraphRuntimeRegistry.GetLogicGraphs();

				if (selectedId >= allGraphs.Count || selectedId < 0)
					return;

				var weakReference = allGraphs[selectedId];
				if (!weakReference.TryGetTarget(out var graphInstance))
					return;
				
				ShowSpecificGraph(graphInstance);
			}
			else
			{
				m_RuntimeGraphTree.SetSelection(Array.Empty<int>(), TreeViewSelectionOptions.None);
			}
		}

		private void ShowGraphByType(Type graphType)
		{
			if (m_TypePreviewInstance != null)
			{
				if (m_TypePreviewInstance.GetType() == graphType)
					return;
			}

			m_TypePreviewInstance = null;
			try
			{
				m_TypePreviewInstance = Activator.CreateInstance(graphType) as AbstractLogicGraph;
				m_TypePreviewInstance.Construct();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			ShowSpecificGraph(m_TypePreviewInstance);
		}

		private void Update()
		{
			if(m_LogicGraphView != null)
				m_LogicGraphView.Update();
		}
	}
}