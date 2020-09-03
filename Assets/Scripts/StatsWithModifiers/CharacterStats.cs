using Sirenix.OdinInspector;
using StatsWithModifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : StatsList, IDamageable
{
	public Slider HealthBar;

	private void Reset()
	{
		list = new List<Stat>()
		{
			new Stat(StatId.Health),
			new Stat(StatId.Stamina),
			new Stat(StatId.Food)
		};
	}

	private void OnValidate()
	{
		HealthBar.value = this[StatId.Health].Value;
		HealthBar.maxValue = this[StatId.Health].Max;
	}

	private void Awake()
	{
		HealthBar.value = this[StatId.Health].Value;
		HealthBar.maxValue = this[StatId.Health].Max;
	}

	private void OnEnable()
	{
		this[StatId.Health].onValueChange += Health_onValueChange;
		this[StatId.Health].onMaxChange += Health_onMaxChange;
	}

	private void Health_onMaxChange(Stat stat)
	{
		HealthBar.maxValue = stat.Max;
	}

	private void OnDisable()
	{
		this[StatId.Health].onValueChange -= Health_onValueChange;
		this[StatId.Health].onMaxChange -= Health_onMaxChange;
	}

	private void Health_onValueChange(Stat health)
	{
		HealthBar.value = health.Value;
		if (health.Value <= 0)
			Destroy(gameObject);
	}

	public void ReceiveDamage(float damage, Damage.Type type)
	{
		this[StatId.Health].Value -= damage;
	}
}
