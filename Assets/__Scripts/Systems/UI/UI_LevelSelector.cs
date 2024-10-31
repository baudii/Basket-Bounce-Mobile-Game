using System.Collections.Generic;
using UnityEngine;
using BasketBounce.Systems;
using KK.Common;

namespace BasketBounce.UI
{
	public class UI_LevelSelector : MonoBehaviour
	{
		[SerializeField] UI_LevelIcon levelIconPrefab;
		[SerializeField] Transform gridParent;
		List<UI_LevelIcon> levels;

		int levelAmount;
		public int MaxTotalStars => levelAmount * 3;

		public void Init(int levelAmount)
		{
			this.levelAmount = levelAmount;
			levels = new List<UI_LevelIcon>();
			for (int i = 0; i < levelAmount; i++)
			{
				var levelIcon = Instantiate(levelIconPrefab, gridParent);
				levels.Add(levelIcon);
			}
		}

		public int GetTotalEarnedStars()
		{
			int totalStars = 0;

			for (int i = 0; i < levels.Count; i++)
			{
				totalStars += PlayerPrefs.GetInt(LevelManager.LEVEL_EARNED_STARS_KEY + i, 0);
			}

			return totalStars;
		}

		public void UpdateLevelSelector()
		{
			int currentLevel = LevelManager.Instance.CurrentLevel;

			int lastLevel = PlayerPrefs.GetInt(LevelManager.LAST_DISCOVERED_LEVEL_KEY, 0);

			for (int i = 0; i < levels.Count; i++)
			{
				int stars = PlayerPrefs.GetInt(LevelManager.LEVEL_EARNED_STARS_KEY + i, 0);

				levels[i].UpdateCell(stars, i, i <= lastLevel, currentLevel == i);
			}
		}

		public void LoadLevel()
		{
			int selectedLevel = UI_LevelIcon.SelectedLevel;
			this.SmartLog("Loading level:", selectedLevel);

			if (selectedLevel == -1)
				return;

			LevelManager.Instance.LoadLevel(selectedLevel);
		}
	}
}