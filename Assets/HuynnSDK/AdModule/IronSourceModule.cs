using UnityEngine;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.AdModule
{
    /// <summary>
    /// Module cho IronSource
    /// </summary>
    public class IronSourceModule : BaseAdNetworkModule
    {
        public override string NetworkId => "ironsource";

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);

            string appKey = config.ironSourceAppKey;

            if (string.IsNullOrEmpty(appKey))
            {
                LogWarning("IronSource App Key not configured");
                return;
            }

            // TODO: Initialize IronSource SDK
            // IronSource.Agent.validateIntegration();
            // IronSource.Agent.init(appKey);

            LogInfo($"IronSource initializing with App Key: {appKey}");
            isInitialized = true; // Tạm thời set true để test
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            if (!isInitialized)
            {
                LogError("IronSource not initialized. Cannot load ad unit.");
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
                LogError("IronSource not initialized. Cannot show ad.");
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
