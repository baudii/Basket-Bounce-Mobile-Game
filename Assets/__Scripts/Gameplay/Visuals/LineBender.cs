using KK.Common;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBounce.Gameplay.Visuals
{
    public class LineBender : MonoBehaviour
    {
		[SerializeField] LineRenderer linePrefab;
		[SerializeField] protected Transform startPoint, endPoint;
		[SerializeField] protected Vector3[] intermediatePoints;
		LineRenderer[] lines;

#if UNITY_EDITOR
		[SerializeField] bool validate;		
		private void OnValidate()
		{
			if (validate)
				Bend();	
		}
#endif

		private LineRenderer GetItem(int i)
		{
			if (lines[i] == null)
				return Instantiate(linePrefab, transform);

			return lines[i];
		}

		protected void Bend()
		{
			if (lines == null)
				lines = new LineRenderer[2 + intermediatePoints.Length];

			this.SmartLog("Positions count:", lines.Length);
			lines[0] = GetItem(0);
			lines[0].positionCount = 2;
			lines[0].SetPosition(0, startPoint.position);
			this.SmartLog("Set start of point:", 1);

			int i = 0;
			for (i = 0; i < intermediatePoints.Length; ++i)
			{
				lines[i].SetPosition(1, intermediatePoints[i]);
				this.SmartLog("Set end of point:", i + 1);

				lines[i + 1] = GetItem(i + 1);
				lines[i + 1].positionCount = 2;
				lines[i + 1].SetPosition(0, intermediatePoints[i]);
				this.SmartLog("Set start of point:", i + 2);
			}

			lines[i].SetPosition(1, endPoint.position);
			this.SmartLog("Set end of point:", i + 2);
		}
    }
}
