using DG.Tweening;
using UnityEngine;

public class UI_TweenScale : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 targetScale = new Vector3(1,1,1);
	[SerializeField] float toSeconds, fromSeconds;
	[SerializeField] Ease ease;
	[SerializeField] bool unscaledTime;
	Vector3 initialScale;
	private void Start()
	{
		initialScale = transform.localScale;
	}
	public void TweenTo()
	{
		target.DOScale(targetScale, toSeconds).SetEase(ease);
	}

	public void TweenFrom()
	{
		target.DOScale(initialScale, fromSeconds).SetEase(ease);
	}

	public void TweenToFrom()
	{
		DOTween.Kill(target);
		transform.localScale = initialScale;
		target.DOScale(targetScale, toSeconds).OnComplete(() => target.DOScale(initialScale, fromSeconds).SetEase(ease).SetUpdate(unscaledTime)).SetEase(ease).SetUpdate(unscaledTime);
	}

}
