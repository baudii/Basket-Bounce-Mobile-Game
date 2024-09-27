using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_LevelCompleted : MonoBehaviour
{
	[SerializeField] UI_Stars ui_stars;
	[SerializeField] TextMeshProUGUI nextStarText;
	[SerializeField] GameObject GJGO;

	[Header("Bounces Text Animation")]
	[SerializeField, Tooltip("Colors for text animation. Elements: 0 - 1 star, 1 - 2 stars, 2 - 3 stars")] Color[] colors;
	[SerializeField] float textGlowDuration;
	[SerializeField] TextMeshProUGUI bouncesText;
	[SerializeField] float bouncesTexOffsetValue;
	[SerializeField] float textFallDuration;
	[SerializeField] Ease ease;
	[SerializeField] float initialScale;
	

	public void SetStars(ScoreData scoreData, int bounces)
	{
		// Отображаем результаты прохождения
		ui_stars.SetStars(scoreData.stars);
		bouncesText.text = bounces.ToString();
		
		// Стартовое положение анимации
		bouncesText.transform.localScale *= initialScale;
		bouncesText.transform.localPosition += Vector3.down * bouncesTexOffsetValue;
		bouncesText.alpha = 0;

		// Делай задержку в 1 фрейм, потому что DOTween не реагирует на изменения выше
		this.Co_DelayedExecute(() =>
		{
			// Анимируем возвращение к изначальному состоянию
			bouncesText.DOFade(1, textFallDuration).SetEase(ease).SetUpdate(true).OnComplete(() =>
			{
				bouncesText.DOColor(colors[scoreData.stars - 1], textGlowDuration).SetUpdate(true).OnComplete(() =>
				{
					// Анимируем цвет текста
					bouncesText.DOColor(Color.white, textGlowDuration).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
				});
			});
			bouncesText.transform.DOScale(1, textFallDuration).SetEase(ease).SetUpdate(true);
			bouncesText.transform.DOLocalMove(Vector3.zero, textFallDuration).SetEase(ease).SetUpdate(true);
		}, 1);
		

		// Туглим требования к следующей звезде (если требований нет, то показываем это)
		if (scoreData.nextStarBounceRequirement <= 0)
		{
			GJGO.SetActive(true);
			nextStarText.gameObject.SetActive(false);
		}
		else
		{
			GJGO.SetActive(false);
			nextStarText.gameObject.SetActive(true);
			nextStarText.text = "NEXT : " + scoreData.nextStarBounceRequirement;
		}
	}

	private void OnDisable()
	{
		bouncesText.DOKill(false);
		bouncesText.color = Color.white;
	}

	[ContextMenu("Set 3 stars")]
	public void Set3Stars() => ui_stars.SetStars(3);
}
