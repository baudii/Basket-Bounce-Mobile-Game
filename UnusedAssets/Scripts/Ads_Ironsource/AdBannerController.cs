using UnityEngine;

public class AdBannerController : MonoBehaviour
{
    public void Start()
    {
        IronSourceBannerEvents.onAdLoadedEvent += OnBannerLoad;
        IronSourceBannerEvents.onAdLoadFailedEvent += OnBannerLoadError;
        IronSourceBannerEvents.onAdClickedEvent += OnBannerClick;
        IronSourceBannerEvents.onAdScreenPresentedEvent += OnBannerPresented;
        IronSourceBannerEvents.onAdScreenDismissedEvent += OnBannerDismiss;
        IronSourceBannerEvents.onAdLeftApplicationEvent += OnBannerLeftApp;

        AdManager.Instance.LoadBanner();
    }

    #region Banner Events
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
    #endregion
}
