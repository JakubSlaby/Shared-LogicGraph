using UnityEngine;
using WhiteSparrow.Shared.LogicGraph.Core;

namespace WhiteSparrow.Shared.LogicGraph.StateGraph
{
	public abstract class AbstractStateGraph : AbstractLogicGraph
	{
		private StateGraphStructure m_fsmStructure;
		
		protected override IGraphStructure GetStructureInstance()
		{
			return m_fsmStructure = new StateGraphStructure();
		}

		protected override void DoStart()
		{
			flowWrapper.AddFlow(new StateGraphFlow(m_fsmStructure.StartNode));
		}

		protected override void DoUpdate()
		{
			flowWrapper.Update(Time.deltaTime);
		}

		protected override void DoStop()
		{
			
		}
	}


}