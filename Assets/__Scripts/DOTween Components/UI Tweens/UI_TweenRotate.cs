using DG.Tweening;
using UnityEngine;
namespace BasketBounce.DOTweenComponents.UI
{
	public class UI_TweenRotate : MonoBehaviour
	{
		[SerializeField] float duration;
		[SerializeField] bool independantUpdate = true;
		[SerializeField] Transform target;

		Quaternion initialRotation;
		private void Awake()
		{
			if (target == null)
				target = transform;
			initialRotation = target.rotation;
		}

		private void OnEnable()
		{
			RotateLoop();
		}

		private void OnDisable()
		{
			target.DOKill();
			target.rotation = initialRotation;
		}

		private void RotateLoop()
		{
			target.DORotate(new Vector3(0, 0, 360), duration, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(independantUpdate);
		}

	}
}