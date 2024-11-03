using KK.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasketBounce.Gameplay.Visuals
{
    public class LineBender : MonoBehaviour
    {
		[SerializeField] LineRenderer linePrefab;
		[SerializeField] protected Transform startPoint, endPoint;
		
		Stack<Vector3> intermediatePoints;
		LineRenderer[] lines;

		public void Init(Vector3 start)
		{
			startPoint.position = start;
			intermediatePoints = new Stack<Vector3>();
		}
		
		private LineRenderer GetItem(int i)
		{
			if (lines[i] == null)
				return Instantiate(linePrefab, transform);

			return lines[i];
		}

		public void Bend()
		{
			if (lines == null)
				lines = new LineRenderer[2 + intermediatePoints.Count];

			lines[0] = GetItem(0);
			lines[0].positionCount = 2;
			lines[0].SetPosition(0, startPoint.position);

			int i = 0;
			for (i = 0; i < intermediatePoints.Count; ++i)
			{
				lines[i].SetPosition(1, intermediatePoints.ElementAt(i));

				lines[i + 1] = GetItem(i + 1);
				lines[i + 1].positionCount = 2;
				lines[i + 1].SetPosition(0, intermediatePoints.ElementAt(i));
			}

			lines[i].SetPosition(1, endPoint.position);
		}

		public void AddIntermediate(Vector3 point)
		{
			intermediatePoints.Push(point);
		}

		public void Finish()
		{
			var endPos = intermediatePoints.Pop();

			endPoint.position = endPos;
		}
    }
}
