using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace SelectableList
{
	public class ListParent : MonoBehaviour
	{
		/// <summary>
		/// GameObject listElement, object assignedObject
		/// </summary>
		public event Action<GameObject, object> onSelectedUpdate;
		/// <summary>
		/// GameObject listElement, object assignedObject
		/// </summary>
		public event Action<GameObject, object> onCreateElement;

		[SerializeField, Required]
		private ListElement elementPrefab;

		[ShowInInspector, ReadOnly]
		private ListElement _selected;
		/// <summary>
		/// Invokes onSelectedUpdate
		/// </summary>
		public ListElement Selected 
		{ 
			get => _selected;
			private set
			{
				_selected = value;
				onSelectedUpdate?.Invoke(_selected?.gameObject, _selected?.assignedObject);
			}
		}
				
		private List<ListElement> elements = new List<ListElement>();

		/// <summary>
		/// Creates all list elements and assigns data
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="data">List of objects assigned to list elements</param>
		public void Draw<T>(List<T> data)
		{
			Clear();
			for (int i = 0; i < data.Count; i++)
				AddElement(data[i]);
		}

		/// <summary>
		/// Destroys all ListElements
		/// </summary>
		public void Clear()
		{
			for (int i = elements.Count - 1; i >= 0; i--)
				RemoveElementAt(i);
		}

		/// <summary>
		/// Adds list element and assign data element
		/// </summary>
		/// <returns>Returns instantiated ListElement</returns>
		public ListElement AddElement(object assignedObject)
		{
			ListElement element = Instantiate(elementPrefab, transform);
			element.assignedObject = assignedObject;
			element.gameObject.SetActive(true);
			element.onSelect += Element_onSelect;
			elements.Add(element);
			onCreateElement?.Invoke(element.gameObject, assignedObject);
			return element;
		}

		/// <summary>
		/// Removes list element with specified assignedObject;
		/// </summary>
		public void RemoveElement(object assignedObject) => RemoveElementAt(elements.FindIndex(e => e.assignedObject == assignedObject));

		private void RemoveElementAt(int index)
		{
			ListElement element = elements[index];
			element.onSelect -= Element_onSelect;
			Destroy(element.gameObject);
			elements.RemoveAt(index);

			if (Selected == element)
				SelectIndex(index);
		}

		/// <summary>
		/// Invokes EventSystem.current.SetSelectedGameObject
		/// </summary>
		public void SelectIndex(int index)
		{
			if (elements.Count > 0)
			{
				EventSystem.current.SetSelectedGameObject(elements[Mathf.Clamp(index, 0, elements.Count - 1)].gameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				Selected = null;
			}
		}

		private void Element_onSelect(ListElement element) => Selected = element;

	}
}