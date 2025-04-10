using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionSystem : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private GameSystem gameSystem;

	#endregion

	#region Fields

	private List<GameAction> reactions = new();
	public bool IsPerforming { get; private set; } = false;

	private static readonly Dictionary<Type, List<Action<GameAction>>> preSubs = new();
	private static readonly Dictionary<Type, List<Action<GameAction>>> postSubs = new();
	private static readonly Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

	#endregion

	#region Public Methods

	public void Perform(GameAction action, Action OnPerformFinished = null)
	{
		Debug.Log($"[ActionSystem] Perform chamado com GA: {action.GetType().Name}\nStackTrace:\n" + new System.Diagnostics.StackTrace());

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

	public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
	{
		Type type = typeof(T);
		IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
		performers[type] = wrappedPerformer;
	}

	public static void DetachPerformer<T>() where T : GameAction
	{
		performers.Remove(typeof(T));
	}

	public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

		void wrapped(GameAction action) => reaction((T)action);

		if (!subs.TryGetValue(typeof(T), out var list))
		{
			list = new List<Action<GameAction>>();
			subs[typeof(T)] = list;
		}

		bool alreadyExists = list.Exists(existing =>
			existing.Method == reaction.Method &&
			existing.Target == reaction.Target);

		if (!alreadyExists)
		{
			list.Add(wrapped);
			Debug.Log($"[ActionSystem] SubscribeReaction: {reaction.Method.DeclaringType.Name}.{reaction.Method.Name} para {typeof(T).Name} ({timing})");
		}
		else
		{
			Debug.LogWarning($"[ActionSystem] Reação já estava registrada para {typeof(T).Name} ({timing}): {reaction.Method.DeclaringType.Name}.{reaction.Method.Name}");
		}
	}

	public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
	{
		var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
		if (!subs.TryGetValue(typeof(T), out var list) || list.Count == 0)
			return;

		int removed = list.RemoveAll(sub =>
			sub.Method == reaction.Method &&
			sub.Target == reaction.Target);

		if (removed > 0)
			Debug.Log($"[ActionSystem] UnsubscribeReaction: {reaction.Method.DeclaringType.Name}.{reaction.Method.Name} removido de {typeof(T).Name} ({timing})");
	}

	#endregion

	#region Internal Flow Logic

	private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
	{
		// Pre
		reactions = action.PreReactions ?? new List<GameAction>();
		PerformSubscribers(action, preSubs);
		yield return PerformReactions();

		// Perform
		reactions = action.PerformReactions ?? new List<GameAction>();
		yield return PerformPerformer(action);
		yield return PerformReactions();

		// Post
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
		else
			Debug.LogWarning($"[ActionSystem] Nenhum performer registrado para: {action.GetType().Name}");
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
