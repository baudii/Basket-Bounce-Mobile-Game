using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using KK.Common;
using System;
using UnityEngine;

namespace BasketBounce.UI
{
	public class UI_MainMenuPlayButton : MonoBehaviour
	{
		int levelSetCache;

		public Action<int> OnStartGame;

		private void Start()
		{
			LoadLevelSet();
		}

		public void LoadLevelSet()
		{
			levelSetCache = PlayerPrefs.GetInt(LevelManager.GetLastLevelSetIdKey(), 0);
			if (levelSetCache == 0)
				gameObject.SetActive(false);
		}

		public void CacheLevelSet(int levelSet)
		{
			levelSetCache = levelSet;
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		public void StartGame()
		{
			DIContainer.GetDependency(out GameManager gameManager);
			gameManager.SubmitLevelSet(levelSetCache);
		}
	}
}
