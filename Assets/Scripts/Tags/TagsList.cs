using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Tags
{
	[HideMonoScript]
	public class TagsList : MonoBehaviour
	{
		[SerializeField, ListDrawerSettings(Expanded = true), AssetSelector(DrawDropdownForListElements = false)]
		private List<Tag> _tags = new List<Tag>();

		public bool Contains(Tag tag) => _tags.Contains(tag);

		public bool ContainsAny(List<Tag> tags) => tags.FirstOrDefault(t => Contains(t));

		private void OnValidate()
		{
			_tags.RemoveAll(t => t == null);
		}
	}
}