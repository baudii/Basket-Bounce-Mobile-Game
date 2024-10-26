using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using BasketBounce.DOTweenComponents.UI;
using BasketBounce.Systems;
using KK.Common;


namespace BasketBounce.UI
{
	public class UI_Overview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] Scrollbar scrollbar;
		[SerializeField] DollyCameraController dolly;
		[SerializeField] CinemachineVirtualCamera vcam;
		[SerializeField] GestureDetector gestureDetector;
		[SerializeField] UI_TweenFadeLoop fadeLoop;
		[SerializeField] Transform ballTransform;
		[SerializeField] GameObject inivisbleButtonDisabler;
		[SerializeField] UnityEvent OnClick;

		[Header("Fade animation")]
		[SerializeField] float duration;
		[SerializeField] MaskableGraphic[] maskableGraphics;

		float[] initialAlphas;


		private void Awake()
		{
			initialAlphas = new float[maskableGraphics.Length];
			scrollbar.interactable = true;

			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				initialAlphas[i] = maskableGraphics[i].color.a;
			}

			LevelManager.Instance.OnLevelSetup.AddListener(SetActive);
		}

		private void OnDestroy()
		{
			LevelManager.Instance.OnLevelSetup.RemoveListener(SetActive);
		}

		private void SetActive(LevelData levelData)
		{
			if (dolly.PathLength <= 3f)
				gameObject.SetActive(false);
			else
				Enable();
		}

		private void Enable()
		{
			gameObject.SetActive(true);
			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				maskableGraphics[i].DOFade(initialAlphas[i], 0);
			}
			inivisbleButtonDisabler.SetActive(false);
			scrollbar.value = 0;
			vcam.Follow = ballTransform;
			scrollbar.interactable = true;
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

		public void OnPointerDown(PointerEventData _)
		{
			vcam.Follow = null;
			OnClick?.Invoke();
			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				maskableGraphics[i].DOFade(1, duration);
			}
		}

		public void OnPointerUp(PointerEventData _)
		{
			vcam.Follow = ballTransform;
			scrollbar.value = 0; 
			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				maskableGraphics[i].DOFade(initialAlphas[i], duration);
			}
		}

		// Method is called from scrollbar OnValueChanged event
		// value is between 0 and 1
		public void OnValueChanged(float value)
		{
			dolly.SetPathPosition(value);
		}
	}
}