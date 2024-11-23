using UnityEngine;
using KK.Common.Gameplay;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using UnityEngine.Events;

namespace BasketBounce.Gameplay.Mechanics
{
	public class BouncePad : Activator, IBallReleaseHandler, IResetableItem
	{
		[SerializeField] float force;
		[SerializeField] Animator animator;
		[SerializeField] Transform arrows;

		LevelData levelData;

		private void Start()
		{
			
		}

		public void Handle()
		{
			arrows.gameObject.SetActive(false);
		}

		public void ResetState()
		{
			arrows.gameObject.SetActive(true);
		}

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