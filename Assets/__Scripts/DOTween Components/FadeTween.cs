using UnityEngine;
using DG.Tweening;

namespace BasketBounce.DOTweenComponents
{
	public class FadeTween : AbstractTween
	{
		[SerializeField] SpriteRenderer rend;
		[SerializeField] protected float targetAlpha;

		protected override Tween GetTween()
		{
			return rend.DOFade(targetAlpha, duration);
		}
	}
}