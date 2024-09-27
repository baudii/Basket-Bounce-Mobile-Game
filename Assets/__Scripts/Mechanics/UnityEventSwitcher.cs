using UnityEngine;
using UnityEngine.Events;

public class UnityEventSwitcher : Switcher
{
	[SerializeField] UnityEvent onActivate;
	[SerializeField] UnityEvent onDeactivate;
	public override void Activation()
	{
		if (IsActivated)
			onActivate?.Invoke();
		else
			onDeactivate?.Invoke();
	}
}
