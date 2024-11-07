using KK.Common;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace BasketBounce.Systems
{
	public static class Bootstrap
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void AutostartGame()
		{
			GameManager gameManager = new GameManager();
			gameManager.Init();

			DIContainer.Init();
			DIContainer.Register(gameManager);

			SceneEntryPoint.Cts = new CancellationTokenSource();
			Application.quitting += RevokeToken;

			if (GestureDetector.DisableBootstrap)
				return;

			var menuEntry = new GameObject().AddComponent<MainMenuEntry>();
			_ = menuEntry.Enter();
		}

		private static void RevokeToken()
		{
			SceneEntryPoint.Cts.Cancel();
			SceneEntryPoint.Cts.Dispose();
		}
	}
}
