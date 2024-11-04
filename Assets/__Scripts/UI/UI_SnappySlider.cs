using KK.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Diagnostics;

namespace BasketBounce.UI
{
	public class UI_SnappySlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
		[SerializeField] ScrollRect scrollRect;
		[SerializeField] RectTransform content;
		[SerializeField, Range(0, 1), Tooltip("Amount of seconds to be considered as \"fast\" snap")] float fastModeSeconds;
		[SerializeField, Range(0, 1), Tooltip("Affects only slow snap mode. " +
			"Predicts the direction you are trying to snap. If 1 is set, even " +
			"the smallest movement can trigger the snap. If zero is set, item " +
			"must reach halfway point to trigger the snap.")] 
		float predictedSnap;
		[SerializeField] float snapSpeed;

		[SerializeField] UnityEvent<int> OnSnap;
		Stopwatch timer;

		float targetPosition;
		float startT;

		int currentIndex = 0;

		private void OnEnable()
		{
			timer = new Stopwatch();
			OnSnap?.Invoke(0);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			var t = scrollRect.horizontalNormalizedPosition;
			timer.Stop();
			if (timer.Elapsed.TotalSeconds > fastModeSeconds)
			{
				SnapToClosestElement();
			}
			else
			{
				SnapByDirection(t - startT);
			}
			OnSnap?.Invoke(currentIndex);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			StopAllCoroutines();
			scrollRect.horizontalNormalizedPosition = targetPosition;

			startT = scrollRect.horizontalNormalizedPosition;
			timer.Restart();
		}

		private void SnapByDirection(float dir)
		{
			int dirNormalized = Utils.Signum(dir);
			int n = content.childCount;
			currentIndex = Mathf.Clamp(currentIndex + dirNormalized, 0, n - 1);

			targetPosition = (currentIndex) / (n - 1);

			StartCoroutine(SmoothSnap());
		}

		private void SnapToClosestElement()
		{
			float t = scrollRect.horizontalNormalizedPosition;
			int dir = Utils.Signum(t - startT);
			int n = content.childCount;
			int k = Mathf.RoundToInt(t * (n - 1) + predictedSnap * dir);
			currentIndex = Mathf.Clamp(k, 0, n - 1);
			targetPosition = currentIndex / (n - 1);

			StartCoroutine(SmoothSnap());
		}

		private IEnumerator SmoothSnap()
		{
			while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) > 0.001f)
			{
				scrollRect.horizontalNormalizedPosition = 
					Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPosition, snapSpeed * Time.unscaledDeltaTime);
				yield return null;
			}
			scrollRect.horizontalNormalizedPosition = targetPosition;
		}
	}
}
