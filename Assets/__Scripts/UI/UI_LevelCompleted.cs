using UnityEngine;

public class UI_LevelCompleted : MonoBehaviour
{
	[SerializeField] GameObject filledstar1;
	[SerializeField] GameObject filledstar2;
	[SerializeField] GameObject filledstar3;
	public void SetStars(int stars)
	{
		filledstar1.SetActive(false);
		filledstar2.SetActive(false);
		filledstar3.SetActive(false);

		if (stars >= 1)
			filledstar1.SetActive(true);
		if (stars >= 2)
			filledstar2.SetActive(true);
		if (stars >= 3)
			filledstar3.SetActive(true);
	}
}
