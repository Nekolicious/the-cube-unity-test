using UnityEngine;
using UnityEngine.Events;

public class EventShooter : MonoBehaviour
{
    [Header("Collision Event")]
    public UnityEvent onPlayerCollide;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerCollide.Invoke();
        }
    }
}
