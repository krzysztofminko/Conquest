using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(menuName = "Items/Item")]
	public class Item : ScriptableObject
	{
		[Required]
		public ItemEntity prefab;

		//TODO: SerializeReference as in consumable etc..
		[HorizontalGroup("damageable")]
		public bool damageable;
		[HorizontalGroup("damageable"), EnableIf("damageable")]
		public float condition = 100;

		[SerializeReference, HideLabel]
		public Consumable consumable = null;
		[SerializeReference, HideLabel]
		public Equipable equipable = null;
		[SerializeReference, HideLabel]
		public Weapon weapon = null;
		[SerializeReference, HideLabel]
		public Tool tool = null;
	}
}