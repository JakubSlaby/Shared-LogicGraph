using System;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public class FlowPort : AbstractLogicPort, IInvokedPort
	{
		public FlowPort(string id, string label, LogicPortDirection direction, LogicPortType type) : base(id, label, direction, type)
		{
		}

		public void Invoke()
		{
			onPortInvoked?.Invoke(this);
		}
		
		public event Action<AbstractLogicPort> onPortInvoked;
	}
}