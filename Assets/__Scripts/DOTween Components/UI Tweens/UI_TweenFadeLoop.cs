using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace BasketBounce.DOTweenComponents.UI
{
	public class UI_TweenFadeLoop : MonoBehaviour
	{
		[SerializeField] MaskableGraphic target;
		[SerializeField] float oneWayDuration;
		[SerializeField] float startTargetValue;
		[SerializeField] Ease ease;
		float initialAlpha;
		private void Start()
		{
			initialAlpha = target.color.a;
			FadeLoop();
		}

		void FadeLoop()
		{
			target.DOFade(startTargetValue, oneWayDuration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
		}

		public void Fade(float duration)
		{
			target.DOKill();
			target.DOFade(0, duration).SetEase(Ease.Linear);
		}

		public void UnFade()
		{
			target.DOKill(true);
			target.DOFade(initialAlpha, oneWayDuration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
		}
	}
}