namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class OutputPort : AbstractLogicPort
	{
		public OutputPort() : this("out", "Out", LogicPortType.Multiple)
		{
			
		}
		
		public OutputPort(string id, string label, LogicPortType type) : base(id, label, LogicPortDirection.Output, type)
		{
			
		}

	}
}