using BasketBounce.Gameplay.Visuals;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using KK.Common;

namespace BasketBounce.Gameplay
{
    public class GameplayEntry : SceneEntryPoint
    {
		[SerializeField] Ball ball;
		[SerializeField] DollyCameraController dolly;
		[SerializeField] FinishIconHelper finishIconHelper;
		[SerializeField] ReflectionLine reflectionLine;
		[SerializeField] GestureDetector gestureDetector;
		[SerializeField] EventSystem eventSystem;

		public override Task Setup()
		{
			Cts.Token.ThrowIfCancellationRequested();

			DIContainer.Register(ball);
			DIContainer.Register(dolly);
			DIContainer.Register(gestureDetector);

			return Task.CompletedTask;
		}

		public override Task Activate()
		{
			Cts.Token.ThrowIfCancellationRequested();

			DIContainer.GetDependency(out GameManager gameManager);
			DIContainer.GetDependency(out LevelManager levelManager);

			gestureDetector.Init(gameManager, eventSystem);
			ball.Init(gameManager, levelManager, gestureDetector, reflectionLine);
			dolly.Init(levelManager, ball.transform);
			finishIconHelper.Init(levelManager);

			return Task.CompletedTask;
		}
	}
}
