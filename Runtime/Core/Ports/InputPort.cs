namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class InputPort : AbstractLogicPort
	{
		public InputPort() : this("in", "In")
		{
			
		}
		
		public InputPort(string id, string label) : base(id, label, LogicPortDirection.Input, LogicPortType.Multiple)
		{
		}
	}
}