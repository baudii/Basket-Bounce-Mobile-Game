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

		private void Awake()
		{
			var waypoints = new CinemachineSmoothPath.Waypoint[2];
			waypoints[0] = new CinemachineSmoothPath.Waypoint();
			waypoints[0].position = Vector3.zero.WhereY(2);
			waypoints[1] = new CinemachineSmoothPath.Waypoint();
			waypoints[1].position = Vector3.zero.WhereY(2);

			dolly.m_Waypoints = waypoints;

			LevelManager.Instance.OnLevelSetup.AddListener(UpdateDollyWaypoint);
		}

		private void OnDestroy()
		{
			LevelManager.Instance.OnLevelSetup.RemoveListener(UpdateDollyWaypoint);
		}

		public void UpdateDollyWaypoint(LevelData levelData)
		{
			var desiredPos = Vector3.zero.WhereY(2);
			var finPos = levelData.GetFinPos();
			if (finPos.y > 10)
			{
				desiredPos = desiredPos.WhereY(finPos.y - offsetFromFin - 2);
			}
			dolly.m_Waypoints[1].position = desiredPos;
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