using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ball ball))
        {
            animator.SetTrigger("Bounce");

            ball.BounceFromBouncePad(transform.up, force);
        }
    }
}
