using Items;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NCActions{

	[Category("Item")]
	public class Consume : ActionTask
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.item.consumable != null)
			{
				itemEntity.value.item.consumable.ApplyModifiers(agent.gameObject);
				itemEntity.value.Count--;

				EndAction(true);
			}
			else
			{
				EndAction(false);
			}
		}

	}
}