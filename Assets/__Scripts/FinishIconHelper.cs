using UnityEngine;

public class FinishIconHelper : MonoBehaviour
{
	[SerializeField] GameObject body;
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
		bool isVisible = IsInScreen(finishPos);
		if (isVisible && body.activeSelf)
		{
			body.SetActive(false);
		}
		else if (!isVisible && !body.activeSelf)
		{
			body.SetActive(true);
		}
	}

	public void Init(Vector3 finishPosition)
	{
		finishPos = finishPosition;
		transform.position = transform.position.WhereX(finishPos.x);
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
