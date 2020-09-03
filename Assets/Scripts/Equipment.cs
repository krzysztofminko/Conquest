using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HideMonoScript]
public class Equipment : MonoBehaviour
{
    public enum SlotType { Head, Chest, Legs }

    [Serializable]
    public class Slot
    {
        [ReadOnly]
        public SlotType type;
        [ReadOnly]
        public Item item;
    }

    [SerializeField, TableList(AlwaysExpanded = true, IsReadOnly = true)]
    private List<Slot> _slots;

    private void Reset()
    {
        if (_slots == null)
            _slots = new List<Slot>();
        Array values = Enum.GetValues(typeof(SlotType));
        for (int i = 0; i < values.Length; i++)
            _slots.Add(new Slot { type = (SlotType)values.GetValue(i) });
    }

    public void Equip(Item item, SlotType slot)
    {
        if (item)
        {
            Debug.LogError("Can not Equip null object.", this);
        }
        if(item.equipable == null)
        {
            Debug.LogError($"Item ({item}) is not equipable.", item);
        }
        else
        {
            if (_slots.Find(s => s.type == slot).item)
                Unequip(slot);
            _slots.Find(s => s.type == slot).item = item;
            //TODO: add equipable modifiers
        }
    }

    public void Unequip(SlotType slot)
    {
        //TODO: remove equipable modifiers
        _slots.Find(s => s.type == slot).item = null;
    }
}
