using UnityEngine;

public class FollowPosition : MonoBehaviour
{
	public enum UpdateType { Update, LateUpdate, FixedUpdate}

	public UpdateType updateType;

	public Transform follow;

	private Vector3? offset;

	private void Update()
	{
		if (updateType == UpdateType.Update)
			SetPosition();
	}

	private void LateUpdate()
	{
		if (updateType == UpdateType.LateUpdate)
			SetPosition();
	}

	private void FixedUpdate()
	{
		if (updateType == UpdateType.FixedUpdate)
			SetPosition();
	}

	private void SetPosition()
	{
		if (offset == null)
			offset = follow ? transform.position - follow.position : Vector3.zero;
		if (follow)
			transform.position = follow.position + offset.Value;
	}
}
