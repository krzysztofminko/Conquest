using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using StatsWithModifiers;

namespace Items
{
	[Serializable]
	public class Equipable
	{
		public Equipment.SlotType slot;
		public List<StatModifier> modifiers;
	}
}