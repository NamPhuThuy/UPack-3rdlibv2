using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevToi.ThirdLib.Core
{
    /// <summary>
    /// Registry để đăng ký Ad Formats và Networks động
    /// Developers có thể thêm custom formats/networks
    /// </summary>
    public static class AdRegistry
    {
        private static Dictionary<string, AdFormatDefinition> registeredFormats = new Dictionary<string, AdFormatDefinition>();
        private static Dictionary<string, AdNetworkDefinition> registeredNetworks = new Dictionary<string, AdNetworkDefinition>();
        private static bool isInitialized = false;

        /// <summary>
        /// Khởi tạo registry với built-in definitions
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (isInitialized) return;

            // Register built-in formats
            RegisterFormat(AdFormats.Banner);
            RegisterFormat(AdFormats.Interstitial);
            RegisterFormat(AdFormats.Rewarded);
            RegisterFormat(AdFormats.AppOpen);
            RegisterFormat(AdFormats.RewardedInterstitial);
            RegisterFormat(AdFormats.Native);

            // Register built-in networks
            RegisterNetwork(AdNetworks.AdMob);
            RegisterNetwork(AdNetworks.AppLovin);
            RegisterNetwork(AdNetworks.IronSource);
            RegisterNetwork(AdNetworks.UnityAds);

            isInitialized = true;
            Debug.Log($"[AdRegistry] Initialized with {registeredFormats.Count} formats and {registeredNetworks.Count} networks");
        }

        /// <summary>
        /// Đăng ký Ad Format mới
        /// </summary>
        public static void RegisterFormat(AdFormatDefinition format)
        {
            if (format == null || string.IsNullOrEmpty(format.id))
            {
                Debug.LogError("[AdRegistry] Cannot register null or empty format");
                return;
            }

            if (registeredFormats.ContainsKey(format.id))
            {
                Debug.LogWarning($"[AdRegistry] Format '{format.id}' already registered. Overwriting...");
            }

            registeredFormats[format.id] = format;
            Debug.Log($"[AdRegistry] Registered format: {format.displayName} ({format.id})");
        }

        /// <summary>
        /// Đăng ký Ad Network mới
        /// </summary>
        public static void RegisterNetwork(AdNetworkDefinition network)
        {
            if (network == null || string.IsNullOrEmpty(network.id))
            {
                Debug.LogError("[AdRegistry] Cannot register null or empty network");
                return;
            }

            if (registeredNetworks.ContainsKey(network.id))
            {
                Debug.LogWarning($"[AdRegistry] Network '{network.id}' already registered. Overwriting...");
            }

            registeredNetworks[network.id] = network;
            Debug.Log($"[AdRegistry] Registered network: {network.displayName} ({network.id})");
        }

        /// <summary>
        /// Lấy tất cả formats đã đăng ký
        /// </summary>
        public static List<AdFormatDefinition> GetAllFormats()
        {
            if (!isInitialized) Initialize();
            return new List<AdFormatDefinition>(registeredFormats.Values);
        }

        /// <summary>
        /// Lấy tất cả networks đã đăng ký
        /// </summary>
        public static List<AdNetworkDefinition> GetAllNetworks()
        {
            if (!isInitialized) Initialize();
            return new List<AdNetworkDefinition>(registeredNetworks.Values);
        }

        /// <summary>
        /// Lấy format theo ID
        /// </summary>
        public static AdFormatDefinition GetFormat(string id)
        {
            if (!isInitialized) Initialize();
            return registeredFormats.TryGetValue(id, out var format) ? format : null;
        }

        /// <summary>
        /// Lấy network theo ID
        /// </summary>
        public static AdNetworkDefinition GetNetwork(string id)
        {
            if (!isInitialized) Initialize();
            return registeredNetworks.TryGetValue(id, out var network) ? network : null;
        }

        /// <summary>
        /// Kiểm tra format có tồn tại không
        /// </summary>
        public static bool HasFormat(string id)
        {
            if (!isInitialized) Initialize();
            return registeredFormats.ContainsKey(id);
        }

        /// <summary>
        /// Kiểm tra network có tồn tại không
        /// </summary>
        public static bool HasNetwork(string id)
        {
            if (!isInitialized) Initialize();
            return registeredNetworks.ContainsKey(id);
        }

        /// <summary>
        /// Reset registry (dùng cho testing)
        /// </summary>
        public static void Reset()
        {
            registeredFormats.Clear();
            registeredNetworks.Clear();
            isInitialized = false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Initialize cho Editor (vì RuntimeInitialize không chạy trong Editor)
        /// </summary>
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeInEditor()
        {
            Initialize();
        }
#endif
    }
}
