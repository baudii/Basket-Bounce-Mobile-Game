using DG.Tweening;
using UnityEngine;

namespace BasketBounce.DOTweenComponents
{
	public class SimpleMoveTween : AbstractTween
	{
		[SerializeField] Vector3 targetLocalPos;

		protected override Tween GetTween(float duration)
		{
			return transform.DOLocalMove(transform.localPosition + targetLocalPos, duration);
		}
	}
}