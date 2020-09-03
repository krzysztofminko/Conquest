using UnityEngine;
using System.Collections;
using StatsWithModifiers;
using UnityEngine.UI;

public class Health : Stat, IDamageable
{
	[SerializeField]
	private Slider healthBar;

	private void OnEnable()
	{
		onValueChange += Health_onValueChange;
		onMaxChange += Health_onMaxChange;
	}

	private void OnDisable()
	{
		onValueChange -= Health_onValueChange;
		onMaxChange -= Health_onMaxChange;
	}

	private void Health_onValueChange(Stat stat)
	{
		if (healthBar)
			healthBar.value = Value;
		if (Value <= 0)
			Destroy(gameObject);
	}

	private void Health_onMaxChange(Stat stat)
	{
		if (healthBar)
			healthBar.maxValue = Max;
	}

	public void ReceiveDamage(float damage, Damage.Type type)
	{
		Value -= damage;
	}
}
