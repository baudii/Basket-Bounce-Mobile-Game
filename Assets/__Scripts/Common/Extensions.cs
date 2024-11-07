using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KK.Common
{
	public static class Extensions
	{
		#region Vectors
		public static Vector3 WhereX(this Vector3 vector, float x)
		{
			vector.x = x;
			return vector;
		}
		public static Vector3 WhereY(this Vector3 vector, float y)
		{
			vector.y = y;
			return vector;
		}
		public static Vector3 WhereZ(this Vector3 vector, float z)
		{
			vector.z = z;
			return vector;
		}
		public static Vector3 AddTo(this Vector3 vector, float x = 0, float y = 0, float z = 0)
		{
			vector.x += x;
			vector.y += y;
			vector.z += z;
			return vector;
		}
		public static Vector3 Multiply(this Vector3 vector, float x = 1, float y = 1, float z = 1)
		{
			vector.x *= x;
			vector.y *= y;
			vector.z *= z;
			return vector;
		}
		public static Vector3 Multiply(this Vector3 vector, Vector3 otherVector)
		{
			vector.x *= otherVector.x;
			vector.y *= otherVector.y;
			vector.z *= otherVector.z;
			return vector;
		}

		public static Vector3 Divide(this Vector3 vector, float x = 1, float y = 1, float z = 1)
		{
			vector.x /= x;
			vector.y /= y;
			vector.z /= z;
			return vector;
		}
		public static Vector3 Divide(this Vector3 vector, Vector3 otherVector)
		{
			vector.x /= otherVector.x;
			vector.y /= otherVector.y;
			vector.z /= otherVector.z;
			return vector;
		}

		public static Vector2 WhereX(this Vector2 vector, float x)
		{
			vector.x = x;
			return vector;
		}
		public static Vector2 WhereY(this Vector2 vector, float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector2 AddTo(this Vector2 vector, float x = 0, float y = 0)
		{
			vector.x += x;
			vector.y += y;
			return vector;
		}
		public static Vector2 Multiply(this Vector2 vector, float x = 1, float y = 1)
		{
			vector.x *= x;
			vector.y *= y;
			return vector;
		}

		public static Vector2 Direction(this Vector2 from, Vector2 to)
		{
			var heading = to - from;
			return heading.normalized;
		}
		public static Vector3 Direction(this Vector3 from, Vector3 to)
		{
			var heading = to - from;
			return heading.normalized;
		}
		#endregion

		#region String


		public static bool ContainsSpecialChar(this string mainString, string chars)
		{
			foreach (var ch in chars)
			{
				if (mainString.Contains(ch))
					return true;
			}
			return false;
		}

		#endregion

		#region Coroutine
		/// <summary> Calls function with delay (seconds). Coroutine </summary>
		public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, float delay, bool scaledTime = false) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, delay, scaledTime));

		/// <summary> Calls function with delay (Update frames). Coroutine </summary>
		public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, int framesDelay) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, framesDelay));

		/// <summary> Calls function when "predicate" function evaluates to true. Coroutine </summary>
		public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, Func<bool> predicate, float minTime = 0) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, predicate, minTime));

		#endregion

		#region Log
		/// <summary> Logs out a message to the Console in format: "[callerScript]: message" </summary>
		/// <param name="message">Log messages</param>
		public static void Log(this object callerScript, params object[] messages)
		{
			string log = GetLog(callerScript, messages);
			Debug.Log(log);
		}
		/// <summary> Logs out a message to the Console in format: "[callerScript]: message" </summary>
		/// <param name="message">Log messages</param>
		public static void LogError(this object callerContext, params object[] messages)
		{
			string log = GetLog(callerContext, messages);
			Debug.LogError(log);
		}
		/// <summary> Logs out a message to the Console in format: "[callerScript]: message" </summary>
		/// <param name="messages">Log messages</param>
		public static void LogWarning(this object callerScript, params object[] messages)
		{
			string log = GetLog(callerScript, messages);
			Debug.LogWarning(log);
		}

		private static string GetLog(object callerContext, object[] messages)
		{
			string log = "[" + callerContext.GetType().Name + "]";
			foreach (var message in messages)
			{
				if (message != null)
					log += (" " + message.ToString());
				else
					log += (" null");
			}
			return log;
		}

		#endregion

		#region Transform
		/// <summary>
		/// Perform action on every transform in the whole hierarchy of descendants including root parent
		/// </summary>
		public static void ForEachDescendant(this Transform parent, Action<Transform> action)
		{
			var stack = new Stack<Transform>();
			stack.Push(parent);

			while (stack.Count > 0)
			{
				var current = stack.Pop();

				action(current);

				foreach (Transform child in current)
				{
					stack.Push(child);
				}
			}
		}

		/// <summary>
		/// Perform action on every transform in the whole hierarchy of descendants including root parent.
		/// Includes break functionality: return true from the delegate to break out of the loop, false to continue iterating
		/// </summary>
		public static void ForEachDescendant(this Transform parent, Func<Transform, bool> func)
		{
			var stack = new Stack<Transform>();
			stack.Push(parent);

			while (stack.Count > 0)
			{
				var current = stack.Pop();

				if (func(current))
					break;

				foreach (Transform child in current)
				{
					stack.Push(child);
				}
			}
		}

		/// <summary>
		/// Asynchronously perform action on every transform in the whole hierarchy of descendants including root parent.
		/// Includes break functionality: return true from the delegate to break out of the loop, false to continue iterating.
		/// </summary>
		public static async Task ForEachDescendantAsync(this Transform parent, Func<Transform, Task<bool>> func)
		{
			var stack = new Stack<Transform>();
			stack.Push(parent);

			while (stack.Count > 0)
			{
				var current = stack.Pop();

				if (await func(current))
					break;

				foreach (Transform child in current)
				{
					stack.Push(child);
				}
			}
		}


		/// <summary>
		/// Perform action on every direct child.
		/// </summary>
		public static void ForEach(this Transform parent, Action<Transform> action)
		{
			foreach (Transform child in parent)
			{
				action(child);
			}
		}

		/// <summary>
		/// Perform action on every direct child.
		/// Includes break functionality: return true from the delegate to break out of the loop, false to continue iterating
		/// </summary>
		public static void ForEach(this Transform parent, Func<Transform, bool> func)
		{
			foreach (Transform child in parent)
			{
				if (func(child))
					break;
			}
		}
		#endregion

		/// <summary>
		/// Check if layermask contains layer (int).
		/// </summary>
		public static bool Contains(this LayerMask mask, int layer)
		{
			return mask == (mask | (1 << layer));
		}

		/// <summary>
		/// Checks if index is within the give array
		/// </summary>
		/// <param name="array">Array to test</param>
		/// <param name="index">Index</param>
		/// <returns></returns>
		public static bool IsWithinBoundaries(this Array array, int index)
		{
			if (index < 0 || index >= array.Length)
				return false;
			return true;
		}

		/// <summary>
		/// Checks if index is within the give array
		/// </summary>
		/// <param name="collection">Collection to test</param>
		/// <param name="index">Index</param>
		/// <returns></returns>
		public static bool IsWithinBoundaries(this ICollection collection, int index)
		{
			if (index < 0 || index >= collection.Count)
				return false;
			return true;
		}
	}
}