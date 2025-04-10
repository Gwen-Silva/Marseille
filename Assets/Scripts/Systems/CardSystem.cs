using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardSystem : MonoBehaviour
{
	#region Constants

	// ───── Draw Logic ─────────────────────────────
	private const float DRAW_CARD_DELAY = 0.1f;
	private const float RESPAWN_DELAY = 0.1f;

	// ───── Card Visuals ───────────────────────────
	private const float SCALE_TWEEN_DURATION = 0.2f;
	private const float CARD_SCALE = 0.85f;
	private static readonly Vector3 CARD_SCALE_VECTOR = new(CARD_SCALE, CARD_SCALE, CARD_SCALE);
	private const float FLIP_ANGLE = 180f;

	// ───── Combat Animation ───────────────────────
	private const float ATTACK_DURATION = 0.1f;
	private const float RETURN_DURATION = 0.05f;
	private const float TIE_RETURN_DURATION = 0.2f;

	private const float SHAKE_DURATION = 0.5f;
	private const float SHAKE_STRENGTH = 50f;
	private const int SHAKE_VIBRATO = 10;
	private const float SHAKE_RANDOMNESS = 0f;

	private const float CONTACT_OFFSET = 1.4f;
	private const float TIE_CONTACT_OFFSET = 0.5f;

	#endregion

	#region Serialized Fields

	[SerializeField] private Transform discardPoint;
	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;

	[Header("Dependencies")]
	[SerializeField] private ActionSystem actionSystem;
	[SerializeField] private DeckSystem deckSystem;

	#endregion

	#region Properties

	public HorizontalCardHolder PlayerCardHolder => playerHand;
	public HorizontalCardHolder OpponentCardHolder => opponentHand;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
		ActionSystem.AttachPerformer<SelectCardGA>(SelectCardPerformer);
		ActionSystem.AttachPerformer<DeselectCardGA>(DeselectCardPerformer);
		ActionSystem.AttachPerformer<SwapCardGA>(SwapCardPerformer);
		ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
		ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
		ActionSystem.AttachPerformer<FlipCardGA>(FlipCardPerformer);
		ActionSystem.AttachPerformer<DrawInitialCardsGA>(DrawInitialCardsPerformer);
		ActionSystem.AttachPerformer<ClearBoardGA>(ClearBoardPerformer);
		ActionSystem.AttachPerformer<CardCombatAnimationGA>(CardCombatAnimationPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DrawCardGA>();
		ActionSystem.DetachPerformer<SelectCardGA>();
		ActionSystem.DetachPerformer<DeselectCardGA>();
		ActionSystem.DetachPerformer<SwapCardGA>();
		ActionSystem.DetachPerformer<DestroyCardGA>();
		ActionSystem.DetachPerformer<PlayCardGA>();
		ActionSystem.DetachPerformer<FlipCardGA>();
		ActionSystem.DetachPerformer<DrawInitialCardsGA>();
		ActionSystem.DetachPerformer<ClearBoardGA>();
		ActionSystem.DetachPerformer<CardCombatAnimationGA>();
	}

	#endregion

	#region Performers

	/// <summary>Draws cards from the deck into the given card holder.</summary>
	private IEnumerator DrawCardPerformer(DrawCardGA ga)
	{
		List<Card> spawnedCards = new();

		for (int i = 0; i < ga.amount; i++)
		{
			CardData data = null;

			if (ga.forcedValue.HasValue)
			{
				var sourceDeck = ga.targetHolder.IsPlayerCardHolder
					? deckSystem.playerDeck
					: deckSystem.opponentDeck;

				var candidates = sourceDeck.FindAll(c => c.cardValue == ga.forcedValue.Value);
				if (candidates.Count > 0)
				{
					data = candidates[Random.Range(0, candidates.Count)];
					sourceDeck.Remove(data);
				}
				else
				{
					Debug.LogWarning($"[DrawCardGA] Nenhuma carta de valor {ga.forcedValue.Value} encontrada.");
					continue;
				}
			}
			else
			{
				data = ga.targetHolder.IsPlayerCardHolder
					? deckSystem.DrawFromPlayerDeck()
					: deckSystem.DrawFromOpponentDeck();

				if (data == null)
				{
					Debug.Log("Deck vazio! Fim de jogo.");
					bool playerWon = !ga.targetHolder.IsPlayerCardHolder;
					actionSystem.AddReaction(new EndGameGA(playerWon));
					yield break;
				}
			}
			if (ga.targetHolder == null)
				yield break;

			GameObject slotGO = Instantiate(ga.targetHolder.SlotPrefab, ga.targetHolder.transform);
			Card card = slotGO.GetComponentInChildren<Card>();

			card.isPlayerCard = ga.targetHolder.IsPlayerCardHolder;
			card.cardData = data;

			ga.targetHolder.cards.Add(card);
			spawnedCards.Add(card);

			card.name = ga.targetHolder.cards.Count.ToString();

			card.PointerEnterEvent.AddListener(ga.targetHolder.CardPointerEnter);
			card.PointerExitEvent.AddListener(ga.targetHolder.CardPointerExit);
			card.BeginDragEvent.AddListener(ga.targetHolder.BeginDrag);
			card.EndDragEvent.AddListener(ga.targetHolder.EndDrag);

			yield return new WaitForSeconds(DRAW_CARD_DELAY);
		}

		yield return new WaitForSecondsRealtime(RESPAWN_DELAY);

		for (int i = 0; i < ga.targetHolder.cards.Count; i++)
		{
			if (ga.targetHolder.cards[i].cardVisual != null)
				ga.targetHolder.cards[i].cardVisual.UpdateIndex(ga.targetHolder.transform.childCount);
		}

		ga.spawnedCards = spawnedCards;
	}

	/// <summary>Triggers the drawing of both players' initial cards and starts the game.</summary>
	private IEnumerator DrawInitialCardsPerformer(DrawInitialCardsGA ga)
	{
		actionSystem.AddReaction(new DrawCardGA(ga.playerHand, ga.amount));
		actionSystem.AddReaction(new DrawCardGA(ga.opponentHand, ga.amount));
		actionSystem.AddReaction(new StartGameGA());
		yield return null;
	}

	/// <summary>Plays a card from the hand to the board.</summary>
	private IEnumerator PlayCardPerformer(PlayCardGA action)
	{
		CardDisplay display = action.Card;
		Card card = display.OwnerCard;

		card.DisableInteraction();

		display.transform.position = action.TargetSlot.transform.position;
		display.transform.rotation = action.TargetSlot.transform.rotation;

		if (action.IsValueSlot)
			display.ChangeToValueSprite();

		HorizontalCardHolder holder = card.GetComponentInParent<HorizontalCardHolder>();
		holder?.cards.Remove(card);

		Destroy(card.transform.parent.gameObject);

		card.transform.SetParent(action.TargetSlot.transform);
		card.transform.localPosition = Vector3.zero;

		Tween scaleTween = display.transform.DOScale(CARD_SCALE_VECTOR, SCALE_TWEEN_DURATION).SetEase(Ease.OutQuad);
		yield return scaleTween.WaitForCompletion();
	}

	/// <summary>Selects or deselects a card.</summary>
	private IEnumerator SelectCardPerformer(SelectCardGA ga)
	{
		ga.card.Selected = !ga.card.Selected;
		ga.card.SelectEvent.Invoke(ga.card, ga.card.Selected);

		ga.card.transform.localPosition = ga.card.Selected
			? ga.card.cardVisual.transform.up * ga.card.SelectionOffset
			: Vector3.zero;

		yield return null;
	}

	/// <summary>Forces a card to be deselected if it's selected.</summary>
	private IEnumerator DeselectCardPerformer(DeselectCardGA ga)
	{
		if (ga.card.Selected)
		{
			ga.card.Selected = false;
			ga.card.SelectEvent.Invoke(ga.card, false);
		}

		if (ga.card.cardVisual != null)
			ga.card.transform.localPosition = Vector3.zero;

		yield return null;
	}

	/// <summary>Swaps the positions of two cards visually and logically.</summary>
	private IEnumerator SwapCardPerformer(SwapCardGA ga)
	{
		Transform src = ga.sourceCard.transform.parent;
		Transform tgt = ga.targetCard.transform.parent;

		ga.targetCard.transform.SetParent(src);
		ga.targetCard.transform.localPosition = ga.targetCard.Selected
			? new Vector3(0, ga.targetCard.SelectionOffset, 0)
			: Vector3.zero;

		ga.sourceCard.transform.SetParent(tgt);

		if (ga.targetCard.cardVisual != null)
		{
			bool swapRight = ga.targetCard.ParentIndex() > ga.sourceCard.ParentIndex();
			ga.targetCard.cardVisual.Swap(swapRight ? -1 : 1);
		}

		foreach (Card card in ga.parent.GetComponentsInChildren<Card>())
		{
			card.cardVisual?.UpdateIndex(ga.parent.childCount);
		}

		yield return null;
	}

	/// <summary>Destroys a card and its visual representation.</summary>
	private IEnumerator DestroyCardPerformer(DestroyCardGA ga)
	{
		Card card = ga.card;
		if (card == null) yield break;

		HorizontalCardHolder holder = card.GetComponentInParent<HorizontalCardHolder>();
		holder?.cards.Remove(card);

		if (card.cardVisual != null)
		{
			DOTween.Kill(card.cardVisual.transform);
			Destroy(card.cardVisual.gameObject);
		}

		DOTween.Kill(card.transform.parent);
		Destroy(card.transform.parent.gameObject);
		yield return null;
	}

	/// <summary>Flips a card face up or down with animation.</summary>
	private IEnumerator FlipCardPerformer(FlipCardGA ga)
	{
		CardVisual visual = ga.card.cardVisual;
		if (visual == null) yield break;

		visual.isFlipped = !visual.isFlipped;
		float angle = visual.isFlipped ? FLIP_ANGLE : 0f;

		visual.FlipParent.transform.DOLocalRotate(new Vector3(0f, angle, 0f), ga.duration);
		yield return new WaitForSeconds(ga.duration);
	}

	/// <summary>Clears all board slots and discards the cards with animation.</summary>
	private IEnumerator ClearBoardPerformer(ClearBoardGA ga)
	{
		var slots = GameObject.FindObjectsByType<CardDropZone>(FindObjectsSortMode.None)
			.Where(slot => slot.CompareTag("BoardSlot"))
			.ToList();

		foreach (var slot in slots)
		{
			if (slot.transform.childCount <= 0) continue;

			Card card = slot.GetComponentInChildren<Card>();
			if (card == null) continue;

			card.DisableInteraction();

			Sequence seq = DOTween.Sequence();
			seq.Append(card.transform.DOMove(discardPoint.position, 0.2f).SetEase(Ease.InOutQuad));
			seq.OnComplete(() =>
			{
				if (card.cardVisual != null)
					Destroy(card.cardVisual.gameObject);

				Destroy(card.gameObject);
			});

			yield return seq.WaitForCompletion();

			slot.GetComponent<CardDropZone>()?.ResetSlot();
		}

		yield return null;
	}

	/// <summary>Animates card combat based on the combat result.</summary>
	private IEnumerator CardCombatAnimationPerformer(CardCombatAnimationGA ga)
	{
		Transform atk = ga.attacker.cardVisual.transform;
		Transform def = ga.defender.cardVisual.transform;

		Vector3 atkStart = atk.position;
		Vector3 defStart = def.position;
		Vector3 dir = (defStart - atkStart).normalized;
		Vector3 contact = defStart - dir * CONTACT_OFFSET;
		Vector3 mid = (atkStart + defStart) / 2f;

		switch (ga.result)
		{
			case CombatResult.AttackSuccess:
				yield return atk.DOMove(contact, ATTACK_DURATION).SetEase(Ease.OutBack).WaitForCompletion();
				yield return def.DOShakePosition(SHAKE_DURATION, new Vector3(SHAKE_STRENGTH, 0f, 0f), SHAKE_VIBRATO, SHAKE_RANDOMNESS)
					.SetEase(Ease.OutExpo).WaitForCompletion();
				yield return atk.DOMove(atkStart, RETURN_DURATION).WaitForCompletion();
				break;

			case CombatResult.AttackBlocked:
				yield return atk.DOMove(contact, ATTACK_DURATION).SetEase(Ease.InQuad).WaitForCompletion();
				yield return atk.DOShakePosition(SHAKE_DURATION, new Vector3(SHAKE_STRENGTH, 0f, 0f), SHAKE_VIBRATO, SHAKE_RANDOMNESS)
					.SetEase(Ease.OutExpo).WaitForCompletion();
				yield return atk.DOMove(atkStart, RETURN_DURATION).WaitForCompletion();
				break;

			case CombatResult.Tie:
				Vector3 atkDir = (mid - atkStart).normalized;
				Vector3 defDir = (mid - defStart).normalized;

				Vector3 atkContact = atkStart + atkDir * TIE_CONTACT_OFFSET;
				Vector3 defContact = defStart + defDir * TIE_CONTACT_OFFSET;

				yield return DOTween.Sequence()
					.Join(atk.DOMove(atkContact, ATTACK_DURATION).SetEase(Ease.OutQuad))
					.Join(def.DOMove(defContact, ATTACK_DURATION).SetEase(Ease.OutQuad))
					.WaitForCompletion();

				yield return DOTween.Sequence()
					.Join(atk.DOShakePosition(SHAKE_DURATION, new Vector3(SHAKE_STRENGTH, 0f, 0f), SHAKE_VIBRATO, SHAKE_RANDOMNESS).SetEase(Ease.OutExpo))
					.Join(def.DOShakePosition(SHAKE_DURATION, new Vector3(SHAKE_STRENGTH, 0f, 0f), SHAKE_VIBRATO, SHAKE_RANDOMNESS).SetEase(Ease.OutExpo))
					.WaitForCompletion();

				yield return DOTween.Sequence()
					.Join(atk.DOMove(atkStart, TIE_RETURN_DURATION))
					.Join(def.DOMove(defStart, TIE_RETURN_DURATION))
					.WaitForCompletion();
				break;
		}

		yield return null;
	}

	#endregion
}
