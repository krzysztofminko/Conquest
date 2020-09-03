using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(menuName = "Items/Item")]
	public class Item : ScriptableObject
	{
		public ItemEntity prefab;

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