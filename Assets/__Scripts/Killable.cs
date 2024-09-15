using UnityEngine;

public class Killable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ball circle))
        {
            circle.Die(true);
        }
    }
}
