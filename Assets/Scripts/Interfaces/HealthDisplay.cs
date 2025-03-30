using UnityEngine;
using TMPro;
using System.Collections;

public class HealthDisplay : MonoBehaviour
{
	public int maxHealth = 20;
	public int currentHealth = 20;
	public bool isPlayerHealth = true;
	public bool HasGriefShield { get; set; } = false;
	public GameObject ActiveShieldIcon { get; set; }

	[Header("Grief")]
	[SerializeField] private GameObject griefShieldOnEffect;
	[SerializeField] private GameObject griefShieldBreakEffect;
	[SerializeField] private GameObject griefShieldUIIconPrefab;

	[Header("Parameters")]
	[SerializeField] private Transform effectSpawnPoint;
	[SerializeField] private TMP_Text healthText;

	public GameObject GriefShieldOnEffect => griefShieldOnEffect;
	public GameObject GriefShieldBreakEffect => griefShieldBreakEffect;
	public GameObject GriefShieldUIIconPrefab => griefShieldUIIconPrefab;
	public Transform EffectSpawnPoint => effectSpawnPoint;

	private void Start()
	{
		UpdateHealthDisplay();
	}

	public IEnumerator ReduceHealth(int amount)
	{
		if (HasGriefShield && currentHealth - amount <= 0)
		{
			HasGriefShield = false;

			if (griefShieldBreakEffect != null && effectSpawnPoint != null)
			{
				GameObject fx = Instantiate(griefShieldBreakEffect, effectSpawnPoint.position, Quaternion.identity, transform);
				Destroy(fx, 2f);
			}

			if (ActiveShieldIcon != null)
			{
				Destroy(ActiveShieldIcon);
				ActiveShieldIcon = null;
			}

			yield return new WaitForSeconds(0.3f);
			yield break;
		}

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
