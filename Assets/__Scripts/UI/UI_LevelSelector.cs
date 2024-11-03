using System.Collections.Generic;
using UnityEngine;
using BasketBounce.Gameplay.Levels;
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

		LevelManager levelManager;

		public void Init(LevelManager levelManager)
		{
			this.levelManager = levelManager;
			levelManager.OnLevelSetAvailable.AddListener(Setup);
		}

		public void Setup(LevelSet levelSet)
		{
			levelAmount = levelSet.LevelCount;
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
				var key = LevelManager.GetEarnedStarsKey(i, levelManager.LevelSetId);
				totalStars += PlayerPrefs.GetInt(key, 0);
			}

			return totalStars;
		}

		public void UpdateLevelSelector()
		{
			int currentLevel = levelManager.CurrentLevel;

			var lastDiscoveredKey = LevelManager.GetLastDiscoveredLevelKey(levelManager.LevelSetId);

			int lastLevel = PlayerPrefs.GetInt(lastDiscoveredKey, 0);

			for (int i = 0; i < levels.Count; i++)
			{
				var earnedStarsKey = LevelManager.GetEarnedStarsKey(i, levelManager.LevelSetId);
				int stars = PlayerPrefs.GetInt(earnedStarsKey, 0);

				levels[i].UpdateCell(stars, i, i <= lastLevel, currentLevel == i);
			}
		}

		public void Submit()
		{
			int selectedLevel = UI_LevelIcon.SelectedLevel;
			this.Log("Loading level:", selectedLevel);

			if (selectedLevel == -1)
				return;

			levelManager.LoadLevelAsync(selectedLevel);
		}
	}
}