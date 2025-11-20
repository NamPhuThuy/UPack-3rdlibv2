using System;
using UnityEngine;

namespace GameDevToi.ThirdLib.Core
{
    /// <summary>
    /// Ad Unit - sử dụng string IDs thay vì enums để có thể mở rộng
    /// </summary>
    [Serializable]
    public class AdUnit
    {
        public string name = "New Ad Unit";

        [Tooltip("Format ID (e.g., 'banner', 'interstitial', 'rewarded')")]
        public string formatId = "banner";

        [Tooltip("Network ID (e.g., 'admob', 'applovin', 'ironsource')")]
        public string networkId = "admob";

        [Header("Ad Unit IDs")]
        public string androidAdUnitId = "";
        public string iosAdUnitId = "";

        [Header("Settings")]
        public bool isActive = true;
        public int priority = 0;

        [TextArea(2, 4)]
        public string notes = "";

        public AdUnit()
        {
            name = "New Ad Unit";
            formatId = "banner";
            networkId = "admob";
            androidAdUnitId = "";
            iosAdUnitId = "";
            isActive = true;
            priority = 0;
            notes = "";
        }

        public AdUnit(string formatId, string networkId)
        {
            this.name = $"{networkId} {formatId}";
            this.formatId = formatId;
            this.networkId = networkId;
            this.androidAdUnitId = "";
            this.iosAdUnitId = "";
            this.isActive = true;
            this.priority = 0;
            this.notes = "";
        }

        /// <summary>
        /// Lấy Ad Unit ID theo platform hiện tại
        /// </summary>
        public string GetAdUnitId()
        {
#if UNITY_ANDROID
            return androidAdUnitId;
#elif UNITY_IOS
            return iosAdUnitId;
#else
            return androidAdUnitId; // Fallback
#endif
        }

        /// <summary>
        /// Lấy format definition
        /// </summary>
        public AdFormatDefinition GetFormat()
        {
            return AdRegistry.GetFormat(formatId);
        }

        /// <summary>
        /// Lấy network definition
        /// </summary>
        public AdNetworkDefinition GetNetwork()
        {
            return AdRegistry.GetNetwork(networkId);
        }

        /// <summary>
        /// Validate ad unit
        /// </summary>
        public bool IsValid()
        {
            return AdRegistry.HasFormat(formatId) && AdRegistry.HasNetwork(networkId);
        }

        public override string ToString()
        {
            var format = GetFormat();
            var network = GetNetwork();
            return $"{name} ({network?.displayName ?? networkId} - {format?.displayName ?? formatId})";
        }
    }
}
