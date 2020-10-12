using NodeCanvas.Framework;
using ParadoxNotion.Design;
using StatsWithModifiers;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions{

	[Category("Item")]
	public class Consume : ActionTask
	{
		[RequiredField]
		public BBParameter<ItemEntity> itemEntity;

		protected override void OnExecute()
		{
			if (itemEntity.value.item.consumable != null)
			{
				List<StatModifier> modifiers = itemEntity.value.item.consumable.modifiers;
				for (int i = 0; i < modifiers.Count; i++)
					(agent.GetComponent(modifiers[i].Stat.Type) as Stat)?.ApplyModifier(modifiers[i]);

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