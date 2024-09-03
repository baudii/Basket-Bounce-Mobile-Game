using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnter2DCustomEvent : MonoBehaviour
{
    [SerializeField] UnityEvent _event;

    void OnTriggerEnter2D(Collider2D collision)
    {
        _event?.Invoke();
    }
}
