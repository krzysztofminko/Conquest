using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace StatsWithModifiers
{
	[HideMonoScript]
	public abstract class Stat : MonoBehaviour
	{
		//TODO: Implement timed modifiers

		[SerializeField, HideLabel, ProgressBar(0, "Max", Height = 16), HorizontalGroup()]
		private float _value = 100;
		public float Value 
		{ 
			get => _value;
			set 
			{
				if(_value != value)
				{
					_value = Mathf.Clamp(value, 0, Max);
					onValueChange?.Invoke(this);
				}
			}
		}

		[SerializeField, Min(0), LabelWidth(30), HorizontalGroup()]
		private float _max = 100;
		public float Max
		{
			get => _max;
			set
			{
				if (_max != value)
				{
					_max = Mathf.Max(0, value);
					if (Max < Value)
						Value = Max;
					onMaxChange?.Invoke(this);
				}
			}
		}

		public delegate void OnValueChange(Stat stat);
		public event OnValueChange onValueChange;
		public delegate void OnMaxChange(Stat stat);
		public event OnMaxChange onMaxChange;


		public void ApplyModifier(StatModifier modifier)
		{
			if (modifier.AffectMax)
				Max += modifier.Value;
			else
				Value += modifier.Value;
		}

		//TODO: Forbid removing modifiers that was never applied
		public void RemoveModifier(StatModifier modifier)
		{
			if (modifier.Type == StatModifier.ModifierType.Permanent)
			{
				Debug.Log($"Removing permanent modifier ({this}) is impossible.", this);
			}
			else
			{
				if (modifier.AffectMax)
					Max -= modifier.Value;
				else
					Value -= modifier.Value;
			}
		}
	}
}