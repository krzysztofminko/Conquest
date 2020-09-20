using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Storage : MonoBehaviour
{
    [SerializeField]
    private bool _acceptLarge;
    public bool AcceptLarge { get => _acceptLarge; }

    public event Action<int> onAddItemEntity;
    public event Action<int> onRemoveItemEntity;

    public readonly List<ItemEntity> itemsEntities = new List<ItemEntity>();
        
    public int Count(Item item) => itemsEntities.Count(i => i.item == item);

    public void AddItemEntity(ItemEntity itemEntity)
    {
        itemsEntities.Add(itemEntity);
        itemEntity.storage = this;
        itemEntity.gameObject.SetActive(false);
        itemEntity.transform.parent = transform;
        onAddItemEntity?.Invoke(itemsEntities.Count - 1);
    }

    public void RemoveItemEntity(ItemEntity itemEntity)
    {
        int index = itemsEntities.IndexOf(itemEntity);
        itemsEntities.RemoveAt(index);
        itemEntity.storage = null;
        itemEntity.gameObject.SetActive(true);
        itemEntity.transform.parent = null;
        itemEntity.transform.position = transform.position;
        onRemoveItemEntity?.Invoke(index);
    }
}
