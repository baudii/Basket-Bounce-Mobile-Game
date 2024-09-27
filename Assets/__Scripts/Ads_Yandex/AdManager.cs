using UnityEngine;

public class AdManager : MonoBehaviour
{
	[SerializeField] int restartRequirement, gameOverRequirement, levelCompleteRequirement;
	[SerializeField] YandexMobileAdsInterstitialScript interstitialScript;
	[SerializeField] YandexMobileAdsBannerScript bannerScript;
	int gameOverCount, levelCompleteCount, restartCount;

	private void Start()
	{
#if UNITY_ANDROID || UNITY_IOS
		GameManager.Instance.OnLevelComplete.AddListener(OnLevelComplete);
		GameManager.Instance.OnGameOver.AddListener(OnGameOver);
		LevelManager.Instance.OnLevelSetup.AddListener(OnLevelSetup);
		interstitialScript.RequestInterstitial();
		bannerScript.Init();
		bannerScript.RequestBanner();
#endif
	}
	void OnLevelComplete()
	{
		levelCompleteCount++;
		CheckRequirments();
	}

	void OnGameOver()
	{
		gameOverCount++;
		CheckRequirments();
	}

	void OnLevelSetup(LevelData levelData)
	{
		restartCount++;
		CheckRequirments();
	}
	
	void CheckRequirments()
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
