using System;
using System.Collections.Generic;
using StatsWithModifiers;
using UnityEngine;

namespace Items
{
	[Serializable]
	public class Consumable
	{
		[SerializeField]
		private List<StatModifier> modifiers;

		public void ApplyModifiers(GameObject statsOwner)
		{
			for (int i = 0; i < modifiers.Count; i++)
			{
				Stat stat = statsOwner.GetComponent(modifiers[i].Stat.Type) as Stat;
				if (stat)
					stat.ApplyModifier(modifiers[i]);
			}
		}
	}
}