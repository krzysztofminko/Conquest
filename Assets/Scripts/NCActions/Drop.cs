using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	[Category("Storage")]
	public class Drop : ActionTask<Storage>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.holder)
				itemEntity.value.holder.ItemEntity = null;
			if (itemEntity.value.storage == agent)
				agent.RemoveItemEntity(itemEntity.value);

			EndAction(true);
		}
	}
}