using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace Items.Actions
{
	[Category("Items")]
	public class Transfer : ActionTask<Storage>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;
		[RequiredField]
		public BBParameter<Storage> target;
		public BBParameter<bool> fromAgentToTarget;

		protected override void OnExecute()
		{
			if (!itemEntity.value)
			{
				Logger.LogWarning("ItemEntity is null.", context: this);
				EndAction(false);
			}
			else if (!target.value)
			{
				Logger.LogWarning("Storage is null.", context: this);
				EndAction(false);
			}
			else
			{
				if (fromAgentToTarget.value)
					agent.Transfer(itemEntity.value, target.value, 1);
				else
					target.value.Transfer(itemEntity.value, agent, 1);
				EndAction(true);
			}
		}
	}
}