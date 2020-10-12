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
		/// GameObject listElement, object assignedObject. Invoked every time in Selected property setter.
		/// </summary>
		public event Action<GameObject, object> onSelectedUpdate;
		/// <summary>
		/// GameObject listElement, object assignedObject. Invoked in AddElement method.
		/// </summary>
		public event Action<GameObject, object> onCreateElement;

		[SerializeField, Required]
		private ListElement elementPrefab;

		[ShowInInspector, ReadOnly]
		private ListElement _selected;
		/// <summary>
		/// Invokes onSelectedUpdate every time on set
		/// </summary>
		public ListElement Selected 
		{ 
			get => _selected;
			private set
			{
				_selected = value;
				onSelectedUpdate?.Invoke(_selected?.gameObject, _selected?.bindedObject);
			}
		}
				
		private List<ListElement> elements = new List<ListElement>();

		/// <summary>
		/// Creates all list elements and binds data
		/// </summary>
		/// <typeparam name="T">Any data type</typeparam>
		/// <param name="data">List of objects to bind with list elements</param>
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
		/// Adds list element and assign object to it.
		/// </summary>
		/// <returns>Returns instantiated ListElement</returns>
		public ListElement AddElement(object objectToBind)
		{
			ListElement element = Instantiate(elementPrefab, transform);
			element.bindedObject = objectToBind;
			element.gameObject.SetActive(true);
			element.onSelect += ListElement_onSelect;
			element.onDeselect += ListElement_onDeselect;
			elements.Add(element);
			onCreateElement?.Invoke(element.gameObject, objectToBind);
			return element;
		}

		/// <summary>
		/// Removes list element with specified binding;
		/// </summary>
		public void RemoveElement(object objectToBind)
		{
			if (objectToBind == null)
				Debug.LogError($"Parameter {nameof(objectToBind)} can't be null.");
			RemoveElementAt(elements.FindIndex(e => e.bindedObject == objectToBind));
		}

		private void RemoveElementAt(int index)
		{
			if (index < 0 || index >= elements.Count)
				Debug.LogError($"index({index}) out of range ({elements.Count})", this);
			ListElement element = elements[index];
			element.onSelect -= ListElement_onSelect;
			element.onDeselect -= ListElement_onDeselect;
			Destroy(element.gameObject);
			elements.RemoveAt(index);

			if (Selected == element)
				SelectByIndex(index);
		}

		/// <summary>
		/// Get's existing ListElement by it's binding. 
		/// </summary>
		/// <param name="bindedObject"></param>
		/// <returns></returns>
		public ListElement GetElement(object bindedObject) => elements.Find(e => e.bindedObject == bindedObject);

		/// <summary>
		/// Invokes EventSystem.current.SetSelectedGameObject on ListElement found by index
		/// </summary>
		public void SelectByIndex(int index)
		{
			EventSystem.current.SetSelectedGameObject(elements.Count > 0 ? elements[Mathf.Clamp(index, 0, elements.Count - 1)].gameObject : null);
		}

		private void ListElement_onSelect(ListElement element) => Selected = element;
		private void ListElement_onDeselect(ListElement element) => Selected = null;
	}
}