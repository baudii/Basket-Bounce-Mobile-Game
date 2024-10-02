using UnityEngine;
using UnityEngine.Events;

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
		this.SmartLog("Reseting state");

		onResetState?.Invoke();
	}
}
