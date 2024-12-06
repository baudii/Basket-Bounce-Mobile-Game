#if UNITY_ANDROID || UNITY_IOS
using CandyCoded.HapticFeedback;
#endif
using DG.Tweening;
using System;
using UnityEngine;
using BasketBounce.Systems;
using BasketBounce.Gameplay.Visuals;
using BasketBounce.Gameplay.Levels;
using KK.Common;
using UnityEngine.Events;


namespace BasketBounce.Gameplay
{
	public class Ball : MonoBehaviour
	{
		[Header("Roaming phase")]
		[SerializeField] float maxTimeOutOfScreen;
		[SerializeField] float maxStuckTime;
		[SerializeField] float bounceAnimationDuration;
		[SerializeField] float bounceScaleValue;
		[SerializeField] float maxVelocityMagnitude;
		[SerializeField] LayerMask bounceLayerMask;
		[Header("Preparation phase")]
		[SerializeField] float minSpeed;
		[SerializeField] float minStretchDistance, maxStretchDistance;
		[SerializeField, Range(0, 1), Tooltip("Feedback every (x * 100)%. Example: if 0.25 is set, then feedback will be given every 25% of maximum stretch distance")] float feedbackPersentage;
		[SerializeField, Range(0, 1), Tooltip("Will only be used on levels with \"Use Threshhold\" flag set to true!")] float threshHold;
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
		Camera Cam
		{
			get
			{
				if (cam == null)
				{
					cam = Camera.main;
				}
				return cam;
			}
		}

		public Action OnBallReleased;
		public Action OnBallStartStretch;
		public Action OnBallAbortStretch;
		public Action OnBallDie;
		public Action OnResetState;
		public Action<int> OnBallBounce;

		public UnityAction OnStuck;

		bool useThreshhold;
		bool canNotStuck;

		Vector2 initialPosition;
		Vector2 initialScale;

		float speed;

		int bounces;
		int currentStretchPower = 1;

		// For time calcualtion in Update()
		float timeOutOfScreen;
		float maxStuckTimeCurrent;
		float currentStuckTime;
		float maxY;

		// Outer dependancies
		LevelManager levelManager;
		GameManager gameManager;
		GestureDetector gestureDetector;
		ReflectionLine reflectionLine;

		bool isInitialized;
		public void Init(GameManager gameManager, LevelManager levelManager, GestureDetector gestureDetector, ReflectionLine reflectionLine)
		{
			maxStuckTimeCurrent = maxStuckTime;
			timeOutOfScreen = 0;
			currentStuckTime = 0;
			initialScale = transform.localScale;
			initialPosition = transform.position;

			lineController.Init(minSpeed);


			gestureDetector.OnDragStart += StartBallStretch;
			gestureDetector.OnDragUpdate += UpdateBallPosition;
			gestureDetector.OnDragEnd += ReleaseBall;

			this.levelManager = levelManager;
			this.gameManager = gameManager;
			this.gestureDetector = gestureDetector;
			this.reflectionLine = reflectionLine;

			levelManager.OnLevelSetupEvent.AddListener(ResetBall);
			isInitialized = true;
		}


		void Update()
		{
			if (!isInitialized)
				return;

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
					// SHOW STACK SCREEN
					OnStuck?.Invoke();
					currentStuckTime = 0;
				}
			}
			else
			{
				currentStuckTime = 0;
			}
		}

		private void FixedUpdate()
		{
			rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocityMagnitude);
		}

		void SetState(BallState newState)
		{
			this.Log("Switching state:", CurrentState, "->", newState);
			CurrentState = newState;
		}


		void OnCollisionEnter2D(Collision2D collision)
		{
			// this.SmartLog("Name:", collision.gameObject.name, "Layer num:", collision.gameObject.layer, "Layer contains in mask:", bounceLayerMask.Contains(collision.gameObject.layer));

			if (bounceLayerMask.Contains(collision.gameObject.layer))
			{
				OnBounce(collision.contacts[0].normal);
			}
		}

		void OnDestroy()
		{
			if (gestureDetector != null)
			{
				gestureDetector.OnDragStart -= StartBallStretch;
				gestureDetector.OnDragUpdate -= UpdateBallPosition;
				gestureDetector.OnDragEnd -= ReleaseBall;
			}

			if (levelManager != null)
			{
				levelManager.OnLevelSetupEvent.RemoveListener(ResetBall);
			}

			OnBallReleased = null;
			OnBallStartStretch = null;
			OnBallAbortStretch = null;
			OnBallBounce = null;
			OnResetState = null;
		}

		public bool BounceFromBouncePad(Vector2 direction, Vector3 position)
		{
			if (CurrentState != BallState.Roaming)
				return false;

			bounces++;
			OnBallBounce?.Invoke(bounces);
#if UNITY_ANDROID || UNITY_IOS
			HapticFeedback.LightFeedback();
#endif

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
			var screenPos = Cam.WorldToScreenPoint(transform.position);
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
			OnResetState?.Invoke();
			useThreshhold = levelData.UseThreshold;
			canNotStuck = levelData.CanNotStuck;
			maxStuckTimeCurrent = levelData.OverrideStuckTime <= 0 ? maxStuckTime : levelData.OverrideStuckTime;

			//private variables
			speed = 0;
			maxY = float.MinValue;
			currentStuckTime = 0;
			timeOutOfScreen = 0;
			bounces = 0;
			currentStretchPower = 1;


			SetState(BallState.Preparation);

		}

		public void ResetRotation()
		{
			transform.rotation = Quaternion.identity;
			GFX.rotation = Quaternion.identity;
		}

		void AbortStretch()
		{
			transform.DOMove(initialPosition, 0.15f);
			lineController.gameObject.SetActive(false);
			reflectionLine.gameObject.SetActive(false);
			currentStretchPower = 1;
			OnBallAbortStretch?.Invoke();
			SetState(BallState.Preparation);
		}

		void StartBallStretch(Vector2 startPos)
		{
			if (CurrentState != BallState.Preparation)
				return;

			lineController.gameObject.SetActive(true);

			if (levelManager.CurrentLevelData.isReflectionMode)
				reflectionLine.gameObject.SetActive(true);

			OnBallStartStretch?.Invoke();

			SetState(BallState.Stretching);
		}

		bool UpdateBallPosition(Vector2 direction)
		{
			if (CurrentState != BallState.Stretching)
				return false;

			var dot = Vector2.Dot(direction, Vector2.up);
			if (dot < -0.5f)
			{
				AbortStretch();
				return false;
			}
			float clamped = Mathf.Clamp(direction.magnitude, minStretchDistance, maxStretchDistance);

			//Stretch feedback
			if (clamped >= maxStretchDistance * feedbackPersentage * currentStretchPower)
			{
#if UNITY_ANDROID || UNITY_IOS
				HapticFeedback.LightFeedback();
#endif
				currentStretchPower++;
			}
			// 0 <= clamped <= 3
			// 0 <= speed <= ln4
			speed = Mathf.Log(clamped + 1);
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
				AbortStretch();
				return;
			}
#if UNITY_ANDROID || UNITY_IOS
			HapticFeedback.LightFeedback();
#endif

			levelManager.OnBallReleased();

			OnBallReleased?.Invoke();

			if (useThreshhold)
			{
				var dir = Utils.GetClosestDirection2D(direction, threshHold);
				if (dir != Vector2.zero)
				{
					direction = dir;

				}
			}

			rb.velocity = direction.normalized * speed * speedMultiplier;

			animator.SetBool("IsRotating", true);

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

			CurrentState = BallState.Dead;
			rb.velocity *= 0;

			if (showDeathAnimation)
			{
				src.PlayOneShot(deathSFX, 0.2f);
				GFX.gameObject.SetActive(false);
				explodeAnimator.gameObject.SetActive(true);
#if UNITY_ANDROID || UNITY_IOS
				HapticFeedback.LightFeedback();
#endif

				this.Co_DelayedExecute(gameManager.GameOver, () => explodeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Finished"));
			}
			else
			{
				gameManager.GameOver();
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
#if UNITY_ANDROID || UNITY_IOS
			HapticFeedback.LightFeedback();
#endif
			src.Play();
			var ang = Vector2.SignedAngle(normalVector, transform.up);
			var angleOffset = new Vector3(0, 0, ang);
			transform.eulerAngles -= angleOffset;
			GFX.eulerAngles += angleOffset;
			transform.localScale = initialScale;


			transform.DOScaleY(initialScale.y * Mathf.Pow(bounceScaleValue, speed), bounceAnimationDuration).OnComplete(() => transform.DOScaleY(initialScale.y, bounceAnimationDuration));
			bounces++;
			OnBallBounce?.Invoke(bounces);
			if (bounces > 100)
			{
				Die(false);
			}
		}
	}
}