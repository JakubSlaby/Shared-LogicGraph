using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial interface IGraphStructure
	{
		GeometryGraph ToMSAL();
	}
	
	public abstract partial class AbstractGraphStructure
	{
		public GeometryGraph ToMSAL()
		{
			if (m_Nodes == null || m_Nodes.Count == 0)
				return null;
			
			GeometryGraph geometryGraph = new GeometryGraph();

			foreach (var logicNode in m_Nodes)
			{
				var geometricNode = new Node(CurveFactory.CreateRectangle(150,60, new Point(0, 0)), logicNode);
				geometryGraph.Nodes.Add(geometricNode);
			}

			foreach (var logicConnection in m_Connections)
			{
				var geometricEdge = new Edge(geometryGraph.FindNodeByUserData(logicConnection.From.Node),
					geometryGraph.FindNodeByUserData(logicConnection.To.Node));
				geometryGraph.Edges.Add(geometricEdge);
			}

			return geometryGraph;
		}
		
	}
}