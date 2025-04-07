using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Handles damage, healing, and visual feedback for health-related interactions.
/// </summary>
public class HealthSystem : MonoBehaviour
{
	#region Constants

	private const float DAMAGE_MOVE_DURATION = 0.3f;
	private const float DAMAGE_SHAKE_DURATION = 0.5f;
	private const float DAMAGE_SHAKE_STRENGTH = 0.5f;
	private const int DAMAGE_SHAKE_VIBRATO = 10;

	private const float HEAL_SCALE_MULTIPLIER = 1.2f;
	private const float HEAL_SCALE_DURATION = 0.3f;
	private const float HEAL_EFFECT_LIFETIME = 2f;

	#endregion

	#region Serialized Fields

	[SerializeField] private GameObject floatTextPrefab;
	[SerializeField] private GameObject healEffectPrefab;
	[SerializeField] private Transform playerEffectPoint;
	[SerializeField] private Transform opponentEffectPoint;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
		ActionSystem.AttachPerformer<HealHealthGA>(HealHealthPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DealDamageGA>();
		ActionSystem.DetachPerformer<HealHealthGA>();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Displays a floating "Protected" text when a shield absorbs damage.
	/// </summary>
	public void ShowShieldText(HealthDisplay target)
	{
		Transform effectPoint = target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;

		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject textObj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = textObj.GetComponent<FloatText>();

			if (floatText != null)
				floatText.InitializeCustom("Protegido", new Color32(0xA7, 0x92, 0xB5, 255));
		}
	}

	#endregion

	#region Performers

	/// <summary>
	/// Performs the animation and effect of a damage action.
	/// </summary>
	private IEnumerator DealDamagePerformer(DealDamageGA ga)
	{
		if (ga.AttackerCard == null || ga.Target == null)
			yield break;

		Transform attackerTransform = ga.AttackerCard.transform;
		Vector3 originalPosition = attackerTransform.position;

		yield return attackerTransform
			.DOMove(ga.Target.transform.position, DAMAGE_MOVE_DURATION)
			.SetEase(Ease.OutQuad)
			.WaitForCompletion();

		ga.Target.transform.DOShakePosition(DAMAGE_SHAKE_DURATION, DAMAGE_SHAKE_STRENGTH, DAMAGE_SHAKE_VIBRATO);

		yield return attackerTransform
			.DOMove(originalPosition, DAMAGE_MOVE_DURATION)
			.SetEase(Ease.OutQuad)
			.WaitForCompletion();

		Transform effectPoint = ga.Target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;

		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject textObj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = textObj.GetComponent<FloatText>();
			floatText.Initialize(ga.Amount, false);
		}

		yield return ga.Target.ReduceHealth(ga.Amount);

		if (ga.Target.CurrentHealth <= 0)
		{
			bool playerWon = !ga.Target.isPlayerHealth;
			ActionSystem.Instance.AddReaction(new EndGameGA(playerWon));
		}
	}

	/// <summary>
	/// Performs the animation and effect of a healing action.
	/// </summary>
	private IEnumerator HealHealthPerformer(HealHealthGA ga)
	{
		if (ga.Target == null)
			yield break;

		Transform targetTransform = ga.Target.transform;
		Vector3 originalScale = targetTransform.localScale;

		yield return targetTransform
			.DOScale(originalScale * HEAL_SCALE_MULTIPLIER, HEAL_SCALE_DURATION)
			.SetEase(Ease.OutBack)
			.WaitForCompletion();

		targetTransform
			.DOScale(originalScale, HEAL_SCALE_DURATION)
			.SetEase(Ease.InOutQuad);

		Transform effectPoint = ga.Target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;

		if (healEffectPrefab != null && effectPoint != null)
		{
			GameObject vfx = Instantiate(healEffectPrefab, effectPoint.position, Quaternion.identity);
			Destroy(vfx, HEAL_EFFECT_LIFETIME);
		}

		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject textObj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = textObj.GetComponent<FloatText>();
			floatText.Initialize(ga.Amount, true);
		}

		yield return ga.Target.AddHealth(ga.Amount);
	}

	#endregion
}
