using System.Collections.Generic;
using UnityEngine;
using KK.Common;
using BasketBounce.Systems;

namespace BasketBounce.Gameplay.Levels
{
	public class ResetableManager
	{
		private List<IResetableItem> resetables;

		public ResetableManager()
		{
			resetables = new List<IResetableItem>();
		}

		public void Add(IResetableItem item)
		{
			resetables.Add(item);
		}

		public void AddRange(IEnumerable<IResetableItem> items)
		{
			resetables.AddRange(items);
		}

		public void MapResetables(Transform root)
		{
			var components = root.GetComponentsInChildren<IResetableItem>(true);
			AddRange(components);
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