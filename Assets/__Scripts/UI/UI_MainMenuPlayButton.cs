using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;
using KK.Common;
using System;
using UnityEngine;

namespace BasketBounce.UI
{
	public class UI_MainMenuPlayButton : MonoBehaviour
	{
		int levelCached;

		public Action<int> OnStartGame;

		private void Start()
		{
			LoadLevelSet();
		}

		public void LoadLevelSet()
		{
			levelCached = PlayerPrefs.GetInt(LevelManager.GetLastLevelSetIdKey(), 0);
			if (levelCached == 0)
				gameObject.SetActive(false);
		}

		public void CacheLevel(int level)
		{
			levelCached = level;
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		public void StartGame()
		{
			DIContainer.GetDependency(out GameManager gameManager);
			Utils.SafeExecuteAsync(() => gameManager.SubmitLevel(levelCached));
		}
	}
}
