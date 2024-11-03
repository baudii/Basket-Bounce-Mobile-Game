using UnityEngine;
using UnityEngine.EventSystems;
using BasketBounce.Systems;
using BasketBounce.Gameplay.Levels;
using KK.Common;
using System;

namespace BasketBounce.UI
{
	public class UI_Button_Handler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField, Tooltip("Needed to adjust size. If none given - this gameobject's rectTransform will be taken by default")] RectTransform iconRect;
		[SerializeField] bool scaleIcon;

		GameManager gameManager;
		LevelManager levelManager;
		UI_Manager uiManager;

		public Action<int> OnStartGame;

		int levelSetCache;

		private void Awake()
		{
			if (iconRect == null)
			{
				iconRect = (RectTransform)transform;
			}
		}

		public void Init(GameManager gameManager, LevelManager levelManager, UI_Manager uiManager)
		{
			this.gameManager = gameManager;
			this.levelManager = levelManager;
			this.uiManager = uiManager;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (scaleIcon)
			{
				iconRect.transform.localScale *= 0.95f;
				return;
			}
			iconRect.sizeDelta += new Vector2(0, -3);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (scaleIcon)
			{
				// 20/19 = 1/0.95f - �������� �� �������� �����, ����� ������� ����������� �����
				iconRect.transform.localScale *= (20f / 19f);
				return;
			}
			iconRect.sizeDelta += new Vector2(0, 3);
			uiManager.PlayButtonClickSound();
		}

		public void LevelSelect()
		{
			uiManager.ShowLevelSelectScreen();
		}

		public void SubmitLevelLoad()
		{
			uiManager.SubmitUiLevelSelector();
		}

		public void Restart()
		{
			//gm.SetActiveLoadingScreen(true);

			levelManager.SetupLevel();
			levelManager.CurrentLevelData.ResetLevel();
			//uiManager.SetActiveLoadingScreen(false);
			gameManager.ResumeGame();

			// � ������� �������� ������� ����� � ������ ��������
			/*		gm.Co_DelayedExecute(() =>
					{
					}, 0.3f);*/
		}

		public void NextLevel()
		{
			levelManager.NextLevel();
		}

		public void ResumeGame()
		{
			gameManager.ResumeGame();
		}

		public void Back()
		{
			uiManager.Back();
		}

		public void Pause()
		{
			uiManager.ShowPauseScreen();
		}

		public void CacheLevelSet(int levelSet)
		{
			levelSetCache = levelSet + 1;
		}

		public void StartGame()
		{
			OnStartGame.Invoke(levelSetCache);
		}
	}
}