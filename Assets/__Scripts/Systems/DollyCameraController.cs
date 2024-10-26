using Cinemachine;
using UnityEngine;
using KK.Common;

namespace BasketBounce.Systems
{
	public class DollyCameraController : MonoBehaviour
	{
		[SerializeField] float offsetFromFin;
		[SerializeField] CinemachineSmoothPath dolly;
		[SerializeField] CinemachineVirtualCamera cvc;

		CinemachineTrackedDolly ctd;

		public float PathLength { get; private set; }

		private void Awake()
		{
			var waypoints = new CinemachineSmoothPath.Waypoint[2];
			waypoints[0] = new CinemachineSmoothPath.Waypoint();
			waypoints[0].position = Vector3.zero.WhereY(2);
			waypoints[1] = new CinemachineSmoothPath.Waypoint();
			waypoints[1].position = Vector3.zero.WhereY(2);

			dolly.m_Waypoints = waypoints;
		}

		public void UpdateDollyWaypoint(Vector3 finPos)
		{
			var desiredPos = Vector2.zero.WhereY(2 + Mathf.Epsilon);
			var desiredY = finPos.y - offsetFromFin - 2 + Mathf.Epsilon;
			if (finPos.y > 10)
			{
				desiredPos = desiredPos.WhereY(desiredY);
			}
			dolly.m_Waypoints[1].position = desiredPos;
			PathLength = desiredPos.y - 2;
		}

		public void ResetDollyPathPos()
		{
			if (ctd == null)
				ctd = cvc.GetCinemachineComponent<CinemachineTrackedDolly>();

			cvc.gameObject.SetActive(false);
			ctd.m_PathPosition = 0;
			cvc.gameObject.SetActive(true);
		}

		public void SetPathPosition(float value)
		{
			ctd.m_PathPosition = value;
		}
	}
}