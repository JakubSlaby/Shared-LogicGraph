using System;

namespace WhiteSparrow.Shared.LogicGraph.Core
{
	
	public sealed class LogicConnection : AbstractLogicConnection
	{
		
	}
	
	public abstract class InvokedConnection : AbstractLogicConnection, IInvokedConnection, IActivateConnection
	{
		public abstract void Activate();

		public abstract void Deactivate();

		public void Invoke()
		{
			onConnectionInvoked?.Invoke(this);
		}

		public event Action<AbstractLogicConnection> onConnectionInvoked;
		
	}
	
	public abstract partial class AbstractLogicConnection
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
		void Invoke();
		event Action<AbstractLogicConnection> onConnectionInvoked;
	}
	
}