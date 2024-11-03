using BasketBounce.Systems;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelsEntry : BaseGameEntry
    {
		[SerializeField] LevelManager levelManagerPrefab;
		[SerializeField] GameObject BG;

		LevelManager levelManager;

		public override Task Setup(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			levelManager = Instantiate(levelManagerPrefab);

			Instantiate(BG);

			Register(levelManager);

			return Task.CompletedTask;
		}
		public override Task Activate(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			GetDependancy(out GameManager gameManager);
			levelManager.Init(gameManager);
			
			return Task.CompletedTask;
		}
	}
}
