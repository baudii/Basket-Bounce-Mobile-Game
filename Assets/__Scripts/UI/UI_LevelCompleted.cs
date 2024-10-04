using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_LevelCompleted : MonoBehaviour
{
	[SerializeField] UI_Stars ui_stars;
	[SerializeField] TextMeshProUGUI nextStarText;
	[SerializeField] GameObject GJGO;
	[SerializeField] TextDataSO textData;

	[Header("Bounces Text Animation")]
	[SerializeField] TextMeshProUGUI bouncesText;
	[SerializeField] float bouncesTexOffsetValue;
	[SerializeField] float textFallDuration;
	[SerializeField] Ease ease;
	[SerializeField] float initialScale;
	

	public void SetStars(int stars)
	{
		// ���������� ���������� �����������
		ui_stars.SetStars(stars);
		bouncesText.text = (LevelManager.Instance.CurrentLevel + 1).ToString();

		// ��������� ��������� ��������
		bouncesText.transform.localScale = Vector3.one;
		bouncesText.transform.localScale *= initialScale;
		bouncesText.transform.localPosition += Vector3.down * bouncesTexOffsetValue;
		bouncesText.alpha = 0;

		// ����� �������� � 1 �����, ������ ��� DOTween �� ��������� �� ��������� ����
		this.Co_DelayedExecute(() =>
		{
			// ��������� ����������� � ������������ ���������
			bouncesText.DOFade(1, textFallDuration).SetEase(ease).SetUpdate(true);
			/*			
			 *			.OnComplete(() => 
			 *			{
							bouncesText.DOColor(colors[scoreData.stars - 1], textGlowDuration).SetUpdate(true).OnComplete(() =>
							{
								// ��������� ���� ������
								bouncesText.DOColor(Color.white, textGlowDuration).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
							});
						});*/
			bouncesText.transform.DOScale(1, textFallDuration).SetEase(ease).SetUpdate(true);
			bouncesText.transform.DOLocalMove(Vector3.zero, textFallDuration).SetEase(ease).SetUpdate(true);
		}, 1);
		

		// ������ ���������� � ��������� ������ (���� ���������� ���, �� ���������� ���)
		if (stars == 3)
		{
			GJGO.SetActive(true);
			nextStarText.gameObject.SetActive(false);
		}
		else
		{
			GJGO.SetActive(false);
			nextStarText.gameObject.SetActive(true);
			nextStarText.text = textData.texts[Random.Range(0, textData.texts.Length)];
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
