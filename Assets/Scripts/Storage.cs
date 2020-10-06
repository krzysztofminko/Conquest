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

    public event Action<ItemEntity> onAddItemEntity;
    public event Action<ItemEntity> onRemoveItemEntity;
    //TODO: implement updates of item state in UI
    public event Action<ItemEntity> onItemEntityCountChange;


    //TODO: more protection from accessing this list?
    public readonly List<ItemEntity> itemsEntities = new List<ItemEntity>();
        
    public int Count(Item item) => itemsEntities.Count(i => i.item == item);

    public void AddItemEntity(ItemEntity itemEntity)
    {
        itemsEntities.Add(itemEntity);
        itemEntity.storage = this;
        itemEntity.SetParent(transform, false);;
        itemEntity.onDestroy += RemoveItemEntityOnDestroy;
        onAddItemEntity?.Invoke(itemEntity);
    }

    public void RemoveItemEntity(ItemEntity itemEntity)
    {
        onRemoveItemEntity?.Invoke(itemEntity);
        itemsEntities.Remove(itemEntity);
        itemEntity.storage = null;
        itemEntity.SetParent(null, true);
        itemEntity.transform.position = transform.position;
        itemEntity.onDestroy -= RemoveItemEntityOnDestroy;
    }

    private void RemoveItemEntityOnDestroy(ItemEntity itemEntity)
    {
        onRemoveItemEntity?.Invoke(itemEntity);
        itemsEntities.Remove(itemEntity);
    }
}
