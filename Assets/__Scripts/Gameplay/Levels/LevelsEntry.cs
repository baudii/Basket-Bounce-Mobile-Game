using BasketBounce.Systems;
using System.Threading.Tasks;
using UnityEngine;
using KK.Common;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelsEntry : SceneEntryPoint
    {
		[SerializeField] LevelManager levelManagerPrefab;
		[SerializeField] GameObject BG;

		LevelManager levelManager;
		GameManager gameManager;

		public override Task Setup()
		{
			Cts.Token.ThrowIfCancellationRequested();

			levelManager = Instantiate(levelManagerPrefab);

			Instantiate(BG);

			DIContainer.GetDependency(out gameManager);

			DIContainer.Register(levelManager);

			return Task.CompletedTask;
		}
		public async override Task Activate()
		{
			Cts.Token.ThrowIfCancellationRequested();

			DIContainer.InjectIn(levelManager);

			var levelSet = gameManager.RecieveLevelSet();

			await levelManager.Init();
			await levelManager.ActivateLevelSet(levelSet);
		}
	}
}
