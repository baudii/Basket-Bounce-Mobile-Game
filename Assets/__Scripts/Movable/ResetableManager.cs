using System.Collections.Generic;
using UnityEngine;

public class ResetableManager : MonoBehaviour
{
	List<IResetableItem> resetables;

	private void Awake()
	{
		resetables = new List<IResetableItem>();
		transform.ForEachDescendant((descendant) =>
		{
			var components = descendant.GetComponents<IResetableItem>();
			foreach (var component in components)
				resetables.Add(component);
		});
	}

	public void ResetAll()
	{
		if (resetables == null)
			return;
		foreach (var resetable in resetables)
		{
			resetable.ResetState();
		}
	}
}
