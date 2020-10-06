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
	/// On value change, also ItemEntity's parent and holder are changed.
	/// </summary>
	public ItemEntity ItemEntity
	{
		get => _itemEntity;
		set
		{
			if(_itemEntity != value)
			{
				if (_itemEntity)
				{
					_itemEntity.SetParent(null, true);
					_itemEntity.holder = null;
				}
				if (value)
				{
					value.SetParent(itemEntityParent, true);
					value.holder = this;
				}
				_itemEntity = value;
			}
		}
	}

	[SerializeField]
	private Transform itemEntityParent;

	private void Awake()
	{
		if(spawnItem)
			ItemEntity = ItemEntity.Spawn(spawnItem, itemEntityParent? itemEntityParent.position : transform.position + transform.forward, itemEntityParent? itemEntityParent.rotation : Quaternion.identity);
	}
}
