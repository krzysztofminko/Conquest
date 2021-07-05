using Items;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Item")]
	public class Transfer : ActionTask<Storage>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;
		[RequiredField]
		public BBParameter<Storage> target;
		public BBParameter<bool> fromAgentToTarget;

		protected override void OnExecute()
		{
			if (fromAgentToTarget.value)
				agent.Transfer(itemEntity.value, target.value, 1);
			else
				target.value.Transfer(itemEntity.value, agent, 1);
			EndAction(true);
		}
	}
}