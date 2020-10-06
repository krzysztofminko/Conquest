using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SelectableList
{
	public class ListElement : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		public event Action<ListElement> onSelect;
		public event Action<ListElement> onDeselect;

		/// <summary>
		/// Data element assigned to this list element
		/// </summary>
		public object assignedObject;

		public void OnSelect(BaseEventData eventData)
		{
			onSelect?.Invoke(this);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			onDeselect?.Invoke(this);
		}
	}
}