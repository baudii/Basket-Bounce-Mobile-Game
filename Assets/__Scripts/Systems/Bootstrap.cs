using KK.Common;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace BasketBounce.Systems
{
	public static class Bootstrap
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static async void AutostartGame()
		{
			GameManager gameManager = new GameManager();
			gameManager.Init();

			DIContainer.Init();
			DIContainer.Register(gameManager);

			SceneEntryPoint.Cts = new CancellationTokenSource();
			Application.quitting += RevokeToken;


			var operation = Addressables.LoadAssetAsync<GameSettings>("GameSettingsSO");
			await operation.Task;
			var gameSettings = operation.Result;

			if (gameSettings.AutoStartEnabled)
			{
				var menuEntry = new GameObject().AddComponent<MainMenuEntry>();
				await menuEntry.Enter();
			}
		}

		private static void RevokeToken()
		{
			SceneEntryPoint.Cts.Cancel();
			SceneEntryPoint.Cts.Dispose();
		}
	}
}
