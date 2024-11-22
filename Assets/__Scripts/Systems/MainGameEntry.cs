using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using KK.Common;
using System.Threading;

namespace BasketBounce.Systems
{
    public class MainGameEntry : SceneEntryPoint
	{
#if UNITY_EDITOR
		[SerializeField] bool initializeOnStart;
		[SerializeField] int testLevel;
		private void Start()
		{
			if (initializeOnStart)
			{
				Utils.SafeExecuteAsync(Enter);
			}
		}
#endif

		SceneEntryPoint levelsEntryPoint, uiEntryPoint, gameplayEntryPoint;

		public async Task Enter()
		{
			try
			{
				await Setup();
				await Activate();
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GameManager.GetOperationCancelledLog(ex));
			}
		}

		public override async Task Setup()
		{
			var scene = SceneManager.GetSceneByName(SceneNames.MAIN_GAME_ENTRY);
			SceneManager.SetActiveScene(scene);

			await LoadScenes(Cts.Token, SceneNames.LEVELS_ENTRY, SceneNames.GAMEPLAY_ENTRY, SceneNames.UI_ENTRY);

			levelsEntryPoint = GetEntryPoint(SceneNames.LEVELS_ENTRY);
			gameplayEntryPoint = GetEntryPoint(SceneNames.GAMEPLAY_ENTRY);
			uiEntryPoint = GetEntryPoint(SceneNames.UI_ENTRY);

			Task[] tasks = new Task[3];
			tasks[0] = levelsEntryPoint.Setup();
			tasks[1] = gameplayEntryPoint.Setup();
			tasks[2] = uiEntryPoint.Setup();

			await Task.WhenAll(tasks);

			this.Log("Finished setup");
		}

		public override async Task Activate()
		{
			// Порядок важен
			await gameplayEntryPoint.Activate();
			await uiEntryPoint.Activate();
			await levelsEntryPoint.Activate();

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

		private SceneEntryPoint GetEntryPoint(string sceneName)
		{
			var scene = SceneManager.GetSceneByName(sceneName);
			var rootGOs = scene.GetRootGameObjects();

			foreach (var go in rootGOs)
			{
				if (go.TryGetComponent(out SceneEntryPoint goEntry))
				{
					return goEntry;
				}
			}

			throw new EntryPointNotFoundException($"Entry point was not found in the scene {scene.name}");
		}
	}
}
