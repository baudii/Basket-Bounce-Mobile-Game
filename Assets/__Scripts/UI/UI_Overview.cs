using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Overview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] Scrollbar scrollbar;
	[SerializeField] DollyCameraController dolly;
	[SerializeField] CinemachineVirtualCamera vcam;
	[SerializeField] GestureDetector gestureDetector;
	[SerializeField] UI_TweenFadeLoop fadeLoop;
	[SerializeField] Ball ball;
	[SerializeField] GameObject inivisbleButtonDisabler;
	[SerializeField] UnityEvent OnClick;

	[Header("Fade animation")]
	[SerializeField] float duration;
	[SerializeField] MaskableGraphic[] maskableGraphics;

	float[] initialAlphas;


	private void Awake()
	{
		initialAlphas = new float[maskableGraphics.Length];
		LevelManager.Instance.OnLevelSetup.AddListener(Enable);
		scrollbar.interactable = true;

		for (int i = 0; i < maskableGraphics.Length; i++)
		{
			initialAlphas[i] = maskableGraphics[i].color.a;
		}
	}

	private void OnDestroy()
	{
		LevelManager.Instance.OnLevelSetup.RemoveListener(Enable);
	}

	private void Enable(LevelData _)
	{
		gameObject.SetActive(true);
		for (int i = 0; i < maskableGraphics.Length; i++)
		{
			maskableGraphics[i].DOFade(initialAlphas[i], 0);
		}
		inivisbleButtonDisabler.SetActive(false);
		scrollbar.value = 0;
		vcam.Follow = ball.transform;
		gestureDetector.SetActive(true);
		scrollbar.interactable = true;
	}

	public void OnPointerDown(PointerEventData _)
	{
        gestureDetector.SetActive(false);
		vcam.Follow = null;
		OnClick?.Invoke();
	}

	public void OnPointerUp(PointerEventData _)
	{
		gestureDetector.SetActive(true);
		vcam.Follow = ball.transform;
		scrollbar.value = 0;
	}

	// Method is called from scrollbar OnValueChanged event
	// value is between 0 and 1
	public void OnValueChanged(float value)
	{
		dolly.SetPathPosition(value);
	}

	public void Hide()
	{
		inivisbleButtonDisabler.SetActive(true);

		if (fadeLoop.gameObject.activeSelf)
			fadeLoop.Fade(duration);

		foreach (var item in maskableGraphics)
		{
			item.DOKill(true);
			item.DOFade(0, duration);
		}
	}

	public void Show()
	{
		for (int i = 0; i < maskableGraphics.Length; i++)
		{
			maskableGraphics[i].DOKill(false);
			maskableGraphics[i].DOFade(initialAlphas[i], duration).OnComplete(() => 
			{
				scrollbar.interactable = true;
				if (fadeLoop.gameObject.activeSelf)
					fadeLoop.UnFade();
				inivisbleButtonDisabler.SetActive(false);
			});
		}
	}
}
