using BasketBounce.Systems;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using KK.Common;
using BasketBounce.Gameplay.Levels;
using System.Threading;

namespace BasketBounce.UI
{
    public class MainMenuEntry : BaseGameEntry
	{
		const string MAIN_MENU_SCENE_NAME = "MainMenu";
		const string MAIN_MENU_ENTRY_SCENE_NAME = "MainMenuEntry";
		const string START_GAME_BUTTON_TAG = "StartGameButton";
		private async void Start()
		{
			try
			{
				await Setup(destroyCancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				this.Log("Operation was cancelled.", "Message:", ex.Message, "Stack trace:", ex.StackTrace);
			}
		}

		public async void SubmitLevelSet(int levelSet)
		{
			if (levelSet <= 0)
			{
				this.Error($"Incorrect level set index {levelSet}");
				return;
			}
			try
			{
				await Activate(destroyCancellationToken);
				GetDependancy(out LevelManager levelManager);
				await levelManager.ActivateLevelSet(levelSet);
				await SceneManager.UnloadSceneAsync(MAIN_MENU_ENTRY_SCENE_NAME).AsTask(levelManager.destroyCancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				this.Log("Operation was cancelled.", "Message:", ex.Message, "Stack trace:", ex.StackTrace);
			}
		}

		public override async Task Setup(CancellationToken token)
		{
			GameManager gameManager = new GameManager();
			await gameManager.StartLoading().AsTask(token);

			gameManager.Init();
			Init();
			Register(gameManager);

			await SceneManager.LoadSceneAsync(MAIN_MENU_SCENE_NAME, LoadSceneMode.Additive).AsTask(token);
			var scene = SceneManager.GetSceneByName(MAIN_MENU_SCENE_NAME);
			var root = scene.GetRootGameObjects();
			Transform startGameButton = null;
			foreach (var child in root)
			{
				child.transform.ForEachDescendant(child =>
				{
					if (child.tag == START_GAME_BUTTON_TAG)
					{
						startGameButton = child;
						return true;
					}
					return false;
				});

				if (startGameButton != null)
					break;
			}

			startGameButton.GetComponent<UI_Button_Handler>().OnStartGame += SubmitLevelSet;

			await Task.Delay(500, token);

			gameManager.InMenu();
		}

		public override async Task Activate(CancellationToken token)
		{
			GetDependancy(out GameManager gameManager);
			gameManager.StartLoading();

			await SceneManager.LoadSceneAsync(MainGameEntry.MAIN_ENTRY_SCENE_NAME, LoadSceneMode.Additive).AsTask(token);
			await SceneManager.UnloadSceneAsync(MAIN_MENU_SCENE_NAME).AsTask(token);
			var scene = SceneManager.GetSceneByName(MainGameEntry.MAIN_ENTRY_SCENE_NAME);
			var root = scene.GetRootGameObjects();

			MainGameEntry gameEntry = null;
			foreach (var child in root)
				child.TryGetComponent(out gameEntry);

			if (gameEntry == null)
				throw new EntryPointNotFoundException($"Entry point was not found in scene: {MainGameEntry.MAIN_ENTRY_SCENE_NAME}");

			await gameEntry.Enter();
			token.ThrowIfCancellationRequested();
		}
	}
}
