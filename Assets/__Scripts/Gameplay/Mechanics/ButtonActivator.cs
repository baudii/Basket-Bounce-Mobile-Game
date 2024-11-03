using UnityEngine;
using KK.Common;
using KK.Common.Gameplay;
using BasketBounce.Gameplay.Visuals;

namespace BasketBounce.Gameplay.Mechanics
{
	public class ButtonActivator : Activator
	{
		[SerializeField] SpriteRenderer sr;
		[SerializeField] Sprite pressed, idle;
		[SerializeField] LayerMask layerMask;
		[SerializeField] WireBender wirePrefab;
		[SerializeField] bool validate;

		int touches;
		bool isPressed;

		WireBender[] wires;


		private void Awake()
		{
			touches = 0;
			isPressed = false;
		}

		private void OnValidate()
		{
			if (!validate)
				return;

			if (wires == null)
			{
				wires = new WireBender[switchers.Count];
			}
		}

		private void UpdateState()
		{
			if (touches > 0 && !isPressed)
			{
				Toggle();
				if (sr != null)
					sr.sprite = pressed;
				isPressed = true;
			}
			else if (touches == 0 && isPressed)
			{
				if (sr != null)
					sr.sprite = idle;
				isPressed = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (layerMask.Contains(collision.gameObject.layer))
			{
				touches++;
				UpdateState();
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (layerMask.Contains(collision.gameObject.layer))
			{
				touches--;
				UpdateState();
			}
		}
	}
}