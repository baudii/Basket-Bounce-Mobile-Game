using System;
using UnityEngine;

namespace KK.Common
{
    public class DestroyNotifier : MonoBehaviour
    {
		private Action<GameObject> OnDestroyed;

		public void Subscribe(Action<GameObject> action)
		{
			OnDestroyed += action;
		}

		private void OnDestroy()
		{
			OnDestroyed?.Invoke(gameObject);
			OnDestroyed = null;
		}
	}
}
