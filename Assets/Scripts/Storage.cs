using Items;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Storage : MonoBehaviour
{
    [SerializeField]
    private bool _acceptLarge;
    public bool AcceptLarge { get => _acceptLarge; }

    public readonly List<ItemEntity> itemsEntities = new List<ItemEntity>();
        
    public int Count(Item item) => itemsEntities.Count(i => i.item == item);

    public void AddItemEntity(ItemEntity itemEntity)
    {
        itemsEntities.Add(itemEntity);
        itemEntity.gameObject.SetActive(false);
    }

    public void RemoveItemEntity(ItemEntity itemEntity)
    {
        itemsEntities.Remove(itemEntity);
        itemEntity.gameObject.SetActive(true);
    }
}
