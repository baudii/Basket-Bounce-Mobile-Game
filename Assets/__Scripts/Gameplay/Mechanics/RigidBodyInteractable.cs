using BasketBounce.Systems;
using KK.Common;
using UnityEngine;

namespace BasketBounce.Gameplay
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class RigidBodyInteractable : MonoBehaviour, IResetableItem
    {
		Rigidbody2D rb;

		Vector3 initialPosition;
		bool isFalling;
		bool hasFallen;
		float startTime;

		bool isInitialized;
		private void Awake()
		{
			initialPosition = transform.position;
			rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			if (Time.time - startTime < 0.5f)
				return;

            if (hasFallen)
				return;

            if (isFalling && rb.velocity.magnitude <= 0.1f)
			{
				isFalling = false;
				rb.isKinematic = true;
				rb.velocity = Vector3.zero;
				hasFallen = true;
				this.Log("Has fallen");
			}
			else if (!isFalling && rb.velocity.magnitude > 0.1f)
			{
				this.Log("Started falling");
				isFalling = true;
			}
		}

		public void ResetState()
		{
			transform.position = initialPosition;
			rb.isKinematic = false;
			isFalling = false;
			hasFallen = false;
			startTime = Time.time;
		}
    }
}
