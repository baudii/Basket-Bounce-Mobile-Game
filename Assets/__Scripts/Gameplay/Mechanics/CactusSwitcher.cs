using UnityEngine;
using KK.Common;
using KK.Common.Gameplay;
using BasketBounce.Systems;
using BasketBounce.DOTweenComponents;

namespace BasketBounce.Gameplay.Mechanics
{
	public class CactusSwitcher : Switcher, IResetableItem
	{
		[SerializeField] SpriteRenderer sr;
		[SerializeField] SpriteRenderer shadowSr;
		[SerializeField] bool oneWay;
		[SerializeField] Vector2 targetSize;
		[SerializeField] float targetTime;
		Vector2 initialSize;
		SpriteTween spriteTween;
		bool grown;
		private void Awake()
		{
			initialSize = sr.size;
			spriteTween = new SpriteTween(sr, this);
		}

		public override void Activation()
		{
			if (oneWay && grown)
				return;

			if (IsActivated)
			{
				spriteTween.TweenSize(initialSize, targetSize, targetTime).OnUpdate(() => shadowSr.size = sr.size);
			}
			else
			{
				spriteTween.TweenSize(targetSize, initialSize, targetTime).OnUpdate(() => shadowSr.size = sr.size);
			}
			grown = true;
		}

		public void ResetState()
		{
			this.Log("Resetting state of Cactus");
			grown = false;
			spriteTween.KillTween();
			sr.size = initialSize;
			shadowSr.size = initialSize;
			Deactivate();
		}
	}
}