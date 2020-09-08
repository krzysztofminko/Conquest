using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class OverrideAnimator : MonoBehaviour
{
	private Animator animator;
	private AnimatorOverrideController animatorOverrideController;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		animator.runtimeAnimatorController = animatorOverrideController;
		
	}

	public void ChangeStateAnimationClip(string stateName, AnimationClip animationClip)
	{
		animatorOverrideController[stateName] = animationClip;
	}
}
