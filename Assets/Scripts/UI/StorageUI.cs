using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class StorageUI : MonoBehaviour
{
    [SerializeField, Required]
    private TextMeshProUGUI nameText;
    [SerializeField, Required]
    private RectTransform listParent;
    [SerializeField, Required]
    private Button itemButton;

    [SerializeField, ReadOnly]
    private Storage _storage;
    public Storage Storage
    {
        get => _storage;
        set => Visible = _storage = value;
    }
    [SerializeField, ReadOnly]
    private bool _visible;
    public bool Visible
    {
        get => _visible;
        private set => _visible = enabled = canvas.enabled = value;
    }

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        Storage = Player.instance.GetComponent<Storage>();
    }

    private void Update()
    {
        nameText.text = Storage.name;
        Button button = null;
        int count = Mathf.Max(Storage.items.Count, listParent.childCount);
        for (int i = 0; i < count; i++)
        {
            //Deactivate button
            if(i >= Storage.items.Count)
                listParent.GetChild(i).gameObject.SetActive(false);

            //Instantiate new button or get from list
            if (i >= listParent.childCount)
            {
                button = Instantiate(itemButton, listParent);
            }
            else if (Storage.items.Count > 0)
                button = listParent.GetChild(i).GetComponent<Button>();

            //Setup and activate button
            if (Storage.items.Count > 0)
            {
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{Storage.items[i].name} x{Storage.Count(Storage.items[i])}";
            }
        }
    }
}
