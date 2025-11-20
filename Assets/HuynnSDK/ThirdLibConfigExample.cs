using UnityEngine;
using GameDevToi.ThirdLib;
using System.Collections.Generic;

namespace GameDevToi.ThirdLib.Example
{
    /// <summary>
    /// Ví dụ cách sử dụng ThirdLibConfig trong runtime
    /// </summary>
    public class ThirdLibConfigExample : MonoBehaviour
    {
        private void Start()
        {
            // Lấy instance của config
            ThirdLibConfig config = ThirdLibConfig.Instance;

            if (config != null)
            {
                // Lấy Google AdMob App ID theo platform hiện tại
                string adMobAppId = config.GetGoogleAdMobAppId();
                Debug.Log($"Google AdMob App ID: {adMobAppId}");

                // Lấy Facebook App ID
                Debug.Log($"Facebook App ID: {config.facebookAppId}");
                Debug.Log($"Facebook Client Token: {config.facebookClientToken}");


                // Ví dụ lấy ad units
                ExampleGetAdUnits();
            }
        }

        // Ví dụ khởi tạo Google Mobile Ads
        public void InitializeGoogleMobileAds()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config != null)
            {
                string appId = config.GetGoogleAdMobAppId();
                // Khởi tạo Google Mobile Ads với appId
                Debug.Log($"Initializing Google Mobile Ads with App ID: {appId}");
            }
        }

        // Ví dụ khởi tạo Facebook SDK
        public void InitializeFacebookSDK()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config != null)
            {
                string appId = config.facebookAppId;
                string clientToken = config.facebookClientToken;
                // Khởi tạo Facebook SDK
                Debug.Log($"Initializing Facebook SDK with App ID: {appId}");
            }
        }

        // Ví dụ lấy và sử dụng Ad Units
        public void ExampleGetAdUnits()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config == null) return;

            // Lấy Banner Ad Unit từ AdMob
            AdUnit bannerAdMob = config.GetAdUnit(AdFormat.Banner, AdNetwork.AdMob);
            if (bannerAdMob != null)
            {
                string bannerId = bannerAdMob.GetAdUnitId();
                Debug.Log($"AdMob Banner ID: {bannerId}");
            }

            // Lấy tất cả Interstitial Ad Units (đã sắp xếp theo priority)
            List<AdUnit> interstitialAds = config.GetActiveAdUnits(AdFormat.Interstitial);
            Debug.Log($"Found {interstitialAds.Count} active Interstitial ad units");
            foreach (var ad in interstitialAds)
            {
                Debug.Log($"- {ad.network} Interstitial (Priority: {ad.priority}): {ad.GetAdUnitId()}");
            }

            // Lấy Rewarded Ad Unit từ AppLovin
            AdUnit rewardedAppLovin = config.GetAdUnit(AdFormat.Rewarded, AdNetwork.AppLovin);
            if (rewardedAppLovin != null)
            {
                Debug.Log($"AppLovin Rewarded ID: {rewardedAppLovin.GetAdUnitId()}");
            }
        }

        // Ví dụ load Banner Ad
        public void LoadBannerAd()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config == null) return;

            // Lấy tất cả banner ads theo priority
            List<AdUnit> banners = config.GetActiveAdUnits(AdFormat.Banner);

            if (banners.Count > 0)
            {
                // Load banner với priority cao nhất
                AdUnit primaryBanner = banners[0];
                string adUnitId = primaryBanner.GetAdUnitId();

                Debug.Log($"Loading {primaryBanner.network} Banner with ID: {adUnitId}");
                // Gọi SDK tương ứng để load banner

                // Nếu fail, có thể fallback sang banner tiếp theo
                if (banners.Count > 1)
                {
                    AdUnit fallbackBanner = banners[1];
                    Debug.Log($"Fallback: {fallbackBanner.network} Banner - {fallbackBanner.GetAdUnitId()}");
                }
            }
        }
    }
}
