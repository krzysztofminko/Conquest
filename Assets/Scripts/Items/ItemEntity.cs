using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace Items
{
	[RequireComponent(typeof(Collider), typeof(Rigidbody)), HideMonoScript]
	public class ItemEntity : MonoBehaviour
	{
		public event Action<ItemEntity> onDestroy;
		public event Action<ItemEntity> onStateChange;

		[DisableInPlayMode]
		public Item item;
		[SerializeField, ReadOnly, Min(1)]
		private int _count = 1;
		public int Count
		{
			get => _count;
			set
			{
				if (_count != value)
				{
					_count = value;
					onStateChange?.Invoke(this);
					if (value < 1)
						Destroy(gameObject);
				}
			}
		}


		[ReadOnly]
		public float condition;
		[ReadOnly]
		public ItemHolder holder;
		[ReadOnly]
		public Storage storage;


		private new Collider collider;
		private new Rigidbody rigidbody;

		private void Awake()
		{
			collider = GetComponent<Collider>();
			rigidbody = GetComponent<Rigidbody>();
		}

		public static ItemEntity Spawn(Item item, Vector3 position, Quaternion rotation)
		{
			if (!item.prefab)
				Debug.LogError($"{item} has no prefab.", item);

			ItemEntity entity = Instantiate(item.prefab, position, rotation);
			entity.item = item;
			entity.condition = item.damageable != null ? item.damageable.condition : 0;
			return entity;
		}

		public void SetParent(Transform parent, bool gameObjectActive)
		{
			gameObject.SetActive(gameObjectActive);
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

		private void OnDestroy()
		{
			if (!AppState.IsQuitting)
			{
				onDestroy?.Invoke(this);
			}
		}
	}
}