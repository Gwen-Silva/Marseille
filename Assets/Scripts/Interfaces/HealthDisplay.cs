using UnityEngine;
using TMPro;
using System.Collections;

public class HealthDisplay : MonoBehaviour
{
	#region Constants

	private const float SHIELD_BREAK_DELAY = 0.3f;
	private const float HEALTH_UPDATE_DELAY = 0.2f;
	private const float FX_DESTROY_DELAY = 2f;

	#endregion

	#region Public Fields

	public int maxHealth = 20;
	public int currentHealth = 20;
	public bool isPlayerHealth = true;

	public bool HasGriefShield { get; set; } = false;
	public GameObject ActiveShieldIcon { get; set; }

	#endregion

	#region Serialized Fields

	[Header("Grief")]
	[SerializeField] private GameObject griefShieldOnEffect;
	[SerializeField] private GameObject griefShieldBreakEffect;
	[SerializeField] private GameObject griefShieldUIIconPrefab;

	[Header("Parameters")]
	[SerializeField] private Transform effectSpawnPoint;
	[SerializeField] private TMP_Text healthText;

	#endregion

	#region Properties

	public GameObject GriefShieldOnEffect => griefShieldOnEffect;
	public GameObject GriefShieldBreakEffect => griefShieldBreakEffect;
	public GameObject GriefShieldUIIconPrefab => griefShieldUIIconPrefab;
	public Transform EffectSpawnPoint => effectSpawnPoint;
	public int CurrentHealth => currentHealth;
	public int MaxHealth => maxHealth;

	#endregion

	#region Unity Methods

	private void Start()
	{
		UpdateHealthDisplay();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Reduces health by a given amount, considering Grief Shield protection.
	/// </summary>
	public IEnumerator ReduceHealth(int amount)
	{
		if (HasGriefShield && currentHealth - amount <= 0)
		{
			HasGriefShield = false;

			if (griefShieldBreakEffect != null && effectSpawnPoint != null)
			{
				GameObject fx = Instantiate(griefShieldBreakEffect, effectSpawnPoint.position, Quaternion.identity, transform);
				Destroy(fx, FX_DESTROY_DELAY);
			}

			if (ActiveShieldIcon != null)
			{
				Destroy(ActiveShieldIcon);
				ActiveShieldIcon = null;
			}

			yield return new WaitForSeconds(SHIELD_BREAK_DELAY);
			yield break;
		}

		currentHealth = Mathf.Max(currentHealth - amount, 0);
		UpdateHealthDisplay();

		yield return new WaitForSeconds(HEALTH_UPDATE_DELAY);
	}

	/// <summary>
	/// Increases health by a given amount, clamped to maximum health.
	/// </summary>
	public IEnumerator AddHealth(int amount)
	{
		currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
		UpdateHealthDisplay();

		yield return new WaitForSeconds(HEALTH_UPDATE_DELAY);
	}

	#endregion

	#region Private Methods

	private void UpdateHealthDisplay()
	{
		if (healthText != null)
			healthText.text = currentHealth.ToString();
	}

	#endregion
}
