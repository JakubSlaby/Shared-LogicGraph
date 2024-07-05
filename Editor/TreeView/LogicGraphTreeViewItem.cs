using System;
using UnityEditor.IMGUI.Controls;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
    public class LogicGraphTreeViewItem : TreeViewItem
    {
        public Type GraphType { get; private set; }
        
        public LogicGraphTreeViewItem(int id, int depth, Type graphType) : base(id, depth, graphType.Name)
        {
            GraphType = graphType;
        }
    }
}