using Sirenix.OdinInspector;
using System.Collections.Generic;
using Tags;
using UnityEngine;

[HideMonoScript]
public class Gatherable : MonoBehaviour, IDamageable
{
    [SerializeField, Required]
    private GameObject _prefab;
    public GameObject Prefab { get => _prefab; }


    [SerializeField, Min(1)]
    private int count = 1;

    public Damage.Type damageType;

    //TODO: readonly properties
    public float hp, hpMax = 100;

    public void ReceiveDamage(float damage, Damage.Type type)
    {
        if ((damageType & type) != 0)
        {
            hp -= damage;
            if (hp <= 0)
            {
                for (int i = 0; i < count; i++)
                    Instantiate(Prefab, transform.position + Vector3.up * i, Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                Destroy(gameObject);
            }
        }
    }
}
