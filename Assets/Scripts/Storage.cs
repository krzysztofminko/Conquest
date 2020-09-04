using Items;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Storage : MonoBehaviour
{
    [SerializeField]
    private List<ItemEntity> items;

    public void AddItem(ItemEntity itemEntity)
    {
        items.Add(itemEntity);
        itemEntity.gameObject.SetActive(false);
    }

    public void RemoveItem(ItemEntity itemEntity)
    {
        items.Remove(itemEntity);
        itemEntity.gameObject.SetActive(true);
    }

    public int Count(Item item) => items.Count(i => i.item == item);
}
