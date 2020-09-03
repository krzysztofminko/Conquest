using UnityEngine;
using System.Collections;
using Items;
using Sirenix.OdinInspector;

public class ItemEntity : MonoBehaviour
{
	[InlineEditor(Expanded = true)]
	public Item item;
}
