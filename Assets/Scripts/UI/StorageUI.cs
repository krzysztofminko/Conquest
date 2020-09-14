using Sirenix.OdinInspector;
using System;
using System.Collections;
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
    private RectTransform listParent;
    [SerializeField, Required]
    private Button itemButton;

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
    }

    private void Start()
    {
        storage = Player.instance.GetComponent<Storage>();
    }

    private void Update()
    {
        nameText.text = storage.name;
        Button button = null;
        int count = Mathf.Max(storage.items.Count, listParent.childCount);
        for (int i = 0; i < count; i++)
        {
            //Deactivate button
            if(i >= storage.items.Count)
                listParent.GetChild(i).gameObject.SetActive(false);

            //Instantiate new button or get from list
            if (i >= listParent.childCount)
            {
                button = Instantiate(itemButton, listParent);
            }
            else if (storage.items.Count > 0)
                button = listParent.GetChild(i).GetComponent<Button>();

            //Setup and activate button
            if (storage.items.Count > 0)
            {
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{storage.items[i].name} x{storage.Count(storage.items[i])}";
            }
        }
    }
}
