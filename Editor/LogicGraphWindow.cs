using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class LogicGraphWindow : EditorWindow
	{
		[MenuItem(itemName: "Tools/Logic Graph Window")]
		private static void MenuOption()
		{
			var window = EditorWindow.GetWindow<LogicGraphWindow>();
			window.Show();
		}

		
		
		private void OnEnable()
		{
			if (m_LogicGraphView == null)
			{
				m_LogicGraphView = new LogicGraphView();
				m_LogicGraphView.StretchToParentSize();
				rootVisualElement.Add(m_LogicGraphView);
			}

			
			
			var graphs = LogicGraphRegistry.GetLogicGraphs();
			foreach (var weakReference in graphs)
			{
				if (!weakReference.TryGetTarget(out var graph))
					continue;

				var button = new Button();
				button.text = graph.GetType().Name;
				button.clicked += () => ShowSpecificGraph(button, graph);
				
				rootVisualElement.Add(button);
			}
		}


		private LogicGraphView m_LogicGraphView;
			
		private void ShowSpecificGraph(Button button, AbstractLogicGraph target)
		{
			button.visible = false;
			m_LogicGraphView.SetGraph(target);
		}
	}
}