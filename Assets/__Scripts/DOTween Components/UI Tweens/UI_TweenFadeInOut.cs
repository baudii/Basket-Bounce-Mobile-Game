using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TweenFadeInOut : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI headerTextField;
	[SerializeField] TextMeshProUGUI levelTextField;
	[SerializeField] RectTransform textRect;
	[SerializeField] MaskableGraphic target;
	[SerializeField] float targetAlpha;
	[SerializeField] float duration;
	[SerializeField] float delay;
	float initialAlpha;
	float initialTextXPos;

	private void Awake()
	{
		initialTextXPos = textRect.localPosition.x;
		initialAlpha = target.color.a;
	}
	public void StartAnimation(string header, int level)
	{
		levelTextField.text = "Level " + level.ToString();
		headerTextField.text = header;

		// Appear
		textRect.DOLocalMoveX(0, duration).SetUpdate(true);
		target.DOFade(targetAlpha, duration).SetUpdate(true);


		// Dissapear
		textRect.DOLocalMoveX(initialTextXPos, duration)
			.SetDelay(duration + delay)
			.SetUpdate(true);

		target.DOFade(initialAlpha, duration)
			.SetDelay(delay + duration)
			.SetUpdate(true)
			.OnComplete(() => gameObject.SetActive(false));
	}

	public void SlowStop()
	{
		if (isActiveAndEnabled)
		{
			target.DOKill(false);
			textRect.DOKill(false);

			textRect.DOLocalMoveX(initialTextXPos, duration)
				.SetUpdate(true);

			target.DOFade(initialAlpha, duration)
				.SetUpdate(true)
				.OnComplete(() =>
				{
					gameObject.SetActive(false);
					DOTween.Kill(textRect);
				});
		}
	}
}
