using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FreeLookCameraDistance : MonoBehaviour
{
	[SerializeField, Required]
	private CinemachineFreeLook freeLookCamera;

	[SerializeField, Min(0)]
	private float speed = 10;
	[SerializeField, PropertyRange("min", "max")]
	public float distance;
	[SerializeField, Min(0)]
	public float min = 0;
	[SerializeField, Min(0)]
	public float max = 1;

	private void Awake()
	{
		SetCameraDistance(distance);
	}

	private void OnValidate()
	{
		SetCameraDistance(distance);
	}

	private void Update()
	{
		SetCameraDistance(distance - Input.GetAxis("CameraZoom") * speed * Time.deltaTime);
	}

	private void SetCameraDistance(float distance)
	{
		distance = Mathf.Clamp(distance, min, max);
		freeLookCamera.m_Orbits[0].m_Height = distance;
		freeLookCamera.m_Orbits[0].m_Radius = distance * 0.5f;
		freeLookCamera.m_Orbits[1].m_Height = 0;
		freeLookCamera.m_Orbits[1].m_Radius = distance;
		freeLookCamera.m_Orbits[2].m_Height = -distance;
		freeLookCamera.m_Orbits[2].m_Radius = distance * 0.5f;
		this.distance = distance;
	}
}