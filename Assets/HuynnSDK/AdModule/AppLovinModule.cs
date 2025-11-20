using UnityEngine;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.AdModule
{
    /// <summary>
    /// Module cho AppLovin MAX
    /// </summary>
    public class AppLovinModule : BaseAdNetworkModule
    {
        public override string NetworkId => "applovin";

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);

            string sdkKey = config.appLovinSdkKey;

            if (string.IsNullOrEmpty(sdkKey))
            {
                LogWarning("AppLovin SDK Key not configured");
                return;
            }

            // TODO: Initialize AppLovin MAX SDK
            // MaxSdk.SetSdkKey(sdkKey);
            // MaxSdk.InitializeSdk();

            LogInfo($"AppLovin initializing with SDK Key: {sdkKey}");
            isInitialized = true; // Tạm thời set true để test
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            if (!isInitialized)
            {
                LogError("AppLovin not initialized. Cannot load ad unit.");
                return;
            }

            string adUnitId = adUnit.GetAdUnitId();
            var formatDef = adUnit.GetFormat();
            string formatName = formatDef?.displayName ?? adUnit.formatId;
            LogInfo($"Loading {formatName} ad with ID: {adUnitId}");

            // TODO: Implement actual ad loading logic
        }

        public override void ShowAd(string formatId, string placementId = null)
        {
            if (!isInitialized)
            {
                LogError("AppLovin not initialized. Cannot show ad.");
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
    }
}
