using UnityEngine;
using GameDevToi.ThirdLib.Core;
using GoogleMobileAds.Api;

namespace GameDevToi.ThirdLib.AdModule
{
    /// <summary>
    /// Module cho Google AdMob
    /// </summary>
    public class AdMobModule : BaseAdNetworkModule
    {
        public override string NetworkId => "admob";

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);

            string appId = config.GetGoogleAdMobAppId();

            if (string.IsNullOrEmpty(appId) || appId.Contains("xxxxxxxx"))
            {
                LogWarning("AdMob App ID not configured properly");
                return;
            }

            MobileAds.Initialize(initStatus =>
            {
                isInitialized = true;
                LogInfo("AdMob initialized successfully");
            });

            LogInfo($"AdMob initializing with App ID: {appId}");
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            if (!isInitialized)
            {
                LogError("AdMob not initialized. Cannot load ad unit.");
                return;
            }

            string adUnitId = adUnit.GetAdUnitId();
            var formatDef = adUnit.GetFormat();
            string formatName = formatDef?.displayName ?? adUnit.formatId;
            LogInfo($"Loading {formatName} ad with ID: {adUnitId}");

            string formatId = adUnit.formatId;
            if (formatId == "banner") LoadBanner(adUnitId);
            else if (formatId == "interstitial") LoadInterstitial(adUnitId);
            else if (formatId == "rewarded") LoadRewarded(adUnitId);
            else if (formatId == "appopen") LoadAppOpen(adUnitId);
            else if (formatId == "rewarded_interstitial") LoadRewardedInterstitial(adUnitId);
            else if (formatId == "native") LoadNative(adUnitId);
            else LogWarning($"Unknown ad format: {formatId}");
        }

        public override void ShowAd(string formatId, string placementId = null)
        {
            if (!isInitialized)
            {
                LogError("AdMob not initialized. Cannot show ad.");
                return;
            }

            var formatDef = AdRegistry.GetFormat(formatId);
            string formatName = formatDef?.displayName ?? formatId;
            LogInfo($"Showing {formatName} ad");
            // TODO: Implement show ad logic
        }

        public override bool IsAdReady(string formatId, string placementId = null)
        {
            if (!isInitialized) return false;

            // TODO: Implement ad ready check
            return false;
        }
        private void LoadBanner(string adUnitId)
        {
            LogInfo($"Loading Banner: {adUnitId}");
            // TODO: Implement banner loading
        }

        private void LoadInterstitial(string adUnitId)
        {
            LogInfo($"Loading Interstitial: {adUnitId}");
            // TODO: Implement interstitial loading
        }

        private void LoadRewarded(string adUnitId)
        {
            LogInfo($"Loading Rewarded: {adUnitId}");
            // TODO: Implement rewarded loading
        }

        private void LoadAppOpen(string adUnitId)
        {
            LogInfo($"Loading App Open: {adUnitId}");
            // TODO: Implement app open loading
        }

        private void LoadRewardedInterstitial(string adUnitId)
        {
            LogInfo($"Loading Rewarded Interstitial: {adUnitId}");
            // TODO: Implement rewarded interstitial loading
        }

        private void LoadNative(string adUnitId)
        {
            LogInfo($"Loading Native: {adUnitId}");
            // TODO: Implement native loading
        }
    }
}
