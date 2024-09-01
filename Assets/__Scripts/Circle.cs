using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Circle : MonoBehaviour
{
    [SerializeField] float bounceAnimationDuration;
    [SerializeField] float bounceScaleValue;
    [SerializeField] float inputReadOffset;
    [SerializeField] Rigidbody2D rb;
    [SerializeField, Range(1, 15)] float speedMultiplier;
    [SerializeField] float minStretchDistance, maxStretchDistance;
    Vector2 initialPosition;
    bool canStretch;
    float speed;

    Vector2 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        GestureDetector.OnDragStart += StartBallStretch;
        GestureDetector.OnDragUpdate += UpdateBallPosition;
        GestureDetector.OnDragEnd += ReleaseBall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Wall wall))
        {
            OnBounce(collision.contacts[0].normal);
        }
    }

    private void OnDestroy()
    {
        GestureDetector.OnDragUpdate -= UpdateBallPosition;
        GestureDetector.OnDragEnd -= ReleaseBall;
    }

    void StartBallStretch(Vector2 startPos)
    {
        canStretch = true;
        if (startPos.y > inputReadOffset)
        {
            canStretch = false;
        }
    }

    void UpdateBallPosition(Vector2 direction)
    {
        if (!canStretch)
            return;

        float clamped = Mathf.Clamp(direction.magnitude + 1, minStretchDistance, maxStretchDistance);
        float lnMagnitude = Mathf.Log(clamped);
        var newPos = (initialPosition - direction.normalized * lnMagnitude);

        speed = lnMagnitude;
        transform.position = newPos;
    }

    void ReleaseBall(Vector2 direction)
    {
        if (!canStretch)
            return;

        UpdateBallPosition(direction);

        rb.velocity = direction.normalized * speed * speedMultiplier;
    }

    public void OnBounce(Vector2 normalVector)
    {
        var ang = Vector2.SignedAngle(normalVector, transform.up);
        transform.eulerAngles -= new Vector3(0, 0, ang);
        transform.localScale = initialScale;

        transform.DOScaleY(bounceScaleValue, bounceAnimationDuration).OnComplete(() => transform.DOScaleY(initialScale.y, bounceAnimationDuration));
    }

    private bool IsBounceUp(Vector2 vector)
    {
        if (Vector2.Dot(Vector2.up, vector) > 0.3f || Vector2.Dot(Vector2.down, vector) > 0.3f)
            return true;

        return false;
    }
}
