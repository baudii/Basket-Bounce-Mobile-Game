using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace BasketBounce.UI
{
	public class UI_StarAnimation : MonoBehaviour
	{
		[SerializeField] MaskableGraphic mainGraphic;

		Vector2 initialPos;
		Vector3 initialScale;
		Color initialColor;
		bool initialized;

		private void Awake()
		{
			initialized = true;
			initialPos = transform.localPosition;
			initialScale = transform.localScale;
			initialColor = mainGraphic.color;
		}

		public void ResetState()
		{
			if (!initialized)
				return;
			gameObject.SetActive(false);
			transform.localPosition = initialPos;
			transform.localScale = initialScale;
			mainGraphic.color = initialColor;
		}

		public Tween[] GetTweens(float duration, Ease ease)
		{
			Tween[] tweens = new Tween[3];
			tweens[0] = transform.DOLocalMove(Vector2.zero, duration).SetUpdate(true).SetEase(ease).OnStart(() => gameObject.SetActive(true)).SetAutoKill(false);
			tweens[1] = transform.DOScale(Vector2.one, duration).SetUpdate(true).SetEase(ease).SetAutoKill(false);
			tweens[2] = mainGraphic.DOFade(1, duration).SetUpdate(true).SetEase(ease).SetAutoKill(false);

			return tweens;
		}

	}
}