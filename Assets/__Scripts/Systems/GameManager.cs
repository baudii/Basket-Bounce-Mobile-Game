using KK.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BasketBounce.Systems
{
	public class GameManager : IDisposable
	{
		// Last opened means the last level that can be chosen from level select - Switches to next when level is finished
		public const string LEVEL_SET_DEPENDANCY_KEY = "LevelSetNum";
		
		public static string GetOperationCancelledLog(OperationCanceledException ex)
		{
			return $"Operation was cancelled. Message: {ex.Message}\nStack trace: {ex.StackTrace}";
		}

		public int? CachedLevelSet { get; private set; }


		public enum GameState
		{
			InGame,
			Loading,
			InMenu
		}

		[HideInInspector]
		public UnityEvent OnInGameEnterEvent;
		[HideInInspector] 
		public UnityEvent OnInGameExitEvent;
		[HideInInspector]
		public UnityEvent OnGameOverEvent;

		public Func<Task> OnSubmitLevelSet;

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
			OnSubmitLevelSet = null;
		}

		async Task SetStateAsync(GameState state, CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			this.Log(currentState, "->", state);

			if (state == currentState)
				return;

			await OnExitStateAsync(currentState, token);

			currentState = state;

			switch (state)
			{
				case GameState.InGame:
					Time.timeScale = 1;
					OnInGameEnterEvent?.Invoke();
					break;
				case GameState.Loading:
					this.Log("Loading \"loading scene\"");
					await SceneManager.LoadSceneAsync(SceneNames.LOADING, LoadSceneMode.Additive).AsTask(token);
					break;
			}
		}

		Task OnExitStateAsync(GameState state, CancellationToken token)
		{
			switch (state)
			{
				case GameState.InGame:
					Time.timeScale = 0;
					OnInGameExitEvent?.Invoke();
					break;
				case GameState.Loading:
					this.Log("Unloading \"loading scene\"");
					return SceneManager.UnloadSceneAsync(SceneNames.LOADING).AsTask(token);
			}
			return Task.CompletedTask;
		}

		public Task ResumeGame(CancellationToken token = default)
		{
			return SetStateAsync(GameState.InGame, token);
		}

		public Task InMenu(CancellationToken token = default)
		{
			return SetStateAsync(GameState.InMenu, token);
		}

		public Task StartLoading(CancellationToken token = default)
		{
			return SetStateAsync(GameState.Loading, token);
		}

		public void GameOver()
		{
			OnGameOverEvent?.Invoke();
		}

		public async void SubmitLevelSet(int levelSet)
		{
			try
			{
				this.Log("Level set:", levelSet);
				CachedLevelSet = levelSet;
				await OnSubmitLevelSet?.Invoke();
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GetOperationCancelledLog(ex));
			}
		}

		public int RecieveLevelSet()
		{
			if (CachedLevelSet == null)
				throw new InvalidOperationException("Before recieving cached level set, you must submit it first");

			var levelSet = (int)CachedLevelSet;
			CachedLevelSet = null;
			return levelSet;
		}

		public async Task HomeAsync()
		{
			try
			{
				await SceneManager.LoadSceneAsync(SceneNames.MAIN_MENU_ENTRY).AsTask(SceneEntryPoint.Cts.Token);
				var scene = SceneManager.GetSceneByName(SceneNames.MAIN_MENU_ENTRY);
				var rootGo = scene.GetRootGameObjects();
				SceneEntryPoint entry = null;
				foreach (var go in rootGo)
				{
					if (go.TryGetComponent(out entry))
						break;
				}
				if (entry == null)
					throw new EntryPointNotFoundException($"Entry point was not found in scene {SceneNames.MAIN_MENU_ENTRY}");

				await entry.Setup();
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GetOperationCancelledLog(ex));
			}
		}
	}
}
