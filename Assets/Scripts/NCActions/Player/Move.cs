using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Player")]
	public class Move : ActionTask
	{
		public BBParameter<bool> enabled = true;

		public BBParameter<float> walkSpeed = 4;
		public BBParameter<float> runSpeed = 8;
		public BBParameter<float> turnSpeed = 600;
		public BBParameter<AnimationClip> idleAnimation;
		public BBParameter<AnimationClip> moveAnimation;

		public bool playerIsRunning;

		private OverrideAnimator overrideAnimator;
		private Animator animator;

		private CharacterController characterController;

		protected override string OnInit()
		{
			overrideAnimator = agent.GetComponent<OverrideAnimator>();
			characterController = agent.GetComponent<CharacterController>();
			animator = agent.GetComponent<Animator>();

			if (!overrideAnimator)
				return "No OverrideAnimator component on agent game object.";
			if (!characterController)
				return "Player has no CharacterController.";
			if (!animator)
				return "Player has no Animator.";

			overrideAnimator.ChangeStateAnimationClip("EmptyIdle", idleAnimation.value);
			overrideAnimator.ChangeStateAnimationClip("EmptyMove", moveAnimation.value);

			return null;
		}

		protected override void OnUpdate()
		{

			if (enabled.value)
			{
				if (Input.GetButtonDown("Run"))
					playerIsRunning = !playerIsRunning;
				float speed = playerIsRunning ? runSpeed.value : walkSpeed.value;
				Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
				if (movement.sqrMagnitude > 0)
				{
					agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.rotation * movement.normalized, Vector3.up)), turnSpeed.value * Time.deltaTime);
				}
				else
				{
					speed = 0;
				}
					characterController.SimpleMove(Camera.main.transform.rotation * movement);
					animator.SetFloat("MoveSpeed", speed);
				}
			else
			{
				animator.SetFloat("MoveSpeed", 0);
			}
		}
	}
}