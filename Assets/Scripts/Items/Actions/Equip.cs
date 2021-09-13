using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace Items.Actions{

	[Category("Items")]
	public class Equip : ActionTask<Equipment>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.item.equipable == null)
			{
				Logger.LogWarning($"Item is not equipable ({itemEntity.value.item.name}).", context: this);
				EndAction(false);
			}
			else
			{
				if (agent.IsEquiped(itemEntity.value))
					agent.Unequip(itemEntity.value);
				else
					agent.Equip(itemEntity.value, itemEntity.value.item.equipable.slot);

				EndAction(true);
			}
		}
	}
}