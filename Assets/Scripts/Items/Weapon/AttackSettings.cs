using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using Tags;
using Sirenix.OdinInspector;
using UnityEngine.SocialPlatforms;
using System.Runtime.Remoting.Messaging;

[Serializable, InlineProperty]
public class AttackSettings
{
	public string name;
	public AnimationClip animation;
	public Projectile projectile;
	//[Min(0)]
	//public float duration = 1;
	[PropertyRange(0, "GetAnimationLength")]
	public float damageDelay = 0;
	public bool targetOnly;
	public float range = 1;
	[ListDrawerSettings(Expanded = true)]
	public List<Damage> damage = new List<Damage>();

	private float GetAnimationLength() => animation ? animation.length : 0;
}