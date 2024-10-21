using DG.Tweening;
using UnityEngine;
namespace BasketBounce.DOTweenComponents.UI
{
	public class UI_TweenRotate : MonoBehaviour
	{
		[SerializeField] float duration;

		Quaternion initialRotation;
		private void Awake()
		{
			initialRotation = transform.rotation;
		}

		private void OnEnable()
		{
			RotateLoop();
		}

		private void OnDisable()
		{
			transform.DOKill();
			transform.rotation = initialRotation;
		}

		private void RotateLoop()
		{
			transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(true);
		}

	}
}