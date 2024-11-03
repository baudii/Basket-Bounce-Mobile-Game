using BasketBounce.Gameplay.Visuals;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Threading;
using KK.Common;

namespace BasketBounce.Gameplay
{
    public class GameplayEntry : BaseGameEntry
    {
		[SerializeField] Ball ballPrefab;
		[SerializeField] GameObject camPrefab;
		[SerializeField] EventSystem eventSystemPrefab;
		[SerializeField] ReflectionLine reflectionLinePrefab;
		[SerializeField] GameObject musicPrefab;
		[SerializeField] GestureDetector gestureDetectorPrefab;

		GestureDetector gestureDetector;
		EventSystem eventSystem;
		ReflectionLine reflectionLine;
		FinishIconHelper finishIconHelper;
		DollyCameraController dolly;
		Ball ball;

		public override Task Setup(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			eventSystem = Instantiate(eventSystemPrefab, null);

			var camGo = Instantiate(camPrefab, null);

			gestureDetector = Instantiate(gestureDetectorPrefab, null);

			dolly = camGo.GetComponentInChildren<DollyCameraController>();
			finishIconHelper = camGo.GetComponentInChildren<FinishIconHelper>();

			ball = Instantiate(ballPrefab);
			reflectionLine = Instantiate(reflectionLinePrefab);

			Register(ball);
			Register(dolly);
			Register(gestureDetector);

			return Task.CompletedTask;
		}

		public override async Task Activate(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			GetDependancy(out GameManager gameManager);
			GetDependancy(out LevelManager levelManager);

			await InstantiateAsync(musicPrefab).GetOperation().AsTask(token);
			gestureDetector.Init(gameManager, eventSystem);
			ball.Init(gameManager, levelManager, gestureDetector, reflectionLine);
			dolly.Init(levelManager, ball.transform);
			finishIconHelper.Init(levelManager);
		}
	}
}
