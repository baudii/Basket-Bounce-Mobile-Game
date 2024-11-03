using UnityEngine;
using BasketBounce.Models;
using BasketBounce.Gameplay.Levels;
using BasketBounce.Systems;

namespace BasketBounce.Ads
{
	public class AdManager : MonoBehaviour
	{
		[SerializeField] int restartRequirement, gameOverRequirement, levelCompleteRequirement;
		[SerializeField] YandexMobileAdsInterstitialScript interstitialScript;
		[SerializeField] YandexMobileAdsBannerScript bannerScript;
		int gameOverCount, levelCompleteCount, restartCount;

		public void Init(GameManager gameManager, LevelManager levelManager)
		{
#if UNITY_ANDROID || UNITY_IOS
			gameManager.OnGameOverEvent.AddListener(OnGameOver);
			levelManager.OnFinishedLevelEvent.AddListener(OnLevelComplete);
			levelManager.OnLevelSetupEvent.AddListener(OnLevelSetup);

			interstitialScript.RequestInterstitial();
			bannerScript.Init();
			bannerScript.RequestBanner();
#endif
		}
		void OnLevelComplete(ScoreData _)
		{
			levelCompleteCount++;
			CheckRequirements();
		}

		void OnGameOver()
		{
			gameOverCount++;
			CheckRequirements();
		}

		void OnLevelSetup(LevelData levelData)
		{
			restartCount++;
			CheckRequirements();
		}

		void CheckRequirements()
		{
			if (gameOverCount >= gameOverRequirement || levelCompleteCount >= levelCompleteRequirement || restartCount >= restartRequirement)
			{
				interstitialScript.ShowInterstitial();
				levelCompleteCount = 0;
				gameOverCount = 0;
				restartCount = 0;
			}
		}
	}
}