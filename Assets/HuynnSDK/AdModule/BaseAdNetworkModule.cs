using UnityEngine;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.AdModule
{
    /// <summary>
    /// Base class cho các module mạng quảng cáo
    /// Implement các logic chung, các module cụ thể kế thừa và override
    /// </summary>
    public abstract class BaseAdNetworkModule : IAdNetworkModule
    {
        protected bool isInitialized = false;
        protected ThirdLibConfig config;

        public abstract string NetworkId { get; }

        public bool IsInitialized => isInitialized;

        public virtual void Initialize(ThirdLibConfig config)
        {
            this.config = config;
            var networkDef = AdRegistry.GetNetwork(NetworkId);
            string displayName = networkDef?.displayName ?? NetworkId;
            Debug.Log($"[{displayName}] Initializing...");
        }

        public abstract void LoadAdUnit(AdUnit adUnit);

        public abstract void ShowAd(string formatId, string placementId = null);

        public abstract bool IsAdReady(string formatId, string placementId = null);

        public virtual void DestroyAd(string formatId)
        {
            var formatDef = AdRegistry.GetFormat(formatId);
            string formatName = formatDef?.displayName ?? formatId;
            LogInfo($"Destroying {formatName} ad");
        }

        public virtual void HideBanner()
        {
            LogWarning("HideBanner not implemented for this network");
        }

        public virtual void ShowBanner()
        {
            LogWarning("ShowBanner not implemented for this network");
        }

        protected void LogInfo(string message)
        {
            var networkDef = AdRegistry.GetNetwork(NetworkId);
            string displayName = networkDef?.displayName ?? NetworkId;
            Debug.Log($"[{displayName}] {message}");
        }

        protected void LogWarning(string message)
        {
            var networkDef = AdRegistry.GetNetwork(NetworkId);
            string displayName = networkDef?.displayName ?? NetworkId;
            Debug.LogWarning($"[{displayName}] {message}");
        }

        protected void LogError(string message)
        {
            var networkDef = AdRegistry.GetNetwork(NetworkId);
            string displayName = networkDef?.displayName ?? NetworkId;
            Debug.LogError($"[{displayName}] {message}");
        }
    }
}