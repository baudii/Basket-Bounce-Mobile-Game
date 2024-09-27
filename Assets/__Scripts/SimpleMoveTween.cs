using DG.Tweening;
using UnityEngine;

public class SimpleMoveTween : MonoBehaviour
{
	[SerializeField] bool activateOnStart;
	[SerializeField] bool isLooping;
	[SerializeField] bool isUnscaledTime;
	[SerializeField] Vector3 targetPosition;
	[SerializeField] float duration;
	[SerializeField] float delay;
	[SerializeField] Ease ease;

	private void Start()
	{
		if (activateOnStart)
			Move();
	}

	public void Move()
	{
		var i = transform.DOMove(targetPosition, duration).SetUpdate(isUnscaledTime).SetEase(ease).SetDelay(delay);
		if (isLooping)
			i.SetLoops(-1, LoopType.Yoyo);
	}
}
