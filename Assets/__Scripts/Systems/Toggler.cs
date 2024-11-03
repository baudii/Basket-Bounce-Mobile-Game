using UnityEngine;

namespace BasketBounce.Systems
{
	public class Toggler : MonoBehaviour
	{
		[SerializeField] GameObject[] toggles;

		private void OnDisable()
		{
			foreach (GameObject go in toggles)
				if (go != null)
					go.SetActive(false);
		}

		private void OnEnable()
		{
			foreach (GameObject go in toggles)
				if (go != null)
					go.SetActive(true);
		}
	}
}