using UnityEngine;
using UnityEngine.Events;
using BasketBounce.Systems;
using KK.Common;
using KK.Common.Gameplay;

namespace BasketBounce.Gameplay.Mechanics
{
	public class UnityEventSwitcher : Switcher, IResetableItem
	{
		[SerializeField] UnityEvent onActivate;
		[SerializeField] UnityEvent onDeactivate;
		[SerializeField] UnityEvent onResetState;
		public override void Activation()
		{
			if (IsActivated)
				onActivate?.Invoke();
			else
				onDeactivate?.Invoke();
		}

		public void ResetState()
		{
			this.Log("Reseting state");

			onResetState?.Invoke();
		}
	}
}
