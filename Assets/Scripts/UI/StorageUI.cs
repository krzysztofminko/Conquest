using Items;
using SelectableList;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class StorageUI : MonoBehaviour
{
    [SerializeField, Required]
    private TextMeshProUGUI nameText;
    [SerializeField, Required]
    private ListParent _listParent;
    public ListParent ListParent { get => _listParent; private set => _listParent = value; }

    [SerializeField, ReadOnly]
    private Storage _storage;
    public Storage Storage 
    {
        get => _storage;
        set
        {
            enabled = canvas.enabled = raycaster.enabled = value;
            if (_storage != value)
            {
                //Clear old storage
                if (_storage)
                {
                    _storage.onAddItemEntity -= Storage_onAddItemEntity;
                    _storage.onRemoveItemEntity -= Storage_onBeforeRemoveItemEntity;
                    ListParent.Clear();
                }

                //Assign
                _storage = value;

                //Setup new storage
                if (_storage)
                {
                    _storage.onAddItemEntity += Storage_onAddItemEntity;
                    _storage.onRemoveItemEntity += Storage_onBeforeRemoveItemEntity;
                    ListParent.Draw(_storage.itemsEntities);
                    nameText.text = _storage.name;
                }
            }
        } 
    }

    private Canvas canvas;
    private GraphicRaycaster raycaster;


    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
        ListParent.onCreateElement += SetupListElement;
    }

    private void SetupListElement(GameObject element, object assignedObject)
    {
        ItemEntity itemEntity = assignedObject as ItemEntity;
        element.GetComponentInChildren<TextMeshProUGUI>().text = itemEntity.item.name + (itemEntity.item.IsStackable ? $" x{itemEntity.Count}" : "");
    }

    private void Storage_onAddItemEntity(ItemEntity itemEntity) 
    {
        ListParent.AddElement(itemEntity);
    }

    private void Storage_onBeforeRemoveItemEntity(ItemEntity itemEntity)
    {
        ListParent.RemoveElement(itemEntity);
    }
}
