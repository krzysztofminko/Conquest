using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Items.Actions
{
	[Category("Items")]
	public class ItemEntityIsEquipable : ConditionTask
	{
		//[RequiredField] <- rises "BBParameter ... not set" error
		public BBParameter<ItemEntity> itemEntity;

		protected override string info
		{
			get => itemEntity + " is equipable";
		}

		protected override bool OnCheck() => itemEntity.value.item.equipable != null;
	}
}