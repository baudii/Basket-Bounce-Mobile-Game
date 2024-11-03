using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BasketBounce.Systems
{
	public class GameManager : IDisposable
	{
		// Last opened means the last level that can be chosen from level select - Switches to next when level is finished
		public const string LOADING_SCENE_NAME = "LoadingScene";

		public enum GameState
		{
			InGame,
			Loading,
			InMenu
		}

		AsyncOperation asyncOperation;

		[HideInInspector]
		public UnityEvent OnInGameEnterEvent;
		[HideInInspector] 
		public UnityEvent OnInGameExitEvent;
		[HideInInspector]
		public UnityEvent OnGameOverEvent;


		GameState currentState;

		public GameManager()
		{
			OnInGameEnterEvent = new UnityEvent();
			OnInGameExitEvent = new UnityEvent();
			OnGameOverEvent = new UnityEvent();

			currentState = GameState.InMenu;
		}

		public void Init()
		{
#if UNITY_IOS || UNITY_ANDROID
			//Application.targetFrameRate = 120;
#endif
		}

		public void Dispose()
		{
			OnInGameEnterEvent = null;
			OnInGameExitEvent = null;
			OnGameOverEvent = null;
		}

		void SetState(GameState state)
		{
			if (state == currentState)
				return;

			OnExitState(currentState);

			switch (state)
			{
				case GameState.InGame:
					Time.timeScale = 1;
					OnInGameEnterEvent?.Invoke();
					break;
				case GameState.Loading:
					asyncOperation = SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);
					break;
			}
			currentState = state;
		}

		void OnExitState(GameState state)
		{
			switch (state)
			{
				case GameState.InGame:
					Time.timeScale = 0;
					OnInGameExitEvent?.Invoke();
					break;
				case GameState.Loading:
					SceneManager.UnloadSceneAsync(LOADING_SCENE_NAME);
					break;
			}
		}

		public void ResumeGame()
		{
			SetState(GameState.InGame);
		}

		public void InMenu()
		{
			SetState(GameState.InMenu);
		}

		public AsyncOperation StartLoading()
		{
			SetState(GameState.Loading);
			return asyncOperation;
		}

		public void GameOver()
		{
			OnGameOverEvent?.Invoke();
		}
	}
}
