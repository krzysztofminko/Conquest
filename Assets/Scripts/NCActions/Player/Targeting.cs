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
			target.SetValue(Physics.OverlapSphere(agent.transform.position + Vector3.up + agent.transform.forward, 1, layerMask, QueryTriggerInteraction.Collide)
				.OrderBy(c => Distance.Manhattan2D(agent.transform.position, c.transform.position))
				.FirstOrDefault(c => c.GetComponent<GameObject>())?
				.GetComponent<GameObject>());
		}
	}
}