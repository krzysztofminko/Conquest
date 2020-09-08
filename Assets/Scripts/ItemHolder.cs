using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using Items;

[HideMonoScript]
public class ItemHolder : MonoBehaviour
{
	[SerializeField]
	private Item spawnItem;

	[SerializeField, ReadOnly]
	private ItemEntity _itemEntity;
	/// <summary>
	/// On value change, also ItemEntity's parent is changed.
	/// </summary>
	public ItemEntity ItemEntity
	{
		get => _itemEntity;
		set
		{
			if(_itemEntity != value)
			{
				_itemEntity?.SetParent(null);
				value?.SetParent(parent);
				_itemEntity = value;
			}
		}
	}

	[SerializeField]
	private Transform parent;

	private void Awake()
	{
		if(spawnItem)
			ItemEntity = ItemEntity.Spawn(spawnItem, parent? parent.position : transform.position + transform.forward, parent? parent.rotation : Quaternion.identity);
	}
}
