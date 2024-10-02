﻿using UnityEngine;
using DG.Tweening;
using System;

public class Ball : MonoBehaviour
{
    [Header("Roaming phase")]
    [SerializeField] float maxTimeOutOfScreen;
    [SerializeField] float maxStuckTime;
    [SerializeField] float bounceAnimationDuration;
    [SerializeField] float bounceScaleValue;
    [SerializeField] LayerMask bounceLayerMask;
    [Header("Preparation phase")]
    [SerializeField] float minSpeed;
    [SerializeField] float minStretchDistance, maxStretchDistance;
    [SerializeField] float inputReadOffset;
    [SerializeField, Range(0,1), Tooltip("Will only be used on levels with \"Use Threshhold\" flag set to true!")] float threshHold;
    [SerializeField, Range(1, 15)] float speedMultiplier;
    [Header("Audio")]
    [SerializeField] AudioSource src;
    [SerializeField] AudioClip releaseSFX, bouncePadSFX, deathSFX;
    [Header("Inner dependencies")]
    [SerializeField] Animator animator;
    [SerializeField] Animator explodeAnimator;
    [SerializeField] Transform GFX;
    [SerializeField] LineController lineController;
    [SerializeField] Rigidbody2D rb;
    [Header("Outer dependencies")]
    [SerializeField] ReflectionLine reflectionLine;
    [SerializeField] UI_BounceCounter ui_bounceCounter;

    public enum BallState
    {
        Preparation,
        Stretching,
        Roaming,
        Finished,
        Dead
    }
    public BallState CurrentState { get; private set; }
    Camera cam;

    public Action OnBallReleased;

    bool useThreshhold;
    bool canNotStuck;

    Vector2 initialPosition;
    Vector2 initialScale;

    float speed;

    int bounces;

    // For time calcualtion in Update()
    float timeOutOfScreen;
    float maxStuckTimeCurrent;
    float currentStuckTime;
    float maxY;


    void Start()
    {
        maxStuckTimeCurrent = maxStuckTime;
		timeOutOfScreen = 0;
        currentStuckTime = 0;
        cam = Camera.main;
        initialScale = transform.localScale;
        initialPosition = transform.position;

        lineController.Init(minSpeed);


		GestureDetector.OnDragStart += StartBallStretch;
        GestureDetector.OnDragUpdate += UpdateBallPosition;
        GestureDetector.OnDragEnd += ReleaseBall;

        LevelManager.Instance.OnLevelSetup.AddListener(ResetBall);
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

        if (IsBallStuck() && !canNotStuck)
        {
            currentStuckTime += Time.deltaTime;
            if (currentStuckTime > maxStuckTimeCurrent && rb.velocity.y < 1)
            {
                GameManager.Instance.ShowStuckScreen();
                currentStuckTime = 0;
            }
        }
        else
        {
            currentStuckTime = 0;
        }
    }

    void SetState(BallState newState)
    {
        CurrentState = newState;
        this.SmartLog(newState);
    }


    void OnCollisionEnter2D(Collision2D collision)
	{
        this.SmartLog("Name:", collision.gameObject.name, "Layer num:", collision.gameObject.layer, "Layer contains in mask:", bounceLayerMask.Contains(collision.gameObject.layer));

        if (bounceLayerMask.Contains(collision.gameObject.layer))
        {
            OnBounce(collision.contacts[0].normal);
        }
	}

    void OnDestroy()
    {
        GestureDetector.OnDragStart -= StartBallStretch;
        GestureDetector.OnDragUpdate -= UpdateBallPosition;
        GestureDetector.OnDragEnd -= ReleaseBall;

        LevelManager.Instance.OnLevelSetup.RemoveListener(ResetBall);

        OnBallReleased = null;

	}

    public bool BounceFromBouncePad(Vector2 direction, Vector3 position)
    {
        /*        var f = direction * force;
                var reflected = rb.velocity;
                var dot = Vector2.Dot(rb.velocity, direction);
                if (dot < 0)
                    reflected = Vector2.Reflect(rb.velocity, direction);
                this.SmartLog(f, direction, reflected, rb.velocity, dot);
                var newdir = (reflected.normalized + f).normalized;

                rb.velocity = newdir * speed * force * speedMultiplier;*/
        if (CurrentState != BallState.Roaming)
            return false;

        bounces++;
		ui_bounceCounter.OnBounce(bounces);

		// вылет с центра transform.position = position + (Vector3)direction * 0.3f;
		rb.velocity = direction * speed * speedMultiplier;
        src.PlayOneShot(bouncePadSFX, 0.4f);
        return true;
    }

    bool IsBallStuck()
    {
        if (CurrentState != BallState.Roaming)
            return false;

        if (transform.position.y > maxY)
        {
            maxY = transform.position.y + 0.5f;
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

    void ResetBall(LevelData levelData)
    {
        //transform
        transform.position = initialPosition;
        ResetRotation();

        //dependancies
        GFX.gameObject.SetActive(true);
        lineController.gameObject.SetActive(false);
        explodeAnimator.gameObject.SetActive(false);
        reflectionLine.gameObject.SetActive(false);
        rb.velocity *= 0;
        animator.SetBool("IsRotating", false);
        animator.speed = 1;
        ui_bounceCounter.ResetState();
        useThreshhold = levelData.UseThreshold;
		canNotStuck = levelData.CanNotStuck;
        maxStuckTimeCurrent = levelData.OverrideStuckTime <= 0 ? maxStuckTime : levelData.OverrideStuckTime;

        //private variables
        maxY = float.MinValue;
        currentStuckTime = 0;
        timeOutOfScreen = 0;
        bounces = 0;
        SetState(BallState.Preparation);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        GFX.rotation = Quaternion.identity;
    }

    void ResetBallSlow()
    {
        transform.DOMove(initialPosition, 0.15f);
        lineController.gameObject.SetActive(false);
        reflectionLine.gameObject.SetActive(false);
        SetState(BallState.Preparation);
    }

    void StartBallStretch(Vector2 startPos)
    {
        if (CurrentState != BallState.Preparation || startPos.y > inputReadOffset)
            return;

        lineController.gameObject.SetActive(true);

        if (LevelManager.Instance.isReflectionMode)
            reflectionLine.gameObject.SetActive(true);
        

        SetState(BallState.Stretching);
    }
    
    bool UpdateBallPosition(Vector2 direction)
    {
        if (CurrentState != BallState.Stretching)
            return false;

        var dot = Vector2.Dot(direction, Vector2.up);
        if (dot < -0.5f)
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
        float hasSpeed = speed >= minSpeed ? 1 : 0;
        if (reflectionLine.gameObject.activeSelf)
            reflectionLine.UpdateReflections(initialPosition, direction.normalized, clamped * hasSpeed);

        return true;
    }

    void ReleaseBall(Vector2 direction)
    {
        if (CurrentState != BallState.Stretching)
            return;

        lineController.gameObject.SetActive(false);
        reflectionLine.gameObject.SetActive(false);

        if (speed < minSpeed)
        {
            ResetBallSlow();
            return;
        }

        LevelManager.Instance.OnBallReleased();

        OnBallReleased?.Invoke();

        if (useThreshhold)
        {
            var dir = Utils.GetClosestDirection(direction, threshHold);
			if (dir != Vector2.zero)
            {
                direction = dir;

			}
        }

        rb.velocity = direction.normalized * speed * speedMultiplier;

        animator.SetBool("IsRotating", true);
        this.SmartLog("Current stetch:", Mathf.Exp(speed), "Max stretch: ", maxStretchDistance);

        // speed = ln(x)
        // x ∈ [minStretchDistance, maxStretchDistance]
        // animator.speed = x / maxStretchDistance => animator.speed ∈ [0,1]
        animator.speed = Mathf.Exp(speed) / maxStretchDistance;
        src.PlayOneShot(releaseSFX, 1.2f);

        SetState(BallState.Roaming);
    }


    public void Die(bool showDeathAnimation)
    {
        if (CurrentState != BallState.Roaming)
            return;
        /*        if (isDead || finished || !gameObject.activeSelf || !enabled)
                    return;*/

        CurrentState = BallState.Dead;
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
        SetState(BallState.Finished);
        rb.velocity *= 0;
                
        return bounces;
    }
    float lastBounceTime;
    public void OnBounce(Vector2 normalVector)
    {
        if (Time.time - lastBounceTime < 0.1f || CurrentState != BallState.Roaming)
            return;
        lastBounceTime = Time.time;

        src.Play();
        var ang = Vector2.SignedAngle(normalVector, transform.up);
        var angleOffset = new Vector3(0, 0, ang);
        transform.eulerAngles -= angleOffset;
        GFX.eulerAngles += angleOffset;
        transform.localScale = initialScale;


		transform.DOScaleY(initialScale.y * Mathf.Pow(bounceScaleValue, speed), bounceAnimationDuration).OnComplete(() => transform.DOScaleY(initialScale.y, bounceAnimationDuration));
        bounces++;
        ui_bounceCounter.OnBounce(bounces);
    }
}