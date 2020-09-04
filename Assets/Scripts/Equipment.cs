using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public Transform parent;
        [ReadOnly]
        public ItemEntity itemEntity;
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

    public Slot this [SlotType slotType]
    {
        get => _slots.Find(s => s.type == slotType);
    }

    public bool IsEquiped(ItemEntity itemEntity) => _slots.Find(s => s.itemEntity == itemEntity) != null;

    public void Equip(ItemEntity itemEntity, SlotType slotType)
    {
        if (!itemEntity)
        {
            Debug.LogError("Can not Equip null object.", this);
        }
        if(itemEntity.item.equipable == null)
        {
            Debug.LogError($"Item ({itemEntity.item}) is not equipable.", itemEntity);
        }
        else
        {
            Slot slot = this[slotType];
            if (slot.itemEntity)
                Unequip(slot.itemEntity);
            slot.itemEntity = itemEntity;
            itemEntity.SetParent(slot.parent);
            //TODO: add equipable modifiers
        }
    }

    public void Unequip(ItemEntity itemEntity)
    {
        //TODO: remove equipable modifiers
        if (!itemEntity)
            Debug.LogError("Item entity can't be null.", this);
        
        Slot slot = _slots.Find(s => s.itemEntity == itemEntity);
        if (slot == null)
        {
            Debug.LogError($"Trying to Unequip already not equiped item {itemEntity}", this);
        }
        else
        {
            slot.itemEntity.SetParent(null);
            slot.itemEntity = null;
        }
    }
}
