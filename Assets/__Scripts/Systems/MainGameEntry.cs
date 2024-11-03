using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using KK.Common;
using System.Threading;

namespace BasketBounce.Systems
{
    public class MainGameEntry : BaseGameEntry
	{
		public const string MAIN_ENTRY_SCENE_NAME = "MainEntry";
		const string LEVEL_SCENE_NAME = "LevelsEntry";
		const string GAMEPLAY_SCENE_NAME = "GameplayEntry";
		const string UI_SCENE_NAME = "UIEntry";

		BaseGameEntry levelsEntryPoint, uiEntryPoint, gameplayEntryPoint;

		public async Task Enter()
		{
			try
			{
				var scene = SceneManager.GetSceneByName(MAIN_ENTRY_SCENE_NAME);
				SceneManager.SetActiveScene(scene);
				await Setup(destroyCancellationToken);
				await Activate(destroyCancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				this.Log("Operation was cancelled.", "Message:", ex.Message, "Stack trace:", ex.StackTrace);
			}
		}

		public async override Task Setup(CancellationToken token)
		{
			await LoadScenes(token, LEVEL_SCENE_NAME, GAMEPLAY_SCENE_NAME, UI_SCENE_NAME);

			levelsEntryPoint = GetEntryPoint(LEVEL_SCENE_NAME);
			gameplayEntryPoint = GetEntryPoint(GAMEPLAY_SCENE_NAME);
			uiEntryPoint = GetEntryPoint(UI_SCENE_NAME);

			Task[] tasks = new Task[3];
			tasks[0] = levelsEntryPoint.Setup(token);
			tasks[1] = gameplayEntryPoint.Setup(token);
			tasks[2] = uiEntryPoint.Setup(token);

			await Task.WhenAll(tasks);

			this.Log("Finished setup");
		}

		public override async Task Activate(CancellationToken token)
		{
			// Порядок важен
			await gameplayEntryPoint.Activate(token);
			await uiEntryPoint.Activate(token);
			await levelsEntryPoint.Activate(token);

			await SceneManager.UnloadSceneAsync(LEVEL_SCENE_NAME).AsTask(token);
			await SceneManager.UnloadSceneAsync(GAMEPLAY_SCENE_NAME).AsTask(token);
			await SceneManager.UnloadSceneAsync(UI_SCENE_NAME).AsTask(token);

			this.Log("Finished activation");
		}

		private async Task LoadScenes(CancellationToken token, params string[] sceneNames)
		{
			token.ThrowIfCancellationRequested();

			Task[] tasks = new Task[sceneNames.Length];
			for (int i = 0; i < sceneNames.Length; i++)
			{
				tasks[i] = SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive).AsTask(token);
			}
			await Task.WhenAll(tasks);
		}

		private BaseGameEntry GetEntryPoint(string sceneName)
		{
			var scene = SceneManager.GetSceneByName(sceneName);
			var rootGOs = scene.GetRootGameObjects();

			foreach (var go in rootGOs)
			{
				if (go.TryGetComponent(out BaseGameEntry goEntry))
				{
					return goEntry;
				}
			}

			throw new EntryPointNotFoundException($"Entry point was not found in the scene {scene.name}");
		}
	}
}
