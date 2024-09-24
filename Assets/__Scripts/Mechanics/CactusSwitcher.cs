using UnityEngine;

public class CactusSwitcher : Switcher, IResetableItem
{
	[SerializeField] SpriteRenderer sr;
	[SerializeField] SpriteRenderer shadowSr;
	[SerializeField] Vector2 targetSize;
	[SerializeField] float targetTime;
	Vector2 initialSize;
	SpriteTween spriteTween;
	private void Awake()
	{
		initialSize = sr.size;
		spriteTween = new SpriteTween(sr, this);
	}

	public override void Activation()
	{
		if (IsActivated)
		{
			spriteTween.TweenSize(initialSize, targetSize, targetTime).OnUpdate(() => shadowSr.size = sr.size);
		}
		else
		{
			spriteTween.TweenSize(targetSize, initialSize, targetTime).OnUpdate(() => shadowSr.size = sr.size);
		}
	}

	public void ResetState()
	{
		this.SmartLog("Resetting state of Cacts");
		spriteTween.KillTween();
		sr.size = initialSize;
		shadowSr.size = initialSize;
		Deactivate();
	}
}
