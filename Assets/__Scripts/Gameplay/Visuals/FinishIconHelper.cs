using UnityEngine;
using KK.Common;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Models;

namespace BasketBounce.Gameplay.Visuals
{
	public class FinishIconHelper : MonoBehaviour
	{
		[SerializeField] GameObject body;
		LevelManager levelManager;
		LevelData currentLevelData;
		public void Init(LevelManager levelManager)
		{
			levelManager.OnLevelSetupEvent.AddListener(Setup);
			this.levelManager = levelManager;
			levelManager.OnFinishedLevelEvent.AddListener(OnFinishedLevel);
		}

		private void OnDestroy()
		{
			if (levelManager != null)
			{
				levelManager.OnLevelSetupEvent.RemoveListener(Setup);
			}
		}

		private void Update()
		{
			if (currentLevelData == null)
				return;

			bool isVisible = currentLevelData.IsFinishInScreen();
			if (isVisible && body.activeSelf)
			{
				body.SetActive(false);
			}
			else if (!isVisible && !body.activeSelf)
			{
				body.SetActive(true);
			}
		}

		public void Setup(LevelData levelData)
		{
			transform.position = transform.position.WhereX(levelData.GetFinPos().x);
			currentLevelData = levelData;
		}

		public void OnFinishedLevel(ScoreData _)
		{
			currentLevelData = null;
		}
	}
}