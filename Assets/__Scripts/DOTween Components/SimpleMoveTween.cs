using DG.Tweening;
using UnityEngine;

public class SimpleMoveTween : AbstractTween
{
	[SerializeField] Vector3 targetLocalPos;

	protected override Tween GetTween()
	{
		return transform.DOLocalMove(targetLocalPos, duration);
	}
}
