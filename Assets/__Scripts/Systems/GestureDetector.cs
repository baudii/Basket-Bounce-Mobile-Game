using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using KK.Common;

namespace BasketBounce.Systems
{
	public class GestureDetector : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] bool debugDragUpdate;
#endif
		private Camera cam;

		public static UnityAction<Vector2> OnSwipePerformed;

		public Action<Vector2> OnDragStart;
		public Action<Vector2> OnDragEnd;
		public Func<Vector2, bool> OnDragUpdate;

		private InputMaster input;

		private EventSystem eventSystem;
		private GameManager gameManager;

		bool isDragging;

		public void Init(GameManager gameManager, EventSystem eventSystem)
		{
			input = new InputMaster();

			this.eventSystem = eventSystem;
			this.gameManager = gameManager;
			this.Log(gameManager);
			gameManager.OnInGameEnterEvent.AddListener(EnableInput);
			gameManager.OnInGameExitEvent.AddListener(DisableInput);

			cam = Camera.main == null ? throw new Exception("Wrong operation order. Camera is null.") : Camera.main;

			InitDrag();
			input.Disable();
		}

		private void OnDestroy()
		{
			if (gameManager != null)
			{
				gameManager.OnInGameEnterEvent.RemoveListener(EnableInput);
				gameManager.OnInGameExitEvent.RemoveListener(DisableInput);
			}

			OnDragStart = null;
			OnDragUpdate = null;
			OnDragEnd = null;
		}

		private void InitDrag()
		{
			input.Gesture.FingerPressed.performed += ctx => StartCoroutine(DragUpdate());
			input.Gesture.FingerPressed.canceled += ctx =>
			{
				isDragging = false;
			};
		}

		private Vector2 GetTouchPosition()
		{
			Vector3 screenPos = input.Gesture.Position.ReadValue<Vector2>();
			if (screenPos.magnitude == Mathf.Infinity)
				return Vector2.zero;

			return cam.ScreenToWorldPoint(screenPos.WhereZ(cam.nearClipPlane));
		}


		IEnumerator DragUpdate()
		{
			isDragging = true;

			yield return null;

			Vector2 startPos = GetTouchPosition();

			if (eventSystem.currentSelectedGameObject != null)
			{
				this.Log("Touching UI element");
				yield break;
			}

			OnDragStart?.Invoke(startPos);

			Vector2 direction = Vector2.zero;

			bool isUpdateSuccessful = true;

			while (isDragging && isUpdateSuccessful)
			{

				direction = (startPos - GetTouchPosition());

#if UNITY_EDITOR
				if (debugDragUpdate)
				{
					this.Log("Inside drag update while loop");
					this.Log("Direction:", direction);
				}
#endif
				if (OnDragUpdate != null)
					isUpdateSuccessful = OnDragUpdate.Invoke(direction);

				yield return null;
			}

			if (isUpdateSuccessful)
				OnDragEnd?.Invoke(direction);

			this.Log("Exiting drag update coroutine");
		}

		public void EnableInput() => input.Enable();
		public void DisableInput() => input.Disable();

		public void SetActive(bool isActive)
		{
			this.Log("Setting input to: ", isActive);
			if (isActive)
				input.Enable();
			else
				input.Disable();
		}
	}
}