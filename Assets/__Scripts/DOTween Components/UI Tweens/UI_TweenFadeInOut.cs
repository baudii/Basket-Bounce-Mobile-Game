using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TweenFadeInOut : MonoBehaviour
{
	[SerializeField] MaskableGraphic target;
	[SerializeField] float targetAlpha;
	[SerializeField] float duration;
	[SerializeField] float delay;

	[Header("Level text animation")]
	[SerializeField] RectTransform textRect;
	[SerializeField] TextMeshProUGUI levelTextField;
	[SerializeField] TextMeshProUGUI headerTextField;
	[SerializeField] float textAnimDuration;
	[SerializeField, Tooltip("Excluding level text duration")] float delayBetweenAppear, delayToDissapear;
	float initialAlpha;
	// float initialTextXPos; - old

	private void Awake()
	{
		// initialTextXPos = textRect.localPosition.x; - old

		levelTextField.DOFade(0, 0);
		headerTextField.DOFade(0, 0);
		initialAlpha = target.color.a;
	}
	public void StartAnimation(string header, int level)
	{
		levelTextField.text = "Level " + level.ToString();
		headerTextField.text = header;

		// Appear
		// textRect.DOLocalMoveX(0, duration).SetUpdate(true); Previous animation (text was moving from right)
		levelTextField.DOFade(1, textAnimDuration).SetUpdate(true);
		headerTextField.DOFade(1, textAnimDuration).SetDelay(delayBetweenAppear).SetUpdate(true);
		target.DOFade(targetAlpha, duration).SetUpdate(true);

		// Dissapear
		// textRect.DOLocalMoveX(initialTextXPos, duration).SetDelay(duration + delay).SetUpdate(true); Previous animation dissapear
		levelTextField.DOFade(0, duration).SetDelay(delay - delayBetweenAppear + textAnimDuration).SetUpdate(true);
		headerTextField.DOFade(0, duration).SetDelay(delay - delayBetweenAppear + textAnimDuration).SetUpdate(true);

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
			levelTextField.DOKill(false);
			headerTextField.DOKill(false);

			// Old animation
			// textRect.DOKill(false);
			// textRect.DOLocalMoveX(initialTextXPos, duration).SetUpdate(true);

			levelTextField.DOFade(0, duration).SetUpdate(true);
			headerTextField.DOFade(0, duration).SetUpdate(true);

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