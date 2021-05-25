using System;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Layout;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraphEditor
{
	public class QuickTestLogicGraphWindow : EditorWindow
	{
		public enum Algorithm
		{
			RankingLayoutSettings = 0,
			MdsLayoutSettings = 1,
			FastIncrementalLayoutSettings = 2,
			SugiyamaLayoutSettings = 3
		}

		[Serializable]
		public class SerializedLayoutSettings
		{
			public double PackingAspectRatio = PackingConstants.GoldenRatio;
			public PackingMethod PackingMethod;
			public double NodeSeparation = 10;
			public double ClusterMargin = 10;
		}

		[Serializable]
		public class SerializedFastIncrementalLayoutSettings
		{
			public bool ApproximateRepulsion = true;
			public double InitialStepSize = 1.4;
			[Range(0.1f, 0.98f)]
			public double Decay = .9;

			[Range(0.1f, 0.98f)]
			public double Friction = 0.8;

			public double RepulsiveForceConstant = 1;
			public double AttractiveForceConstant = 1;
			public double GravityConstant = 1;
			public bool InterComponentForces = true;
			public bool ApplyForces = true;
			public bool AvoidOverlaps = true;
			public bool RespectEdgePorts;
			public bool RouteEdges;
			public bool ApproximateRouting = true;
			public bool LogScaleEdgeForces = true;
			public double DisplacementThreshold = 0.1;
			public bool Converged;
		}
		
		[Serializable]
		public class SerializedSuguiyamaLayoutSettings
		{
			public double LayerSeparation;
			public bool LayeringOnly;
			public int RepetitionCoefficientForOrdering = 1;
			public int NoGainAdjacentSwapStepsBound = 5;
			public int GroupSplit = 2;
			public double LabelCornersPreserveCoefficient = 0.1;
			public int BrandesThreshold = 600;
			public double MaxAspectRatioEccentricity = 5.0;
		}

		[SerializeField]
		private Algorithm m_Algorithm = Algorithm.RankingLayoutSettings;

		[SerializeField]
		private SerializedLayoutSettings m_LayoutSettings = new SerializedLayoutSettings();

		[SerializeField]
		private SerializedFastIncrementalLayoutSettings m_FastIncrementalLayoutSettings = new SerializedFastIncrementalLayoutSettings();
		[SerializeField]
		private SerializedSuguiyamaLayoutSettings m_SuguiyamaLayoutSettings = new SerializedSuguiyamaLayoutSettings();
		
		// [MenuItem(itemName: "Tools/Logic Graph Window")]
		private static void MenuOption()
		{
			var window = EditorWindow.GetWindow<QuickTestLogicGraphWindow>();
			window.Show();
		}

		private WeakReference<AbstractLogicGraph> m_referenceToGraph;
		private GeometryGraph m_GeometryGraph;

		private LogicGraphView m_LogicGraphView;
		
		private void OnEnable()
		{
			var graphs = LogicGraphRuntimeRegistry.GetLogicGraphs();
			foreach (var weakReference in graphs)
			{
				if (!weakReference.TryGetTarget(out var graph))
					continue;
				
				var button = new Button();
				button.text = graph.GetType().Name;
				button.clicked += () => ShowSpecificGraph(graph);
				
				rootVisualElement.Add(button);
			}

			m_LogicGraphView = new LogicGraphView();
			
			rootVisualElement.Add(m_LogicGraphView);
		}

		
		
		private void ShowSpecificGraph(AbstractLogicGraph target)
		{
		}


		// private void OnGUI()
		// {
		// 	var graphs = LogicGraphRegistry.GetLogicGraphs();
		// 	foreach (var weakReference in graphs)
		// 	{
		// 		if (!weakReference.TryGetTarget(out var graph))
		// 			continue;
		//
		// 		if (GUILayout.Button(graph.GetType().Name))
		// 		{
		// 			m_referenceToGraph = weakReference;
		// 			
		// 			Selection.activeObject = this;
		// 			
		// 			string dot = graph.structure.ExportDot();
		// 			EditorGUIUtility.systemCopyBuffer = dot;
		//
		//
		// 			GeometryGraph gg = graph.structure.ToMSAL();
		// 			
		// 			// RankingLayout rl = new RankingLayout(rlsettings, gg);
		// 			// rl.Run();
		// 			LayoutAlgorithmSettings settings = null;
		// 			switch (m_Algorithm)
		// 			{
		// 				default:
		// 				case Algorithm.RankingLayoutSettings:
		// 					settings = new RankingLayoutSettings();
		// 					break;
		// 				case Algorithm.MdsLayoutSettings:
		// 					MdsLayoutSettings mds = new MdsLayoutSettings();
		// 					mds.RotationAngle = Math.PI/2;
		// 					settings = mds;
		// 					break;
		// 				case Algorithm.FastIncrementalLayoutSettings:
		// 					FastIncrementalLayoutSettings f = new FastIncrementalLayoutSettings();
		// 					f.ApproximateRepulsion = m_FastIncrementalLayoutSettings.ApproximateRepulsion;
		// 					f.InitialStepSize = m_FastIncrementalLayoutSettings.InitialStepSize;
		// 					f.Decay = m_FastIncrementalLayoutSettings.Decay;
		// 					f.Friction = m_FastIncrementalLayoutSettings.Friction;
		// 					f.RepulsiveForceConstant = m_FastIncrementalLayoutSettings.RepulsiveForceConstant;
		// 					f.AttractiveForceConstant = m_FastIncrementalLayoutSettings.AttractiveForceConstant;
		// 					f.GravityConstant = m_FastIncrementalLayoutSettings.GravityConstant;
		// 					f.InterComponentForces = m_FastIncrementalLayoutSettings.InterComponentForces;
		// 					f.ApplyForces = m_FastIncrementalLayoutSettings.ApplyForces;
		// 					f.AvoidOverlaps = m_FastIncrementalLayoutSettings.AvoidOverlaps;
		// 					f.RespectEdgePorts = m_FastIncrementalLayoutSettings.RespectEdgePorts;
		// 					f.RouteEdges = m_FastIncrementalLayoutSettings.RouteEdges;
		// 					f.ApproximateRouting = m_FastIncrementalLayoutSettings.ApproximateRouting;
		// 					f.LogScaleEdgeForces = m_FastIncrementalLayoutSettings.LogScaleEdgeForces;
		// 					f.DisplacementThreshold = m_FastIncrementalLayoutSettings.DisplacementThreshold;
		// 					f.Converged = m_FastIncrementalLayoutSettings.Converged;
		// 					settings = f;
		// 					break;
		// 				case Algorithm.SugiyamaLayoutSettings:
		// 					SugiyamaLayoutSettings s = new SugiyamaLayoutSettings();
		// 					s.Transformation = PlaneTransformation.Rotation(Math.PI/2);
		// 					s.LayerSeparation = m_SuguiyamaLayoutSettings.LayerSeparation;
		// 					s.LayeringOnly = m_SuguiyamaLayoutSettings.LayeringOnly;
		// 					s.RepetitionCoefficientForOrdering = m_SuguiyamaLayoutSettings.RepetitionCoefficientForOrdering;
		// 					s.NoGainAdjacentSwapStepsBound = m_SuguiyamaLayoutSettings.NoGainAdjacentSwapStepsBound;
		// 					s.GroupSplit = m_SuguiyamaLayoutSettings.GroupSplit;
		// 					s.LabelCornersPreserveCoefficient = m_SuguiyamaLayoutSettings.LabelCornersPreserveCoefficient;
		// 					s.BrandesThreshold = m_SuguiyamaLayoutSettings.BrandesThreshold;
		// 					s.MaxAspectRatioEccentricity = m_SuguiyamaLayoutSettings.MaxAspectRatioEccentricity;
		// 					settings = s;
		// 					
		// 					break;
		// 			}
		//
		// 			settings.PackingAspectRatio = m_LayoutSettings.PackingAspectRatio;
		// 			settings.PackingMethod = m_LayoutSettings.PackingMethod;
		// 			settings.NodeSeparation = m_LayoutSettings.NodeSeparation;
		// 			settings.ClusterMargin = m_LayoutSettings.ClusterMargin;
		// 			
		// 			LayoutHelpers.CalculateLayout(gg, settings, null);
		//
		// 			gg.UpdateBoundingBox();
		//
		// 			Rect pos = new Rect(Vector2.zero, position.size);
		// 			Vector2 currentCenter = new Vector2((float)gg.BoundingBox.Center.X, (float)gg.BoundingBox.Center.Y);
		// 			Vector2 delta = pos.center- currentCenter;
		// 			
		// 			gg.Translate(new Point(delta.x, delta.y));
		//
		// 			m_GeometryGraph = gg;
		// 		}
		//
		//
		// 		if (m_GeometryGraph == null || m_referenceToGraph == null)
		// 			return;
		//
		// 		if (!m_referenceToGraph.TryGetTarget(out graph))
		// 			return;
		//
		// 		int edges = m_GeometryGraph.Edges.Count;
		// 		foreach (var edge in m_GeometryGraph.Edges)
		// 		{
		// 			Point endingPoint;
		// 			if (edge.Curve != null)
		// 			{
		// 				Point lastPoint = edge.Curve[0];
		// 				double length = edge.Curve.Length;
		// 				for (int i = 1; i <= 10; i++)
		// 				{
		// 					double t = edge.Curve.GetParameterAtLength(i / 10f * length);
		// 					var point = edge.Curve[t];
		//
		// 					Vector3 start = new Vector3((float) lastPoint.X, (float) lastPoint.Y, 0);
		// 					Vector3 end = new Vector3((float) point.X, (float) point.Y, 0);
		//
		// 					Handles.DrawLine(start, end);
		// 					lastPoint = point;
		// 				}
		//
		// 				endingPoint = lastPoint;
		// 			}
		// 			else
		// 			{
		// 				Vector3 start = new Vector3((float) edge.Source.Center.X, (float) edge.Source.Center.Y, 0);
		// 				Vector3 end = new Vector3((float) edge.Target.Center.X, (float) edge.Target.Center.Y, 0);
		//
		// 				endingPoint = edge.Target.Center;
		// 				
		// 				Handles.DrawLine(start, end);
		// 			}
		//
		// 			Rect arrowRect = new Rect(new Vector2((float) endingPoint.X, (float) endingPoint.Y), new Vector2(15, 15));
		// 			arrowRect.position -= arrowRect.size * 0.5f;
		// 			
		// 			GUI.DrawTexture(arrowRect, Texture2D.whiteTexture);
		// 		}
		// 		
		// 		
		// 		
		// 		BeginWindows();
		// 		int l = m_GeometryGraph.Nodes.Count;
		// 		for(int i=0; i<l; i++)
		// 		{
		// 			var node = m_GeometryGraph.Nodes[i];
		// 			var logicNode = node.UserData as AbstractLogicNode;
		// 			Rect position = new Rect(new Vector2((float)node.Center.X, (float)node.Center.Y),
		// 				new Vector2((float)node.Width, (float)node.Height));
		// 			position.position -= position.size * 0.5f;
		// 			GUI.Window(i, position, WindowDraw, logicNode.GetType().Name);
		// 		}
		// 		EndWindows();
		// 	}
		//
		// }

		
	}

}