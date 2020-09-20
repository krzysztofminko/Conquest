using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace SelectableList
{
	public class ListParent : MonoBehaviour
	{
		public event Action<int> onSelectedChange;
		public event Action<GameObject, int> onCreateElement;

		[SerializeField, Required]
		private ListElement elementPrefab;

		[ShowInInspector, ReadOnly]
		private int _selected = -1;
		public int Selected 
		{ 
			get => _selected;
			private set
			{
				_selected = value;
				onSelectedChange?.Invoke(_selected);
			}
		}
				
		private List<ListElement> elements = new List<ListElement>();

		public void Draw(int count)
		{
			Clear();
			for (int i = 0; i < count; i++)
				InsertElement(i);
		}

		public void Clear()
		{
			for (int i = elements.Count - 1; i >= 0; i--)
				RemoveElement(i);
		}

		public ListElement InsertElement(int index)
		{
			ListElement element = Instantiate(elementPrefab, transform);
			element.gameObject.SetActive(true);
			element.transform.SetSiblingIndex(index);
			element.onSelect += Element_onSelect;
			elements.Insert(index, element);
			onCreateElement?.Invoke(element.gameObject, index);
			return element;
		}

		public void RemoveElement(int index)
		{
			elements[index].onSelect -= Element_onSelect;
			Destroy(elements[index].gameObject);
			elements.RemoveAt(index);

			if (Selected == index)
				Select(index);
		}
		
		public void Select(int index)
		{
			if (elements.Count > 0)
			{
				EventSystem.current.SetSelectedGameObject(elements[Mathf.Clamp(index, 0, elements.Count - 1)].gameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				Selected = -1;
			}
		}

		private void Element_onSelect(ListElement element)
		{
			Selected = elements.IndexOf(element);
		}

	}
}