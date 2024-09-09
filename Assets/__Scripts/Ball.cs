using UnityEngine;
using DG.Tweening;
using System;

public class Ball : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float maxTimeOutOfScreen;
    [SerializeField] AudioClip releaseSFX;
    [SerializeField] AudioSource src;
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

    public Action OnDeath;

    int bounces;

    public bool finished;

    float timeOutOfScreen;

    void Start()
    {
        cam = Camera.main;
        initialScale = transform.localScale;
        initialPosition = transform.position;
        GestureDetector.OnDragStart += StartBallStretch;
        GestureDetector.OnDragUpdate += UpdateBallPosition;
        GestureDetector.OnDragEnd += ReleaseBall;

        GameManager.Instance.OnRestart.AddListener(ResetBall);
    }

    void Update()
    {
        if (!IsBallInScreen())
        {
            timeOutOfScreen += Time.deltaTime;
            if (timeOutOfScreen > maxTimeOutOfScreen)
            {
                Die();
            }
        }
        else
        {
            timeOutOfScreen = 0;
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
        GestureDetector.OnDragStart -= StartBallStretch;
        GestureDetector.OnDragUpdate -= UpdateBallPosition;
        GestureDetector.OnDragEnd -= ReleaseBall;

        GameManager.Instance.OnRestart.RemoveListener(ResetBall);
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
        //transform
        transform.position = initialPosition;
        ResetRotation();

        //dependancies
        rb.velocity *= 0;
        lineController.Disable();
        animator.SetBool("IsRotating", false);
        animator.speed = 1;

        //private variables
        timeOutOfScreen = 0;
        bounces = 0;
        isDead = false;
        isCircleRoaming = false;
        finished = false;
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        GFX.rotation = Quaternion.identity;
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

        animator.SetBool("IsRotating", true);
        this.SmartLog(Mathf.Exp(speed) + "stretch: " + maxStretchDistance);
        animator.speed = Mathf.Exp(speed) / maxStretchDistance;
        src.PlayOneShot(releaseSFX, 1.2f);
    }

    public void Die()
    {
        if (isDead || finished || !gameObject.activeSelf || !enabled)
            return;
        isDead = true;
        rb.velocity *= 0;
        GameManager.Instance.GameOver();
    }

    public int OnFinish()
    {
        finished = true;
        rb.velocity *= 0;
        return bounces;
    }

    public void OnBounce(Vector2 normalVector)
    {
        src.Play();
        var ang = Vector2.SignedAngle(normalVector, transform.up);
        var angleOffset = new Vector3(0, 0, ang);
        transform.eulerAngles -= angleOffset;
        GFX.eulerAngles += angleOffset;
        transform.localScale = initialScale;

        transform.DOScaleY(initialScale.y * Mathf.Pow(bounceScaleValue, speed), bounceAnimationDuration).OnComplete(() => transform.DOScaleY(initialScale.y, bounceAnimationDuration));
        bounces++;
    }
}
