using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TypeReferences;

namespace StatsWithModifiers
{
	[System.Serializable]
	public class StatModifier
	{
		public enum ModifierType { Permanent, Timed, Equipable }

		[SerializeField, HorizontalGroup()]
		private StatId _statId;
		public StatId StatId { get => _statId;}

		[SerializeField, HorizontalGroup(50), LabelText("Max"), LabelWidth(30)]
		private bool _affectMax;
		public bool AffectMax { get => _affectMax; }

		[SerializeField]
		private ModifierType _type;
		public ModifierType Type { get => _type; }

		[SerializeField]
		private float _value;
		public float Value { get => _value; }

		[SerializeField, ShowIf("typeTimed")]
		private float _duration;
		public float Duration { get => _duration; }

		private bool typeTimed => Type == ModifierType.Timed;
	}
}