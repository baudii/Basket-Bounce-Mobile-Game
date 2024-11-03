using System.Collections.Generic;
using UnityEngine;
using KK.Common;
using BasketBounce.Systems;

namespace BasketBounce.Gameplay.Levels
{
	public class ResetableManager
	{
		private List<IResetableItem> resetables;

		public ResetableManager(Transform transform)
		{
			resetables = new List<IResetableItem>();
			transform.ForEachDescendant((descendant) =>
			{
				var components = descendant.GetComponents<IResetableItem>();
				resetables.AddRange(components);
			});
		}

		public void ResetAll()
		{
			foreach (var resetable in resetables)
			{
				resetable.ResetState();
			}
		}
	}
}