using UnityEngine;
using TMPro;
using System.Collections;

public class HealthDisplay : MonoBehaviour
{
	public int maxHealth = 20;
	public int currentHealth = 20;
	public bool isPlayerHealth = true;

	[SerializeField] private TMP_Text healthText;

	private void Start()
	{
		UpdateHealthDisplay();
	}

	public IEnumerator ReduceHealth(int amount)
	{
		currentHealth = Mathf.Max(currentHealth - amount, 0);
		UpdateHealthDisplay();

		yield return new WaitForSeconds(0.2f);
	}

	public IEnumerator AddHealth(int amount)
	{
		currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
		UpdateHealthDisplay();

		yield return new WaitForSeconds(0.2f);
	}

	private void UpdateHealthDisplay()
	{
		if (healthText != null)
			healthText.text = currentHealth.ToString();
	}
}
