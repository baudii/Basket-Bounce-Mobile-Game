using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KK.Common
{
	public static class Utils
	{

		public static IEnumerator Co_DelayedExecute(Action action, int framesDelay)
		{
			for (int i = 0; i < framesDelay; i++)
			{
				yield return null;
			}

			action();
		}

		public static IEnumerator Co_DelayedExecute(Action action, float delay, bool scaledTime = true)
		{
			if (scaledTime)
				yield return new WaitForSeconds(delay);
			else
				yield return new WaitForSecondsRealtime(delay);

			action();
		}

		public static IEnumerator Co_DelayedExecute(Action action, Func<bool> predicate, float minTime)
		{
			if (minTime > 0)
				yield return new WaitForSeconds(minTime);
			yield return new WaitUntil(predicate);
			action();
		}

		public static async Task DelayedExecuteAsync(Action task, int milliseconds, CancellationToken token = default)
		{
			try
			{
				await Task.Delay(milliseconds, token);
				task();
			}
			catch (OperationCanceledException) { }
			catch (Exception e) { throw e; }
		}

		public static async Task DelayedExecuteAsync(Action action, Func<CancellationToken, Task<bool>> predicate, CancellationToken token = default)
		{
			try
			{
				if (await predicate(token))
					action();
			}
			catch (OperationCanceledException) { }
			catch (Exception e) { throw e; }
		}

		public static TextMesh CreateWorldText(string text, Vector3 localPosition, bool is3D = false, int fontSize = 40)
		{
			var localEulerAngles = Vector3.zero;
			if (is3D)
				localEulerAngles.x = 90;
			return CreateWorldText(null, text, localPosition, Vector3.one / 20, localEulerAngles, fontSize, Color.white, TextAnchor.UpperLeft, TextAlignment.Left, 100);
		}

		public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, Vector3 scale, Vector3 localEulerAngles, int fontSize, Color color, TextAnchor textAnchor, TextAlignment alignment, int sortingOrder)
		{
			GameObject gameObject = new GameObject("World_text", typeof(TextMesh));
			Transform transform = gameObject.transform;
			transform.SetParent(parent, false);
			transform.localPosition = localPosition;
			transform.localScale = scale;
			transform.localEulerAngles = localEulerAngles;

			TextMesh textMesh = gameObject.GetComponent<TextMesh>();
			textMesh.anchor = textAnchor;
			textMesh.alignment = alignment;
			textMesh.text = text;
			textMesh.fontSize = fontSize;
			textMesh.color = color;
			textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

			return textMesh;
		}

		public static bool DoesElementExistInArray<T>(T element, T[] array) where T : IEquatable<T>
		{
			foreach (T el in array)
			{
				if (el.Equals(element))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns sign of a number. Acts just like mathematical version would.
		/// </summary>
		/// <param name="x">Input number of a function</param>
		/// <returns>For x>0 returns 1, for x<0 returns -1, for x=0 returns 0</returns>
		public static int Signum(float x)
		{
			if (x > 0)
				return 1;
			if (x < 0)
				return -1;
			return 0;
		}

		public static Vector2 GetClosestDirection(Vector2 v, float threshHold = 0.1f)
		{
			if (Vector2.Dot(Vector2.up, v.normalized) > threshHold)
				return Vector2.up;

			if (Vector2.Dot(Vector2.right, v.normalized) > threshHold)
				return Vector2.right;

			if (Vector2.Dot(Vector2.left, v.normalized) > threshHold)
				return Vector2.left;

			if (Vector2.Dot(Vector2.down, v.normalized) > threshHold)
				return Vector2.down;

			return Vector2.zero;
		}
	}

	[Serializable]
	public struct Point4Int
	{
		public int x, y, z, w;

		public Point4Int(int x, int y, int z, int w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
	}

	[Serializable]
	public struct AudioClipOneShot
	{
		public AudioClip clip;
		[Range(0, 1)] public float volume;
		[Range(0, 1.5f)] public float randomizer;
	}
}