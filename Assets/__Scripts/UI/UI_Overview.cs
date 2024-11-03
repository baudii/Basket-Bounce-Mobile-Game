using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using BasketBounce.DOTweenComponents.UI;
using BasketBounce.Gameplay;
using BasketBounce.Gameplay.Levels;


namespace BasketBounce.UI
{
	public class UI_Overview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] Scrollbar scrollbar;
		[SerializeField] UI_TweenFadeLoop fadeLoop;
		[SerializeField] GameObject inivisbleButtonDisabler;
		[SerializeField] UnityEvent OnClick;

		[Header("Fade animation")]
		[SerializeField] float duration;
		[SerializeField] MaskableGraphic[] maskableGraphics;

		float[] initialAlphas; 

		DollyCameraController _dolly;
		LevelManager _levelManager;
		public void Init(LevelManager levelManager, DollyCameraController dolly)
		{
			initialAlphas = new float[maskableGraphics.Length];
			scrollbar.interactable = true;

			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				initialAlphas[i] = maskableGraphics[i].color.a;
			}
			_levelManager = levelManager;
			_dolly = dolly;

			levelManager.OnLevelSetupEvent.AddListener(SetActive);
		}

		private void OnDestroy()
		{
			_levelManager.OnLevelSetupEvent.RemoveListener(SetActive);
		}

		private void SetActive(LevelData levelData)
		{
			if (_dolly.PathLength <= 3f)
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
			_dolly.EnableFollow();
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
			_dolly.DisableFollow();
			OnClick?.Invoke();
			for (int i = 0; i < maskableGraphics.Length; i++)
			{
				maskableGraphics[i].DOFade(1, duration);
			}
		}

		public void OnPointerUp(PointerEventData _)
		{
			_dolly.EnableFollow();
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
			_dolly.SetPathPosition(value);
		}
	}
}