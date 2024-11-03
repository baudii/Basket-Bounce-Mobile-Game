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
	public class YandexMobileAdsBannerScript : MonoBehaviour
	{
		private Banner banner;

		public void Init()
		{
			//Sets COPPA restriction for user age under 13
			MobileAds.SetAgeRestrictedUser(true);
			// Set sticky banner width
			// Or set inline banner maximum width and height
			// BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
			BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
			// Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
			string adUnitId = "demo-banner-yandex";

			banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

			banner.OnAdLoaded += HandleAdLoaded;
			banner.OnAdFailedToLoad += HandleAdFailedToLoad;
			banner.OnReturnedToApplication += HandleReturnedToApplication;
			banner.OnLeftApplication += HandleLeftApplication;
			banner.OnAdClicked += HandleAdClicked;
			banner.OnImpression += HandleImpression;
		}

		public void RequestBanner()
		{
			if (banner != null)
			{
				banner.Destroy();
			}

			var builder = new AdRequest.Builder();

			var request = builder.Build();

			banner.LoadAd(request);
		}

		// Example how to get screen width for request
		private int GetScreenWidthDp()
		{
			int screenWidth = (int)Screen.safeArea.width;
			return ScreenUtils.ConvertPixelsToDp(screenWidth);
		}

		private void DisplayMessage(string message)
		{
			message = message + (message.Length == 0 ? "" : "\n--------\n" + message);
			print(message);
		}

		#region Banner callback handlers

		public void HandleAdLoaded(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdLoaded event received");
			banner.Show();
		}

		public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
		{
			DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);
		}

		public void HandleLeftApplication(object sender, EventArgs args)
		{
			DisplayMessage("HandleLeftApplication event received");
		}

		public void HandleReturnedToApplication(object sender, EventArgs args)
		{
			DisplayMessage("HandleReturnedToApplication event received");
		}

		public void HandleAdLeftApplication(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdLeftApplication event received");
		}

		public void HandleAdClicked(object sender, EventArgs args)
		{
			DisplayMessage("HandleAdClicked event received");
		}

		public void HandleImpression(object sender, ImpressionData impressionData)
		{
			var data = impressionData == null ? "null" : impressionData.rawData;
			DisplayMessage("HandleImpression event received with data: " + data);
		}

		#endregion
	}
}