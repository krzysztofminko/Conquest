using Items;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Storage : MonoBehaviour
{
    [SerializeField]
    private List<ItemEntity> itemsEntities;
        
    public int Count(Item item) => itemsEntities.Count(i => i.item == item);

    //TODO: redesign this element!
    public readonly List<Item> items = new List<Item>();

    public void AddItem(ItemEntity itemEntity)
    {
        if (!itemsEntities.Find(i => i.item == itemEntity.item))
            items.Add(itemEntity.item);

        itemsEntities.Add(itemEntity);
        itemEntity.gameObject.SetActive(false);
    }

    public void RemoveItem(ItemEntity itemEntity)
    {
        itemsEntities.Remove(itemEntity);
        itemEntity.gameObject.SetActive(true);
        if (Count(itemEntity.item) == 0)
            items.Remove(itemEntity.item);
    }
}
