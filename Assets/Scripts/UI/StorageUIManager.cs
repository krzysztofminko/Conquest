using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class StorageUIManager : MonoBehaviour
{
    private static StorageUIManager instance;

    [SerializeField, Required]
    private StorageUI playerStorageUI;
    [SerializeField, Required]
    private StorageUI targetStorageUI;

    private CanvasGroup canvasGroup;

    private void Awake() 
    { 
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        ShowStorageUI(false);
    }

    public static void ShowStorageUI(bool show)
    {
        instance.canvasGroup.alpha = show ? 1 : 0;
        instance.canvasGroup.blocksRaycasts = show;
        instance.playerStorageUI.Visible = show;
    }
    public static void SetTargetStorage(Storage target = null)
    {
        instance.targetStorageUI.Visible = instance.targetStorageUI.storage = target;
    }
}
