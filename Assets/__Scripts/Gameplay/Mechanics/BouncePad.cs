using UnityEngine;
using KK.Common.Gameplay;

namespace BasketBounce.Gameplay.Mechanics
{
	public class BouncePad : Activator
	{
		[SerializeField] float force;
		[SerializeField] Animator animator;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Ball ball))
			{
				bool bounced = ball.BounceFromBouncePad(transform.up, transform.position);
				if (bounced)
					animator.SetTrigger("Bounce");

				ActivateAll(true);
			}
		}
	}
}