using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//TODO: Use OdinInspector
//TODO: Add hold effect/icon
//TODO: assign icons to keys not axes
public class InputHints : MonoBehaviour
{
	[System.Serializable]
	private class Hint
	{
		public string axis;
		public string text;
		public Sprite icon;
		public Transform transformComponent;
		public Image imageComponent;
		public TextMeshProUGUI textComponent;
		public bool dontDestroy;
	}

	[System.Serializable]
	private class AxisIcons
	{
		public string axis;
		public Sprite keyboard;
		public Sprite controller;
	}

	[System.Serializable]
	private class AxisHoldDuration
	{
		public string axis;
		public float duration;
	}

	public static InputHints instance;
	private static bool controller;
	private static List<Hint> hints = new List<Hint>();
	private static List<AxisHoldDuration> axisHoldDurations = new List<AxisHoldDuration>();

	[SerializeField] private Transform listParent;
	[SerializeField] private Transform prefab;
	[SerializeField, Min(0)] private float defaultHoldDuration = 1;
	[SerializeField] private List<AxisIcons> axisIcons = new List<AxisIcons>();


	private void Awake()
	{
		//TODO: validate for only one posible instance
		instance = this;
	}

	private void LateUpdate()
	{
		//TODO: better controls switching
		string[] joys = Input.GetJoystickNames();
		controller = joys.Length > 0 && joys.Any(j => !string.IsNullOrEmpty(j));

		for (int i = hints.Count - 1; i >= 0; i--)
			if (!hints[i].dontDestroy)
			{
				Destroy(hints[i].transformComponent.gameObject);
				hints.RemoveAt(i);
			}
			else
			{
				hints[i].dontDestroy = false;
			}

		//Update held buttons
		for (int i = axisHoldDurations.Count - 1; i >= 0; i--)
		{
			if (Input.GetButtonUp(axisHoldDurations[i].axis))
				axisHoldDurations.RemoveAt(i);
			else
				axisHoldDurations[i].duration += Time.deltaTime;
		}
	}


	private static void SetHint(string axis, string text, bool hold)
	{
		Hint hint = hints.Find(h => h.axis == axis);

		if (hold)
			text = "(hold) " + text;

		if (hint == null)
		{
			hint = new Hint();
			hint.axis = axis;
			hint.text = text == null ? axis : text;
			hint.transformComponent = Instantiate(instance.prefab, instance.listParent);
			hint.transformComponent.gameObject.SetActive(true);
			hint.imageComponent = hint.transformComponent.GetChild(0).GetComponent<Image>();
			hint.textComponent = hint.transformComponent.GetChild(1).GetComponent<TextMeshProUGUI>();
			hints.Add(hint);
		}
		else
		{
			hints.Remove(hint);
			hints.Add(hint);
			hint.transformComponent.SetAsLastSibling();
		}
		AxisIcons ai = instance.axisIcons.Find(a => a.axis == hint.axis);
		hint.icon = ai == null? null : (controller? ai.controller : ai.keyboard);
		hint.imageComponent.sprite = hint.icon;
		hint.textComponent.text = text == null ? axis : text;
		hint.dontDestroy = true;
	}

	public static bool GetButtonDown(string axis, string text = null)
	{
		SetHint(axis, text, false);
		return Input.GetButtonDown(axis);
	}

	public static bool GetButtonUp(string axis, string text = null)
	{
		SetHint(axis, text, false);
		return Input.GetButtonUp(axis);
	}

	public static bool GetButtonHold(string axis, string text = null)
	{
		SetHint(axis, text, true);

		if (Input.GetButtonDown(axis))
			axisHoldDurations.Add(new AxisHoldDuration { axis = axis, duration = 0 });

		if (Input.GetButton(axis))
		{
			AxisHoldDuration ahd = axisHoldDurations.Find(ad => ad.axis == axis);
			if (ahd != null && ahd.duration > instance.defaultHoldDuration)
			{
				axisHoldDurations.Remove(ahd);
				return true;
			}
		}
		return false;
	}
}
