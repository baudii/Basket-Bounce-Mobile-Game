using UnityEngine;
using DG.Tweening;

namespace BasketBounce.DOTweenComponents
{
	public abstract class AbstractTween : MonoBehaviour
	{
		[SerializeField] float duration;
		[SerializeField] bool activateOnStart;
		[SerializeField] bool isLoop;
		[SerializeField] LoopType loopType = LoopType.Yoyo;
		[SerializeField] bool isUnscaledTime;
		[SerializeField] float initialDelay;
		[SerializeField] float loopDelay;
		[SerializeField] Ease ease;

		Tween tween;

		private void Start()
		{
			if (activateOnStart)
				Animate();
		}

		protected abstract Tween GetTween(float duration);

		protected virtual void Animate()
		{
			Sequence seq = DOTween.Sequence();
			tween = GetTween()
					.SetUpdate(isUnscaledTime)
					.SetEase(ease);
			seq.Append(tween).AppendInterval(loopDelay).SetDelay(initialDelay);

			if (isLoop)
				seq.SetLoops(-1, loopType);
		}
	}
}