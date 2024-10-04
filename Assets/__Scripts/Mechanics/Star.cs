using DG.Tweening;
using UnityEngine;

public class Star : MonoBehaviour, IResetableItem
{
	[SerializeField] float duration;
	[SerializeField, Range(0,2)] float targetScale;
	private void Start()
	{
		transform.DOScale(targetScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Ball ball))
		{
			ball.CollectStar();
			gameObject.SetActive(false);
			// Создать анимацию
			// Звук
		}
	}

	public void ResetState()
	{
		gameObject.SetActive(true);
	}
}
