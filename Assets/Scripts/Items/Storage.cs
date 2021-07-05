using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items
{
    [HideMonoScript]
    public class Storage : MonoBehaviour
    {
        [SerializeField]
        private bool _acceptLarge;
        public bool AcceptLarge { get => _acceptLarge; }

        public event Action<ItemEntity> onAddItemEntity;
        public event Action<ItemEntity> onRemoveItemEntity;

        //TODO: more protection from accessing this list?
        public readonly List<ItemEntity> itemsEntities = new List<ItemEntity>();

        public int Count(Item item) => itemsEntities.Count(i => i.item == item);

        public void Transfer(ItemEntity itemEntity, Storage target, int count)
        {
            if (!itemEntity.item.IsStackable)
            {
                RemoveItemEntity(itemEntity);
                target.AddItemEntity(itemEntity);
            }
            else
            {
                count = Mathf.Clamp(count, 1, itemEntity.Count);
                itemEntity.Count -= count;

                ItemEntity newItemEntity = null;
                newItemEntity = target.itemsEntities.Find(i => i.item == itemEntity.item);
                if (newItemEntity)
                {
                    newItemEntity.Count += count;
                }
                else
                {
                    newItemEntity = ItemEntity.Spawn(itemEntity.item, target.transform.position, Quaternion.identity);
                    newItemEntity.Count = count;
                    target.AddItemEntity(newItemEntity);
                }
            }
        }

        public void AddItemEntity(ItemEntity itemEntity)
        {
            if (itemEntity.item.IsStackable)
            {
                ItemEntity existingItemEntity = itemsEntities.Find(i => i.item == itemEntity.item);
                if (existingItemEntity)
                {
                    existingItemEntity.Count += itemEntity.Count;
                    Destroy(itemEntity.gameObject);
                }
                else
                {
                    itemsEntities.Add(itemEntity);
                    itemEntity.storage = this;
                    itemEntity.SetParent(transform, false); ;
                    itemEntity.onDestroy += RemoveItemEntityOnDestroy;
                    onAddItemEntity?.Invoke(itemEntity);
                }
            }
            else
            {
                itemsEntities.Add(itemEntity);
                itemEntity.storage = this;
                itemEntity.SetParent(transform, false); ;
                itemEntity.onDestroy += RemoveItemEntityOnDestroy;
                onAddItemEntity?.Invoke(itemEntity);
            }

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
}