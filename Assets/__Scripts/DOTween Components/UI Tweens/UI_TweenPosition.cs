using UnityEngine;
using DG.Tweening;
public class UI_TweenPosition : MonoBehaviour
{
	[SerializeField] Vector2 newPos;
	[SerializeField] float duration;
	[SerializeField] float delay;
	[SerializeField] Ease ease;
	[SerializeField] bool activateOnEnable;
	RectTransform rectToMove;
	Vector2 initialPosition;

	private void Awake()
	{
		rectToMove = (RectTransform)transform;
		initialPosition = rectToMove.position;
	}

	private void OnEnable()
	{
		if (activateOnEnable)
			Move();
	}

	public void Move()
	{
		// установить в начальную позицию
		rectToMove.position = initialPosition;
		// санимировать
		rectToMove.DOLocalMove(newPos, duration).SetEase(ease).SetDelay(delay);

	}
}
