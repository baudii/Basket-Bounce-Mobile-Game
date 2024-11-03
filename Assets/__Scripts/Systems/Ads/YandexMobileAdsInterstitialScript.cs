/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for Android (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace BasketBounce.Ads_Yandex
{
	public class YandexMobileAdsInterstitialScript : MonoBehaviour
	{
		private InterstitialAdLoader interstitialAdLoader;
		private Interstitial interstitial;

		public void Awake()
		{
			interstitialAdLoader = new InterstitialAdLoader();
			interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
			interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
		}

		public void RequestInterstitial()
		{
			//Sets COPPA restriction for user age under 13
			MobileAds.SetAgeRestrictedUser(true);

			// Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
			string adUnitId = "demo-interstitial-yandex";

			if (interstitial != null)
			{
				interstitial.Destroy();
			}

			interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
			DisplayMessage("Interstitial is requested");
		}

		public void ShowInterstitial()
		{
			if (interstitial == null)
			{
				DisplayMessage("Interstitial is not ready yet");
				return;
			}

			interstitial.OnAdClicked += HandleAdClicked;
			interstitial.OnAdShown += HandleAdShown;
			interstitial.OnAdFailedToShow += HandleAdFailedToShow;
			interstitial.OnAdImpression += HandleImpression;
			interstitial.OnAdDismissed += HandleAdDismissed;

			interstitial.Show();
		}

		private AdRequestConfiguration CreateAdRequest(string adUnitId)
		{
			return new AdRequestConfiguration.Builder(adUnitId).Build();
		}

		private void DisplayMessage(string message)
		{
			message = message + (message.Length == 0 ? "" : "\n--------\n" + message);
			print(message);
		}

		#region Interstitial request callback handlers
		public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
		{
			DisplayMessage("HandleAdLoaded event received");

			interstitial = args.Interstitial;
		}

		public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");
			RequestInterstitial();
		}
		#endregion

		#region Interstitial load callback handlers
		public void HandleAdClicked(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdClicked event received");
		}

		public void HandleAdShown(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdShown event received");
		}

		public void HandleAdDismissed(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdDismissed event received");

			interstitial.Destroy();
			interstitial = null;
			RequestInterstitial();
		}

		public void HandleImpression(object sender, ImpressionData impressionData)
		{
			var data = impressionData == null ? "null" : impressionData.rawData;
			DisplayMessage($"HandleImpression event received with data: {data}");
		}

		public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
		{
			DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");
		}

		#endregion
	}
}