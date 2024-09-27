using System.Collections.Generic;
using UnityEngine;

public class ResetableManager : MonoBehaviour
{
	List<IResetableItem> resetables;

	private void Awake()
	{
		resetables = new List<IResetableItem>();
		foreach (Transform t in transform)
		{
			var resetables = t.GetComponents<IResetableItem>();
			foreach (var imovable in resetables)
				this.resetables.Add(imovable);
		}
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
