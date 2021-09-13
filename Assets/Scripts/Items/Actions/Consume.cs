using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace Items.Actions{

	[Category("Items")]
	public class Consume : ActionTask
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.item.consumable == null)
			{
				Logger.LogWarning($"Item is not consumable ({itemEntity.value.item.name}).", context: this);
				EndAction(false);
			}
			else 
			{ 
				itemEntity.value.item.consumable.ApplyModifiers(agent.gameObject);
				itemEntity.value.Count--;

				EndAction(true);
			}
		}

	}
}