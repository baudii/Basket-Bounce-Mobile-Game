using DG.Tweening;
using UnityEngine;

public class Breakable : MonoBehaviour, IResetableItem
{
	[Header("Blink")]
	[SerializeField] SpriteRenderer sr;
	[SerializeField] Color blinkColor;
	[SerializeField] float blinkDuration;
	[SerializeField] BreakType breakType;
	[SerializeField] Collider2D coll;
	[Header("Fall")]
	[SerializeField] float fallDelay;
	[SerializeField] float gravityScale;
	[SerializeField] Rigidbody2D rb;
	[Header("Dissapear")]
	[SerializeField] Animator animator;

	const string ANIMATOR_TRIGGER_NAME = "Dissapear";

	Sequence blinkSequence;

	bool broken = false;

	Vector2 initialPos;

	enum BreakType
	{
		Fall,
		Dissapear
	}
	private void Awake()
	{
		initialPos = transform.position;
		Color initial = sr.color;
		blinkSequence = DOTween.Sequence();

		blinkSequence.Append(sr.DOColor(blinkColor, blinkDuration)).Append(sr.DOColor(initial, blinkDuration));
		blinkSequence.Play().SetLoops(-1);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.TryGetComponent(out Ball ball) && !broken)
		{
			//Break();
			blinkSequence.Pause();
			this.Co_DelayedExecute(Break, fallDelay);
			broken = true;
		}
	}

	private void Break()
	{
		switch (breakType)
		{
			case BreakType.Fall:

				rb.isKinematic = false;
				rb.gravityScale = gravityScale;
				coll.enabled = false;
				break;
			case BreakType.Dissapear:
				animator.SetTrigger(ANIMATOR_TRIGGER_NAME);
				break;
		}
	}

	public void CompleteBreak()
	{
		gameObject.SetActive(false);
	}

	public void ResetState()
	{
		this.SmartLog("Hre");
		blinkSequence.Restart();
		if (rb != null)
		{
			this.SmartLog("Hre");
			rb.isKinematic = true;
			rb.gravityScale = 0;
			rb.velocity *= 0;
			rb.angularVelocity *= 0;
			coll.enabled = true;
		}
		if (animator != null)
			animator.ResetTrigger(ANIMATOR_TRIGGER_NAME);

		transform.position = initialPos;
		broken = false;
	}
}
