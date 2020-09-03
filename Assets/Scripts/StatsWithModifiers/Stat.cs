using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace StatsWithModifiers
{
	public enum StatId { None, Health, Stamina, Food }

	[Serializable]
	public class Stat
	{
		//TODO: Implement timed modifiers

		[SerializeField, HideLabel, HorizontalGroup("Stat"), ReadOnly]
		private StatId _id;
		public StatId Id { get => _id; private set => _id = value; }

		[SerializeField, HideLabel, ProgressBar(0, "Max"), HorizontalGroup("Value")]
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

		[SerializeField, Min(0), HideLabel, HorizontalGroup("Max")]
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

		public Stat(StatId Id)
		{
			this.Id = Id;
		}
	}
}