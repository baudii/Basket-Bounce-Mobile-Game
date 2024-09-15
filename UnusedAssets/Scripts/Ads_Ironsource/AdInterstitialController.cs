using UnityEngine;

public class AdInterstitialController : MonoBehaviour
{
	public void Start()
	{
		IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
		IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
		IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
		IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
		IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
		IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
		IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        AdManager.Instance.LoadInterstitial();
    }
    #region Interstitial Events
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Interstitial is ready. AdInfo " + adInfo);
	}

	void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
	{
		this.SmartWarning("Interstitial load failed. Error " + ironSourceError);
	}

	void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
	{
        this.SmartLog("Interstitial ad was opened. AdInfo " + adInfo);
	}

	void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Interstitial was clicked. AdInfo " + adInfo);
	}

	void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Interstitial ad was successfully shown. AdInfo " + adInfo);
	}

	void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
	{
        this.SmartLog("Intersitital show failed. Error " + ironSourceError + ", AdInfo " + adInfo);
	}

	void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
	{
		this.SmartLog("Interstitial ad was closed. AdInfo " + adInfo);

		AdManager.Instance.LoadInterstitial();
	}
    #endregion
}

