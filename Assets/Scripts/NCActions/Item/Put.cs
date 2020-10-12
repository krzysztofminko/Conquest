using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Item")]
	public class Put : ActionTask<ItemHolder>
	{
		public BBParameter<ItemEntity> _itemEntity;

		private ItemEntity itemEntity;
		private bool processed;

		private OverrideAnimator overrideAnimator;
		private Animator animator;

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
			//Check if item entity exists
			if (!agent.ItemEntity)
				Debug.LogError("agent.ItemEntity == null, can't Put it.", agent);

			//Init
			itemEntity = _itemEntity.value;
			processed = false;

			//Start animation if it exists
			if (itemEntity.item.putAnimation)
			{
				overrideAnimator.ChangeStateAnimationClip("EmptyAction", itemEntity.item.putAnimation);
				animator.SetTrigger("Action");
			}
		}

		protected override void OnUpdate()
		{
			//Process
			if (!processed && elapsedTime > itemEntity.item.putDelay)
			{
				//Put
				agent.ItemEntity = null;

				//Set carrying pose
				if (itemEntity.item.carryAnimation)
					overrideAnimator.ChangeStateAnimationClip("EmptyUpperIdle", null);

				processed = true;
			}

			//Finish
			if (elapsedTime > (itemEntity.item.putAnimation ? itemEntity.item.putAnimation.length : 0))
			{
				EndAction(true);
			}
		}
	}
}