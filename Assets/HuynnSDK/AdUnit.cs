using System;
using UnityEngine;

namespace GameDevToi.ThirdLib
{
    /// <summary>
    /// Định dạng quảng cáo
    /// </summary>
    public enum AdFormat
    {
        Banner,
        Interstitial,
        Rewarded,
        AppOpen,
        RewardedInterstitial,
        Native
    }

    /// <summary>
    /// Mạng quảng cáo
    /// </summary>
    public enum AdNetwork
    {
        AdMob,
        AppLovin,
        IronSource,
        UnityAds
    }

    /// <summary>
    /// Đơn vị quảng cáo
    /// </summary>
    [Serializable]
    public class AdUnit
    {
        public string name = "New Ad Unit";
        public AdFormat format = AdFormat.Banner;
        public AdNetwork network = AdNetwork.AdMob;

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
            format = AdFormat.Banner;
            network = AdNetwork.AdMob;
            androidAdUnitId = "";
            iosAdUnitId = "";
            isActive = true;
            priority = 0;
            notes = "";
        }

        public AdUnit(AdFormat format, AdNetwork network)
        {
            this.name = $"{network} {format}";
            this.format = format;
            this.network = network;
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

        public override string ToString()
        {
            return $"{name} ({network} - {format})";
        }
    }
}
