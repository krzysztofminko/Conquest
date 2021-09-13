using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
	[RequireComponent(typeof(Animator)), HideMonoScript]
	public class IgnoreAnimationEvents : MonoBehaviour
	{
		[SerializeField]
		private bool _ignore = true;
		public bool Ignore
		{
			get => _ignore;
			set
			{
				if (!animator)
					animator = GetComponent<Animator>();
				animator.fireEvents = !_ignore;
				_ignore = value;
			}
		}

		private Animator animator;

		private void Awake() => Ignore = _ignore;
	}
}