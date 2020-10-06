using Items;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using StatsWithModifiers;
using System;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions{

	[Category("Storage")]
	public class Consume : ActionTask<Storage>
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