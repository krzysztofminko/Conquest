using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace SelectableList
{
	public class ListParent : MonoBehaviour
	{
		[SerializeField, Required]
		private ListElement elementPrefab;

		[SerializeField, ReadOnly]
		private int _selected;
		public int Selected { get => _selected; private set => _selected = value; }

		public event Action<ListElement, int> onSelect;
		public event Action<ListElement, int> onElementSetup;

		[SerializeField, HideInInspector]
		private List<ListElement> elements = new List<ListElement>();

		public void UpdateList(int listCount)
		{
			ListElement element = null;
			for (int i = 0; i < Mathf.Max(listCount, elements.Count); i++)
			{
				int id;

				//Deactivate button
				if (i >= listCount)
					elements[i].gameObject.SetActive(false);

				if (listCount > 0)
				{
					//Instantiate new button or get from list
					if (i >= elements.Count)
					{
						element = Instantiate(elementPrefab, transform);
						id = i;
						element.onSelect += () => { Selected = id; onSelect?.Invoke(element, id); };
						elements.Add(element);
					}
					else
					{
						element = elements[i];
					}

					//Setup and activate button
					element.gameObject.SetActive(true);
					id = i;
					onElementSetup?.Invoke(element, id);
				}
			}
		}
	}
}