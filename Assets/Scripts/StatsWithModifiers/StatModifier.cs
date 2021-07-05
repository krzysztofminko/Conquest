using UnityEngine;
using Sirenix.OdinInspector;
using TypeReferences;

namespace StatsWithModifiers
{
	[System.Serializable]
	public class StatModifier
	{
		public enum ModifierType { Permanent, Timed, Equipable }

		[SerializeField, HorizontalGroup(), Inherits(typeof(Stat), AllowAbstract = false, ShortName = true, ExpandAllFolders = true)]
		private TypeReference _stat;
		public TypeReference Stat { get => _stat;}

		[SerializeField, HorizontalGroup(50), LabelText("Max"), LabelWidth(30)]
		private bool _affectMax;
		public bool AffectMax { get => _affectMax; }

		[SerializeField]
		private ModifierType _type;
		public ModifierType Type { get => _type; }

		[SerializeField]
		private float _value;
		public float Value { get => _value; }

		[SerializeField, ShowIf("i_typeTimed")]
		private float _duration;
		public float Duration { get => _duration; }

		private bool i_typeTimed => Type == ModifierType.Timed;
	}
}