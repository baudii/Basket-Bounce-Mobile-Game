using TMPro;
using UnityEngine;

public class UI_GameFinished : MonoBehaviour
{
	[SerializeField] UI_LevelSelector levelSelector;

	[SerializeField] TextMeshProUGUI totalStarsTextField;

	private void OnEnable()
	{
		SetStars();
	}

	public void SetStars()
	{
		int starsEarned = levelSelector.GetTotalEarnedStars();

		totalStarsTextField.text = starsEarned + " / " + levelSelector.MaxTotalStars;
	}
}
