using DG.Tweening;
using TMPro;
using UnityEngine;
using BasketBounce.DOTweenComponents.UI;
using BasketBounce.Gameplay;
using KK.Common;
using BasketBounce.Systems;

namespace BasketBounce.UI
{
	public class UI_BounceCounter : MonoBehaviour, IInitializable
	{
		[SerializeField] TextMeshProUGUI textObject;
		[SerializeField] UI_TweenScale tweenScale;
		[SerializeField] UI_TweenFade tweenFade;
		Ball ball;

		public void Init()
		{
			DIContainer.GetDependency(out ball);
			ball.OnBallBounce += OnBounce;
			ball.OnResetState += ResetState;
		}

		private void OnDestroy()
		{
			if (ball == null)
				return;

			ball.OnBallBounce -= OnBounce;
			ball.OnResetState -= ResetState;
		}

		public void OnBounce(int bounces)
		{
			textObject.text = bounces.ToString();
			tweenScale.TweenToFrom();
			tweenFade.Unfade().OnComplete(() => tweenFade.Fade());
		}

		public void ResetState()
		{
			textObject.text = "0";
		}
	}
}