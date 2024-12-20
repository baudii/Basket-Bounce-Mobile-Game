using UnityEngine;

namespace BasketBounce.Gameplay.Mechanics
{
	public class Killable : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Ball ball))
			{
				ball.Die(true);
			}
		}
	}
}