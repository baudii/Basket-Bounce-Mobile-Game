using UnityEngine;
using KK.Common.Gameplay;
using BasketBounce.Systems;
using BasketBounce.Systems.Interfaces;

namespace BasketBounce.Gameplay.Mechanics
{
	public class BouncePad : Activator, ISetuppableItem, IResetableItem
	{
		[SerializeField] float force;
		[SerializeField] Animator animator;
		[SerializeField] Transform arrows;

		public void OnSetup(LevelData levelData)
		{
			levelData.OnBallReleasedEvent.AddListener(() =>
			{
				arrows.gameObject.SetActive(false);
			});
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