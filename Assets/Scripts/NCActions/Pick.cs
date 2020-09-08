using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("ItemHolder")]
	public class Pick : ActionTask<ItemHolder>
	{
		public BBParameter<ItemEntity> _itemEntity;

		private ItemEntity itemEntity;
		private float duration;
		
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
			itemEntity = _itemEntity.value;

			duration = 0;
			if (itemEntity.item.pickAnimation)
			{
				overrideAnimator.ChangeStateAnimationClip("Action", itemEntity.item.pickAnimation);
				animator.SetTrigger("Action");
				duration = itemEntity.item.pickAnimation.length;
			}
		}

		protected override void OnUpdate()
		{
			if(elapsedTime > duration)
			{

			}

			if (!_itemEntity.value)
				EndAction(false);


		}
	}
}