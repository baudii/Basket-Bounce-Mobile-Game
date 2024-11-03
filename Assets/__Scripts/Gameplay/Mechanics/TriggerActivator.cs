using UnityEngine;

namespace BasketBounce.Gameplay.Mechanics
{
	public class TriggerActivator : MonoBehaviour
	{
		[SerializeField] float force;
		[SerializeField] LayerMask movableLayers;
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Rigidbody2D rb))
			{
				rb.AddForce(transform.up * force, ForceMode2D.Impulse);
			}
		}
	}
}