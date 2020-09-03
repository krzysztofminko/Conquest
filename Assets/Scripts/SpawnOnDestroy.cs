using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class SpawnOnDestroy : MonoBehaviour
{
    [System.Serializable]
    public class PrefabCount
    {
        [HorizontalGroup("Prefab"), HideLabel, Required]
        public GameObject prefab;
        [HorizontalGroup("Count"), HideLabel, Min(1)]
        public int count = 1;
    }

    [SerializeField, Required, TableList(AlwaysExpanded = true), HideLabel]
    private List<PrefabCount> _prefabs = new List<PrefabCount>();
    public List<PrefabCount> Prefabs { get => _prefabs; }

    private bool isQuitting;

    public void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void OnDestroy()
    {
        if (!isQuitting)
        {
            int count = 1;
            for (int p = 0; p < Prefabs.Count; p++)
                for (int i = 0; i < Prefabs[p].count; i++)
                {
                    Instantiate(Prefabs[p].prefab, transform.position + Vector3.up * count, Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                    count++;
                }
        }
    }
}
