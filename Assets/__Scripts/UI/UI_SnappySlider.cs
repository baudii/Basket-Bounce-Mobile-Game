using KK.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace BasketBounce.UI
{
	public class UI_SnappySlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
		[SerializeField] ScrollRect scrollRect;
		[SerializeField] RectTransform content;
		[SerializeField, Range(0, 1)] float predictedSnap;
		[SerializeField] float snapSpeed;

		[SerializeField] UnityEvent<int> OnSnap;

		float targetPosition;
		float startT;

		int currentIndex = 0;

		public void OnPointerUp(PointerEventData eventData)
		{
			var t = scrollRect.horizontalNormalizedPosition;

			SnapToNeighbour(t - startT);
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			StopAllCoroutines();
			scrollRect.horizontalNormalizedPosition = targetPosition;

			startT = scrollRect.horizontalNormalizedPosition;
		}

		private void SnapToNeighbour(float dir)
		{
			int dirNormalized = Utils.Signum(dir);
			int n = content.childCount;
			currentIndex = Mathf.Clamp(currentIndex + dirNormalized, 0, n-1);

			targetPosition = (currentIndex) / (n - 1);
			OnSnap?.Invoke(currentIndex);

			StartCoroutine(SmoothSnap());
		}

		private void SnapToClosestElement()
		{
			float t = scrollRect.horizontalNormalizedPosition;
			int dir = Utils.Signum(startT - t);
			int n = content.childCount;
			float k = Mathf.Round(t * (n - 1) - predictedSnap * dir);
			targetPosition = k / (n - 1);

			StartCoroutine(SmoothSnap());
		}

		private IEnumerator SmoothSnap()
		{
			targetPosition = Mathf.Clamp(targetPosition, 0f, 1f);

			while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) > 0.001f)
			{
				scrollRect.horizontalNormalizedPosition = 
					Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPosition, snapSpeed * Time.deltaTime);
				yield return null;
			}
			scrollRect.horizontalNormalizedPosition = targetPosition;
		}
	}
}
