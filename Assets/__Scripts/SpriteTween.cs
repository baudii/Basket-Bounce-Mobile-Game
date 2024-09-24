using System;
using System.Collections;
using UnityEngine;

public class SpriteTween
{
	MonoBehaviour caller;
	SpriteRenderer sr;
	Vector2 targetSize;
	float prevT;

	Action OnUpdateAction;

	public SpriteTween(SpriteRenderer sr, MonoBehaviour caller)
	{
		this.sr = sr;
		this.caller = caller;
	}

	public SpriteTween TweenSize(Vector2 initialSize, Vector2 targetSize, float targetTime)
	{
		caller.StopAllCoroutines();

		this.targetSize = targetSize;
		caller.StartCoroutine(TweenCoroutine(initialSize, targetTime));
		return this;
	}

	public void KillTween()
	{
		caller.StopAllCoroutines();
		prevT = 0;
		sr.size = targetSize;
	}

	public void OnUpdate(Action action)
	{
		OnUpdateAction = action;
	}

	IEnumerator TweenCoroutine(Vector2 startSize, float targetTime)
	{
		float t = 0;
			
		if (prevT != 0)
			t = 1 - prevT;
		do
		{
			t += Time.deltaTime / targetTime;
			prevT = t;
			sr.size = Vector2.Lerp(startSize, targetSize, t);
			OnUpdateAction?.Invoke();
			yield return null;
		} while (t < 1);
	}
}
