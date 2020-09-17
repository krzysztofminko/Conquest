using Items;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Storage Storage { get; private set; }
    public ItemHolder ItemHolder { get; private set; }

    private void Awake()
    {
        Instance = this;
        Storage = GetComponent<Storage>();
        ItemHolder = GetComponent<ItemHolder>();
    }
}