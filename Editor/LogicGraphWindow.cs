using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Prototype.Ranking;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class LogicGraphWindow : EditorWindow
	{
		public enum Algorithm
		{
			RankingLayoutSettings = 0,
			MdsLayoutSettings = 1,
			FastIncrementalLayoutSettings = 2,
			SugiyamaLayoutSettings = 3
		}

		[SerializeField]
		private Algorithm m_Algorithm = Algorithm.RankingLayoutSettings;
		
		[MenuItem(itemName: "Tools/Logic Graph Window")]
		private static void MenuOption()
		{
			var window = EditorWindow.GetWindow<LogicGraphWindow>();
			window.Show();
		}

		public double RepulsiveForceConstant = 1.0;

		private WeakReference<AbstractLogicGraph> m_referenceToGraph;
		private GeometryGraph m_GeometryGraph;

		private void OnGUI()
		{
			var graphs = LogicGraphRegistry.GetLogicGraphs();
			foreach (var weakReference in graphs)
			{
				if (!weakReference.TryGetTarget(out var graph))
					continue;

				if (GUILayout.Button(graph.GetType().Name))
				{
					m_referenceToGraph = weakReference;
					
					Selection.activeObject = this;
					
					string dot = graph.structure.ExportDot();
					EditorGUIUtility.systemCopyBuffer = dot;


					GeometryGraph gg = graph.structure.ToMSAL();
					
					// RankingLayout rl = new RankingLayout(rlsettings, gg);
					// rl.Run();
					LayoutAlgorithmSettings settings = null;
					switch (m_Algorithm)
					{
						default:
						case Algorithm.RankingLayoutSettings:
							settings = new RankingLayoutSettings();
							break;
						case Algorithm.MdsLayoutSettings:
							MdsLayoutSettings mds = new MdsLayoutSettings();
							mds.RotationAngle = Math.PI;
							settings = mds;
							break;
						case Algorithm.FastIncrementalLayoutSettings:
							settings = new FastIncrementalLayoutSettings();
							break;
						case Algorithm.SugiyamaLayoutSettings:
							SugiyamaLayoutSettings s = new SugiyamaLayoutSettings();
							s.Transformation = PlaneTransformation.Rotation(Math.PI);
							settings = s;
							
							break;
					}
					
					LayoutHelpers.CalculateLayout(gg, settings, null);

					gg.UpdateBoundingBox();

					Rect pos = new Rect(Vector2.zero, position.size);
					Vector2 currentCenter = new Vector2((float)gg.BoundingBox.Center.X, (float)gg.BoundingBox.Center.Y);
					Vector2 delta = pos.center- currentCenter;
					
					gg.Translate(new Point(delta.x, delta.y));

					m_GeometryGraph = gg;
				}


				if (m_GeometryGraph == null || m_referenceToGraph == null)
					return;

				if (!m_referenceToGraph.TryGetTarget(out graph))
					return;

				int edges = m_GeometryGraph.Edges.Count;
				foreach (var edge in m_GeometryGraph.Edges)
				{
					Point endingPoint;
					if (edge.Curve != null)
					{
						Point lastPoint = edge.Curve[0];
						double length = edge.Curve.Length;
						for (int i = 1; i <= 10; i++)
						{
							double t = edge.Curve.GetParameterAtLength(i / 10f * length);
							var point = edge.Curve[t];

							Vector3 start = new Vector3((float) lastPoint.X, (float) lastPoint.Y, 0);
							Vector3 end = new Vector3((float) point.X, (float) point.Y, 0);

							Handles.DrawLine(start, end);
							lastPoint = point;
						}

						endingPoint = lastPoint;
					}
					else
					{
						Vector3 start = new Vector3((float) edge.Source.Center.X, (float) edge.Source.Center.Y, 0);
						Vector3 end = new Vector3((float) edge.Target.Center.X, (float) edge.Target.Center.Y, 0);

						endingPoint = edge.Target.Center;
						
						Handles.DrawLine(start, end);
					}

					Rect arrowRect = new Rect(new Vector2((float) endingPoint.X, (float) endingPoint.Y), new Vector2(15, 15));
					arrowRect.position -= arrowRect.size * 0.5f;
					
					GUI.DrawTexture(arrowRect, Texture2D.whiteTexture);
				}
				
				
				
				BeginWindows();
				int l = m_GeometryGraph.Nodes.Count;
				for(int i=0; i<l; i++)
				{
					var node = m_GeometryGraph.Nodes[i];
					var logicNode = node.UserData as AbstractLogicNode;
					Rect position = new Rect(new Vector2((float)node.Center.X, (float)node.Center.Y),
						new Vector2((float)node.Width, (float)node.Height));
					position.position -= position.size * 0.5f;
					GUI.Window(i, position, WindowDraw, logicNode.GetType().Name);
				}
				EndWindows();
			}

		}

		private void WindowDraw(int id)
		{
		}
	}

}