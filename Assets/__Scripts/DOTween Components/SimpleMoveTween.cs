using DG.Tweening;
using UnityEngine;

namespace BasketBounce.DOTweenComponents
{
	public class SimpleMoveTween : AbstractTween
	{
		[SerializeField] Vector3 targetLocalPos;

		protected override Tween GetTween()
		{
			return transform.DOLocalMove(targetLocalPos, duration);
		}
	}
}