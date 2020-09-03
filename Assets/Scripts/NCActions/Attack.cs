using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using Tags;
using UnityEngine;
using Utilities;

namespace NodeCanvas.Tasks.Actions{

	[Category("ItemHolder")]
	public class Attack : ActionTask<ItemHolder>
	{
		public BBParameter<GameObject> _target;
		private GameObject target;
		public LayerMask layerMask;

		private bool damageProcessed;

		private AttackSettings attack;
		private List<IDamageable> targets = new List<IDamageable>();

		private OverrideAnimator overrideAnimator;
		private Animator animator;
		private ParticleSystem trail;


		protected override string OnInit()
		{
			overrideAnimator = agent.GetComponent<OverrideAnimator>();
			animator = agent.GetComponent<Animator>();

			if (!overrideAnimator)
				return "No OverrideAnimator component on agent game object.";
			if (!animator)
				return "No Animator component on agent game object.";
			return null;
		}
		protected override void OnExecute()
		{
			if (!agent.RightHandItem ||
				agent.RightHandItem.weapon == null ||
				agent.RightHandItem.weapon.attacks.Count == 0)
				EndAction(false);

			damageProcessed = false;
			target = _target.value;
			attack = agent.RightHandItem.weapon.attacks[0];//TODO: other attacks

			if (!attack.animation)
				Debug.LogError($"Weapon ({agent.RightHandItem}) attack has no animation assigned.", agent.RightHandItem);

			//Start animation
			overrideAnimator.ChangeStateAnimationClip("EmptyAction", attack.animation);
			animator.SetTrigger("Action");

			trail = agent.RightHandItemEntity? agent.RightHandItemEntity.GetComponentInChildren<ParticleSystem>() : null;
		}

		protected override void OnUpdate()
		{
			//Swing trail
			if (trail)
			{
				if (trail.isPlaying && elapsedTime > attack.trailEnd)
					trail.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				else if (!trail.isPlaying && elapsedTime > attack.trailStart && elapsedTime < attack.trailEnd)
					trail.Play();
			}

			if (!damageProcessed && elapsedTime > attack.damageDelay)
			{
				//Collect targets
				targets.Clear();
				if (attack.targetOnly)
				{
					if (target && (target.transform.position - agent.transform.position).sqrMagnitude < attack.range * attack.range)
					{
						IDamageable[] damageables = target.GetComponents<IDamageable>();
						for (int d = 0; d < damageables.Length; d++)
							targets.Add(damageables[d]);
					}
				}
				else
				{
					Collider[] colliders = Physics.OverlapSphere(agent.transform.position + Vector3.up * attack.range, attack.range, layerMask, QueryTriggerInteraction.Collide);
					for (int c = 0; c < colliders.Length; c++)
					{
						if (colliders[c].gameObject != agent.gameObject)
						{
							if (Vector3.Angle(colliders[c].transform.position - agent.transform.position, agent.transform.forward) < 90)
							{
								IDamageable[] damageables = colliders[c].gameObject.GetComponents<IDamageable>();
								for (int d = 0; d < damageables.Length; d++)
									targets.Add(damageables[d]);
							}
						}
					}
				}

				//Hit something
				if (targets.Count > 0)
				{
					//Deal damage
					if (attack.damage.Count > 0)
						for (int t = 0; t < targets.Count; t++)
							if (targets[t] != null)
								for (int d = 0; d < attack.damage.Count; d++)
									targets[t].ReceiveDamage(attack.damage[d].value, attack.damage[d].type);
				}

				damageProcessed = true;
			}

			if (elapsedTime > attack.animation.length)
			{
				EndAction(true);
			}
		}
	}
}