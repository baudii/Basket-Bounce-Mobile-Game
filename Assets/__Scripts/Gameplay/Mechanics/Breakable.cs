using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System.Threading.Tasks;

using BasketBounce.UI;
using BasketBounce.Gameplay.Visuals;
using BasketBounce.Systems;
using KK.Common;
using KK.Common.Gameplay;


namespace BasketBounce.Gameplay.Mechanics
{
	public class Breakable : Switcher, IResetableItem, ILevelValidator
	{
		[Header("Dependencies")]
		[SerializeField] SpriteRenderer sr;
		[SerializeField] Collider2D coll;
		[SerializeField] Rigidbody2D rb;
		[SerializeField] GFX_Shadow gfx_shadow;
		[Header("Fall")]
		[SerializeField, Min(1)] int hitsToFall = 1;
		[SerializeField] float fallDelay;
		[SerializeField] float gravityScale;
		[SerializeField] UnityEvent OnFall;
		[SerializeField] UnityEvent OnReset;
		[SerializeField] Color blinkColor;
		[SerializeField] Vector3 overrideTextPosition;
		[SerializeField] Vector3 localOffsetTextPosition;
		private static GameObject _textPrefab;
		private BreakableCounter_WorldUI _textObj;
		private async Task<GameObject> GetTextPrefabAsync()
		{
			if (_textPrefab == null)
			{
				var result = Addressables.LoadAssetAsync<GameObject>(key: "WT_Counter");
				await result.Task;
				_textPrefab = result.Result;
			}

			return _textPrefab;
		}

#if UNITY_EDITOR
		[SerializeField] bool defaultValues;
		private void OnValidate()
		{
			if (defaultValues)
			{
				foreach (Transform child in transform)
				{
					if (child.TryGetComponent(out GFX_Shadow shadow))
					{
						gfx_shadow = shadow;
						break;
					}
				}
				if (!TryGetComponent(out rb))
					rb = gameObject.AddComponent<Rigidbody2D>();
				if (!TryGetComponent(out coll))
					coll = gameObject.AddComponent<BoxCollider2D>();
				if (!TryGetComponent(out sr))
					sr = gameObject.AddComponent<SpriteRenderer>();

				rb.isKinematic = true;
				gravityScale = 6;

				blinkColor = new Color32(12 * 16 + 1, 5 * 16 + 13, 32, 255);
			}
		}

		private void OnDrawGizmos()
		{
			var position = transform.position + localOffsetTextPosition;
			position = GetOverridenPosition(position);

			Gizmos.DrawIcon(position, "breakable_icon.png");
		}
#endif
		const float blinkDuration = 1;

		Color initialColor;
		int currentHits;

		bool broken = false;

		Vector2 initialPos;

		private void Awake()
		{
			initialColor = sr.color;
			initialPos = transform.position;
		}

		private async void Start()
		{
			if (hitsToFall > 0)
			{
				var textPrefab = await GetTextPrefabAsync();
				var textObj = Instantiate(textPrefab);
				textObj.transform.rotation = Quaternion.identity;
				textObj.transform.SetParent(transform);
				textObj.transform.localPosition = localOffsetTextPosition;
				textObj.transform.position = GetOverridenPosition(transform.position);
				_textObj = textObj.GetComponent<BreakableCounter_WorldUI>();
			}
			UpdateBlinkColor();
		}

		private Vector3 GetOverridenPosition(Vector3 position)
		{
			if (overrideTextPosition.x != 0)
				position.x = overrideTextPosition.x;

			if (overrideTextPosition.y != 0)
				position.y = overrideTextPosition.y;

			if (overrideTextPosition.z != 0)
				position.z = overrideTextPosition.z;
			
			return position;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.transform.TryGetComponent(out Ball ball))
			{
				if (ball.CurrentState != Ball.BallState.Roaming)
					return;

				Activation();
			}
		}

		void UpdateBlinkColor()
		{
			_textObj?.SetText(Mathf.Max(hitsToFall - currentHits, 1).ToString());

			sr.DOKill();
			sr.color = blinkColor;
			sr.DOColor(initialColor, blinkDuration * .5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				sr.DOColor(blinkColor, blinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
			});
		}

		public override void Activation()
		{
			if (broken)
				return;

			currentHits++;
			UpdateBlinkColor();
			Deactivate();
			if (currentHits >= hitsToFall)
			{
				//Break();
				sr.DOKill();
				sr.color = initialColor;
				_textObj?.gameObject.SetActive(false);
				this.Co_DelayedExecute(Break, fallDelay);
				broken = true;
				OnFall?.Invoke();
			}
		}

		private void Break()
		{
			sr.DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, 0), 0.5f).SetEase(Ease.InSine);
			gfx_shadow.gameObject.SetActive(false);
			rb.isKinematic = false;
			rb.gravityScale = gravityScale;
			coll.enabled = false;
		}

		public void CompleteBreak()
		{
			gameObject.SetActive(false);
		}

		public void ResetState()
		{
			StopAllCoroutines();
			if (rb != null)
			{
				rb.isKinematic = true;
				rb.gravityScale = 0;
				rb.velocity *= 0;
				rb.angularVelocity *= 0;
				coll.enabled = true;
			}
			_textObj?.gameObject.SetActive(true);
			transform.position = initialPos;
			gfx_shadow.gameObject.SetActive(true);
			broken = false;
			currentHits = 0;

			Deactivate();

			currentHits = 0;
			UpdateBlinkColor();

			OnReset?.Invoke();
		}

		public void Validate()
		{
#if UNITY_EDITOR
			// C15D20
			blinkColor = new Color32(12 * 16 + 1, 5 * 16 + 13, 32, 255);
#endif
		}
	}
}