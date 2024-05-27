using System;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
    public class LogicGraphEditorSidebar : VisualElement
    {
        private Toolbar m_Toolbar;
        private IMGUIContainer m_TreeViewContainer;
        private LogicGraphEditorWindowState m_State;
        
        private LogicGraphTypeTree m_ScriptGraphTree;
        private RuntimeGraphTree m_RuntimeGraphTree;

        private EditorToolbarButton m_TabScriptTree;
        private EditorToolbarButton m_TabRuntimeTree;
        

        public event Action<AbstractLogicGraphTreeView> OnSelectionChange;
        
        public LogicGraphEditorSidebar(LogicGraphEditorWindowState state)
        {
            this.AddToClassList("logicGraph-sidebar");
            
            m_State = state;
            
            hierarchy.Add(m_Toolbar = new Toolbar());
            
            
            m_Toolbar.Add(m_TabScriptTree = new EditorToolbarButton("Scripts", OnShowScriptTree));
            m_TabScriptTree.AddToClassList("tab");
            m_Toolbar.Add(m_TabRuntimeTree = new EditorToolbarButton("Runtime", OnShowRuntimeTree));
            m_TabRuntimeTree.AddToClassList("tab");
            UpdateTabValues();
            
            
            
            hierarchy.Add(m_TreeViewContainer = new IMGUIContainer(OnTreeViewGUI));
            
            if (m_ScriptGraphTree == null)
            {
                m_ScriptGraphTree = new LogicGraphTypeTree(m_State.m_ScriptGraphTreeState);
                m_ScriptGraphTree.Reload();
            }

            if (m_RuntimeGraphTree == null)
            {
                m_RuntimeGraphTree = new RuntimeGraphTree(m_State.m_RuntimeGraphTreeState);
                m_RuntimeGraphTree.Reload();
            }
            
            this.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);

        }

        private void OnShowScriptTree()
        {
            m_State.m_SelectedTreeView = LogicGraphEditorWindowState.LogicGraphTreeView.Script;
            UpdateTabValues();
        }

        private void OnShowRuntimeTree()
        {
            m_State.m_SelectedTreeView = LogicGraphEditorWindowState.LogicGraphTreeView.Runtime;
            UpdateTabValues();
        }

        private void UpdateTabValues()
        {
            m_TabScriptTree.EnableInClassList("active",
                m_State.m_SelectedTreeView == LogicGraphEditorWindowState.LogicGraphTreeView.Script);
            m_TabRuntimeTree.EnableInClassList("active",
                m_State.m_SelectedTreeView == LogicGraphEditorWindowState.LogicGraphTreeView.Runtime);
        }


        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            m_RuntimeGraphTree.Reload();
            
            LogicGraphRuntimeRegistry.onGraphRegistered -= OnRuntimeGraphRegistered;
            LogicGraphRuntimeRegistry.onGraphRegistered += OnRuntimeGraphRegistered;
            
            m_ScriptGraphTree.OnSelectionChanged += OnTreeViewSelectionChange;
            m_RuntimeGraphTree.OnSelectionChanged += OnTreeViewSelectionChange;

        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            LogicGraphRuntimeRegistry.onGraphRegistered -= OnRuntimeGraphRegistered;
            
            m_ScriptGraphTree.OnSelectionChanged -= OnTreeViewSelectionChange;
            m_RuntimeGraphTree.OnSelectionChanged -= OnTreeViewSelectionChange;
        }
        
        private void OnTreeViewSelectionChange(AbstractLogicGraphTreeView obj)
        {
            OnSelectionChange?.Invoke(obj);
        }

        private void OnRuntimeGraphRegistered(WeakReference<AbstractLogicGraph> obj)
        {
            if(m_RuntimeGraphTree != null)
                m_RuntimeGraphTree.Reload();
        }

        private void OnTreeViewGUI()
        {
            Rect imguiRect = new Rect(0, 0, m_TreeViewContainer.layout.width, m_TreeViewContainer.layout.height);
            switch (m_State.m_SelectedTreeView)
            {
                case LogicGraphEditorWindowState.LogicGraphTreeView.Script:
                    m_ScriptGraphTree.OnGUI(imguiRect);
                    break;
                case LogicGraphEditorWindowState.LogicGraphTreeView.Runtime:
                    m_RuntimeGraphTree.OnGUI(imguiRect);
                    break;
            }
        }
    }
}