using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Storage : MonoBehaviour
{
    [Serializable, InlineProperty]
    public class Slot
    {
        [Required]
        public Item item;
        [Min(1)]
        public int count = 1;
    }

    [Button]
    private void ClearEmptySlots() => _slots.RemoveAll(s => s.item == null || s.count < 1);

    [SerializeField]
    private List<Slot> _slots;

    private void OnValidate()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].count = _slots.FindAll(s => s.item == _slots[i].item).Sum(s => s.count);
            _slots.RemoveAll(s => s.item == _slots[i].item && s != _slots[i]);
        }
    }


    public int Count(Item item)
    {
        Slot slot = GetSlot(item);
        return slot == null ? 0 : slot.count;
    } 

    public void Remove(Item item, int count = 1)
    {
        if (count < 1)
        {
            Debug.LogError($"Can't remove less ({count}) than 1 {item} from storage.",this);
        }
        else
        {
            Slot slot = GetSlot(item);
            if(count > slot.count)
            {
                Debug.LogError($"Can't remove more ({count}) than maximum ({slot.count}) of {item} from storage.", this);
            }
            else
            {
                slot.count -= count;
                if (slot.count == 0)
                    _slots.Remove(slot);
            }
        }
    }

    public void Add(Item item, int count = 1)
    {
        if (count < 1)
        {
            Debug.LogError($"Can't add less ({count}) than 1 {item} to storage.", this);
        }
        else
        {
            Slot slot = GetSlot(item);
            if (slot != null)
            {
                slot.count += count;
            }
            else
            {
                _slots.Add(new Slot { item = item, count = count });
            }
        }
    }

    private Slot GetSlot(Item item) => _slots.Find(s => s.item == item);

}
