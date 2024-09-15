using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class GestureDetector : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float threshHold;
    [SerializeField] private float minSwipeLength;
    [SerializeField] private float maxSwipeTime;
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

        GameManager.Instance.OnInGameStateEnter.AddListener(() => InputToggler(true));
        GameManager.Instance.OnInGameStateExit.AddListener(() => InputToggler(false));
    }

    void InputToggler(bool val)
    {
        if (val) input.Enable();
        else input.Disable();
    }

    private void Start()
    {
        InitDrag();
    }

    private void InitSwipes()
    {
        input.Gesture.FingerPressed.performed += ctx => StartCoroutine(SwipeCheck(GetTouchPosition()));
        input.Gesture.FingerPressed.canceled += ctx => StopAllCoroutines();
    }

    private void InitDrag()
    {
        input.Gesture.FingerPressed.started += ctx => StartCoroutine(DragUpdate());
        input.Gesture.FingerPressed.canceled += ctx =>
        {
            isDragging = false;
        };
    }

    private Vector2 GetTouchPosition()
    {
        Vector3 screenPos = input.Gesture.Position.ReadValue<Vector2>();
        
        return cam.ScreenToWorldPoint(screenPos.WhereZ(10));
    }

    IEnumerator SwipeCheck(Vector2 startPos)
    {
        var endPos = Vector2.zero;
        var distance = 0f;
        var time = 0f;

        while (distance < minSwipeLength)
        {
            endPos = GetTouchPosition();
            distance = Vector2.Distance(endPos, startPos);
            time += Time.deltaTime;

            if (time > maxSwipeTime) yield break;
            yield return null;
        }

        EndSwipe(startPos, endPos);
    }

    IEnumerator DragUpdate()
    {
        isDragging = true;
        yield return null;
        Vector2 startPos = GetTouchPosition();
        Vector2 direction = Vector2.zero;

        OnDragStart?.Invoke(startPos);

        bool isUpdateSuccessful = true;

        while (isDragging && isUpdateSuccessful)
        {
            direction = (startPos - GetTouchPosition());

            if (OnDragUpdate != null)
                isUpdateSuccessful = OnDragUpdate.Invoke(direction);

            yield return null;
        }

        if (isUpdateSuccessful)
            OnDragEnd?.Invoke(direction);
    }

    private void EndSwipe(Vector2 startPos, Vector2 endPos)
    {
        Vector2 swipe = (endPos - startPos).normalized;
        var direction = GetSwipeDirection(swipe);

        if (direction == Vector2.zero) return;

        OnSwipePerformed?.Invoke(direction);
    }

    private Vector2 GetSwipeDirection(Vector2 swipe)
    {
        if (Vector2.Dot(Vector2.up, swipe) > threshHold)
            return Vector2.up;

        if (Vector2.Dot(Vector2.right, swipe) > threshHold)
            return Vector2.right;

        if (Vector2.Dot(Vector2.left, swipe) > threshHold)
            return Vector2.left; 

        if (Vector2.Dot(Vector2.down, swipe) > threshHold)
            return Vector2.down;

        return Vector2.zero;
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            input.Enable();
        else
            input.Disable();
    }
}
