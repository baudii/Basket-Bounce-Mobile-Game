using UnityEngine;

public class AdRequirementController : MonoBehaviour
{
    [SerializeField] InterstitialRequirement gameOverRequirement, levelCompleteRequirement;
    void Start()
    {
        GameManager.Instance.OnGameOver.AddListener(OnGameOver);
        GameManager.Instance.OnLevelComplete.AddListener(OnLevelComplete);
        AdManager.Instance.OnShowInterstitial.AddListener(ResetRequirements);
    }

    void ResetRequirements()
    {
        gameOverRequirement.Reset();
        levelCompleteRequirement.Reset();
    }

    void OnGameOver()
    {
        gameOverRequirement.OnShow();
        if (gameOverRequirement.IsRequirementMet())
        {
            this.SmartLog("Gameover requirement met:", gameOverRequirement.showRequirement, "Showing ad.");
            AdManager.Instance.ShowInterstitiaL();
        }
    }

    void OnLevelComplete()
    {
        levelCompleteRequirement.OnShow();
        if (levelCompleteRequirement.IsRequirementMet())
        {
            this.SmartLog("LevelComplete requirement met:", levelCompleteRequirement.showRequirement, "Showing ad.");
            AdManager.Instance.ShowInterstitiaL();
        }
    }
}

[System.Serializable]
public class InterstitialRequirement
{
    public int showRequirement;
    int shown;

    public InterstitialRequirement()
    {
        shown = 0;
    }

    public void OnShow() => shown++;
    public bool IsRequirementMet() => shown % showRequirement == 0;
    public void Reset() => shown = 0;
}