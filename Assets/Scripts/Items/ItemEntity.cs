using UnityEngine;
using System.Collections;
using Items;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using StatsWithModifiers;

[RequireComponent(typeof(Collider), typeof(Rigidbody)), HideMonoScript]
public class ItemEntity : MonoBehaviour
{
	[DisableInPlayMode]
	public Item item;
	[ReadOnly]
	public float condition;
	[ReadOnly]
	public ItemHolder holder;

	private new Collider collider;
	private new Rigidbody rigidbody;

	private void Awake()
	{
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public static ItemEntity Spawn(Item item, Vector3 position, Quaternion rotation)
	{
		if(!item.prefab)
			Debug.LogError($"{item} has no prefab.", item);

		ItemEntity entity = Instantiate(item.prefab, position, rotation);
		entity.item = item;
		entity.condition = item.damageable != null ? item.damageable.condition : 0;
		return entity;
	}

	public void SetParent(Transform parent)
	{
		transform.parent = parent;
		collider.enabled = !parent;
		rigidbody.isKinematic = parent;
		if (parent)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		else
		{
			//if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5, NavMesh.AllAreas))
				//transform.position = hit.position;
		}
	}
}
