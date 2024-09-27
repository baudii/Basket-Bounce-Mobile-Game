using UnityEngine;

public class FinishIconHelper : MonoBehaviour
{
	Camera cam;
	float ScreenHeight;
	Vector3 finishPos;


	private void Awake()
	{
		ScreenHeight = Screen.height;
		cam = Camera.main;
	}

	private void Update()
	{
		this.SmartLog("Updating");
		if (IsInScreen(finishPos))
		{
			Disable();
		}
	}

	public void Init(Vector3 finishPosition)
	{
		finishPos = finishPosition;
		transform.position = transform.position.WhereX(finishPos.x);
		Enable();
	}

	public void Enable()
	{
		gameObject.SetActive(true);
	}

	public void Disable()
	{
		gameObject.SetActive(false);
	}

	bool IsInScreen(Vector3 pos)
	{
		Vector3 screenPos = cam.WorldToScreenPoint(pos);
		if (screenPos.y <= ScreenHeight)
		{
			return true;
		}
		return false;
	}
}
