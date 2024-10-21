using System.Collections.Generic;
using UnityEngine;
using KK.Common;
using BasketBounce.Systems.Interfaces;

namespace BasketBounce.Systems
{
	public class ResetableManager : MonoBehaviour
	{
		private List<IResetableItem> resetables;

		public void Fill()
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
}