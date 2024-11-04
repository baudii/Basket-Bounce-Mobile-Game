using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using KK.Common;

namespace BasketBounce.Systems
{
    public class MainMenuEntry : SceneEntryPoint
	{
		GameManager gameManager;
		public async Task Enter()
		{
			try
			{
				await Setup();
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GameManager.GetOperationCancelledLog(ex));
			}
		}

		public async Task SubmitLevelSet()
		{
			try
			{
				await Activate();
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GameManager.GetOperationCancelledLog(ex));
			}
		}

		public override async Task Setup()
		{
			DIContainer.GetDependency(out GameManager gameManager);

			await SceneManager.LoadSceneAsync(SceneNames.MAIN_MENU).AsTask(Cts.Token);

			await gameManager.StartLoading(Cts.Token);
			var scene = SceneManager.GetSceneByName(SceneNames.MAIN_MENU);
			var root = scene.GetRootGameObjects();


			gameManager.OnSubmitLevelSet += SubmitLevelSet;

			await Task.Delay(500, Cts.Token);

			await gameManager.InMenu(Cts.Token);
		}

		public override async Task Activate()
		{
			DIContainer.GetDependency(out GameManager gameManager);
			gameManager.OnSubmitLevelSet -= SubmitLevelSet;
			await gameManager.StartLoading(Cts.Token);

			await SceneManager.LoadSceneAsync(SceneNames.MAIN_ENTRY, LoadSceneMode.Additive).AsTask(Cts.Token);
			await SceneManager.UnloadSceneAsync(SceneNames.MAIN_MENU).AsTask(Cts.Token);
			var scene = SceneManager.GetSceneByName(SceneNames.MAIN_ENTRY);
			var root = scene.GetRootGameObjects();

			MainGameEntry gameEntry = null;
			foreach (var child in root)
				child.TryGetComponent(out gameEntry);

			if (gameEntry == null)
				throw new EntryPointNotFoundException($"Entry point was not found in scene: {SceneNames.MAIN_ENTRY}");

			await gameEntry.Enter();
		}
	}
}
