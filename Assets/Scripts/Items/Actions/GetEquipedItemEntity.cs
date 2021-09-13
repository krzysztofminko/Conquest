using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Items.Actions
{
	[Category("Items")]
	public class GetEquipedItemEntity : ActionTask<Equipment>
	{
		public Equipment.SlotType slot;
		[RequiredField]
		public BBParameter<ItemEntity> equipedItemEntity;

		protected override void OnExecute()
		{
			equipedItemEntity.value = agent[slot].itemEntity;
			EndAction(true);
		}
	}
}