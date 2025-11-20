using UnityEngine;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.AdModule
{
    /// <summary>
    /// Module cho Unity Ads
    /// </summary>
    public class UnityAdsModule : BaseAdNetworkModule
    {
        public override string NetworkId => "unityads";

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);

            // Unity Ads thường dùng Game ID từ Project Settings
            // Nhưng có thể init từ code nếu cần

            LogInfo("Unity Ads initializing...");

            // TODO: Initialize Unity Ads SDK
            // Advertisement.Initialize(gameId, testMode, initializationListener);

            isInitialized = true; // Tạm thời set true để test
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            if (!isInitialized)
            {
                LogError("Unity Ads not initialized. Cannot load ad unit.");
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
                LogError("Unity Ads not initialized. Cannot show ad.");
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
