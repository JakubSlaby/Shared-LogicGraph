using System;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public static class LogicPortExtensions
	{
		public static void Connect(this AbstractLogicNode from, AbstractLogicNode to)
		{
			Connect(from, to, null);
		}
		public static void Connect(this AbstractLogicNode from, AbstractLogicNode to, AbstractLogicConnection connection)
		{
			from.Structure.Connect(from, to, connection);
		}

		public static void Connect(this AbstractLogicNode from, AbstractLogicPort to)
		{
			Connect(from, to, null);
		}
		public static void Connect(this AbstractLogicNode from, AbstractLogicPort to, AbstractLogicConnection connection)
		{
			var fromOutputPorts = from.GetOutputPorts();
			if(fromOutputPorts.Length == 0) throw new Exception("Cannot Connect nodes. No output ports.");
			if(fromOutputPorts.Length > 1) throw new Exception("Cannot Connect nodes. More than 1 output port - cannot determine default.");

			Connect(fromOutputPorts[0], to, connection);
		}

		public static void Connect(this AbstractLogicPort from, AbstractLogicNode to)
		{
			Connect(from, to, null);
		}
		public static void Connect(this AbstractLogicPort from, AbstractLogicNode to, AbstractLogicConnection connection)
		{
			var toInputPorts = to.GetInputPorts();
			if(toInputPorts.Length == 0) throw new Exception("Cannot Connect nodes. No input ports.");
			if(toInputPorts.Length > 1) throw new Exception("Cannot Connect nodes. More than 1 input port - cannot determine default.");

			Connect(from, toInputPorts[0], connection);
		}

		public static void Connect(this AbstractLogicPort from, AbstractLogicPort to)
		{
			Connect(from, to, null);
		}
		public static void Connect(this AbstractLogicPort from, AbstractLogicPort to, AbstractLogicConnection connection)
		{
			from.Structure.Connect(from, to, connection);
		}
	}
}