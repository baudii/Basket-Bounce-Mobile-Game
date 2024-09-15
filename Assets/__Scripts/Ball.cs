using UnityEngine;
using DG.Tweening;
using System;
using UnityEditorInternal;

public class Ball : MonoBehaviour
{
    [SerializeField] float stuckSpeed;
    [SerializeField] Animator animator;
    [SerializeField] float maxTimeOutOfScreen;
    [SerializeField] float maxStuckTime;
    [SerializeField] Transform GFX;
    [SerializeField] Animator explodeAnimator;
    [SerializeField] LineController lineController;
    [SerializeField] float bounceAnimationDuration;
    [SerializeField] float bounceScaleValue;
    [SerializeField] float inputReadOffset;
    [SerializeField] Rigidbody2D rb;
    [SerializeField, Range(1, 15)] float speedMultiplier;
    [SerializeField] float minSpeed;
    [SerializeField] float minStretchDistance, maxStretchDistance;
    [Header("Audio")]
    [SerializeField] AudioSource src;
    [SerializeField] AudioClip releaseSFX, bouncePadSFX, deathSFX;

    Vector2 initialPosition;
    bool startedStretch;
    bool isDead;
    float speed;

    Camera cam;
    Vector2 initialScale;

    bool isCircleRoaming;

    public Action OnDeath;

    int bounces;

    [HideInInspector]
    public bool finished;

    float timeOutOfScreen;
    float stuckTime;
    float maxY;

    void Start()
    {
        timeOutOfScreen = 0;
        stuckTime = 0;
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
                Die(false);
                timeOutOfScreen = 0;
            }
        }
        else
        {
            timeOutOfScreen = 0;
        }

        if (IsBallStuck() && isCircleRoaming)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > maxStuckTime)
            {
                GameManager.Instance.ShowStuckScreen();
                stuckTime = 0;
            }
        }
        else
        {
            stuckTime = 0;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Wall wall))
        {
            this.SmartLog("Contact point:", collision.contacts[0].point.y, "maxY:", maxY);
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

    public void BounceFromBouncePad(Vector2 direction, float force)
    {
        var f = direction * force;
        var reflected = rb.velocity;
        var dot = Vector2.Dot(rb.velocity, direction);
        if (dot < 0)
            reflected = Vector2.Reflect(rb.velocity, direction);
        this.SmartLog(f, direction, reflected, rb.velocity, dot);
        var newdir = (reflected.normalized + f).normalized;

        rb.velocity = newdir * speed * force * speedMultiplier;
        src.PlayOneShot(bouncePadSFX, 0.4f);
    }

    bool IsBallStuck()
    {
        if (transform.position.y <= maxY)
            return true;

        maxY = transform.position.y + 0.1f;
        return false;
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
        GFX.gameObject.SetActive(true);
        rb.velocity *= 0;
        lineController.Disable();
        explodeAnimator.gameObject.SetActive(false);
        animator.SetBool("IsRotating", false);
        animator.speed = 1;

        //private variables
        maxY = float.MinValue;
        stuckTime = 0;
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

    bool UpdateBallPosition(Vector2 direction)
    {
        if (!startedStretch)
            return false;

        if (direction.y < 0)
        {
            transform.DOMove(initialPosition, 0.15f);
            lineController.Disable();
            return false;
        }

        float clamped = Mathf.Clamp(direction.magnitude + 1, minStretchDistance, maxStretchDistance);
        // speed = lnx, where x: 1 <= x <= 4, so 0 <= speed <= ln4
        speed = Mathf.Log(clamped); 
        //speed = Mathf.Sqrt(clamped);
        var newPos = (initialPosition - direction.normalized * speed);

        transform.position = newPos;

        lineController.UpdateState(speed, direction.normalized);

        return true;
    }

    void ReleaseBall(Vector2 direction)
    {
        if (!startedStretch)
            return;

        lineController.Disable();

        if (speed < minSpeed)
        {
            transform.DOMove(initialPosition, 0.15f);
            lineController.Disable();
            return;
        }


        rb.velocity = direction.normalized * speed * speedMultiplier;

        isCircleRoaming = true;

        animator.SetBool("IsRotating", true);
        this.SmartLog("Current stetch:", Mathf.Exp(speed), "Max stretch: ", maxStretchDistance);

        // speed = ln(x)
        // x ∈ [minStretchDistance, maxStretchDistance]
        // animator.speed = x / maxStretchDistance => animator.speed ∈ [0,1]
        animator.speed = Mathf.Exp(speed) / maxStretchDistance;
        this.SmartLog(Mathf.Exp(speed) / maxStretchDistance);
        src.PlayOneShot(releaseSFX, 1.2f);
    }


    public void Die(bool showDeathAnimation)
    {
        if (isDead || finished || !gameObject.activeSelf || !enabled)
            return;

        isDead = true;
        rb.velocity *= 0;

        if (showDeathAnimation)
        {
            src.PlayOneShot(deathSFX, 0.2f);
            GFX.gameObject.SetActive(false);
            explodeAnimator.gameObject.SetActive(true);
            this.Co_DelayedExecute(GameManager.Instance.GameOver, () => explodeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Finished"));
        }
        else
        {
            GameManager.Instance.GameOver();
        }
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
