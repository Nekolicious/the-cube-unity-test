using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventSO<T> : ScriptableObject
{
    private List<GameEventListener<T>> listeners = new();

    public void TriggerEvent(T value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered(value);
        }
    }

    public void AddListener(GameEventListener<T> listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void RemoveListener(GameEventListener<T> listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
