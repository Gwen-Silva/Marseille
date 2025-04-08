using UnityEngine;

public abstract class MonoState<T> : MonoBehaviour where T : MonoState<T>
{
    private static T sharedInstance;

    protected virtual void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this as T;
            OnFirstAwake();
        }
    }

    protected virtual void OnDestroy()
    {
        if (sharedInstance == this)
        {
            sharedInstance = null;
        }
    }

    protected virtual void OnFirstAwake() { }

    public static T Shared => sharedInstance;

    public static bool HasInstance => sharedInstance != null;
}
