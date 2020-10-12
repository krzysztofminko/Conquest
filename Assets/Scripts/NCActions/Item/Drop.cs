using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("Item")]
	public class Drop : ActionTask
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.holder)
				itemEntity.value.holder.ItemEntity = null;
			if (itemEntity.value.storage)
				itemEntity.value.storage.RemoveItemEntity(itemEntity.value);

			EndAction(true);
		}
	}
}