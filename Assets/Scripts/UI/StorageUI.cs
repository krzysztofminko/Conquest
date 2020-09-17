using SelectableList;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [ReadOnly]
    public Storage storage;
    [SerializeField, ReadOnly]
    private bool _visible;
    public bool Visible
    {
        get => _visible;
        set => _visible = enabled = canvas.enabled = raycaster.enabled = value;
    }


    private Canvas canvas;
    private GraphicRaycaster raycaster;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
        storage = Player.Instance.GetComponent<Storage>();
        ListParent.onElementSetup += SetupListElement;
    }

    private void Update()
    {
        nameText.text = storage.name;
        ListParent.UpdateList(storage.itemsEntities.Count);
    }

    private void SetupListElement(ListElement element, int id)
    {
        ItemEntity itemEntity = storage.itemsEntities[id];
        element.GetComponentInChildren<TextMeshProUGUI>().text = itemEntity.item.name + (itemEntity.item.IsStackable ? $" x{itemEntity.count}" : "");
    }
}
