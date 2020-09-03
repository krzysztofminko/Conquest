using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;

namespace StatsWithModifiers
{
	[HideMonoScript]
	public abstract class StatsList : MonoBehaviour
	{/*
		[SerializeField, HideLabel, TableList(AlwaysExpanded = true), ListDrawerSettings(DraggableItems = true)]
		protected List<Stat> list = new List<Stat>();
		
		public Stat this[StatId index]
		{
			get => list.FirstOrDefault(s => s.Id == index);
		}
		
		public void ApplyModifier(StatModifier modifier)
		{
			if (modifier.AffectMax)
				this[modifier.StatId].Max += modifier.Value;
			else
				this[modifier.StatId].Value += modifier.Value;
		}

		public void RemoveModifier(StatModifier modifier)
		{
			if (modifier.Type == StatModifier.ModifierType.Permanent)
			{
				Debug.Log($"Removing permanent modifier ({modifier.StatId}) is impossible.");
			}
			else
			{
				if (modifier.AffectMax)
					this[modifier.StatId].Max -= modifier.Value;
				else
					this[modifier.StatId].Value -= modifier.Value;
			}
		}*/
	}
}