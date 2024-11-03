using UnityEngine;
using UnityEngine.UI;
using BasketBounce.Systems;
using BasketBounce.Gameplay;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Models;
using KK.Common;



#if UNITY_ANDROID || UNITY_IOS
using CandyCoded.HapticFeedback;
#endif

namespace BasketBounce.UI
{
	public class UI_Manager : MonoBehaviour
	{
		public enum MenuState
		{
			MainMenu,
			GameOver,
			LevelCompleted,
			LevelSelect,
			Paused,
			GameFinished,
			None
		}

		[Header("References")]
		[SerializeField] GameObject gameOverScreen;
		[SerializeField] UI_LevelCompleted levelCompleteScreen;
		[SerializeField] UI_PauseController pauseScreen;
		[SerializeField] UI_LevelSelector levelSelectorScreen;
		[SerializeField] UI_GameFinished gameFinishedScreen;
		[SerializeField] Image mainUiBgImage;
		[SerializeField] Image gameCompleteBgImage;
		[SerializeField] AudioSource src;
		[SerializeField] UI_BounceCounter bounceCounter;
		[SerializeField] UI_Overview overview;

		[Header("Level Header")]
		[SerializeField] UI_LevelNameController levelNameMainUI;

		[Header("Assets")]
		[SerializeField] GameAssets_SO gameAssets;
		public GameAssets_SO GameAssets => gameAssets;

		InputMaster input;
		GameManager gameManager;
		Ball ball;
		LevelManager levelManager;

		MenuState currentState, prevState;

		bool isInitialized;

		public void Init(GameManager gameManager, LevelManager levelManager, Ball ball)
		{
			this.gameManager = gameManager;
			this.ball = ball;
			this.levelManager = levelManager;

			input = new InputMaster();
			input.Enable();
			input.Taps.Tap.performed += ctx => Vibrate();

			this.gameManager.OnInGameEnterEvent.AddListener(OnInGameEnter);
			this.gameManager.OnInGameExitEvent.AddListener(OnInGameExit);
			this.gameManager.OnGameOverEvent.AddListener(ShowGameOverScreen);

			this.levelManager.OnLevelSetupEvent.AddListener(OnLevelSetup);
			this.levelManager.OnLevelIsLoadedEvent.AddListener(OnLevelIsLoaded);
			this.levelManager.OnFinishedGameEvent.AddListener(OnFinishedGame);
			this.levelManager.OnFinishedLevelEvent.AddListener(ShowLevelCompleteScreen);

			this.ball.OnBallStartStretch += HideOverview;
			this.ball.OnBallAbortStretch += ShowOverview;
			this.ball.OnStuck += ShowStuckScreen;

			isInitialized = true;
		}

		private void OnDestroy()
		{
			if (!isInitialized)
				return;

			input.Dispose();

			if (gameManager != null)
			{
				gameManager.OnInGameEnterEvent.RemoveListener(OnInGameEnter);
				gameManager.OnInGameExitEvent.RemoveListener(OnInGameExit);
				gameManager.OnGameOverEvent.RemoveListener(ShowGameOverScreen);
			}

			if (levelManager != null)
			{
				levelManager.OnLevelSetupEvent.RemoveListener(OnLevelSetup);
				levelManager.OnLevelIsLoadedEvent.RemoveListener(OnLevelIsLoaded);
				levelManager.OnFinishedGameEvent.RemoveListener(OnFinishedGame);
				levelManager.OnFinishedLevelEvent.RemoveListener(ShowLevelCompleteScreen);
			}

			if (ball != null)
			{
				ball.OnBallAbortStretch -= ShowOverview;
				ball.OnBallStartStretch -= HideOverview;
				ball.OnStuck -= ShowStuckScreen;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Clear Prefs")]
		public void ClearPrefs()
		{
			PlayerPrefs.DeleteAll();
		}
#endif

		void Vibrate()
		{
			src.Play();
#if (UNITY_IOS || UNITY_ANDROID)
			HapticFeedback.LightFeedback();
#endif
		}

		void SetState(MenuState state)
		{
			if (state == currentState)
				return;

			this.Log($"{currentState} -> {state}");

			OnExitState(currentState);

			switch (state)
			{
				case MenuState.MainMenu:
					break;
				case MenuState.GameOver:
					gameOverScreen.SetActive(true);
					break;
				case MenuState.LevelCompleted:
					levelCompleteScreen.gameObject.SetActive(true);
					levelSelectorScreen.UpdateLevelSelector();
					src.PlayOneShot(GameAssets.WinSound, 0.3f);
					break;
				case MenuState.LevelSelect:
					levelSelectorScreen.gameObject.SetActive(true);
					break;
				case MenuState.Paused:
					pauseScreen.gameObject.SetActive(true);
					break;
				case MenuState.GameFinished:
					gameFinishedScreen.gameObject.SetActive(true);
					gameCompleteBgImage.gameObject.SetActive(true);
					break;
			}

			gameManager.InMenu();
			prevState = currentState;
			currentState = state;
		}

		void OnExitState(MenuState state)
		{
			switch (state)
			{
				case MenuState.MainMenu:
					break;
				case MenuState.GameOver:
					gameOverScreen.SetActive(false);
					break;
				case MenuState.LevelCompleted:
					levelCompleteScreen.gameObject.SetActive(false);
					break;
				case MenuState.LevelSelect:
					levelSelectorScreen.gameObject.SetActive(false);
					break;
				case MenuState.Paused:
					pauseScreen.gameObject.SetActive(false);
					break;
				case MenuState.GameFinished:
					gameFinishedScreen.gameObject.SetActive(false);
					break;
			}
		}

		public void Back()
		{
			SetState(prevState);
			prevState = MenuState.None;
		}


		public void PlayButtonClickSound() => src.PlayOneShot(GameAssets.BlopSound, 0.6f);

		public void PlayMainSound() => src.Play();



		public void HideOverview() => overview.Hide();
		public void ShowOverview() => overview.Show();

		public void SubmitUiLevelSelector()
		{
			levelSelectorScreen.Submit();
		}

		public void OnLevelSetup(LevelData _)
		{
			levelSelectorScreen.UpdateLevelSelector();
		}

		void OnLevelIsLoaded(LevelData currentLevelData)
		{
			var header = currentLevelData.LevelHeader;
			var level = currentLevelData.LevelNum + 1;

			levelNameMainUI.gameObject.SetActive(true);
			levelNameMainUI.StartAnimation(header, level);
		}

		public void ShowStuckScreen()
		{
			pauseScreen.InitStuck();
			SetState(MenuState.Paused);
		}

		public void ShowPauseScreen()
		{
			pauseScreen.InitPause();
			SetState(MenuState.Paused);
		}


		void DisableAll()
		{
			mainUiBgImage.gameObject.SetActive(false);
			gameCompleteBgImage.gameObject.SetActive(false);
			gameOverScreen.SetActive(false);
			levelCompleteScreen.gameObject.SetActive(false);
			pauseScreen.gameObject.SetActive(false);
			levelSelectorScreen.gameObject.SetActive(false);
			gameFinishedScreen.gameObject.SetActive(false);
		}

		void OnInGameEnter()
		{
			DisableAll();
			bounceCounter.gameObject.SetActive(true);
			SetState(MenuState.None);
		}

		void OnInGameExit()
		{
			mainUiBgImage.gameObject.SetActive(true);
			bounceCounter.gameObject.SetActive(false);
		}

		void OnFinishedGame()
		{
			SetState(MenuState.GameFinished);
		}

		public void ShowLevelCompleteScreen(ScoreData scoreData)
		{
			SetState(MenuState.LevelCompleted);
			levelCompleteScreen.SetStars(scoreData);
		}

		public void ShowGameOverScreen()
		{
			SetState(MenuState.GameOver);
		}

		public void ShowLevelSelectScreen()
		{
			SetState(MenuState.LevelSelect);
		}

		public void ShowFinishedScreen()
		{
			SetState(MenuState.GameFinished);
		}

	}
}