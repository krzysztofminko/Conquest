using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace Items.Actions{

	[Category("Items")]
	public class Drop : ActionTask
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (!itemEntity.value)
			{
				Logger.LogWarning("ItemEntity is null.", context: this);
				EndAction(false);
			}
			else
			{
				if (itemEntity.value.holder)
					itemEntity.value.holder.ItemEntity = null;
				if (itemEntity.value.storage)
					itemEntity.value.storage.RemoveItemEntity(itemEntity.value);

				EndAction(true);
			}
		}
	}
}