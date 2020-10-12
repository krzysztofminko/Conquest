using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("Item")]
	public class Pick : ActionTask<ItemHolder>
	{
		public BBParameter<ItemEntity> _itemEntity;

		private ItemEntity itemEntity;
		private bool processed;
		
		private OverrideAnimator overrideAnimator;
		private Animator animator;
		private Storage storage;

		protected override string OnInit()
		{
			overrideAnimator = agent.GetComponent<OverrideAnimator>();
			animator = agent.GetComponent<Animator>();
			storage = agent.GetComponent<Storage>();

			if (!overrideAnimator)
				return "No OverrideAnimator component on agent game object.";
			if (!animator)
				return "No Animator component on agent game object.";
			if (!storage)
				return "No Storage component on agent game object.";
			return null;
		}

		protected override void OnExecute()
		{
			//Init
			itemEntity = _itemEntity.value;
			processed = false;

			//Start animation if it exists
			if (itemEntity.item.pickAnimation)
			{
				overrideAnimator.ChangeStateAnimationClip("EmptyAction", itemEntity.item.pickAnimation);
				animator.SetTrigger("Action");
			}
		}

		protected override void OnUpdate()
		{
			//Check item entity
			if (!itemEntity || 
				(itemEntity.holder && itemEntity.holder != agent)
				)
			{
				EndAction(false);
			}
			else
			{
				//Process
				if (!processed && elapsedTime > itemEntity.item.pickDelay)
				{
					//Pick
					if (itemEntity.item.IsLarge)
						agent.ItemEntity = itemEntity;
					else
						storage.AddItemEntity(itemEntity);

					//Set carrying pose
					if (itemEntity.item.carryAnimation)
						overrideAnimator.ChangeStateAnimationClip("EmptyUpperIdle", itemEntity.item.carryAnimation);

					processed = true;
				}

				//Finish
				if(elapsedTime > (itemEntity.item.pickAnimation ? itemEntity.item.pickAnimation.length : 0))
				{
					EndAction(true);
				}
			}
		}
	}
}