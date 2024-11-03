using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BasketBounce.Systems
{
    public abstract class BaseGameEntry : MonoBehaviour
    {
		private static Dictionary<Type, object> dependancies;

		public void Init()
		{
			if (dependancies != null)
				throw new InvalidOperationException($"Dependancies initialized twice. Maybe you have 2 entry points?");
			dependancies = new Dictionary<Type, object>();
		}

		public void Register<T>(T obj)
		{
			dependancies.Add(typeof(T), obj);
		}

		public void GetDependancy<T>(out T result)
		{
			var obj = dependancies[typeof(T)];
			if (obj == null)
				throw new ArgumentException($"Dependancy of type {typeof(T)} was found but the value is null");
			result = (T)obj;
		}

		public abstract Task Setup(CancellationToken token);
		public abstract Task Activate(CancellationToken token);
    }
}
