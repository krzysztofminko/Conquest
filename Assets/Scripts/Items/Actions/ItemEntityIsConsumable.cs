using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Items.Conditions
{
	[Category("Items")]
	public class ItemEntityIsConsumable : ConditionTask
	{
		//[RequiredField] <- rises "BBParameter ... not set" error
		public BBParameter<ItemEntity> itemEntity;

		protected override string info
		{
			get => itemEntity + " is consumable";
		}

		protected override bool OnCheck() => itemEntity.value.item.consumable != null;
	}
}