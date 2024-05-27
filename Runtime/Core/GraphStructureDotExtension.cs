using System.Text;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial interface IGraphStructure
	{
		string ExportDot();
	}
	
	public abstract partial class AbstractGraphStructure 
	{
		public string ExportDot()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("digraph namedGraph {");
			foreach (var node in AllNodes)
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