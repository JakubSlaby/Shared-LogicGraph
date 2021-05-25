using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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

		private IMGUIContainer m_TreeViewContainer;
		[SerializeField]
		private LogicGraphTreeState m_TypeTreeState;

		[SerializeField]
		private LogicGraphTypeTree m_TypeTree;


		private AbstractLogicGraph m_TypePreviewInstance;
		
		private void OnEnable()
		{
			Construct();

			m_TypeTree.OnSelectionChanged -= OnSelectionChanged;
			m_TypeTree.OnSelectionChanged += OnSelectionChanged;
		}

		private void OnDisable()
		{
			m_TypeTree.OnSelectionChanged -= OnSelectionChanged;

		}


		private void Construct()
		{
			VisualTreeAsset template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/Repositories/LogicGraph/Editor/Styling/LogicGraphWindow.uxml");
			VisualElement ui = template.CloneTree();
			ui.style.flexGrow = new StyleFloat(1);
			this.rootVisualElement.Add(ui);

			if (m_TypeTreeState == null)
			{
				m_TypeTreeState = new LogicGraphTreeState();
			}

			if (m_TypeTree == null)
			{
				m_TypeTree = new LogicGraphTypeTree(m_TypeTreeState);
				m_TypeTree.Reload();
				
			}

			m_TreeViewContainer = ui.Q<IMGUIContainer>("TypeListContainer");
			m_TreeViewContainer.onGUIHandler += HandleTreeViewGUI;

			m_LogicGraphView = new LogicGraphView();
			m_LogicGraphView.style.flexGrow = new StyleFloat(1);
			var logicGraphContainer = ui.Q<VisualElement>("ContentContainer");
			logicGraphContainer.Add(m_LogicGraphView);
		}

		private void HandleTreeViewGUI()
		{
			
			m_TypeTree.OnGUI(new Rect(0, 0, m_TreeViewContainer.layout.width,m_TreeViewContainer.layout.height));
		}

		private LogicGraphView m_LogicGraphView;
		
		
		private void ShowSpecificGraph(AbstractLogicGraph target)
		{
			m_LogicGraphView.SetGraph(target);
		}
		
		
		private void OnSelectionChanged()
		{
			var selection = m_TypeTree.GetSelection();
			if (selection.Count == 0)
				return;
			int selectedId = selection[0];
			var allGraphTypes = LogicGraphEditorRegistry.GetAllGraphTypes();

			if (selectedId >= allGraphTypes.Length || selectedId < 0)
				return;

			Type type = allGraphTypes[selectedId];
			ShowGraphByType(type);
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
	}
}