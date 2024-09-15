using UnityEngine;
using DG.Tweening;
using System;

public class Ball : MonoBehaviour
{
    [Header("Roaming phase")]
    [SerializeField] float maxTimeOutOfScreen;
    [SerializeField] float maxStuckTime;
    [SerializeField] float bounceAnimationDuration;
    [SerializeField] float bounceScaleValue;
    [Header("Preparation phase")]
    [SerializeField] float minSpeed;
    [SerializeField] float minStretchDistance, maxStretchDistance;
    [SerializeField] float inputReadOffset;
    [SerializeField, Range(1, 15)] float speedMultiplier;
    [Header("Audio")]
    [SerializeField] AudioSource src;
    [SerializeField] AudioClip releaseSFX, bouncePadSFX, deathSFX;
    [Header("Dependancies")]
    [SerializeField] Animator animator;
    [SerializeField] Animator explodeAnimator;
    [SerializeField] Transform GFX;
    [SerializeField] LineController lineController;
    [SerializeField] Rigidbody2D rb;

    public State CurrentState { get; private set; }

    Vector2 initialPosition;
    float speed;

    Camera cam;
    Vector2 initialScale;


    public Action OnDeath;

    int bounces;

    float timeOutOfScreen;
    float stuckTime;
    float maxY;

    public enum State
    {
        Preparation,
        Stretching,
        Roaming,
        Finished,
        Dead
    }

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

        if (IsBallStuck())
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

    void SetState(State newState)
    {
        CurrentState = newState;
        this.SmartLog(newState);
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

    public bool BounceFromBouncePad(Vector2 direction, float force)
    {
        /*        var f = direction * force;
                var reflected = rb.velocity;
                var dot = Vector2.Dot(rb.velocity, direction);
                if (dot < 0)
                    reflected = Vector2.Reflect(rb.velocity, direction);
                this.SmartLog(f, direction, reflected, rb.velocity, dot);
                var newdir = (reflected.normalized + f).normalized;

                rb.velocity = newdir * speed * force * speedMultiplier;*/
        if (CurrentState != State.Roaming)
            return false;

        rb.velocity = direction * speed * speedMultiplier;
        src.PlayOneShot(bouncePadSFX, 0.4f);
        return true;
    }

    bool IsBallStuck()
    {
        if (CurrentState != State.Roaming)
            return false;

        if (transform.position.y > maxY)
        {
            maxY = transform.position.y + 0.1f;
            return false;
        }

        return true;
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
        SetState(State.Preparation);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        GFX.rotation = Quaternion.identity;
    }

    void StartBallStretch(Vector2 startPos)
    {
        if (CurrentState != State.Preparation || startPos.y > inputReadOffset)
            return;

        lineController.Init();
        SetState(State.Stretching);
    }

    void ResetBallSlow()
    {
        transform.DOMove(initialPosition, 0.15f);
        lineController.Disable();
        SetState(State.Preparation);
    }

    bool UpdateBallPosition(Vector2 direction)
    {
        if (CurrentState != State.Stretching)
            return false;

        var dot = Vector2.Dot(direction, Vector2.up);
        this.SmartLog("Dot:", dot);
        if (Vector2.Dot(direction, Vector2.up) < -1f)
        {
            ResetBallSlow();
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
        if (CurrentState != State.Stretching)
            return;

        lineController.Disable();

        if (speed < minSpeed)
        {
            ResetBallSlow();
            return;
        }


        rb.velocity = direction.normalized * speed * speedMultiplier;

        animator.SetBool("IsRotating", true);
        this.SmartLog("Current stetch:", Mathf.Exp(speed), "Max stretch: ", maxStretchDistance);

        // speed = ln(x)
        // x ∈ [minStretchDistance, maxStretchDistance]
        // animator.speed = x / maxStretchDistance => animator.speed ∈ [0,1]
        animator.speed = Mathf.Exp(speed) / maxStretchDistance;
        src.PlayOneShot(releaseSFX, 1.2f);

        SetState(State.Roaming);
    }


    public void Die(bool showDeathAnimation)
    {
        if (CurrentState != State.Roaming)
            return;
        /*        if (isDead || finished || !gameObject.activeSelf || !enabled)
                    return;*/

        CurrentState = State.Dead;
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
        SetState(State.Finished);
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
