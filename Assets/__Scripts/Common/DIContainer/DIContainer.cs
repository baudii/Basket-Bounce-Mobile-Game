using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KK.Common
{
	public static class DIContainer
	{
		private static Dictionary<(Type, string), object> dependencies;

		public static void Init()
		{
			if (dependencies != null)
				return;

			dependencies = new Dictionary<(Type, string), object>();
		}

		public static void Register<T>(T obj, string tag = null)
		{
			lock (dependencies)
			{
				(Type, string) key = (typeof(T), tag);

				if (dependencies.ContainsKey(key))
					throw new ArgumentException($"Key with tag {tag} and Type {typeof(T)} already exists in dictionary");

				if (obj.TryGetGameObject(out var go))
				{
					var notifier = go.AddComponent<DestroyNotifier>();
					notifier.Subscribe(_ =>
					{
						UnregisterDependency(obj, tag);
					});
				}
				dependencies.Add(key, obj);
			}
		}

		public static void GetDependency<T>(out T result, string tag = null)
		{
			lock (dependencies)
			{
				var obj = dependencies[(typeof(T), tag)];

				if (obj == null)
					throw new ArgumentException($"Dependency of type '{typeof(T)}' and tag '{tag}' was found but the value is null");

				result = (T)obj;
			}
		}

		public static void UnregisterDependency<T>(T obj, string tag)
		{
			lock (dependencies)
			{
				(Type, string) key = (typeof(T), tag);

				if (!dependencies.TryGetValue(key, out object value))
					throw new ArgumentException($"No dependency found with tag '{tag}' and type '{typeof(T).Name}'.");

				if (!Equals(value, obj))
					throw new ArgumentException($"The provided object does not match the registered dependency for tag '{tag}' and type '{typeof(T).Name}'. Expected: {value}, but received: {obj}.");

				dependencies.Remove(key);
			}
		}

		public static void InjectIn(params object[] objs)
		{
			foreach (object obj in objs)
				InjectIn(obj);
		}

		private static void InjectIn(object obj)
		{

			var members = obj.GetType().GetMembers(BindingFlags.NonPublic |
													BindingFlags.Public |
													BindingFlags.Instance |
													BindingFlags.DeclaredOnly);

			foreach (var member in members)
			{
				if (member.GetCustomAttribute(typeof(KKInjectAttribute)) is KKInjectAttribute injectAttr)
				{
					if (member is MethodInfo method)
					{
						var parameters = method.GetParameters();

						object[] dependencies = new object[parameters.Length];
						for (int i = 0; i < parameters.Length; i++)
						{
							dependencies[i] = DIContainer.dependencies[(parameters[i].ParameterType, injectAttr.Tag)];
						}

						method.Invoke(obj, dependencies);
					}
					else if (member is PropertyInfo property)
					{
						var val = dependencies[(property.PropertyType, injectAttr.Tag)];
						property.SetValue(obj, val);
					}
					else if (member is FieldInfo field)
					{
						var val = dependencies[(field.FieldType, injectAttr.Tag)];
						field.SetValue(obj, val);
					}
					else
					{
						throw new ArgumentException($"Provided object {obj} has [Inject] attribute assigned to neither method, field or property. This behavior is not supported");
					}

					// Dependency injected successfully
				}
			}
		}

		private static bool TryGetGameObject(this object obj, out GameObject gameObj)
		{
			if (obj is GameObject go)
			{
				gameObj = go;
				return true;
			}
			else if (obj is Component comp)
			{
				gameObj = comp.gameObject;
				return true;
			}

			gameObj = null;
			return false;
		}
	}
}
