using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BasketBounce.DOTweenComponents.UI
{
	public class UI_TweenColor : MonoBehaviour
	{
		[SerializeField] bool startFromCurrentColor;
		[SerializeField] Color startColor, endColor;
		[SerializeField] float duration;
		[SerializeField] Image targetImage;

		private void OnEnable()
		{
			if (!startFromCurrentColor)
				targetImage.color = startColor;
			targetImage.DOColor(endColor, duration).SetUpdate(true);
		}

		private void OnDisable()
		{
			transform.DOKill();
		}
	}
}