using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using Items;

public class ItemHolder : MonoBehaviour
{
	[SerializeField, Required, DisableInPlayMode]
	private Transform rightHandParent;

	[SerializeField, DisableInPlayMode]
	private Item _rightHandItem;
	public Item RightHandItem
	{
		get => _rightHandItem;
		set
		{
			if (_rightHandItem != value)
				SetRightHandItem(value);
		}
	}

	[SerializeField, ReadOnly]
	private ItemEntity _rightHandItemEntity;
	public ItemEntity RightHandItemEntity { get => _rightHandItemEntity; }

	private void Awake()
	{
		SetRightHandItem(_rightHandItem);
	}

	private void SetRightHandItem(Item item)
	{
		if (_rightHandItem)
		{
			if (_rightHandItemEntity)
				Destroy(_rightHandItemEntity);
			_rightHandItemEntity = null;
			_rightHandItem = null;
		}

		if (item)
		{
			_rightHandItem = item;
			if (_rightHandItem.prefab)
			{
				_rightHandItemEntity = Instantiate(_rightHandItem.prefab, rightHandParent.position, rightHandParent.rotation, rightHandParent);
				_rightHandItemEntity.item = _rightHandItem;
			}
		}

		_rightHandItem = item;
	}
}
