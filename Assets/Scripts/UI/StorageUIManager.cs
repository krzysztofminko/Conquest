using Sirenix.OdinInspector;
using UnityEngine;
using SelectableList;
using UnityEngine.AI;

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
        if (SelectedItemEntity)
        {
            if (PlayerStorageIsSelected)
            {
                if (InputHints.GetButtonDown("Drop", "Drop"))
                {
                    Player.Instance.Storage.RemoveItemEntity(SelectedItemEntity);
                    SelectedItemEntity.transform.position = Player.Instance.transform.position;
                }
                if (InputHints.GetButtonDown("Equip", "Equip"))
                {
                    SelectedItemEntity.gameObject.SetActive(true);
                    Player.Instance.ItemHolder.ItemEntity = SelectedItemEntity;
                }
            }
        }
    }

    public void ShowStorageUI(bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.blocksRaycasts = playerUI.Visible = enabled = show;

        if (show)
        {
            playerUI.ListParent.onSelect += SelectPlayerItemEntity;
        }
        else
        {
            playerUI.ListParent.onSelect -= SelectPlayerItemEntity;
        }
    }

    public void SetTargetStorage(Storage target = null)
    {
        targetUI.Visible = targetUI.storage = target;

        if (target)
        {
            targetUI.ListParent.onSelect += SelectTargetItemEntity;
        }
        else
        {
            targetUI.ListParent.onSelect -= SelectTargetItemEntity;
        }
    }

    public void SelectPlayerItemEntity(ListElement element, int id)
    {
        SelectedItemEntity = playerUI.storage.itemsEntities[id];
        PlayerStorageIsSelected = true;
    }

    public void SelectTargetItemEntity(ListElement element, int id)
    {
        SelectedItemEntity = targetUI.storage.itemsEntities[id];
        PlayerStorageIsSelected = false;
    }
}
