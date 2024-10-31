using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using BasketBounce.Systems.Interfaces;
using KK.Common;
using KK.Common.Gameplay;
using UnityEngine.AddressableAssets;
using BasketBounce.Gameplay.Visuals;
using System.Linq;

namespace BasketBounce.Gameplay.Mechanics
{
	public class DoorSwitcher : Switcher, IResetableItem
	{
		[SerializeField] DoorMovement[] doorMovements;

		[Header("Events")]
		[SerializeField] UnityEvent OnMoveUpdate;
		[SerializeField] UnityEvent OnResetState;
		[SerializeField] bool activateOnAwake;
		[SerializeField] bool canClose;
		[Header("Wire")]
		[SerializeField] bool disableWire;
		[SerializeField] Vector3 wireLocalOffset;
		Ball ball;
		Sequence sequence;

		Vector3 initialPosition, initialRotation;

		bool isOpened = false;

		GameObject _wirePrefab;
		const string _assetKey = "Door_Wire";

		private void Awake()
		{
			initialPosition = transform.position;
			initialRotation = transform.eulerAngles;
		}

		private async void Start()
		{
			if (disableWire ||
				!doorMovements.Any(movement => movement.MoveActions
								.Any(action => action.Type == DoorMoveAction.ActionType.Position)))
				return;

			if (_wirePrefab == null)
			{
				var res = Addressables.LoadAssetAsync<GameObject>(_assetKey);
				await res.Task;

				_wirePrefab = res.Result;
			}
			var wireGo = Instantiate(_wirePrefab, transform.parent);
			var wire = wireGo.GetComponent<WireBender>();
			var currentPos = transform.position + wireLocalOffset;
			wire.Init(currentPos);
			int i = 1;
			foreach (var movement in doorMovements)
			{
				var pos = movement.MoveActions.Where(item => item.Type == DoorMoveAction.ActionType.Position)
											  .Select(item => item.Target)
											  .FirstOrDefault();
				if (pos != Vector3.zero)
				{
					currentPos += pos;
					wire.AddIntermediate(currentPos);
				}
				i++;
			}
			wire.Finish();
			wire.Bend();
		}

		private void OnEnable()
		{
			if (activateOnAwake)
			{
				if (ball == null)
					ball = FindObjectOfType<Ball>();
				ball.OnBallReleased += Enable;
			}
		}

		private void OnDisable()
		{
			if (ball != null)
			{
				ball.OnBallReleased -= Enable;
			}
		}

		public override void Activation()
		{
			if (IsActivated && !isOpened)
			{
				SequantialDoorOpen();
				isOpened = true;
			}
			else if (canClose)
			{
				SequantialDoorClose();
				isOpened = false;
			}
		}

		void SequantialDoorOpen()
		{
			sequence?.Kill();
			sequence = DOTween.Sequence(transform);
			float currentTime = 0;
			Vector3 positionOffset = transform.localPosition;
			foreach (var doorMovement in doorMovements)
			{
				float duration = doorMovement.Duration;

				foreach (var action in doorMovement.MoveActions)
				{
					Tween tween = null;
					switch (action.Type)
					{
						case DoorMoveAction.ActionType.Rotation:
							tween = transform.DORotate(action.Target, duration, RotateMode.FastBeyond360).SetEase(action.Ease);
							break;
						case DoorMoveAction.ActionType.Position:
							positionOffset += action.Target;
							tween = transform.DOLocalMove(positionOffset, duration).SetEase(action.Ease);
							break;
					}
					sequence.Insert(currentTime, tween);
				}
				currentTime += duration;
			}
			sequence.Play().OnUpdate(() => OnMoveUpdate?.Invoke());
		}

		void SequantialDoorClose()
		{
			sequence?.Kill();
			sequence = DOTween.Sequence(transform);
			float currentTime = 0;
			for (int i = doorMovements.Length - 1; i >= 0; i--)
			{
				float duration = doorMovements[i].Duration;
				if (i == 0)
				{
					this.SmartLog(initialPosition, initialRotation);
					Tween tween = null;
					tween = transform.DOMove(initialPosition, duration);
					sequence.Insert(currentTime, tween);

					tween = transform.DORotate(initialRotation, duration, RotateMode.FastBeyond360);
					sequence.Insert(currentTime, tween);
					break;
				}
				foreach (var action in doorMovements[i - 1].MoveActions)
				{
					Tween tween = null;
					switch (action.Type)
					{
						case DoorMoveAction.ActionType.Rotation:
							tween = transform.DORotate(action.Target, duration, RotateMode.FastBeyond360).SetEase(action.Ease);
							break;
						case DoorMoveAction.ActionType.Position:
							tween = transform.DOMove(action.Target, duration).SetEase(action.Ease);
							break;
					}
					sequence.Insert(currentTime, tween);
				}
				currentTime += duration;
			}
			sequence.Play().OnUpdate(() => OnMoveUpdate?.Invoke());
		}

		public void ResetState()
		{
			sequence.Kill();
			transform.position = initialPosition;
			transform.eulerAngles = initialRotation;
			isOpened = false;
			Deactivate();
			OnResetState?.Invoke();
		}
	}

	[Serializable]
	public class DoorMovement
	{
		public float Duration;
		public DoorMoveAction[] MoveActions;
	}

	[Serializable]
	public class DoorMoveAction
	{
		public enum ActionType
		{
			Position, Rotation
		}

		public ActionType Type;
		public Vector3 Target;
		public Ease Ease;
	}
}