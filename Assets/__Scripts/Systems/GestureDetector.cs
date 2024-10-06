using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GestureDetector : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] bool checkDragCoroutine;	
#endif
	[SerializeField] private Camera cam;

	public static UnityAction<Vector2> OnSwipePerformed;

	public static UnityAction<Vector2> OnDragStart;
	public static Func<Vector2, bool> OnDragUpdate;
	public static UnityAction<Vector2> OnDragEnd;

	private InputMaster input;

	bool isDragging;

	private void Awake()
	{
		input = new InputMaster();
		input.Enable();

		GameManager.Instance.OnInGameStateEnter.AddListener(() => input.Enable());
		GameManager.Instance.OnInGameStateExit.AddListener(() => input.Disable());
	}

	private void Start()
	{
		if (cam == null)
			cam = Camera.main;
		InitDrag();
	}

	private void OnDestroy()
	{
		OnDragStart = null;
		OnDragUpdate = null;
		OnDragEnd = null;
	}

	private void InitDrag()
	{
		this.SmartLog("Initializing drag input");
		input.Gesture.FingerPressed.performed += ctx => StartCoroutine(DragUpdate());
		input.Gesture.FingerPressed.canceled += ctx =>
		{
			isDragging = false;
		};
	}

	private Vector2 GetTouchPosition()
	{
		Vector3 screenPos = input.Gesture.Position.ReadValue<Vector2>();

		return cam.ScreenToWorldPoint(screenPos.WhereZ(cam.nearClipPlane));
	}


	IEnumerator DragUpdate()
	{
		this.SmartLog("Started drag update coroutine");
		LevelManager.Instance.OnClickAnywhere();
		isDragging = true;

		yield return null;

		Vector2 startPos = GetTouchPosition();

		if (EventSystem.current.currentSelectedGameObject != null)
		{
			this.SmartLog("Touching UI element");
			yield break;
		}

		OnDragStart?.Invoke(startPos);

		Vector2 direction = Vector2.zero;

		bool isUpdateSuccessful = true;

		while (isDragging && isUpdateSuccessful)
		{

			direction = (startPos - GetTouchPosition());

#if UNITY_EDITOR
			if (checkDragCoroutine)
			{
				this.SmartLog("Inside drag update while loop");
				this.SmartLog("Direction:", direction);
			}
#endif
			if (OnDragUpdate != null)
				isUpdateSuccessful = OnDragUpdate.Invoke(direction);

			yield return null;
		}

		if (isUpdateSuccessful)
			OnDragEnd?.Invoke(direction);

		this.SmartLog("Exiting drag update coroutine");
	}



	public void SetActive(bool isActive)
	{
		this.SmartLog("Setting input to: ", isActive);
		if (isActive)
			input.Enable();
		else
			input.Disable();
	}
}
