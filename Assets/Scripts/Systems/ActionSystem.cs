using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionSystem : Singleton<ActionSystem>
{
	#region Fields

	private List<GameAction> reactions = new();
	public bool IsPerforming { get; private set; } = false;

	private static readonly Dictionary<Type, List<Action<GameAction>>> preSubs = new();
	private static readonly Dictionary<Type, List<Action<GameAction>>> postSubs = new();
	private static readonly Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

	#endregion

	#region Public Methods

	/// <summary>
	/// Performs the given GameAction immediately, executing all pre, performer, and post reactions.
	/// </summary>
	/// <param name="action">The GameAction to perform.</param>
	/// <param name="OnPerformFinished">Optional callback after completion.</param>
	public void Perform(GameAction action, Action OnPerformFinished = null)
	{
		if (IsPerforming) return;

		IsPerforming = true;
		StartCoroutine(Flow(action, () =>
		{
			IsPerforming = false;
			OnPerformFinished?.Invoke();
		}));
	}

	/// <summary>
	/// Adds a GameAction to the reaction queue.
	/// </summary>
	public void AddReaction(GameAction action)
	{
		reactions ??= new List<GameAction>();
		reactions.Add(action);
	}

	/// <summary>
	/// Attaches a coroutine performer for a specific GameAction type.
	/// </summary>
	public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
	{
		Type type = typeof(T);
		IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
		performers[type] = wrappedPerformer;
	}

	/// <summary>
	/// Detaches the performer for the specified GameAction type.
	/// </summary>
	public static void DetachPerformer<T>() where T : GameAction
	{
		performers.Remove(typeof(T));
	}

	/// <summary>
	/// Subscribes a reaction to be executed at the specified timing for a GameAction.
	/// </summary>
	public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

		void wrapped(GameAction action) => reaction((T)action);

		if (!subs.TryGetValue(typeof(T), out var list))
		{
			list = new List<Action<GameAction>>();
			subs[typeof(T)] = list;
		}

		list.Add(wrapped);
	}

	/// <summary>
	/// Unsubscribes a previously registered reaction.
	/// </summary>
	public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
		if (!subs.TryGetValue(typeof(T), out var list)) return;

		list.RemoveAll(sub =>
		{
			// Create a wrapper and compare method info for precise match
			var method1 = ((Action<GameAction>)((a) => reaction((T)a))).Method;
			return sub.Method.Equals(method1);
		});
	}

	#endregion

	#region Internal Flow Logic

	private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
	{
		reactions = action.PreReactions ?? new List<GameAction>();
		PerformSubscribers(action, preSubs);
		yield return PerformReactions();

		reactions = action.PerformReactions ?? new List<GameAction>();
		yield return PerformPerformer(action);
		yield return PerformReactions();

		reactions = action.PostReactions ?? new List<GameAction>();
		PerformSubscribers(action, postSubs);
		yield return PerformReactions();

		OnFlowFinished?.Invoke();
	}

	private IEnumerator PerformReactions()
	{
		foreach (var reaction in reactions)
			yield return Flow(reaction);
	}

	private IEnumerator PerformPerformer(GameAction action)
	{
		if (performers.TryGetValue(action.GetType(), out var performer))
			yield return performer(action);
	}

	private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
	{
		if (subs.TryGetValue(action.GetType(), out var list))
		{
			foreach (var sub in list)
				sub(action);
		}
	}

	#endregion
}
