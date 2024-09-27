using DG.Tweening;
using UnityEngine;

public class Breakable : MonoBehaviour, IResetableItem
{
	[Header("Dependencies")]
	[SerializeField] SpriteRenderer sr;
	[SerializeField] Collider2D coll;
	[SerializeField] Rigidbody2D rb;
	[Header("Fall")]
	[SerializeField] float fallDelay;
	[SerializeField] float gravityScale;

	const float blinkDuration = 0.5f;
	const byte r = 255, g = 107, b = 0, a = 255;
	Color blinkColor;


	bool broken = false;

	Vector2 initialPos;

	private void Awake()
	{
		initialPos = transform.position;
		Color initial = sr.color;
		blinkColor = new Color32(r, g, b ,a);
		sr.DOColor(blinkColor, blinkDuration).SetLoops(-1, LoopType.Yoyo);
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.TryGetComponent(out Ball ball) && !broken)
		{
			//Break();
			DOTween.Pause(sr);
			this.Co_DelayedExecute(Break, fallDelay);
			broken = true;
		}
	}

	private void Break()
	{
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
		DOTween.Restart(sr);
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.gravityScale = 0;
			rb.velocity *= 0;
			rb.angularVelocity *= 0;
			coll.enabled = true;
		}

		transform.position = initialPos;
		broken = false;
	}
}
