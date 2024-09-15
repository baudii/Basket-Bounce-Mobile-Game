using UnityEngine;
using UnityEngine.Events;

public class AdManager : MonoBehaviour
{
	public static string uniqueUserId = "demoUserUnity";

	public static AdManager Instance => instance;
	static AdManager instance;

    public UnityEvent OnShowInterstitial;

	public void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}

        InitIronsource();
    }

    void InitIronsource()
    {
#if UNITY_ANDROID
        string appKey = "1f8c3aea5";
#elif UNITY_IPHONE
        string appKey = "1f8c3751d";
#else
        string appKey = "unexpected_platform";
#endif

        IronSource.Agent.validateIntegration();

        this.SmartLog("Unity version: " + IronSource.unityVersion());

        //Set User ID For Server To Server Integration
        IronSource.Agent.setUserId(uniqueUserId);

        // SDK init
        IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);

        string id = IronSource.Agent.getAdvertiserId();
        this.SmartLog("(IronSource.Agent.getAdvertiserId() = " + id);
    }

    void OnApplicationPause(bool isPaused)
	{
		this.SmartLog("isPaused = " + isPaused);
		IronSource.Agent.onApplicationPause(isPaused);
	}

	public void LoadBanner()
	{
        this.SmartLog("Banner load attempt");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void LoadInterstitial()
    {
        this.SmartLog("Interstitial load attempt");
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitiaL()
    {
        this.SmartLog("Interstitial show attempt");
        if (IronSource.Agent.isInterstitialReady())
        {
            OnShowInterstitial?.Invoke();
            IronSource.Agent.showInterstitial();
        }
        else
        {
            this.SmartLog("IronSource.Agent.isInterstitialReady = False");
        }
    }


    // Not Implemented Yet:
    public void LoadRewarded() { }

    public void ShowRewarded() { }
}

