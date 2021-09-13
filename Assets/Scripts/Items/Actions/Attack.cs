using Damageable;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace Items.Actions
{

	[Category("Items")]
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
			//Cache agent components
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
			//Checks
			if (!agent.ItemEntity) 
			{
				Logger.LogWarning("Agent is not holding anything.", context: this);
				EndAction(false);
			}
			else if (agent.ItemEntity.item.weapon == null)
			{
				Logger.LogWarning($"Item is not a weapon ({agent.ItemEntity.item.name}).", context: this);
				EndAction(false);
			}
			else if (agent.ItemEntity.item.weapon.attacks.Count == 0)
			{
				Logger.LogWarning($"Weapon has no attacks defined ({agent.ItemEntity.item.name}).", context: this);
				EndAction(false);
			}
			else
			{
				//Init
				damageProcessed = false;
				target = _target.value; //Save inital Target value to prevent from changes before the end of action
				attack = agent.ItemEntity.item.weapon.attacks[0];//TODO: other attacks

				//Start animation if it exists
				if (attack.animation)
				{
					overrideAnimator.ChangeStateAnimationClip("EmptyAction", attack.animation);
					animator.SetTrigger("Action");
				}

				//Get trail particle
				trail = agent.ItemEntity ? agent.ItemEntity.GetComponentInChildren<ParticleSystem>() : null;
			}
		}

		protected override void OnUpdate()
		{
			//Control trail
			if (trail)
			{
				if (trail.isPlaying && elapsedTime > attack.trailEnd)
					trail.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				else if (!trail.isPlaying && elapsedTime > attack.trailStart && elapsedTime < attack.trailEnd)
					trail.Play();
			}

			//Damage frame
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

				//Deal damage
				if (targets.Count > 0)
				{
					if (attack.damage.Count > 0)
						for (int t = 0; t < targets.Count; t++)
							if (targets[t] != null)
								for (int d = 0; d < attack.damage.Count; d++)
									targets[t].ReceiveDamage(attack.damage[d].value, attack.damage[d].type);
				}

				//End damage frame
				damageProcessed = true;
			}

			//End action
			if (elapsedTime > (attack.animation ? attack.animation.length * 0.75f : 0))
			{
				EndAction(true);
			}
		}
	}
}