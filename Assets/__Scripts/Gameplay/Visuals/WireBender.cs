using UnityEngine;
using KK.Common;

namespace BasketBounce.Gameplay.Visuals
{
    public class WireBender : LineBender
    {
		[SerializeField] float offsetMagnitude = 0.5f;
		[SerializeField] bool bendOnStart;


		private void Start()
		{
			if (bendOnStart)
			{
				Init(startPoint.position);
				Bend();
			}
		}

		private void AuthomaticIntermediatePoints(Vector3 startPos, Vector3 endPos)
		{

			var v = (endPos - startPos);

			if (Mathf.Abs(v.x) < 1)
			{
				startPoint.position = startPos.AddTo(x: offsetMagnitude);
				endPoint.position = endPos.AddTo(x: offsetMagnitude);

				var intermediatePoints = new Vector3[2];
				intermediatePoints[0] = new Vector3(startPos.x + offsetMagnitude - Mathf.Sign(startPos.x), startPos.y);
				intermediatePoints[1] = new Vector3(startPos.x + offsetMagnitude - Mathf.Sign(startPos.x), endPos.y);
			}
			else if (Mathf.Abs(v.y) < 1)
			{
				var offset = Utils.Signum(v.x) * offsetMagnitude;
				startPoint.position = startPos.AddTo(x: offset);
				endPoint.position = endPos.AddTo(x: -offset);

				var intermediatePoints = new Vector3[2];
				intermediatePoints[0] = new Vector3(startPos.x + v.x / 2, startPos.y);
				intermediatePoints[1] = new Vector3(startPos.x + v.x / 2, endPos.y);
			}
		}
    }
}
