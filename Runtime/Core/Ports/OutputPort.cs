namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class OutputPort : FlowPort
	{
		public OutputPort(string id, string label, LogicPortType type) : base(id, label, LogicPortDirection.Output, type)
		{
		}

	}
}