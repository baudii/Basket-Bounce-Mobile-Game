using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            animator.SetTrigger("Bounce");
            rb.AddForce(transform.up * force, ForceMode2D.Impulse);
        }
    }
}
