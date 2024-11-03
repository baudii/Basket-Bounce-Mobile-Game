using DG.Tweening;
using UnityEngine;


namespace BasketBounce.DOTweenComponents
{
	public class TweenScaleWiggle : MonoBehaviour
	{
		[SerializeField] float delay;
		[SerializeField] float duration;
		[SerializeField] float targetScale;
		[SerializeField] Ease ease;

		Vector3 initialScale;
		Sequence seq;

		private void Start()
		{
			initialScale = transform.localScale;
			seq = DOTween.Sequence(transform);
			seq.Append(transform.DOScale(targetScale, duration)).
				Insert(duration, transform.DOScale(initialScale, duration)).
				SetDelay(delay).
				SetEase(ease).
				SetLoops(-1);
		}

		void OnEnable()
		{
			seq.Play();
		}

		public void StopWiggle()
		{
			seq.Pause();

			transform.localScale = initialScale;
		}
	}
}