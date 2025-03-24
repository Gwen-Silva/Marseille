using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class ActionSystem : Singleton<ActionSystem>
{
	private List<GameAction> reactions = new();
	public bool IsPerforming { get; private set; } = false;
	private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
	private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
	private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

	public void Perform(GameAction action, System.Action OnPerformFinished = null)
	{
		if (IsPerforming) return;
		IsPerforming = true;
		StartCoroutine(Flow(action, () =>
		{
			IsPerforming = false;
			OnPerformFinished?.Invoke();
		}));
	}

	public void AddReaction(GameAction action)
	{
		reactions ??= new List<GameAction>();
		reactions.Add(action);
	}

	private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
	{
		reactions = action.PreReactions ?? new List<GameAction>();
		PerformSubscribers(action, preSubs);
		yield return null;

		reactions = action.PerformReactions ?? new List<GameAction>();
		yield return PerformPerformer(action);
		yield return PerformReactions();

		reactions = action.PostReactions ?? new List<GameAction>();
		PerformSubscribers(action, postSubs);
		yield return PerformReactions();

		OnFlowFinished?.Invoke();
	}

	private IEnumerator PerformPerformer(GameAction action)
	{
		Type type = action.GetType();
		if (performers.TryGetValue(type, out var performer))
		{
			yield return performer(action);
		}
	}

	private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
	{
		Type type = action.GetType();
		if (subs.TryGetValue(type, out var subscriberList))
		{
			foreach (var sub in subscriberList)
			{
				sub(action);
			}
		}
	}

	private IEnumerator PerformReactions()
	{
		foreach (var reaction in reactions)
		{
			yield return Flow(reaction);
		}
	}

	public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
	{
		Type type = typeof(T);
		IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
		performers[type] = wrappedPerformer;
	}

	public static void DetachPerformer<T>() where T : GameAction
	{
		Type type = typeof(T);
		performers.Remove(type);
	}

	public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
		void wrappedReaction(GameAction action) => reaction((T)action);

		if (!subs.TryGetValue(typeof(T), out var reactionList))
		{
			reactionList = new List<Action<GameAction>>();
			subs[typeof(T)] = reactionList;
		}

		reactionList.Add(wrappedReaction);
	}

	public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
		if (subs.TryGetValue(typeof(T), out var reactionList))
		{
			reactionList.RemoveAll(sub => sub.Equals((Action<GameAction>)(a => reaction((T)a))));
		}
	}
}
