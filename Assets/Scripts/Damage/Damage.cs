using Sirenix.OdinInspector;
using System;

namespace Damageable
{
	[Serializable]
	public class Damage
	{
		[Flags]
		public enum Type
		{
			None = 0,
			Cut = 1 << 0,
			Blunt = 1 << 1,
			Stab = 1 << 2,
			Fire = 1 << 3,
			Explosion = 1 << 4,
			Chop = 1 << 5,
			Mine = 1 << 6,
			All = Cut | Blunt | Stab | Fire | Explosion | Chop | Mine
		}

		public Type type;
		public float value;
	}
}