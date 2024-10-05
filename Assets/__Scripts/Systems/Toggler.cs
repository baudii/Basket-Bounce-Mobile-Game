using UnityEngine;

public class Toggler : MonoBehaviour
{
	[SerializeField] GameObject[] toggles;

	private void OnDisable()
	{
		foreach (GameObject go in toggles)
		{
			if (go != null) // This checks if the GameObject is not destroyed
			{
				go.SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		foreach (GameObject go in toggles)
		{
			if (go != null) // This checks if the GameObject is not destroyed
			{
				go.SetActive(true);
			}
		}
	}
}
