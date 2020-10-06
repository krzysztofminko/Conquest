using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	[Category("Storage")]
	public class Equip : ActionTask<Equipment>
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.item.equipable != null)
			{
				if (agent.IsEquiped(itemEntity.value))
					agent.Unequip(itemEntity.value);
				else
					agent.Equip(itemEntity.value, itemEntity.value.item.equipable.slot);

				EndAction(true);
			}
			else
			{
				EndAction(false);
			}
		}
	}
}