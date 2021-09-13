using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace PlayerActions
{
	[Category("Player")]
	public class PlayerMovement : ActionTask
	{
		public BBParameter<float> turnSpeed = 1000;
		public BBParameter<float> walkSpeed = 4;
		public BBParameter<float> runSpeed = 6;

		private Quaternion targetRotation;
		private CharacterController characterController;
		private Animator animator;
		private bool strafing;
		private bool crouching;
		private bool swimming;

		protected override string OnInit()
		{
			characterController = agent.GetComponent<CharacterController>();
			animator = agent.GetComponent<Animator>();
			if (!animator)
				animator = agent.GetComponentInChildren<Animator>();

			if (!characterController)
				return "Agent has no CharacterController component.";
			if (!animator)
				return "Agent has no Animator component.";
			return null;
		}

		protected override void OnUpdate()
		{
			//Get input
			Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			if(Input.GetKeyDown(KeyCode.Z))
				strafing = !strafing;
			if (Input.GetKeyDown(KeyCode.C))
				crouching = !crouching;
			if (Input.GetKeyDown(KeyCode.X))
				swimming = !swimming;

			//Move and rotate
			Vector3 finalDirection = Vector3.ProjectOnPlane(Camera.main.transform.rotation * inputDirection, Vector3.up);
			float finalSpeed = (walkSpeed.value + Mathf.Max(0, (runSpeed.value - walkSpeed.value) * Input.GetAxis("Run"))) * finalDirection.magnitude;
			if (finalDirection.sqrMagnitude > 0)
				targetRotation = Quaternion.LookRotation(strafing ? Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) : finalDirection);
			agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, turnSpeed.value * Time.deltaTime);
			characterController.SimpleMove(finalDirection * finalSpeed);

			//Update animator
			animator.SetFloat("MoveSpeed", Mathf.Clamp01(Input.GetAxis("Run") > 0 ? finalSpeed / runSpeed.value : finalSpeed / walkSpeed.value));
			animator.SetFloat("DirectionX", strafing ? inputDirection.x : 0);
			animator.SetFloat("DirectionZ", strafing ? inputDirection.z : 1);
			animator.SetFloat("Run", Input.GetAxis("Run"));
			animator.SetBool("Crouch", crouching);
			animator.SetBool("Swim", swimming);
		}

		protected override void OnStop()
		{
			animator.SetFloat("MoveSpeed", 0);
		}

		protected override void OnPause()
		{
			animator.SetFloat("MoveSpeed", 0);
		}
	}
}