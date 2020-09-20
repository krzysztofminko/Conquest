using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	[Category("Storage")]
	public class Transfer : ActionTask<Storage>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;
		[RequiredField]
		public BBParameter<Storage> target;
		public BBParameter<bool> fromPlayerToTarget;

		protected override void OnExecute()
		{
			if (fromPlayerToTarget.value)
			{
				agent.RemoveItemEntity(itemEntity.value);
				target.value.AddItemEntity(itemEntity.value);
			}
			else
			{
				target.value.RemoveItemEntity(itemEntity.value);
				agent.AddItemEntity(itemEntity.value);
			}
			EndAction(true);
		}
	}
}