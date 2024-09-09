using UnityEngine;
using System.Collections;

public class IronSourceInitializer : MonoBehaviour
{
	public static string uniqueUserId = "demoUserUnity";

	void Awake ()
	{
		#if UNITY_ANDROID
        string appKey = "1f8c3aea5";
		#elif UNITY_IPHONE
        string appKey = "1f8c3751d";
		#else
        string appKey = "unexpected_platform";
		#endif

		IronSource.Agent.validateIntegration ();

		this.SmartLog ("Unity version: " + IronSource.unityVersion());

		// Add Banner Events
		IronSourceBannerEvents.onAdLoadedEvent += OnBannerLoad;
		IronSourceBannerEvents.onAdLoadFailedEvent += OnBannerLoadError;
		IronSourceBannerEvents.onAdClickedEvent += OnBannerClick;
		IronSourceBannerEvents.onAdScreenPresentedEvent += OnBannerPresented;
		IronSourceBannerEvents.onAdScreenDismissedEvent += OnBannerDismiss;
		IronSourceBannerEvents.onAdLeftApplicationEvent += OnBannerLeftApp;

		// SDK init
		IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);


        string id = IronSource.Agent.getAdvertiserId();
        this.SmartLog("(IronSource.Agent.getAdvertiserId() = " + id);
        //IronSource.Agent.initISDemandOnly (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

        //Set User ID For Server To Server Integration
        IronSource.Agent.setUserId("UserId");
		
		// Load Banner example
		IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
	}

	void OnApplicationPause(bool isPaused)
	{
		this.SmartLog("isPaused = " + isPaused);
		IronSource.Agent.onApplicationPause(isPaused);
	}

	//Banner Events
	void OnBannerLoad(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Banner loaded successfully. AdInfo " + adInfo);
	}

	void OnBannerLoadError(IronSourceError ironSourceError)
	{
        this.SmartLog("Banner loaded with error. Error: " + ironSourceError);
	}

	void OnBannerClick(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Banner was clicked. Info: " + adInfo);
	}

	void OnBannerPresented(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Banner was presented. AdInfo: " + adInfo);
	}

	void OnBannerDismiss(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Banner was dismissed. Adinfo: " + adInfo);
	}

	void OnBannerLeftApp(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Banner left applicaion. AdInfo: " + adInfo);
	}
}
