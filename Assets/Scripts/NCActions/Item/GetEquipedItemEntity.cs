using Items;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Item")]
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