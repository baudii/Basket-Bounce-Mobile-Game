using BasketBounce.Systems;
using System.Threading.Tasks;
using UnityEngine;
using KK.Common;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelsEntry : SceneEntryPoint
    {
		[SerializeField] LevelManager levelManager;

		GameManager gameManager;

		public override Task Setup()
		{
			Cts.Token.ThrowIfCancellationRequested();

			DIContainer.GetDependency(out gameManager);

			DIContainer.Register(levelManager);

			return Task.CompletedTask;
		}
		public async override Task Activate()
		{
			Cts.Token.ThrowIfCancellationRequested();
			await levelManager.Init(gameManager);

			var level = gameManager.GetLevel();

			await levelManager.ActivateLevelSet(level);
		}
	}
}
