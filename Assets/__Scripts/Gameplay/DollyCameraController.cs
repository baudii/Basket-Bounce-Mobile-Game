using Cinemachine;
using UnityEngine;
using KK.Common;
using BasketBounce.Gameplay.Levels;

namespace BasketBounce.Gameplay
{
	public class DollyCameraController : MonoBehaviour
	{
		[SerializeField] float offsetFromFin;
		[SerializeField] CinemachineSmoothPath dolly;
		[SerializeField] CinemachineVirtualCamera cvc;

		CinemachineTrackedDolly ctd;
		Transform followTarget;
		LevelManager levelManager;

		public float PathLength { get; private set; }

		public void Init(LevelManager levelManager, Transform followTarget)
		{
			this.followTarget = followTarget;
			this.levelManager = levelManager;
			
			var waypoints = new CinemachineSmoothPath.Waypoint[2];
			waypoints[0] = new CinemachineSmoothPath.Waypoint();
			waypoints[0].position = Vector3.zero.WhereY(2);
			waypoints[1] = new CinemachineSmoothPath.Waypoint();
			waypoints[1].position = Vector3.zero.WhereY(2 + 0.001f);

			dolly.m_Waypoints = waypoints;

			ctd = cvc.GetCinemachineComponent<CinemachineTrackedDolly>();

			this.levelManager.OnLevelSetupEvent.AddListener(OnLevelSetup);

			EnableFollow();
		}

		private void OnDestroy()
		{
			if (levelManager != null)
			{
				levelManager.OnLevelSetupEvent.RemoveListener(OnLevelSetup);
			}
		}

		private void OnApplicationQuit()
		{
			if (ctd != null)
			{
				ctd.m_PathPosition = 0;
			}
		}

		public void UpdateDollyWaypoint(Vector3 finPos)
		{
			var desiredPos = Vector2.zero.WhereY(2 + 0.001f);
			var desiredY = finPos.y - offsetFromFin - 2 + 0.001f;
			if (finPos.y > 10)
			{
				desiredPos = desiredPos.WhereY(desiredY);
			}
			// this.Log("desiredY:", desiredY, "desiredPos.y:", desiredPos.y, "finPos.y:", finPos.y);
			dolly.m_Waypoints[1].position = desiredPos;
			PathLength = desiredPos.y - 2;
			dolly.InvalidateDistanceCache();
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

		public void EnableFollow()
		{
			cvc.Follow = followTarget;
		}

		public void DisableFollow()
		{
			cvc.Follow = null;
		}

		void OnLevelSetup(LevelData levelData)
		{
			ResetDollyPathPos();
			UpdateDollyWaypoint(levelData.GetFinPos());
		}
	}
}