using Damageable;
using UnityEngine;

namespace Items
{
	public class Projectile : MonoBehaviour
	{
		[Min(0)]
		public int damage = 10;
		[Min(1)]
		public float speed = 10;
		public LayerMask layerMask;

		[Min(0)]
		public float explosionRange;
		[Min(0)]
		public int explosionDamage;

		private float lifeRange = 100;


		private void Update()
		{
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed * Time.deltaTime, layerMask, QueryTriggerInteraction.Ignore))
			{
				IDamageable damageable = hit.collider.GetComponent<IDamageable>();
				if (damageable != null)
					damageable.ReceiveDamage(damage, Damage.Type.All);  //TODO: damage types

				if (explosionRange > 0)
				{
					Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRange, layerMask);
					for (int i = 0; i < colliders.Length; i++)
					{
						damageable = colliders[i].GetComponent<IDamageable>();
						if (damageable != null)
							damageable.ReceiveDamage(damage, Damage.Type.All);
					}
				}
				Destroy(gameObject);
			}
			else
			{
				lifeRange -= speed * Time.deltaTime;

				if (lifeRange < 0)
					Destroy(gameObject);
				else
					transform.position += transform.forward * speed * Time.deltaTime;
			}

		}
	}
}