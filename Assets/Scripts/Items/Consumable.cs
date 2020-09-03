using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using StatsWithModifiers;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector;

namespace Items
{
	[Serializable]
	public class Consumable
	{
		public List<StatModifier> modifiers;
	}
}