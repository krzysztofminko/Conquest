using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Linq;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("Player")]
	public class Targeting : ActionTask<Player>
	{
		public LayerMask layerMask;
		public BBParameter<GameObject> target;

		protected override void OnUpdate()
		{
			target.value = Physics.OverlapSphere(agent.transform.position + Vector3.up + agent.transform.forward, 2, layerMask, QueryTriggerInteraction.Collide)
				.OrderBy(c => Distance.Manhattan2D(agent.transform.position, c.transform.position))
				.FirstOrDefault(c => c.gameObject != agent.gameObject)?
				.gameObject;
		}
	}
}