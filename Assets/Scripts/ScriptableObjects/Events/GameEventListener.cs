using UnityEngine;

public abstract class GameEventListener<T> : MonoBehaviour
{
    public GameEventSO<T> gameEvent;
    public UnityEngine.Events.UnityEvent<T> response;

    private void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void OnEventTriggered(T value)
    {
        response.Invoke(value);
    }
}
