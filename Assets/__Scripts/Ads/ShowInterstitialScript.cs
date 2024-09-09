using UnityEngine;
using System;

public class ShowInterstitialScript : MonoBehaviour
{

 	public static String INTERSTITIAL_INSTANCE_ID = "0";

	// Use this for initialization
	void Start ()
	{
		GameManager.Instance.OnRestart.AddListener(LoadInterstitial);
		GameManager.Instance.OnGameOver.AddListener(ShowInterstitiaL);

		// Add Interstitial Events
		IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
		IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
		IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
		IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
		IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
		IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
		IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

	}

	/************* Interstitial API *************/ 
	public void LoadInterstitial()
	{
		this.SmartLog("Interstitial load attempt");
		IronSource.Agent.loadInterstitial();
	}
	
	public void ShowInterstitiaL()
	{
		this.SmartLog("Interstitial show attempt");
		if (IronSource.Agent.isInterstitialReady()) {
			IronSource.Agent.showInterstitial();
		} else {
            this.SmartLog("IronSource.Agent.isInterstitialReady = False");
		}
	}
	
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
	}
}

