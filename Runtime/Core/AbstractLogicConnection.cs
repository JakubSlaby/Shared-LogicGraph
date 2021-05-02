using System;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	public sealed class LogicConnection : AbstractLogicConnection
	{
		
	}
	public abstract class AbstractLogicConnection
	{
		public AbstractLogicPort From { get; internal set; }
		public AbstractLogicPort To { get; internal set; }
		
		internal IGraphStructure Structure { get; set; }
	}

	public interface IInitializeConnection
	{
		void Initialize();
	}

	public interface IActivateConnection
	{
		void Activate();
		void Deactivate();
	}

	public interface IInvokedConnection
	{
		event Action<IInvokedConnection> onConnectionInvoked;
	}
	
}