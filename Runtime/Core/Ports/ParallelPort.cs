namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class ParallelOutputPort : OutputPort
	{
		public ParallelOutputPort(string id, string label) : base(id, label, LogicPortType.Multiple)
		{
		}
	}
}