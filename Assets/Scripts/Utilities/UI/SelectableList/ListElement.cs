using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SelectableList
{
	public class ListElement : MonoBehaviour, ISelectHandler
	{
		public event Action onSelect;

		public void OnSelect(BaseEventData eventData)
		{
			onSelect?.Invoke();
		}
	}
}