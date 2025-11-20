using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;
using System.Collections.Generic;

namespace GameDevToi.ThirdLib.Example
{
    /// <summary>
    /// Ví dụ cách sử dụng ThirdLibConfig trong runtime với string-based IDs
    /// </summary>
    public class ThirdLibConfigExample : MonoBehaviour
    {
        private void Start()
        {
            // Lấy instance của config
            ThirdLibConfig config = ThirdLibConfig.Instance;

            if (config != null)
            {
                // Lấy App Information
                Debug.Log($"Package Name: {config.androidPackageName}");
                Debug.Log($"App Version: {config.appVersion} (Code: {config.versionCode})");

                // Lấy Google AdMob App ID theo platform hiện tại
                string adMobAppId = config.GetGoogleAdMobAppId();
                Debug.Log($"Google AdMob App ID: {adMobAppId}");

                // Lấy Facebook App ID
                Debug.Log($"Facebook App ID: {config.facebookAppId}");
                Debug.Log($"Facebook Client Token: {config.facebookClientToken}");

                // Lấy AppLovin và IronSource keys
                Debug.Log($"AppLovin SDK Key: {config.appLovinSdkKey}");
                Debug.Log($"IronSource App Key: {config.ironSourceAppKey}");

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

        // Ví dụ lấy và sử dụng Ad Units với string IDs
        public void ExampleGetAdUnits()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config == null) return;

            // Lấy Banner Ad Unit từ AdMob (sử dụng format ID và network ID)
            AdUnit bannerAdMob = config.GetAdUnit("banner", "admob");
            if (bannerAdMob != null)
            {
                string bannerId = bannerAdMob.GetAdUnitId();
                var network = bannerAdMob.GetNetwork();
                var format = bannerAdMob.GetFormat();
                Debug.Log($"{network?.displayName} {format?.displayName} ID: {bannerId}");
            }

            // Lấy tất cả Interstitial Ad Units (đã sắp xếp theo priority)
            List<AdUnit> interstitialAds = config.GetActiveAdUnits("interstitial");
            Debug.Log($"Found {interstitialAds.Count} active Interstitial ad units");
            foreach (var ad in interstitialAds)
            {
                var network = ad.GetNetwork();
                var format = ad.GetFormat();
                Debug.Log($"- {network?.displayName} {format?.displayName} (Priority: {ad.priority}): {ad.GetAdUnitId()}");
            }

            // Lấy Rewarded Ad Unit từ AppLovin
            AdUnit rewardedAppLovin = config.GetAdUnit("rewarded", "applovin");
            if (rewardedAppLovin != null)
            {
                Debug.Log($"AppLovin Rewarded ID: {rewardedAppLovin.GetAdUnitId()}");
            }

            // Lấy App Open Ad Unit từ IronSource
            AdUnit appOpenIronSource = config.GetAdUnit("appopen", "ironsource");
            if (appOpenIronSource != null)
            {
                Debug.Log($"IronSource App Open ID: {appOpenIronSource.GetAdUnitId()}");
            }
        }

        // Ví dụ load Banner Ad với waterfall mediation
        public void LoadBannerAd()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config == null) return;

            // Lấy tất cả banner ads theo priority (sử dụng format ID)
            List<AdUnit> banners = config.GetActiveAdUnits("banner");

            if (banners.Count > 0)
            {
                // Load banner với priority cao nhất
                AdUnit primaryBanner = banners[0];
                string adUnitId = primaryBanner.GetAdUnitId();
                var network = primaryBanner.GetNetwork();
                var format = primaryBanner.GetFormat();

                Debug.Log($"Loading {network?.displayName} {format?.displayName} with ID: {adUnitId}");
                // Gọi SDK tương ứng để load banner
                // Hoặc dùng AdBridge: AdBridge.Instance.ShowAd("banner");

                // Nếu fail, có thể fallback sang banner tiếp theo
                if (banners.Count > 1)
                {
                    AdUnit fallbackBanner = banners[1];
                    var fallbackNetwork = fallbackBanner.GetNetwork();
                    Debug.Log($"Fallback: {fallbackNetwork?.displayName} Banner - {fallbackBanner.GetAdUnitId()}");
                }
            }
            else
            {
                Debug.LogWarning("No banner ad units configured!");
            }
        }

        // Ví dụ lấy tất cả ad units và validate
        public void ValidateAllAdUnits()
        {
            ThirdLibConfig config = ThirdLibConfig.Instance;
            if (config == null || config.adUnits == null) return;

            Debug.Log($"=== Validating {config.adUnits.Count} Ad Units ===");

            int validCount = 0;
            int invalidCount = 0;

            foreach (var adUnit in config.adUnits)
            {
                bool isValid = adUnit.IsValid();
                if (isValid)
                {
                    validCount++;
                    var network = adUnit.GetNetwork();
                    var format = adUnit.GetFormat();
                    Debug.Log($"✓ {adUnit.name}: {network?.displayName} - {format?.displayName}");
                }
                else
                {
                    invalidCount++;
                    Debug.LogWarning($"✗ {adUnit.name}: Invalid (formatId={adUnit.formatId}, networkId={adUnit.networkId})");
                }
            }

            Debug.Log($"\nValid: {validCount}, Invalid: {invalidCount}");
        }
    }
}
