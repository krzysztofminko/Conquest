using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Damageable;
using StatsWithModifiers;

public class Health : Stat, IDamageable
{
	[SerializeField, DisableInPlayMode]
	private Slider healthBar;
	private Canvas healthBarCanvas;

	private void Awake()
	{
		healthBarCanvas = healthBar.transform.parent.GetComponent<Canvas>();
	}

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
		{
			healthBarCanvas.gameObject.SetActive(Value < Max);
			healthBar.value = Value;
		}
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