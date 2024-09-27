using Cinemachine;
using UnityEngine;

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
		waypoints[0].position = Vector3.zero;
		waypoints[1] = new CinemachineSmoothPath.Waypoint();
		waypoints[1].position = Vector3.zero;

		dolly.m_Waypoints = waypoints;
	}
	public void UpdateDollyWaypoint(Vector2 finPos)
	{
		var desiredPos = Vector3.zero;
		if (finPos.y > 4.5f)
		{
			desiredPos = desiredPos.WhereY(finPos.y - offsetFromFin);
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
}
