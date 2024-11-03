using UnityEngine;
using BasketBounce.Systems;

namespace BasketBounce.Gameplay.Visuals
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class GFX_Shadow : MonoBehaviour, ILevelValidator
	{
		[SerializeField] float xOffset = 0.05f;
		[SerializeField] float yOffset = -0.1f;
		[SerializeField] SpriteRenderer mainSr;
		SpriteRenderer sr;

#if UNITY_EDITOR

		[ContextMenu("Adjust")]
		internal void Adjust()
		{
			if (sr == null)
				sr = GetComponent<SpriteRenderer>();

			if (mainSr == null)
				return;

			sr.tileMode = mainSr.tileMode;
			sr.sprite = mainSr.sprite;
			sr.size = new Vector2(mainSr.size.x, mainSr.size.y);

			transform.position = mainSr.transform.position + new Vector3(xOffset, yOffset, 0);
		}
#endif

		public void AdjustPosition()
		{
			if (mainSr == null)
				return;

			transform.position = mainSr.transform.position + new Vector3(xOffset, yOffset, 0);
		}

		public void Validate()
		{
#if UNITY_EDITOR
			Adjust();
#endif
		}
	}
}