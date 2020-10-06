using SelectableList;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class StorageUIManager : MonoBehaviour
{
    public static StorageUIManager Instance { get; private set; }

    [SerializeField, Required]
    private StorageUI playerUI;
    [SerializeField, Required]
    private StorageUI targetUI;

    [SerializeField, ReadOnly]
    private ItemEntity _selectedItemEntity;
    public ItemEntity SelectedItemEntity { get => _selectedItemEntity; private set => _selectedItemEntity = value; }
    [SerializeField, ReadOnly]
    private bool _playerStorageIsSelected = true;
    public bool PlayerStorageIsSelected { get => _playerStorageIsSelected; private set => _playerStorageIsSelected = value; }

    private CanvasGroup canvasGroup;

    private void Awake() 
    { 
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        ShowStorageUI(false);
    }

    private void Update()
    {
        if (!SelectedItemEntity)
        {
            if (playerUI.Storage && playerUI.Storage.itemsEntities.Count > 0)
                playerUI.ListParent.SelectIndex(0);
            else if (targetUI.Storage && targetUI.Storage.itemsEntities.Count > 0)
                targetUI.ListParent.SelectIndex(0);
        }
    }

    public void ShowStorageUI(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.blocksRaycasts = enabled = show;
        playerUI.Storage = show ? Player.Instance.Storage : null;

        if (show)
        {
            playerUI.ListParent.onSelectedUpdate += SelectPlayerItemEntity;
            playerUI.ListParent.SelectIndex(0);
        }
        else
        {
            playerUI.ListParent.onSelectedUpdate -= SelectPlayerItemEntity;
        }
    }


    public void SetTargetStorage(Storage target = null)
    {
        targetUI.Storage = target;

        if (target)
        {
            targetUI.ListParent.onSelectedUpdate += SelectTargetItemEntity;
        }
        else
        {
            targetUI.ListParent.onSelectedUpdate -= SelectTargetItemEntity;
        }
    }

    private void SelectPlayerItemEntity(GameObject listElement, object assignedObject)
    {
        if (listElement)
        {
            SelectedItemEntity = assignedObject as ItemEntity;
            PlayerStorageIsSelected = true;
        }
        else if (targetUI.Storage && targetUI.Storage.itemsEntities.Count > 0)
        {
            targetUI.ListParent.SelectIndex(0);
        }
        else
        {
            SelectedItemEntity = null;
        }
    }

    private void SelectTargetItemEntity(GameObject listElement, object assignedObject)
    {
        if (listElement)
        {
            SelectedItemEntity = assignedObject as ItemEntity;
            PlayerStorageIsSelected = false;
        }
        else if (playerUI.Storage && playerUI.Storage.itemsEntities.Count > 0) 
        { 
            playerUI.ListParent.SelectIndex(0);
        }
        else
        {
            SelectedItemEntity = null;
        }
    }
}
