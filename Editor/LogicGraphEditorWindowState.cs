using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
    public class LogicGraphEditorWindowState : ScriptableObject
    {
        public enum LogicGraphTreeView
        {
            Script = 0,
            Runtime = 1
        }
        
        [SerializeField]
        public LogicGraphTreeView m_SelectedTreeView = LogicGraphTreeView.Script;
        [SerializeField]
        public TreeViewState m_ScriptGraphTreeState = new TreeViewState();
        [SerializeField]
        public TreeViewState m_RuntimeGraphTreeState = new TreeViewState();

        private void OnEnable()
        {
            if (m_ScriptGraphTreeState == null)
                m_ScriptGraphTreeState = new TreeViewState();

            if (m_RuntimeGraphTreeState == null)
                m_RuntimeGraphTreeState = new TreeViewState();
        }
    }
}