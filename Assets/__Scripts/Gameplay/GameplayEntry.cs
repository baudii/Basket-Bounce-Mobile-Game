using BasketBounce.Gameplay.Visuals;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using KK.Common;
using System;

namespace BasketBounce.Gameplay
{
    public class GameplayEntry : SceneEntryPoint
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

		public override Task Setup()
		{
			Cts.Token.ThrowIfCancellationRequested();

			eventSystem = Instantiate(eventSystemPrefab, null);

			var camGo = Instantiate(camPrefab, null);

			gestureDetector = Instantiate(gestureDetectorPrefab, null);

			dolly = camGo.GetComponentInChildren<DollyCameraController>();
			finishIconHelper = camGo.GetComponentInChildren<FinishIconHelper>();

			ball = Instantiate(ballPrefab);
			reflectionLine = Instantiate(reflectionLinePrefab);

			DIContainer.Register(ball);
			DIContainer.Register(dolly);
			DIContainer.Register(gestureDetector);

			return Task.CompletedTask;
		}

		public override async Task Activate()
		{
			Cts.Token.ThrowIfCancellationRequested();

			DIContainer.GetDependency(out GameManager gameManager);
			DIContainer.GetDependency(out LevelManager levelManager);

			await InstantiateAsync(musicPrefab).GetOperation().AsTask(Cts.Token);
			gestureDetector.Init(gameManager, eventSystem);
			ball.Init(gameManager, levelManager, gestureDetector, reflectionLine);
			dolly.Init(levelManager, ball.transform);
			finishIconHelper.Init(levelManager);
		}
	}
}
