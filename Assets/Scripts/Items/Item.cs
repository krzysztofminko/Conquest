using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(menuName = "Items/Item")]
	public class Item : ScriptableObject
	{
		[Required]
		public ItemEntity prefab;

		public AnimationClip pickAnimation;
		[PropertyRange(0, "pickAnimationLenght")]
		public float pickDelay;
		public AnimationClip putAnimation;
		[PropertyRange(0, "putAnimationLenght")]
		public float putDelay;
		public AnimationClip carryAnimation;

		[SerializeField]
		private bool _isLarge;
		public bool IsLarge { get => _isLarge; }

		[SerializeField]
		private bool _isStackable;
		public bool IsStackable { get => _isStackable; }

		[SerializeReference, HideLabel]
		public Damageable damageable = null;
		[SerializeReference, HideLabel]
		public Consumable consumable = null;
		[SerializeReference, HideLabel]
		public Equipable equipable = null;
		[SerializeReference, HideLabel]
		public Weapon weapon = null;
		[SerializeReference, HideLabel]
		public Tool tool = null;

		private float pickAnimationLenght() => pickAnimation ? pickAnimation.length : 0;
		private float putAnimationLenght() => putAnimation ? putAnimation.length : 0;
	}
}