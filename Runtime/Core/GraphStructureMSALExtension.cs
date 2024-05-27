#if UNITY_EDITOR
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
#endif

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public partial interface IGraphStructure
	{
#if UNITY_EDITOR
		GeometryGraph ToMSAL();
#endif
	}
	
	public abstract partial class AbstractGraphStructure
	{
#if UNITY_EDITOR
		public GeometryGraph ToMSAL()
		{
			if (AllNodes == null || AllNodes.Length == 0)
				return null;
			
			GeometryGraph geometryGraph = new GeometryGraph();

			foreach (var logicNode in AllNodes)
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
		
#endif
		
	}
}