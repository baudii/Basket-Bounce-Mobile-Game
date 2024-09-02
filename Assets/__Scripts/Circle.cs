using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.VisionOS;
using System.Runtime.CompilerServices;

public class Circle : MonoBehaviour
{
    [SerializeField] Transform GFX;
    [SerializeField] LineController lineController;
    [SerializeField] float bounceAnimationDuration;
    [SerializeField] float bounceScaleValue;
    [SerializeField] float inputReadOffset;
    [SerializeField] Rigidbody2D rb;
    [SerializeField, Range(1, 15)] float speedMultiplier;
    [SerializeField] float minSpeed;
    [SerializeField] float minStretchDistance, maxStretchDistance;
    Vector2 initialPosition;
    bool startedStretch;
    bool isDead;
    float speed;

    Camera cam;
    Vector2 initialScale;

    bool isCircleRoaming;


    void Start()
    {
        cam = Camera.main;
        initialScale = transform.localScale;
        initialPosition = transform.position;
        GestureDetector.OnDragStart += StartBallStretch;
        GestureDetector.OnDragUpdate += UpdateBallPosition;
        GestureDetector.OnDragEnd += ReleaseBall;
    }

    void Update()
    {
        if (!IsBallInScreen())
        {
            Die();
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Wall wall))
        {
            OnBounce(collision.contacts[0].normal);
        }
    }

    void OnDestroy()
    {
        GestureDetector.OnDragUpdate -= UpdateBallPosition;
        GestureDetector.OnDragEnd -= ReleaseBall;
    }

    bool IsBallInScreen()
    {
        var screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
            return false;
        return true;
    }

    void ResetBall()
    {
        isDead = false;
        isCircleRoaming = false;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        GFX.rotation = Quaternion.identity;
        rb.velocity *= 0;
        lineController.Disable();
    }

    void StartBallStretch(Vector2 startPos)
    {
        if (isCircleRoaming)
        {
            startedStretch = false;
            return;
        }

        startedStretch = true;
        if (startPos.y > inputReadOffset)
        {
            startedStretch = false;
        }
        if (startedStretch)
        {
            lineController.Init();
        }
    }

    void UpdateBallPosition(Vector2 direction)
    {
        if (!startedStretch)
            return;

        float clamped = Mathf.Clamp(direction.magnitude+1, minStretchDistance, maxStretchDistance);
        speed = Mathf.Log(clamped);
        //speed = Mathf.Sqrt(clamped);
        var newPos = (initialPosition - direction.normalized * speed);

        transform.position = newPos;

        lineController.UpdateState(speed, direction.normalized);
    }

    void ReleaseBall(Vector2 direction)
    {
        if (!startedStretch)
            return;

        lineController.Disable();

        if (speed < minSpeed)
        {
            transform.DOMove(initialPosition, 0.15f);
            return;
        }

        rb.velocity = direction.normalized * speed * speedMultiplier;

        isCircleRoaming = true;
    }

    public void Die()
    {
        if (isDead)
            return;
        isDead = true;

        Invoke(nameof(ResetBall), 1f);
        //game end / restart logic
    }

    public void OnBounce(Vector2 normalVector)
    {
        var ang = Vector2.SignedAngle(normalVector, transform.up);
        var angleOffset = new Vector3(0, 0, ang);
        transform.eulerAngles -= angleOffset;
        GFX.eulerAngles += angleOffset;
        transform.localScale = initialScale;

        transform.DOScaleY(bounceScaleValue, bounceAnimationDuration).OnComplete(() => transform.DOScaleY(initialScale.y, bounceAnimationDuration));
    }
}
