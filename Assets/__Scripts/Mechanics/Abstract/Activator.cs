using System.Collections.Generic;
using UnityEngine;

public abstract class Activator : MonoBehaviour
{
	[SerializeField] List<Switcher> switchers;

	protected void ActivateAll(bool value)
	{
		foreach (var switcher in switchers)
		{
			switcher.SetActivation(value);
		}
	}

	protected void Toggle()
	{
		foreach (var switcher in switchers)
		{
			switcher.Toggle();
		}
	}
}
