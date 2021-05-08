using System;
using System.Text;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public abstract partial class AbstractGraphStructure : IGraphStructure
	{
		public string ExportDot()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("digraph namedGraph {");
			foreach (var node in m_Nodes)
			{
				sb.AppendLine(node.GetType().Name);
			}

			foreach (var connection in m_Connections)
			{
				sb.AppendLine($"{connection.From.Node.GetType().Name} -> {connection.To.Node.GetType().Name}");
			}

			sb.AppendLine("}");
			
			return sb.ToString();
		}
		
	}
}